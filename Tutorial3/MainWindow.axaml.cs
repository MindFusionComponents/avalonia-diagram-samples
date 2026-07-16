using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace Tutorial3
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.AlignToGrid = false;
			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.DefaultShape = Shapes.Rectangle;

			var node1 = new OrgChartNode
			{
				Bounds = new Rect(100, 60, 240, 100),
				Title = "CEO",
				FullName = "John Smith",
				Text = "Our beloved leader. \r\n" +
					"The CEO of this great corporation.",
				Image = LoadImage("016.png")
			};
			diagram.Nodes.Add(node1);

			var node2 = new OrgChartNode
			{
				Bounds = new Rect(220, 220, 240, 100),
				Title = "CIO",
				FullName = "Bob Smith",
				Text = "The CIO of this great corporation.",
				Image = LoadImage("ac0026-64.png")
			};
			diagram.Nodes.Add(node2);

			diagram.Factory.CreateDiagramLink(node1, node2);
		}

		private IImage? LoadImage(string imageName)
		{
			try
			{
				var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageName);
				if (File.Exists(path))
				{
					return new Avalonia.Media.Imaging.Bitmap(path);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}