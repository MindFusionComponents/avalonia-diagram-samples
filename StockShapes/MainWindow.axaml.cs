using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace StockShapes
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.SelectAfterCreate = false;

			// Set colors
			diagram.BackBrush = new SolidColorBrush(Colors.Azure);

			diagram.ShapeBrush = new LinearGradientBrush
			{
				StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
				EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
				GradientStops =
				{
					new GradientStop(Color.FromArgb(100, 255, 255, 255), 0),
					new GradientStop(Color.FromArgb(255, 0, 128, 192), 1)
				}
			};

			diagram.NodeEffects.Add(new GlassEffect());

			int i = 0;
			double hsize = 85;
			double vsize = 85;
			int perLine = 8;

			// enum all predefined shapes
			foreach (Shape shape in Shape.Shapes)
			{
				// skip some arrowhead shapes that aren't that useful as node shapes
				if (shape.Outline == null) continue;
				if (shape == ArrowHeads.RevWithCirc) continue;
				if (shape == ArrowHeads.DoubleArrow) continue;

				// create a node for this shape
				ShapeNode node = diagram.Factory.CreateShapeNode(
					(i % perLine) * (hsize + 20) + 8,
					(i / perLine) * (vsize + 60) + 15,
					hsize, vsize, shape);
				node.ToolTip = shape.Id;
				node.Stroke = new SolidColorBrush(Colors.Black);

				// attach text below the box
				var labelText = shape.Id.StartsWith("Bpmn") ?
					"Bpmn\n" + shape.Id.Substring(4) : shape.Id;

				var label = new NodeLabel(node, labelText);
				label.SetEdgePosition(
					2,      // bottom edge
					0, 20); // offset
				node.AddLabel(label);

				i = i + 1;
			}

			diagram.ResizeToFitItems(4, true);
		}
	}
}