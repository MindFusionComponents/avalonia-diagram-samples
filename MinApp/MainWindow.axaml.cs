using Avalonia.Controls;

namespace MinApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

		var diagram = diagramView.Diagram;

		var node1 = diagram.Factory.CreateShapeNode(50, 50, 100, 100);
		node1.Text = "Hello";

		var node2 = diagram.Factory.CreateShapeNode(250, 100, 100, 100);
		node2.Text = "Avalonia";

		var link = diagram.Factory.CreateDiagramLink(node1, node2);
	}
}