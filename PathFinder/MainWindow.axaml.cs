using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MindFusion.Animations;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace PathFinderSample
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.Selection.AllowMultipleSelection = false;

			diagram.LinkShape = LinkShape.Bezier;
			diagram.LinkHeadShapeSize = 8;
			diagram.NodeEffects.Add(new GlassEffect());

			diagram.NodeClicked += OnNodeClicked;
		}

		private void OnNodeClicked(object? source, NodeEventArgs e)
		{
			var diagram = diagramView.Diagram;
			diagram.Selection.Clear();

			if (clickedNode != null)
			{
				var finder = new PathFinder(diagram);

				var from = clickedNode;
				var to = e.Node;

				MindFusion.Diagramming.Avalonia.Path path;

				bool shortest = shortestPathRadio.IsChecked == true;
				if (shortest)
					path = finder.FindShortestPath(from, to, true, false);
				else
					path = finder.FindLongestPath(from, to);

				if (path != null)
					RunAnimation(0, path);

				//clickedNode.ShadowBrush = new SolidColorBrush(Colors.Gray);
				clickedNode = null;
			}
			else
			{
				clickedNode = (ShapeNode)e.Node;
				//clickedNode.ShadowBrush = new SolidColorBrush(Colors.Maroon);
			}
		}

		private void RunAnimation(int index, MindFusion.Diagramming.Avalonia.Path path)
		{
			if (index >= path.Items.Count) return;

			var item = path.Items[index];

			// animate nodes
			if (item is DiagramNode nodeItem)
			{
				UpdateCallback callback = (Animation animation, double progress) =>
				{
					var node = (DiagramNode)animation.Item;
					if (animation.Options.FromValue is Color fromColor)
					{
						node.Brush = new SolidColorBrush(fromColor);
					}
				};

				AnimationOptions options = new AnimationOptions
				{
					Duration = 100,
					FromValue = Colors.Maroon,
					ToValue = Colors.Gray
				};

				var animation = new Animation(nodeItem, options, callback);
				animation.Start();

				animation.AnimationComplete += (s, e) =>
				{
					var node = (DiagramNode)animation.Item;
					if (animation.Options.ToValue is Color toColor)
					{
						node.Brush = new SolidColorBrush(toColor);
					}
					RunAnimation(++index, path);
				};
			}
			else if (item is DiagramLink linkItem)
			{
				// animate links
				var sp = linkItem.StartPoint;
				var length = linkItem.Length;

				UpdateCallback callback = (Animation animation, double animationProgress) =>
				{
					var node = animation.Item as DiagramNode;
					if (node != null)
					{
						var bounds = node.Bounds;
						var p = MindFusion.Utilities.PointAlongLength(
							animationProgress * linkItem.Length, linkItem.ControlPoints.GetArray());
						node.Bounds = new Rect(
							p.X - bounds.Width / 2, p.Y - bounds.Height / 2, bounds.Width, bounds.Height);
					}
				};

				var diagram = diagramView.Diagram;
				var tracer = diagram.Factory.CreateShapeNode(sp.X - 8, sp.Y - 8, 16, 16);
				tracer.Shape = Shapes.Ellipse;
				tracer.Brush = new SolidColorBrush(Colors.Maroon);

				Animation animation = new Animation(
					tracer, new AnimationOptions { Duration = (int)length * 10 }, callback);

				animation.AnimationComplete += (s, e) =>
				{
					diagram.Nodes.Remove(tracer);
					RunAnimation(++index, path);
				};

				animation.Start();
			}
		}

		private void OnRandomGraph(object sender, RoutedEventArgs e)
		{
			RandomGraph(3);
			ApplyLayeredLayout();
		}

		private void RandomGraph(int n)
		{
			var diagram = diagramView.Diagram;
			diagram.ClearAll();
			clickedNode = null;

			// random chains
			for (var i = 0; i < n; ++i)
			{
				var c = diagram.Nodes.Count;
				var g = 2 + random.Next(2);
				for (var j = 0; j < g; ++j)
				{
					var rect = new Rect(0, 0, 80, 80);
					var node = new ShapeNode(diagram);
					node.Bounds = rect;
					node.Brush = new SolidColorBrush(Colors.Gray);
					diagram.Nodes.Add(node);
					if (j > 0)
					{
						var link = new DiagramLink(
							diagram, diagram.Nodes[diagram.Nodes.Count - 2], node);
						diagram.Links.Add(link);
					}
				}
				if (i > 0)
				{
					for (var j = 0; j < 1 + random.Next(2); ++j)
					{
						var link = new DiagramLink(
							diagram, diagram.Nodes[random.Next(c)], diagram.Nodes[c + random.Next(g)]);
						diagram.Links.Add(link);
					}
				}
			}

			// additional chain of nodes and links from first to last node
			if (diagram.Nodes.Count >= 2)
			{
				var firstOldNode = diagram.Nodes[0];
				var lastOldNode = diagram.Nodes[diagram.Nodes.Count - 1];
				var oldNodesCount = diagram.Nodes.Count;

				int chainLength = 3 + random.Next(3);
				var chainNodes = new List<DiagramNode>();

				for (int i = 0; i < chainLength; ++i)
				{
					var node = new ShapeNode(diagram)
					{
						Bounds = new Rect(0, 0, 80, 80),
						Brush = new SolidColorBrush(Colors.Gray)
					};
					diagram.Nodes.Add(node);
					chainNodes.Add(node);
				}

				// Create links along the chain:
				// firstOldNode -> chainNodes[0]
				var linkStart = new DiagramLink(diagram, firstOldNode, chainNodes[0]);
				diagram.Links.Add(linkStart);

				// chainNodes[i] -> chainNodes[i+1]
				for (int i = 0; i < chainLength - 1; ++i)
				{
					var linkMid = new DiagramLink(diagram, chainNodes[i], chainNodes[i + 1]);
					diagram.Links.Add(linkMid);
				}

				// chainNodes[last] -> lastOldNode
				var linkEnd = new DiagramLink(diagram, chainNodes[chainLength - 1], lastOldNode);
				diagram.Links.Add(linkEnd);

				// connect a random link from the new chain to one of the old nodes
				var randomChainNodeSource = chainNodes[random.Next(chainLength)];
				var randomOldNodeDest = diagram.Nodes[random.Next(oldNodesCount)];
				var crossLink1 = new DiagramLink(diagram, randomChainNodeSource, randomOldNodeDest);
				diagram.Links.Add(crossLink1);

				// connect a random link from old nodes to the new chain
				var randomOldNodeSource = diagram.Nodes[random.Next(oldNodesCount)];
				var randomChainNodeDest = chainNodes[random.Next(chainLength)];
				var crossLink2 = new DiagramLink(diagram, randomOldNodeSource, randomChainNodeDest);
				diagram.Links.Add(crossLink2);
			}
		}

		private void ApplyLayeredLayout()
		{
			var diagram = diagramView.Diagram;
			var layout = new MindFusion.Diagramming.Avalonia.Layout.LayeredLayout();
			layout.Orientation = MindFusion.Diagramming.Avalonia.Layout.Orientation.Horizontal;
			layout.StraightenLongLinks = true;
			layout.NodeDistance = 70;
			layout.LayerDistance = 70;
			diagram.Arrange(layout);
			diagram.ResizeToFitItems(40);
		}

		readonly Random random = new Random();
		ShapeNode? clickedNode;
	}
}