using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace Tutorial1
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.ShapeBrush = new SolidColorBrush(Colors.LightBlue);
			diagram.DefaultShape = Shapes.Rectangle;

			var nodeMap = new Dictionary<string, DiagramNode>();
			var bounds = new Rect(0, 0, 60, 22);

			// load the graph xml
			var xml = XDocument.Load("SampleGraph.xml");
			var graph = xml.Element("Graph");
			if (graph != null)
			{
				// load node data
				var nodes = graph.Descendants("Node");
				foreach (var node in nodes)
				{
					var diagramNode = diagram.Factory.CreateShapeNode(bounds);
					var idAttr = node.Attribute("id");
					var nameAttr = node.Attribute("name");
					if (idAttr != null && nameAttr != null)
					{
						nodeMap[idAttr.Value] = diagramNode;
						diagramNode.Text = nameAttr.Value;
					}
				}

				// load link data
				var links = graph.Descendants("Link");
				foreach (var link in links)
				{
					var originAttr = link.Attribute("origin");
					var targetAttr = link.Attribute("target");
					if (originAttr != null && targetAttr != null)
					{
						diagram.Factory.CreateDiagramLink(
							nodeMap[originAttr.Value],
							nodeMap[targetAttr.Value]);
					}
				}

				// arrange the graph
				var layout = new LayeredLayout();
				layout.Margins = new Size(30, 30);
				layout.Arrange(diagram);
			}
		}
	}
}