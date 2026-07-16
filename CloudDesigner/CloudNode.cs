using Avalonia;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace CloudDesigner
{
	public class CloudNode : TemplatedNode
	{
		public CloudNode(Diagram diagram) : base(diagram)
		{
		}

		public CloudNode() : base((Diagram)null!)
		{
		}

		static CloudNode()
		{
			// Set default values for properties
			BrushProperty.OverrideDefaultValue<CloudNode>(new SolidColorBrush(Colors.White));
			StrokeProperty.OverrideDefaultValue<CloudNode>(new SolidColorBrush(Colors.LightGray));
			StrokeThicknessProperty.OverrideDefaultValue<CloudNode>(1.0);
		}

		public string Title
		{
			get => GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		public static readonly StyledProperty<string> TitleProperty =
			AvaloniaProperty.Register<CloudNode, string>(nameof(Title), "New Node");

		public string Description
		{
			get => GetValue(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		public static readonly StyledProperty<string> DescriptionProperty =
			AvaloniaProperty.Register<CloudNode, string>(nameof(Description), "");

		public IImage? Icon
		{
			get => GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public static readonly StyledProperty<IImage?> IconProperty =
			AvaloniaProperty.Register<CloudNode, IImage?>(nameof(Icon), null);

		public IBrush StatusBrush
		{
			get => GetValue(StatusBrushProperty);
			set => SetValue(StatusBrushProperty, value);
		}

		public static readonly StyledProperty<IBrush> StatusBrushProperty =
			AvaloniaProperty.Register<CloudNode, IBrush>(nameof(StatusBrush), Brushes.LimeGreen);
	}
}