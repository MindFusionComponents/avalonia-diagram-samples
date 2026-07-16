using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace LayeredLayout
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.LinkHeadShape = ArrowHeads.BowArrow;
			diagram.LinkHeadShapeSize = 8;
			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.DefaultShape = Shapes.Rectangle;

			diagram.ShapeBrush = new LinearGradientBrush
			{
				StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative),
				EndPoint = new RelativePoint(0.8, 1, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Color.Parse("#FF6F7F9F"), 0),
					new GradientStop(Color.Parse("#FFCFDFFF"), 0.37),
					new GradientStop(Color.Parse("#FF6F7F9F"), 0.86),
					new GradientStop(Color.Parse("#FF2F3F6F"), 1)
				}
			};
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			RandomGraph();
			Arrange();
		}

		private void Arrange()
		{
			var diagram = diagramView.Diagram;
			var layout = new MindFusion.Diagramming.Avalonia.Layout.LayeredLayout();
			layout.Anchoring = Anchoring.Reassign;
			layout.EnforceLinkFlow = true;
			layout.StraightenLongLinks = true;
			layout.NodeDistance = 10;
			layout.LayerDistance = 15;
			layout.Arrange(diagram);

			diagram.ResizeToFitItems(5);
			diagramView.ZoomToFit();
		}

		private void RandomGraph()
		{
			var diagram = diagramView.Diagram;
			diagram.ClearAll();

			for (int i = 0; i < 30; ++i)
			{
				int c = diagram.Nodes.Count;
				int g = 2 + random.Next(15);
				for (int j = 0; j < g; ++j)
				{
					var node = diagram.Factory.CreateShapeNode(0, 0, 40, 40);
					node.AnchorPattern = AnchorPattern.TopInBottomOut;
					if (j > 0)
						diagram.Factory.CreateDiagramLink(diagram.Nodes[diagram.Nodes.Count - 2], node);
				}
				if (i > 0)
				{
					for (int j = 0; j < 1 + random.Next(3); ++j)
						diagram.Factory.CreateDiagramLink(
							diagram.Nodes[random.Next(c)],
							diagram.Nodes[c + random.Next(g)]);
				}
			}
		}

		readonly Random random = new Random();
	}
}