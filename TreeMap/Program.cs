using System;
using Avalonia;
using MindFusion.Diagramming.Avalonia;

namespace TreeMap;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
		    .UseMindFusionDiagramming()
#if DEBUG
			.WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}