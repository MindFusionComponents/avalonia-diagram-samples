using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Svg.Skia;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace CloudDesigner
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.DefaultShape = Shapes.RoundRect;
			diagram.BackBrush = new SolidColorBrush(Colors.White);

			// Set Default Head Shape (Styling for thickness/color is handled in App.axaml)
			diagram.LinkHeadShape = ArrowHeads.Arrow;

			// Add initial description node
			var initialNode = new ShapeNode(diagram)
			{
				StrokeThickness = 1,
				Shape = Shapes.RoundRect,
				Bounds = new Rect(100, 60, 400, 80), 
				TextAlignment = TextAlignment.Center,
				TextVerticalAlignment = AlignmentY.Center,
				EnableStyledText = true,
				PolygonalTextLayout = true,
				FontFamily = "Verdana",
				FontSize = 12,
				Brush = new SolidColorBrush(Color.Parse("#FFF8F0")), 
				Stroke = new SolidColorBrush(Color.Parse("#FFDAB9")), 
				Text = "Cloud Designer: Follow the <b>Sample.png</b> pattern.\n1. Drag a <b>VPC</b> container.\n2. Add <b>Subnets</b> inside.\n3. Drop <b>Resources</b> into subnets."
			};
			diagram.Nodes.Add(initialNode);

			// Enable Grid Snapping for professional alignment
			diagram.AlignToGrid = true;

			// Initialize Palette
			InitializePalette(diagram);

			// Selection Events
			diagram.NodeSelected += OnNodeSelected;
			diagram.NodeDeselected += OnNodeDeselected;

			// Property change handlers
			txtTitle.TextChanged += (s, e) => {
				if (selectedNode != null && !isUpdating)
					selectedNode.Text = txtTitle.Text ?? "";
			};

			txtDesc.TextChanged += (s, e) => {
				if (selectedNode != null && !isUpdating)
					selectedNode.ToolTip = txtDesc.Text ?? "";
			};

			btnArrange.Click += OnArrangeClick;
			btnExport.Click += async (s, e) => await OnExportClick();
		}

		private async Task OnExportClick()
		{
			try
			{
				var diagram = diagramView.Diagram;
				var bounds = diagram.GetContentBounds(false, false);
				if (bounds.Width == 0 || bounds.Height == 0) return;

				// Professional Save File Dialog
				var storage = this.StorageProvider;
				var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
				{
					Title = "Export Cloud Architecture",
					DefaultExtension = ".png",
					SuggestedFileName = "MyCloudInfrastructure.png",
					FileTypeChoices = new[] { FilePickerFileTypes.ImagePng }
				});

				if (file != null)
				{
					var pixelSize = new PixelSize((int)bounds.Right + 50, (int)bounds.Bottom + 50);
					using (var bitmap = new Avalonia.Media.Imaging.RenderTargetBitmap(pixelSize, new Vector(96, 96)))
					{
						bitmap.Render(diagramView);
						using (var stream = await file.OpenWriteAsync())
						{
							bitmap.Save(stream);
						}
					}
					Debug.WriteLine($"Diagram exported to: {file.Path}");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Export Error: {ex.Message}");
			}
		}

		private void OnArrangeClick(object? sender, RoutedEventArgs e)
		{
			var layout = new LayeredLayout();
			layout.Orientation = MindFusion.Diagramming.Avalonia.Layout.Orientation.Horizontal;
			layout.NodeDistance = 40;
			layout.LayerDistance = 60;
			layout.Arrange(diagramView.Diagram);
		}

		private void InitializePalette(Diagram diagram)
		{
			// 1. Containers
			AddContainerItem(diagram, "VPC", Color.Parse("#E1F5FE"), "Containers", DashStyle.Dash);
			AddContainerItem(diagram, "Public Subnet", Color.Parse("#E8F5E9"), "Containers", null);
			AddContainerItem(diagram, "Private Subnet", Color.Parse("#FFF3E0"), "Containers", null);
			AddContainerItem(diagram, "Security Group", Colors.Transparent, "Containers", DashStyle.Dot);

			// 2. Compute
			AddCloudItem(diagram, "Virtual Machine", "virtual-machine.png", "Cloud Resources");
			AddCloudItem(diagram, "Serverless", "serverless.svg", "Cloud Resources");

			// 3. Storage
			AddCloudItem(diagram, "Database", "database.svg", "Cloud Resources");
			AddCloudItem(diagram, "S3 Bucket", "bucket.svg", "Cloud Resources");

			// 4. Networking
			AddCloudItem(diagram, "Load Balancer", "load_balancer.svg", "Cloud Resources");
			AddCloudItem(diagram, "Firewall", "firewall.svg", "Cloud Resources");
			AddCloudItem(diagram, "Gateway", "gateway.svg", "Cloud Resources");
			AddCloudItem(diagram, "Router", "router.svg", "Cloud Resources");

			palette.ExpandCategory("Containers");
			palette.ExpandCategory("Cloud Resources");
		}

		private void AddCloudItem(Diagram diagram, string title, string iconFile, string category)
		{
			var node = new ShapeNode(diagram)
			{
				Bounds = new Rect(0, 0, 100, 100),
				Transparent = true,
				Brush = Brushes.Transparent,
				Stroke = Brushes.Transparent,
				Image = iconFile.EndsWith(".svg") ? LoadSvg(iconFile) : LoadImage(iconFile),
				Text = title,
				TextAlignment = TextAlignment.Center,
				TextVerticalAlignment = AlignmentY.Bottom, 
				FontFamily = "Verdana",
				FontSize = 10,
				FontWeight = FontWeight.Bold
			};
			palette.AddItem(node, category, title);
		}

		private void AddContainerItem(Diagram diagram, string title, Color bgColor, string category, IDashStyle? dashStyle)
		{
			var node = new ContainerNode(diagram)
			{
				Text = title,
				Bounds = new Rect(0, 0, 300, 200),
				Brush = new SolidColorBrush(bgColor),
				Stroke = new SolidColorBrush(bgColor == Colors.Transparent ? Colors.Gray : bgColor.Darken(0.2)),
				StrokeThickness = 2,
				StrokeDashStyle = dashStyle,
				Foldable = true,
				FontWeight = FontWeight.Bold
			};
			palette.AddItem(node, category, title);
		}

		private void OnNodeSelected(object? sender, NodeEventArgs e)
		{
			isUpdating = true;
			selectedNode = e.Node;
			txtTitle.Text = selectedNode.Text;
			txtDesc.Text = selectedNode.ToolTip?.ToString() ?? "";
			isUpdating = false;
		}

		private void OnNodeDeselected(object? sender, NodeEventArgs e)
		{
			isUpdating = true;
			selectedNode = null;
			txtTitle.Text = "";
			txtDesc.Text = "";
			isUpdating = false;
		}

		private IImage? LoadImage(string fileName)
		{
			try
			{
				var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", fileName);
				if (System.IO.File.Exists(path))
					return new Avalonia.Media.Imaging.Bitmap(path);
			}
			catch { }
			return null;
		}

		private IImage? LoadSvg(string fileName)
		{
			try
			{
				var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", fileName);
				if (System.IO.File.Exists(path))
				{
					var svgSource = SvgSource.Load(path);
					return svgSource != null ? new SvgImage { Source = svgSource } : null;
				}
			}
			catch { }
			return null;
		}

		private DiagramNode? selectedNode;
		private bool isUpdating = false;
	}

	public static class ColorExtensions
	{
		public static Color Darken(this Color color, double amount)
		{
			return Color.FromArgb(color.A, 
				(byte)(color.R * (1 - amount)), 
				(byte)(color.G * (1 - amount)), 
				(byte)(color.B * (1 - amount)));
		}
	}
}