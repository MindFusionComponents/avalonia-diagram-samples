using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace Controls
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.DefaultShape = Shapes.RoundRect;

			// Add initial description node
			var initialNode = new ShapeNode(diagram)
			{
				StrokeThickness = 3,
				Shape = Shapes.RoundRect,
				Bounds = new Rect(200, 200, 350, 150),
				TextAlignment = TextAlignment.Center,
				TextVerticalAlignment = AlignmentY.Center,
				EnableStyledText = true,
				PolygonalTextLayout = true,
				FontFamily = "Verdana",
				FontSize = 14,
				Text = "This sample demonstrates the auxiliary controls available with MindFusion.Diagramming for Avalonia:\nthe <b>Overview</b>, the <b>Ruler</b>, the <b>ZoomControl</b> and the <b>Palette</b>."
			};
			diagram.Nodes.Add(initialNode);

			// Connect auxiliary controls
			zoomControl.Target = diagramView;
			overview.DiagramView = diagramView;
			overview.TrackingRectPen = new Pen(Brushes.Red, 3);

			// Populate Palette using user-provided code
			var flowchartShapes = new[] { "Start", "Input", "Process", "Decision" };
			for (var i = 0; i < flowchartShapes.Length; ++i)
			{
				var node = new ShapeNode();
				node.Bounds = new Rect(0, 0, 100, 100);
				node.Shape = Shape.FromId(flowchartShapes[i]);
				node.Text = flowchartShapes[i];
				palette.AddItem(node, "Flowchart Shapes", flowchartShapes[i]);
			}

			var dataShapes = new[] { "Database", "Input", "Delay", "Document", "ManualOperation" };
			for (var i = 0; i < dataShapes.Length; ++i)
			{
				var node = new ShapeNode();
				node.Bounds = new Rect(0, 0, 100, 100);
				node.Shape = Shape.FromId(dataShapes[i]);
				node.Text = dataShapes[i];
				palette.AddItem(node, "Data Shapes", dataShapes[i]);
			}

			var bpmnShapes = new[] { "BpmnStartLink", "BpmnIntermediateLink", "BpmnEndLink", "BpmnStartMessage", "BpmnIntermediateMessage", "BpmnEndMessage" };
			for (var i = 0; i < bpmnShapes.Length; ++i)
			{
				var node = new ShapeNode();
				node.Bounds = new Rect(0, 0, 100, 100);
				node.Shape = Shape.FromId(bpmnShapes[i]);
				node.Text = bpmnShapes[i];
				palette.AddItem(node, "BPMN Shapes", bpmnShapes[i]);
			}

			palette.ExpandCategory("Flowchart Shapes");
		}
	}
}