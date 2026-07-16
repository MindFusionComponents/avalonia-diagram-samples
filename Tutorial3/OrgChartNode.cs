using Avalonia;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace Tutorial3
{
	// DataTemplate is in App.axaml
	public class OrgChartNode : TemplatedNode
	{
		public string Title
		{
			get => GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		public static readonly StyledProperty<string> TitleProperty =
			AvaloniaProperty.Register<OrgChartNode, string>(nameof(Title), "");

		public string FullName
		{
			get => GetValue(FullNameProperty);
			set => SetValue(FullNameProperty, value);
		}

		public static readonly StyledProperty<string> FullNameProperty =
			AvaloniaProperty.Register<OrgChartNode, string>(nameof(FullName), "");

		public IImage? Image
		{
			get => GetValue(ImageProperty);
			set => SetValue(ImageProperty, value);
		}

		public static readonly StyledProperty<IImage?> ImageProperty =
			AvaloniaProperty.Register<OrgChartNode, IImage?>(nameof(Image), null);
	}
}
