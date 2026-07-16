using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace Containers
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.LinkHeadShapeSize = 2;

			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.ShapeBrush = new SolidColorBrush(Colors.White);
			diagram.TextBrush = new SolidColorBrush(Colors.Black);
			diagram.FontFamily = "Open Sans";
			diagram.FontSize = 10;

			// create child nodes for containers
			for (var i = 0; i < 5; i++)
				diagram.Factory.CreateShapeNode(0, 0, 40, 40);
			for (var i = 1; i < 5; i++)
				diagram.Factory.CreateDiagramLink(diagram.Nodes[(int)Math.Floor(i / 2d)], diagram.Nodes[i]);

			for (var i = 5; i < 10; i++)
				diagram.Factory.CreateShapeNode(0, 0, 40, 40);
			for (var i = 1; i < 5; i++)
				diagram.Factory.CreateDiagramLink(diagram.Nodes[(int)Math.Floor(5 + i / 2d)], diagram.Nodes[5 + i]);

			// create containers
			ContainerNode[] ctr = new ContainerNode[2];
			for (var c = 0; c < 2; c++)
			{
				var container = diagram.Factory.CreateContainerNode(0, 0, 40, 40);
				for (var i = c * 5; i < c * 5 + 5; i++)
					container.Add(diagram.Nodes[i]);

				container.Arrange(new TreeLayout { Type = TreeLayoutType.Centered });
				container.Foldable = true;
				container.ZIndex = 0;
				container.Text = "container " + (c + 1);
				container.Brush = new SolidColorBrush(Colors.LightSteelBlue);
				container.HandlesStyle = HandlesStyle.HatchHandles3;

				ctr[c] = container;
			}

			ctr[0].Move(80, 80);
			ctr[1].Move(360, 80);
			diagram.Factory.CreateDiagramLink(ctr[0], ctr[1]);
		}
	}
}