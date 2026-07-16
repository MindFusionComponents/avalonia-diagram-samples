using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace Inheritance
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			CreateClassDiagram(diagramView.Diagram,
				new Type[]
				{
					typeof(DiagramItem),
					typeof(DiagramLink),
					typeof(DiagramNode),
					typeof(ShapeNode),
					typeof(TableNode),
					typeof(ContainerNode),
					typeof(FreeFormNode),
					typeof(SvgNode),
					typeof(PagedContainerNode)
				});
		}

		void CreateClassDiagram(Diagram diagram, Type[] classes)
		{
			var classConstructors = new Dictionary<Type, DiagramNode>();

			// create a table node for each class
			for (var i = 0; i < classes.Length; i++)
			{
				var className = classes[i].Name;
				var node = diagram.Factory.CreateTableNode(80, 80, 160, 180);
				node.RedimTable(1, 0);
				node.Text = className;
				node.Brush = new SolidColorBrush(Colors.White);
				node.CaptionBackBrush = new SolidColorBrush(Colors.LightSteelBlue);
				
				node.FontFamily = "Open Sans";
				node.FontSize = 10;
				node.FontStyle = FontStyle.Italic;
				node.FontWeight = FontWeight.Bold;
				
				node.Scrollable = true;
				node.ConnectionStyle = TableConnectionStyle.Table;

				foreach (var property in classes[i].GetProperties())
				{
					node.AddRow();
					var cell = node[0, node.Rows.Count - 1];
					cell.Text = property.Name;
				}

				classConstructors.Add(classes[i], node);
			}

			// create a diagram link for each prototype inheritance
			foreach (var cc in classConstructors)
			{
				var baseType = cc.Key.BaseType;
				if (baseType != null && classConstructors.ContainsKey(baseType))
				{
					var baseNode = classConstructors[baseType];
					var link = diagram.Factory.CreateDiagramLink(baseNode, cc.Value);
					link.HeadShape = null;
					link.BaseShape = ArrowHeads.Triangle;
					link.BaseShapeSize = 12;
				}
			}

			// arrange as a tree
			var treeLayout = new MindFusion.Diagramming.Avalonia.Layout.TreeLayout();
			treeLayout.LinkStyle = TreeLayoutLinkType.Cascading3;
			diagram.Arrange(treeLayout);
		}
	}
}