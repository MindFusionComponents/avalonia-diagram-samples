using Avalonia;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace MinApp;

/// <summary>
/// A palette control hosting diagram item prototypes that can be dragged directly onto the DiagramView.
/// Inherits from MindFusion's ItemListView.
/// </summary>
public class NodeListView : ItemListView
{
    public NodeListView()
    {
    }

    /// <summary>
    /// Populates the palette with ER modeling node templates.
    /// </summary>
    public void PopulateDefaultPrototypes(Diagram? diagramContext = null)
    {
        var context = diagramContext ?? new Diagram();
        Items.Clear();

        // 1. Prototype TableNode
        var table = new TableNode(context);
        table.Caption = "New Table";
        table.CaptionHeight = 24;
        table.CaptionBackBrush = new SolidColorBrush(Color.FromRgb(30, 41, 59));
        table.CaptionBrush = Brushes.White;
        table.RedimTable(3, 3);
        table.Columns[0].Width = 32;
        table.Columns[1].Width = 78;
        table.Columns[2].Width = 50;
        table.ConnectionStyle = TableConnectionStyle.Rows;

        table[0, 0].Text = "PK";
        table[1, 0].Text = "Id";
        table[2, 0].Text = "INT";

        table[0, 1].Text = "FK";
        table[1, 1].Text = "RefId";
        table[2, 1].Text = "INT";

        table[0, 2].Text = "";
        table[1, 2].Text = "Name";
        table[2, 2].Text = "VARCHAR";

        table.Bounds = new Rect(0, 0, 160, 95);
        table.ToolTip = "Drag to add a new Table";
        Items.Add(table);

        // 2. Prototype ContainerNode (Domain Container)
        var container = new ContainerNode(context);
        container.Caption = "Domain Container";
        container.CaptionHeight = 24;
        container.CaptionBackBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
        container.CaptionBrush = Brushes.White;
        container.Brush = new SolidColorBrush(Color.FromArgb(20, 16, 185, 129));
        container.Stroke = new SolidColorBrush(Color.FromRgb(16, 185, 129));
        container.StrokeThickness = 1.5;
        container.Foldable = true;
        container.AutoGrow = true;
        container.CornerRadius = 8;
        container.Bounds = new Rect(0, 0, 160, 110);
        container.ToolTip = "Drag to create a Domain Group Container";
        Items.Add(container);

        // 3. Prototype ShapeNode (Note / Comment)
        var note = new ShapeNode(context);
        note.Shape = Shapes.RoundRect;
        note.Text = "ER Schema Note";
        note.Brush = new SolidColorBrush(Color.FromRgb(254, 243, 199)); // Light yellow
        note.Stroke = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Amber
        note.StrokeThickness = 1;
        note.TextBrush = new SolidColorBrush(Color.FromRgb(146, 64, 14));
        note.Bounds = new Rect(0, 0, 140, 50);
        note.ToolTip = "Drag to add a sticky annotation note";
        Items.Add(note);
    }
}
