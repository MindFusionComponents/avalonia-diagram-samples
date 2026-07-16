using System;
using System.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MindFusion.Diagramming.Avalonia;

namespace AnchorPoints
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.DefaultShape = Shapes.Rectangle;
			diagram.Bounds = new Rect(0, 0, 3840, 3840);

			diagram.NodeCreated += OnNodeCreated;
			diagram.SnapToAnchor = SnapToAnchor.OnCreateOrModify;
			diagram.SelectAfterCreate = false;
			diagram.LinkShape = LinkShape.Cascading;
			diagram.ShapeHandlesStyle = HandlesStyle.DashFrame;
			diagram.AlignToGrid = false;
			diagram.BackBrush = new SolidColorBrush(Colors.White);
			diagram.LinkSegments = 3;
			diagram.RouteLinks = true;
			diagram.AutoSnapLinks = true;
			diagram.AutoSnapDistance = 10;

			radioButton1.IsChecked = true;

			diagram.NodeEffects.Add(new GlassEffect());
			diagram.NodeEffects.Add(new AeroEffect());

			// define custom anchor point patterns
			apat1 = new AnchorPattern(new AnchorPoint[]
				{
					new AnchorPoint(50, 0, true, true),
					new AnchorPoint(100, 50, true, true),
					new AnchorPoint(50, 100, true, true),
					new AnchorPoint(0, 50, true, true)
				});

			apat2 = new AnchorPattern(new AnchorPoint[]
				{
					new AnchorPoint(10, 0, true, false, MarkStyle.Circle, Brushes.RoyalBlue),
					new AnchorPoint(50, 0, true, false, MarkStyle.Circle, Brushes.Blue),
					new AnchorPoint(90, 0, true, false, MarkStyle.Circle, Brushes.Firebrick),
					new AnchorPoint(10, 100, false, true, MarkStyle.Rectangle),
					new AnchorPoint(50, 100, false, true, MarkStyle.Rectangle),
					new AnchorPoint(90, 100, false, true, MarkStyle.Rectangle),
					new AnchorPoint(0, 50, true, true, MarkStyle.Custom)
				});

			// create a simple flowchart
			var node1 = new ShapeNode(diagram);
			node1.Bounds = new Rect(38.4, 26.88, 96, 69.12);
			node1.Shape = Shapes.Ellipse;
			node1.Text = "Start";
			node1.AnchorPattern = apat1;
			diagram.Nodes.Add(node1);

			var node2 = new ShapeNode(diagram);
			node2.Bounds = new Rect(76.8, 288, 96, 69.12);
			node2.Text = "node 1";
			node2.AnchorPattern = apat2;
			diagram.Nodes.Add(node2);

			var node3 = new ShapeNode(diagram);
			node3.Bounds = new Rect(268.8, 268.8, 96, 69.12);
			node3.Text = "node 2";
			node3.AnchorPattern = apat2;
			diagram.Nodes.Add(node3);

			var node4 = new ShapeNode(diagram);
			node4.Bounds = new Rect(307.2, 384, 96, 69.12);
			node4.Shape = Shapes.Ellipse;
			node4.Text = "End";
			node4.AnchorPattern = apat1;
			diagram.Nodes.Add(node4);

			var decision1 = new ShapeNode(diagram);
			decision1.Bounds = new Rect(76.8, 134.4, 115.2, 76.8);
			decision1.Shape = Shapes.Decision;
			decision1.Text = "check 1";
			decision1.AnchorPattern = AnchorPattern.Decision1In3Out;
			diagram.Nodes.Add(decision1);

			var decision2 = new ShapeNode(diagram);
			decision2.Bounds = new Rect(268.8, 115.2, 115.2, 76.8);
			decision2.Shape = Shapes.Decision;
			decision2.Text = "check 2";
			decision2.AnchorPattern = AnchorPattern.Decision2In2Out;
			diagram.Nodes.Add(decision2);

			var link = new DiagramLink(diagram, decision1, decision2);
			diagram.Links.Add(link);

			diagram.RoutingOptions.TriggerRerouting |= RerouteLinks.WhileCreating;
			diagram.RoutingOptions.Anchoring = Anchoring.Keep;
		}

		void OnRadio1Checked(object sender, RoutedEventArgs e)
		{
			if (sender is RadioButton rb && rb.IsChecked == true)
			{
				var diagram = diagramView?.Diagram;
				if (diagram != null)
				{
					diagram.DefaultShape = Shapes.Rectangle;
				}
			}
		}

		void OnRadio2Checked(object sender, RoutedEventArgs e)
		{
			if (sender is RadioButton rb && rb.IsChecked == true)
			{
				var diagram = diagramView?.Diagram;
				if (diagram != null)
				{
					diagram.DefaultShape = Shapes.Decision;
				}
			}
		}

		void OnNodeCreated(object sender, NodeEventArgs e)
		{
			var node = e.Node as ShapeNode;
			if (node == null)
				return;

			if (node.Shape == Shapes.Decision)
				node.AnchorPattern = AnchorPattern.Decision1In3Out;
			else
				node.AnchorPattern = apat2;
		}

		async void OnLoadClick(object sender, RoutedEventArgs e)
		{
			var topLevel = TopLevel.GetTopLevel(this);
			if (topLevel != null)
			{
				var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title = "Open Diagram XML",
					FileTypeFilter = new[] { new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } } }
				});
				if (files.Count > 0)
				{
					diagramView.Diagram.LoadFromXml(files[0].Path.LocalPath);
				}
			}
		}

		async void OnSaveClick(object sender, RoutedEventArgs e)
		{
			var topLevel = TopLevel.GetTopLevel(this);
			if (topLevel != null)
			{
				var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
				{
					Title = "Save Diagram XML",
					DefaultExtension = "xml",
					ShowOverwritePrompt = true,
					FileTypeChoices = new[] { new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } } }
				});
				if (file != null)
				{
					diagramView.Diagram.SaveToXml(file.Path.LocalPath);
				}
			}
		}

		AnchorPattern apat1;
		AnchorPattern apat2;
	}
}