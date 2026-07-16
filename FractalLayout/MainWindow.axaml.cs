using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace FractalLayout
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.DefaultShape = Shapes.Ellipse;
			diagram.LinkHeadShape = null;
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			var diagram = diagramView.Diagram;
			diagram.ClearAll();

			var root = diagram.Factory.CreateShapeNode(nodeBounds);
			RandomTree(root, 5, 4);
			Arrange(root);
		}

		private void Arrange(DiagramNode root)
		{
			var diagram = diagramView.Diagram;
			var layout = new MindFusion.Diagramming.Avalonia.Layout.FractalLayout();
			layout.Root = root;
			layout.Arrange(diagram);

			diagram.ResizeToFitItems(5);
			diagramView.ZoomToFit();
		}

		private void RandomTree(DiagramNode node, int depth, int minChildren)
		{
			if (depth <= 0)
				return;

			var diagram = node.Parent;
			var children = random.Next(depth) - 1 + minChildren;

			if (diagram.Nodes.Count < 3 && children < 2)
				children = 2;

			for (var i = 0; i < children; ++i)
			{
				// create child node and link
				var child = diagram.Factory.CreateShapeNode(nodeBounds);
				child.Brush = brushes[depth % brushes.Length];
				diagram.Factory.CreateDiagramLink(node, child);

				// build child branch
				RandomTree(child, depth - 1, minChildren);
			}
		}

		private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
		{
			// zoom in or out at the mouse position
			var newZoom = diagramView.ZoomFactor + e.Delta.Y * 2.0;
			if (newZoom > 5)
			{
				var viewPoint = e.GetPosition(diagramView);
				var zoomCenter = diagramView.ViewToDiagram(viewPoint);
				diagramView.SetZoomFactor(newZoom, zoomCenter);
			}

			// stop the scrollviewer from scrolling
			e.Handled = true;
		}

		readonly Random random = new Random();
		readonly Rect nodeBounds = new Rect(0, 0, 50, 50);

		// use different brush for each tree level
		readonly Brush[] brushes = new Brush[]
		{
			new RadialGradientBrush
			{
				Center = RelativePoint.Center,
				GradientOrigin = new RelativePoint(0.3, 0.3, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Colors.LightSteelBlue, 0),
					new GradientStop(Colors.BlueViolet, 1)
				}
			},
			new RadialGradientBrush
			{
				Center = RelativePoint.Center,
				GradientOrigin = new RelativePoint(0.3, 0.3, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Colors.White, 0),
					new GradientStop(Colors.LightBlue, 1)
				}
			},
			new RadialGradientBrush
			{
				Center = RelativePoint.Center,
				GradientOrigin = new RelativePoint(0.3, 0.3, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Colors.White, 0),
					new GradientStop(Colors.DeepSkyBlue, 1)
				}
			},
			new RadialGradientBrush
			{
				Center = RelativePoint.Center,
				GradientOrigin = new RelativePoint(0.3, 0.3, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Colors.LimeGreen, 0),
					new GradientStop(Colors.Green, 1)
				}
			}
		};
	}
}