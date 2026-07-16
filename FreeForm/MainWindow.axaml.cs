using System;
using Avalonia;
using Avalonia.Controls;
using MindFusion.Diagramming.Avalonia;

namespace FreeForm
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.FreeFormTargets = new[]
			{
				Shapes.Rectangle,
				Shapes.Ellipse,
				Shapes.Decision
			};
		}
	}
}