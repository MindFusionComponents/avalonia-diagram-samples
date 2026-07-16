using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace SequenceDiagram
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.AllowSelfLoops = false;
			diagram.AllowLinksRepeat = false;
			diagram.SelectionOnTop = false;
			diagram.NodeCreated += OnNodeCreated;
			diagram.NodeModifying += OnNodeModifying;
			diagram.NodeModified += OnNodeModified;
			diagram.NodeDeleted += OnNodeDeleted;
			diagram.NodeDoubleClicked += OnNodeDoubleClicked;
			diagram.LinkCreated += OnLinkCreated;
			diagram.LinkCreating += OnLinkCreating;
			diagram.LinkDeleted += OnLinkDeleted;
			diagram.EnterInplaceEditMode += OnEnterInplaceEditMode;

			diagram.Selection.AllowMultipleSelection = false;
			diagram.BackBrush = new LinearGradientBrush
			{
				StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
				EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Colors.White, 0),
					new GradientStop(Colors.DodgerBlue, 1)
				}
			};

			GlassEffect glass = new GlassEffect();
			glass.Type = GlassEffectType.Type2;
			diagram.NodeEffects.Add(glass);

			// add object and process shapes to the ItemListView control
			AddToPalette(objectShape, "Object");
			AddToPalette(processShape, "Process");

			// create two default objects
			for (int i = 0; i < 2; i++)
			{
				var node = new ShapeNode(diagram)
				{
					HandlesStyle = HandlesStyle.DashFrame,
					Text = "Object " + (i + 1),
					AllowOutgoingLinks = false,
					AllowIncomingLinks = false,
					Bounds = new Rect((44 * mm) * i + 10, 10, 24 * mm, 24 * mm),
					Shape = objectShape,
					EnabledHandles = AdjustmentHandles.None,
					Brush = new LinearGradientBrush
					{
						StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
						EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative),
						GradientStops =
						{
							new GradientStop(Colors.White, 0),
							new GradientStop(Colors.LimeGreen, 1)
						}
					}
				};
				objects.Add(node);
				diagram.Items.Add(node);
			}

			// add sequence diagram lifelines extending below the object nodes
			AddLifelines();
		}

		void AddToPalette(Shape shape, string label)
		{
			var n1 = new ShapeNode(diagramView.Diagram)
			{
				Shape = shape,
				Bounds = new Rect(0, 0, 25, 25),
				Brush = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255))
			};
			itemListView.Items.Add(n1);
			ItemListView.SetLabel(n1, label);
		}

		private void OnNodeCreated(object sender, NodeEventArgs e)
		{
			var node = (ShapeNode)e.Node;
			node.HandlesStyle = HandlesStyle.DashFrame;

			if (node.Shape == objectShape)
			{
				node.AllowOutgoingLinks = false;
				node.AllowIncomingLinks = false;
				node.EnabledHandles = AdjustmentHandles.None;
				node.Brush = new LinearGradientBrush
				{
					StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
					EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative),
					GradientStops =
					{
						new GradientStop(Colors.White, 0),
						new GradientStop(Colors.LimeGreen, 1)
					}
				};
				objects.Add(node);
				node.Text = "Object " + objects.Count;

				AddLifelines();
			}

			if (node.Shape == processShape)
			{
				node.Brush = new LinearGradientBrush
				{
					StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
					EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative),
					GradientStops =
					{
						new GradientStop(Colors.White, 0),
						new GradientStop(Colors.Red, 1)
					}
				};
				node.TextRotationAngle = -90;
				node.PolygonalTextLayout = true;
				node.TextAlignment = TextAlignment.Center;
				node.TextVerticalAlignment = AlignmentY.Center;
				processes.Add(node);
				node.Text = "Process " + processes.Count;
				node.EnabledHandles =
					AdjustmentHandles.ResizeBottomCenter |
					AdjustmentHandles.Move |
					AdjustmentHandles.ResizeTopCenter;
				node.Constraints.MoveDirection = DirectionConstraint.Vertical;

				// use grouping to associate this process node with its lifeline
				DiagramLink lifeline = NearestLifeline(node);
				node.AttachTo(lifeline, AttachToLink.Point, 0);
				node.Tag = lifeline.Origin;
				node.Bounds = new Rect(
					lifeline.StartPoint.X - 6 * mm,
					node.Bounds.Y, 12 * mm, 24 * mm);
			}

			diagramView.EndEdit(true);
			diagramView.BeginEdit(node);

			// grow the diagram to the right if necessary
			var contentRect = diagramView.Diagram.GetContentBounds(false, false);
			if (contentRect.Right > diagramView.Diagram.Bounds.Right)
			{
				var r = diagramView.Diagram.Bounds;
				r = new Rect(r.Position, new Size(contentRect.Right - r.Left + 30, r.Height));
				diagramView.Diagram.Bounds = r;
			}
		}

		private void OnNodeModified(object sender, NodeEventArgs e)
		{
			// after this node is modified, check if it overlaps the next node
			// on the lifeline and push nodes down if necessary
			if (e.Node.MasterGroup != null && e.Node.MasterGroup.MainItem is DiagramLink)
			{
				var group = e.Node.MasterGroup;
				var nodes = new List<DiagramNode>(group.AttachedNodes.Cast<DiagramNode>());
				nodes.Sort((n1, n2) => n1.Bounds.Top.CompareTo(n2.Bounds.Top));
				var nodesBelow = nodes.Skip(nodes.IndexOf(e.Node) + 1);
				if (nodesBelow.Any() &&
					nodesBelow.First().Bounds.Top <= e.Node.Bounds.Bottom)
				{
					var offset = e.Node.Bounds.Bottom - nodesBelow.First().Bounds.Top + 30;
					foreach (DiagramNode node in nodesBelow)
						node.Move(node.Bounds.X, node.Bounds.Y + offset);
				}
			}

			ArrangeLinks();
		}

		private void OnLinkCreated(object sender, LinkEventArgs e)
		{
			ArrangeLinks();
		}

		// distributes message links between processes so that they are horizontal and
		// do not overlap when there are more than one links connecting the same pair
		private void ArrangeLinks()
		{
			var diagram = diagramView.Diagram;
			var bundles = FindLinkBundles();
			foreach (var bundle in bundles.Values)
			{
				var range = SharedRange(bundle[0].Origin, bundle[0].Destination);
				if (range != null)
				{
					double h = range[1] - range[0];
					double s = h / (bundle.Count + 1);
					double y = range[0] + s;
					foreach (DiagramLink link in bundle)
					{
						link.Shape = LinkShape.Polyline;
						link.SegmentCount = 1;
						link.ReassignAnchorPoints();

						link.StartPoint = new Point(link.StartPoint.X, y);
						link.EndPoint = new Point(link.EndPoint.X, y);
						link.UpdateFromPoints();

						y += s;
					}
				}
				else
				{
					diagram.RoutingOptions.StartOrientation = MindFusion.Diagramming.Avalonia.Orientation.Horizontal;
					diagram.RoutingOptions.EndOrientation = MindFusion.Diagramming.Avalonia.Orientation.Horizontal;

					DiagramLink l = bundle[0];
					DiagramNode left, right;
					if (l.Origin.Bounds.X < l.Destination.Bounds.X)
					{
						left = l.Origin;
						right = l.Destination;
					}
					else
					{
						left = l.Destination;
						right = l.Origin;
					}

					double ly, ry, h = 20, s = h / (bundle.Count + 1);
					if (left.Bounds.Bottom < right.Bounds.Top)
					{
						ly = s + left.Bounds.Bottom - h;
						ry = s + right.Bounds.Top;
					}
					else
					{
						ly = s + left.Bounds.Top;
						ry = s + right.Bounds.Bottom - h;
					}
					foreach (DiagramLink link in bundle)
					{
						link.Shape = LinkShape.Cascading;
						if (link.Origin == left)
						{
							link.StartPoint = new Point(left.Bounds.Right, ly);
							link.EndPoint = new Point(right.Bounds.Left, ry);
						}
						else
						{
							link.StartPoint = new Point(right.Bounds.Left, ry);
							link.EndPoint = new Point(left.Bounds.Right, ly);
						}
						ly += s;
						ry += s;
					}
					diagram.LinkRouter.RouteLinks(bundle);
				}
			}
		}

		private DiagramLink NearestLifeline(DiagramNode node)
		{
			double nodeX = node.GetCenter().X;
			double minDist = double.MaxValue;
			DiagramLink nearest = null!;

			foreach (DiagramLink link in lifelines)
			{
				double distance = Math.Abs(link.StartPoint.X - nodeX);
				if (distance < minDist)
				{
					minDist = distance;
					nearest = link;
				}
			}

			return nearest;
		}

		private void AddLifelines()
		{
			var diagram = diagramView.Diagram;
			for (int i = 0; i < objects.Count; i++)
			{
				if (objects[i].OutgoingLinks.Count == 0)
				{
					var lifeline = new DiagramLink(diagram)
					{
						Tag = "Lifeline",
						Origin = objects[i],
						Destination = diagram.Dummy,
						Locked = true,
						Stroke = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50)),
						StrokeThickness = 2,
						StrokeDashStyle = DashStyle.Dash,
						HeadShape = null
					};
					diagram.Links.Add(lifeline);
					lifelines.Add(lifeline);
				}
			}

			CompactLifelines();
		}

		void CompactLifelines()
		{
			var diagram = diagramView.Diagram;
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].Bounds = new Rect((44 * mm) * i + 10, 10, 24 * mm, 24 * mm);

				double objectX = objects[i].GetCenter().X;
				var lifeline = objects[i].OutgoingLinks[0];
				var startPoint = new Point(objectX, objects[i].Bounds.Bottom);
				var endPoint = new Point(objectX, startPoint.Y + diagram.Bounds.Height);
				if (startPoint != lifeline.StartPoint || endPoint != lifeline.EndPoint)
				{
					lifeline.StartPoint = startPoint;
					lifeline.EndPoint = endPoint;
					lifeline.UpdateFromPoints();

					if (lifeline.SubordinateGroup != null)
					{
						foreach (DiagramNode process in lifeline.SubordinateGroup.AttachedNodes)
							process.Move(objectX - process.Bounds.Width / 2, process.Bounds.Y);
					}
				}
			}
		}

		private void OnNodeDoubleClicked(object sender, NodeEventArgs e)
		{
			diagramView.EndEdit(true);
			diagramView.BeginEdit(e.Node as ShapeNode);
		}

		private void OnNodeDeleted(object sender, NodeEventArgs e)
		{
			var diagram = diagramView.Diagram;
			for (int i = diagram.Items.Count - 1; i >= 0; i--)
			{
				if (diagram.Items[i].Tag == e.Node)
					diagram.Items.Remove(diagram.Items[i]);
			}

			if (objects.Contains(e.Node))
				objects.Remove(e.Node);
			if (processes.Contains(e.Node))
				processes.Remove(e.Node);

			var node = (ShapeNode)e.Node;
			if (node.Shape == objectShape)
				CompactLifelines();
		}

		private void OnEnterInplaceEditMode(object sender, InPlaceEditEventArgs e)
		{
			var textBox = (TextBox)e.EditControl;
			textBox.TextAlignment = TextAlignment.Center;
			textBox.VerticalContentAlignment = VerticalAlignment.Center;

			textBox.SelectAll();

			var node = e.Node as ShapeNode;
			if (node == null || node.Shape != processShape)
				return;

			// display the edit box of process nodes rotated to match
			// how their text will appear with TextRotationAngle set
			double nodeWidth = e.Node.Bounds.Width;
			double nodeHeight = e.Node.Bounds.Height;
			textBox.Width = nodeHeight;
			textBox.Height = nodeWidth;

			textBox.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);
			var transformGroup = new TransformGroup();
			transformGroup.Children.Add(new TranslateTransform(nodeWidth / 2 - nodeHeight / 2, nodeHeight / 2 - nodeWidth / 2));
			transformGroup.Children.Add(new RotateTransform { Angle = -90, CenterX = nodeWidth / 2, CenterY = nodeHeight / 2 });
			textBox.RenderTransform = transformGroup;
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			if (itemListView.SelectedIndex == 0)
				return;

			e.Handled = true;

			// allow dropping process nodes only if they are near a lifeline
			Point mousePos = e.GetPosition(diagramView);
			mousePos = diagramView.ViewToDiagram(mousePos);
			foreach (DiagramNode node in objects)
			{
				if (node.Bounds.Left < mousePos.X && node.Bounds.Right > mousePos.X)
				{
					e.DragEffects = DragDropEffects.Copy;
					return;
				}
			}

			e.DragEffects = DragDropEffects.None;
		}

		private void OnNodeModifying(object sender, NodeValidationEventArgs e)
		{
			var diagram = diagramView.Diagram;
			e.Cancel = e.Node.Bounds.Top < diagram.Nodes[0].Bounds.Bottom;
		}

		private void OnLinkDeleted(object sender, LinkEventArgs e)
		{
			if (lifelines.Contains(e.Link))
				lifelines.Remove(e.Link);
		}

		private void OnLinkCreating(object sender, LinkValidationEventArgs e)
		{
			// do not allow links between processes on the same lifeline
			if (e.Destination != null)
			{
				int idx1 = lifelines.IndexOf(e.Origin.MasterGroup.MainItem as DiagramLink);
				int idx2 = lifelines.IndexOf(e.Destination.MasterGroup.MainItem as DiagramLink);
				e.Cancel = idx1 == idx2;
			}
		}

		private int PairCode(DiagramLink link)
		{
			var diagram = diagramView.Diagram;
			int i1 = link.Origin.ZIndex;
			int i2 = link.Destination.ZIndex;
			return diagram.Items.Count * Math.Max(i1, i2) + Math.Min(i1, i2);
		}

		private Dictionary<int, DiagramLinkCollection> FindLinkBundles()
		{
			var diagram = diagramView.Diagram;
			var bundles = new Dictionary<int, DiagramLinkCollection>();
			foreach (DiagramLink link in diagram.Links)
			{
				if ("Lifeline".Equals(link.Tag))
					continue;

				int code = PairCode(link);
				if (!bundles.ContainsKey(code))
					bundles[code] = new DiagramLinkCollection();
				bundles[code].Add(link);
			}
			return bundles;
		}

		private double[]? SharedRange(DiagramNode n1, DiagramNode n2)
		{
			var r1 = n1.Bounds;
			var r2 = n2.Bounds;

			if (r1.Top > r2.Bottom)
				return null;
			if (r1.Bottom < r2.Top)
				return null;

			return new[] { Math.Max(r1.Top, r2.Top), Math.Min(r1.Bottom, r2.Bottom) };
		}

		readonly Shape objectShape = Shapes.MultiProc;
		readonly Shape processShape = Shapes.Procedure;

		readonly List<DiagramNode> objects = new List<DiagramNode>();
		readonly List<DiagramNode> processes = new List<DiagramNode>();
		readonly List<DiagramLink> lifelines = new List<DiagramLink>();

		const double mm = 4;
	}
}