using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace Tutorial2
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.LinkHeadShapeSize = 6;
			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.ShapeBrush = new SolidColorBrush(Colors.LightBlue);
			diagram.DefaultShape = Shapes.Rectangle;

			var root = diagram.Factory.CreateShapeNode(nodeBounds);
			root.Text = "Project";
			var document = XDocument.Load("SampleTree.xml");
			var projectElement = document.Element("Project");
			if (projectElement != null)
			{
				CreateChildren(root, projectElement);
			}

			var layout = new TreeLayout();
			layout.Type = TreeLayoutType.Cascading;
			layout.Direction = TreeLayoutDirections.LeftToRight;
			layout.LinkStyle = TreeLayoutLinkType.Cascading2;
			layout.NodeDistance = 5;
			layout.LevelDistance = -25; // let horizontal positions overlap
			layout.Margins = new Size(30, 30);
			layout.Arrange(diagram);
		}

		private void CreateChildren(DiagramNode parentDiagNode, XElement parentXmlElement)
		{
			var diagram = diagramView.Diagram;
			foreach (var element in parentXmlElement.Elements("Activity"))
			{
				var node = diagram.Factory.CreateShapeNode(nodeBounds);
				var nameAttr = element.Attribute("Name");
				if (nameAttr != null)
				{
					node.Text = nameAttr.Value;
				}
				diagram.Factory.CreateDiagramLink(parentDiagNode, node);
				CreateChildren(node, element);
			}
		}

		readonly Rect nodeBounds = new Rect(0, 0, 100, 22);
	}
}