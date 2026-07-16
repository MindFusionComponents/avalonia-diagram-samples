using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace TreeMap
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.DefaultShape = Shapes.Rectangle;

			isInitialized = true;
			CreateTreeMap();
		}

		private void CreateTreeMap()
		{
			var diagram = diagramView.Diagram;
			diagram.ClearAll();

			// Load the graph xml
			var xmlDoc = new XmlDocument();
			xmlDoc.Load("Data.xml");

			var cnt = 0;
			var continents = xmlDoc.GetElementsByTagName("continent");

			for (var i = 0; i < continents.Count; i++)
			{
				var continent = continents[i];
				if (continent.Attributes == null) continue;

				var container = diagram.Factory.CreateContainerNode(0, 0, 40, 40);
				container.Tag = new KeyValuePair<float, float>(
					float.Parse(continent.Attributes["area"].Value),
					float.Parse(continent.Attributes["population"].Value));
				container.Text = continent.Attributes["name"].Value;
				container.Id = cnt++;

				var countries = continent.SelectNodes("countries/country");
				if (countries != null)
				{
					for (var k = 0; k < countries.Count; k++)
					{
						var country = countries[k];
						if (country.Attributes == null) continue;

						var shape = diagram.Factory.CreateShapeNode(new Rect(0, 0, 40, 40));
						var area = float.Parse(country.Attributes["area"].Value);
						var population = float.Parse(country.Attributes["population"].Value);
						shape.Tag = new KeyValuePair<float, float>(area, population);
						shape.Text = country.Attributes["name"].Value;
						shape.Id = cnt++;

						var areaQuantifier = "million sq. km.";
						if (area < 1)
						{
							area *= 1000;
							areaQuantifier = "thousand sq. km.";
						}
						var populationQuantifier = "billions";
						if (population < 1)
						{
							population *= 1000;
							populationQuantifier = "millions";
						}

						shape.ToolTip = shape.Text + "\nArea: ~" + area + " " + areaQuantifier + "\nPopulation: ~" + population + " " + populationQuantifier;

						container.Add(shape);
					}
				}
			}

			AssignWeights();
			AssignColors();
			ArrangeDiagram();
		}

		private void AssignWeights()
		{
			var diagram = diagramView.Diagram;
			for (var i = 0; i < diagram.Nodes.Count; i++)
			{
				var node = diagram.Nodes[i];
				// Ignore containers and container captions
				if (node.GetType() == typeof(ContainerNode) || node.Locked)
					continue;

				if (node.GetType() == typeof(ShapeNode))
				{
					KeyValuePair<float, float> values = (KeyValuePair<float, float>)node.Tag;

					if (weightOrder == 0)
						node.Weight = values.Value;
					else if (weightOrder == 1)
						node.Weight = values.Key;
				}
			}
		}

		private void AssignColors()
		{
			var diagram = diagramView.Diagram;
			var largestArea = new Dictionary<DiagramNode, float>();
			var largestPopulation = new Dictionary<DiagramNode, float>();

			for (var i = 0; i < diagram.Nodes.Count; i++)
			{
				var node = diagram.Nodes[i];
				if (node.GetType() == typeof(ContainerNode))
				{
					var area = 0.0f;
					var population = 0.0f;
					var container = (ContainerNode)node;
					for (var j = 0; j < container.Children.Count; j++)
					{
						var child = container.Children[j];
						KeyValuePair<float, float> values = (KeyValuePair<float, float>)child.Tag;

						area = Math.Max(area, values.Key);
						population = Math.Max(population, values.Value);
					}

					largestArea.Add(node, area);
					largestPopulation.Add(node, population);
				}
			}

			for (var i = 0; i < diagram.Nodes.Count; i++)
			{
				var node = diagram.Nodes[i];
				// Ignore containers and container captions
				if (node.GetType() == typeof(ContainerNode) || node.Locked)
					continue;

				if (node.GetType() == typeof(ShapeNode))
				{
					KeyValuePair<float, float> values = (KeyValuePair<float, float>)node.Tag;

					var value = 0f;
					var total = 0f;

					if (node.Container != null)
					{
						if (colorOrder == 0)
						{
							value = values.Value;
							total = largestPopulation[node.Container];
						}
						else if (colorOrder == 1)
						{
							value = values.Key;
							total = largestArea[node.Container];
						}
						node.Brush = new SolidColorBrush(GetScaleColor(value, total));
					}
				}
			}
		}

		private Color GetScaleColor(float value, float total)
		{
			var factor = value / total;
			if (factor < 0 || float.IsNaN(factor))
				factor = 0;
			if (factor > 1)
				factor = 1;

			var step = 1.0 / (colorsScale.Length - 1);
			int index = (int)Math.Floor(factor / step);
			int nextIndex = Math.Min(colorsScale.Length - 1, index + 1);

			var factorBase = factor - (index * step);
			double ratio = factorBase / step;

			var s = colorsScale[index];
			var e = colorsScale[nextIndex];

			byte r = (byte)(s.R + ratio * (e.R - s.R));
			byte g = (byte)(s.G + ratio * (e.G - s.G));
			byte b = (byte)(s.B + ratio * (e.B - s.B));

			return Color.FromRgb(r, g, b);
		}

		private void ArrangeDiagram()
		{
			var diagram = diagramView.Diagram;
			var layout = new MindFusion.Diagramming.Avalonia.Layout.TreeMapLayout();
			layout.Padding = 0;
			layout.ContainerPadding = 4;
			layout.LayoutArea = new Rect(0, 0, 1000, 780);
			diagram.Arrange(layout);
			diagram.ResizeToFitItems(40);

			// Remove the text of smaller nodes
			for (var i = 0; i < diagram.Nodes.Count; i++)
			{
				var node = diagram.Nodes[i];
				if (!(node.GetType() == typeof(ShapeNode)) || node.Locked)
					continue;

				if (node.Bounds.Width < 40 || node.Bounds.Height < 40)
					node.Text = "";
			}

			// Resize caption nodes
			for (var i = 0; i < diagram.Nodes.Count; i++)
			{
				var node = diagram.Nodes[i];
				if (node.Locked && node.Container != null)
				{
					var bounds = node.Bounds;
					bounds = new Rect(bounds.Position, new Size(node.Container.Bounds.Width, Math.Min(120, node.Container.Bounds.Height)));
					node.Bounds = bounds;
				}
			}
		}

		private void OnWeightSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!isInitialized) return;
			weightOrder = weightComboBox.SelectedIndex;
			CreateTreeMap();
		}

		private void OnColorSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!isInitialized) return;
			colorOrder = colorComboBox.SelectedIndex;
			CreateTreeMap();
		}

		private readonly bool isInitialized = false;
		private int weightOrder = 1;
		private int colorOrder = 0;

		private readonly Color[] colorsScale = new Color[]
		{
			Color.FromRgb(224, 233, 233),
			Color.FromRgb(97, 106, 127),
			Color.FromRgb(206, 0, 0)
		};
	}
}