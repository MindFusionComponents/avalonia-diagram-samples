using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MinApp.Models;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace MinApp;

public class TableListItem
{
    public required string Name { get; init; }
    public required string Domain { get; init; }
    public required IBrush DomainBackground { get; init; }
}

public class InspectorRowItem
{
    public required string KeyTag { get; init; }
    public required IBrush KeyColor { get; init; }
    public required string ColumnName { get; init; }
    public required string DataType { get; init; }
}

public partial class MainWindow : Window
{
    private string _dbPath = "";
    private List<TableSchema> _tables = new();
    private List<ForeignKeySchema> _foreignKeys = new();
    private List<DomainGroup> _domains = new();

    private readonly Dictionary<string, TableNode> _tableNodes = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<ContainerNode> _containers = new();
    private readonly List<DiagramLink> _links = new();

    private readonly IBrush _pkBrush = new SolidColorBrush(Color.FromRgb(217, 119, 6));   // Amber
    private readonly IBrush _fkBrush = new SolidColorBrush(Color.FromRgb(37, 99, 235));   // Blue
    private readonly IBrush _pkFkBrush = new SolidColorBrush(Color.FromRgb(124, 58, 237));// Purple
    private readonly IBrush _emptyBrush = Brushes.Transparent;

    public MainWindow()
    {
        InitializeComponent();

        SetupControls();
        LoadDatabaseAndBuildDiagram();
    }

    private void SetupControls()
    {
        var diagram = diagramView.Diagram;
        diagram.EnableStyledText = true;
        diagram.BackBrush = new SolidColorBrush(Color.FromRgb(248, 250, 252));
        diagram.ShowGrid = true;
        diagram.GridStyle = GridStyle.Points;
        diagram.GridSizeX = 14;
        diagram.GridSizeY = 14;
        diagram.GridPen = new Pen(new SolidColorBrush(Color.FromArgb(40, 148, 163, 184)), 1);
        diagram.HitTestPriority = HitTestPriority.ZOrder;

		diagram.RoutingOptions.EvaluateFarPoints = true;
		diagram.RoundedLinks = true;
        if (diagram.LinkRouter is CompositeRouter compositeRouter)
			compositeRouter.MinLinkSpacing = 6;

		diagramView.Behavior = Behavior.DrawLinks;
        diagramView.MiddleButtonActions = MouseButtonActions.Pan;

        // Connect overview control to diagram view
        overview.DiagramView = diagramView;

        // Populate NodeListView with draggable prototypes
        nodeListView.PopulateDefaultPrototypes(diagram);

        // Selection & Click events
        diagram.NodeClicked += (s, e) =>
        {
            if (e.Node is TableNode tn && tn.Tag is TableSchema ts)
            {
                ShowTableDetails(ts);
            }
            else if (e.Node is ContainerNode cn && cn.Tag is DomainGroup dg)
            {
                ShowContainerDetails(dg);
            }
        };

        diagram.LinkClicked += (s, e) =>
        {
            if (e.Link.Tag is ForeignKeySchema fk)
            {
                ShowLinkDetails(fk);
            }
        };

        diagram.Clicked += (s, e) =>
        {
            if (diagram.Selection.Nodes.Count == 0 && diagram.Selection.Links.Count == 0)
            {
                ClearInspector();
            }
        };
    }

    private void LoadDatabaseAndBuildDiagram()
    {
        try
        {
            _dbPath = DatabaseSchemaLoader.FindDatabasePath();
            txtStatusDb.Text = System.IO.Path.GetFileName(_dbPath);
            txtDbNameBadge.Text = System.IO.Path.GetFileName(_dbPath);

            var (tables, fks, domains) = DatabaseSchemaLoader.Load(_dbPath);
            _tables = tables;
            _foreignKeys = fks;
            _domains = domains;

            // Update stats
            txtMetricTables.Text = _tables.Count.ToString();
            txtMetricRelations.Text = _foreignKeys.Count.ToString();
            txtMetricDomains.Text = _domains.Count.ToString();
            txtStatusCounts.Text = $"{_tables.Count} Tables, {_foreignKeys.Count} Relations, {_domains.Count} Domains";

            // Populate table list
            PopulateTableList();

            // Build Diagram
            BuildSchemaDiagram();
        }
        catch (Exception ex)
        {
            txtInspectorTitle.Text = "Database Load Error";
            txtInspectorSubtitle.Text = ex.Message;
        }
    }

    private void PopulateTableList(string? filter = null)
    {
        var items = new List<TableListItem>();
        foreach (var t in _tables)
        {
            if (!string.IsNullOrWhiteSpace(filter) && !t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                continue;

            var domainColor = _domains.Find(d => string.Equals(d.Name, t.Domain, StringComparison.OrdinalIgnoreCase))?.Color ?? Color.FromRgb(100, 116, 139);
            items.Add(new TableListItem
            {
                Name = t.Name,
                Domain = t.Domain,
                DomainBackground = new SolidColorBrush(domainColor)
            });
        }
        lstTables.ItemsSource = items;
    }

    private void BuildSchemaDiagram()
    {
        var diagram = diagramView.Diagram;
        diagram.ClearAll();
        _tableNodes.Clear();
        _containers.Clear();
        _links.Clear();

        // 1. Create TableNodes for each table
        double initialX = 50, initialY = 50;
        foreach (var table in _tables)
        {
            var domain = _domains.Find(d => string.Equals(d.Name, table.Domain, StringComparison.OrdinalIgnoreCase));
            var domainColor = domain?.Color ?? Color.FromRgb(71, 85, 105);

            int colCount = 3;
            int rowCount = Math.Max(1, table.Columns.Count);
            double width = 195;
            double height = 32 + rowCount * 22;

            var tableNode = diagram.Factory.CreateTableNode(initialX, initialY, width, height, colCount, rowCount);
            tableNode.Caption = table.Name;
            tableNode.CaptionHeight = 26;
            tableNode.CaptionBackBrush = new SolidColorBrush(domainColor);
            tableNode.CaptionBrush = Brushes.White;
            tableNode.Brush = Brushes.White;
            tableNode.Stroke = new SolidColorBrush(domainColor);
            tableNode.StrokeThickness = 1.5;
            tableNode.ConnectionStyle = TableConnectionStyle.Rows;
            tableNode.Tag = table;

            tableNode.Columns[0].Width = 38; // PK/FK tag
            tableNode.Columns[1].Width = 92; // Field Name
            tableNode.Columns[2].Width = 65; // Data Type

            for (int r = 0; r < table.Columns.Count; r++)
            {
                var col = table.Columns[r];
                string keyText = "";
                if (col.IsPrimaryKey && col.IsForeignKey) keyText = "PK,FK";
                else if (col.IsPrimaryKey) keyText = "PK";
                else if (col.IsForeignKey) keyText = "FK";

                tableNode[0, r].Text = keyText;
                tableNode[1, r].Text = col.Name;
                tableNode[2, r].Text = col.DataType;

                tableNode.Rows[r].Height = 22;
            }

            _tableNodes[table.Name] = tableNode;

            initialX += 230;
            if (initialX > 900)
            {
                initialX = 50;
                initialY += 260;
            }
        }

        // 2. Create ContainerNodes for each domain (Sales, Customers, Catalog, Inventory)
        foreach (var domain in _domains)
        {
            var container = diagram.Factory.CreateContainerNode(30, 30, 300, 300, true);
            container.Caption = $"{domain.Name} Domain";
            container.CaptionHeight = 26;
            container.Foldable = true;
            container.AutoGrow = true;
            container.CornerRadius = 8;
            container.Margin = 25;
            container.Brush = new SolidColorBrush(Color.FromArgb(16, domain.Color.R, domain.Color.G, domain.Color.B));
            container.Stroke = new SolidColorBrush(domain.Color);
            container.StrokeThickness = 1.8;
            container.CaptionBackBrush = new SolidColorBrush(domain.Color);
            container.CaptionBrush = Brushes.White;
            container.Tag = domain;

            // Group nodes of this domain inside the container
            foreach (var tableName in domain.TableNames)
            {
                if (_tableNodes.TryGetValue(tableName, out var tNode))
                {
                    container.Add(tNode);
                }
            }

            _containers.Add(container);
        }

        // 3. Create Links for Foreign Keys
        foreach (var fk in _foreignKeys)
        {
            if (_tableNodes.TryGetValue(fk.FromTable, out var fromNode) &&
                _tableNodes.TryGetValue(fk.ToTable, out var toNode))
            {
                var fromTableSchema = _tables.Find(t => string.Equals(t.Name, fk.FromTable, StringComparison.OrdinalIgnoreCase));
                var toTableSchema = _tables.Find(t => string.Equals(t.Name, fk.ToTable, StringComparison.OrdinalIgnoreCase));

                if (fromTableSchema != null && toTableSchema != null)
                {
                    int fromRow = fromTableSchema.Columns.FindIndex(c => string.Equals(c.Name, fk.FromColumn, StringComparison.OrdinalIgnoreCase));
                    int toRow = toTableSchema.Columns.FindIndex(c => string.Equals(c.Name, fk.ToColumn, StringComparison.OrdinalIgnoreCase));

                    if (fromRow >= 0 && toRow >= 0)
                    {
                        var link = fromNode.AddRelation(fromRow, Relationship.ManyToOne, toNode, toRow);
                        link.Shape = LinkShape.Cascading;
                        link.AutoRoute = true;
                        link.Stroke = new SolidColorBrush(Color.FromArgb(220, 71, 85, 105)); // Slate 600
                        link.StrokeThickness = 1.5;
                        link.HeadShape = ArrowHeads.PointerArrow;
                        link.HeadBrush = new SolidColorBrush(Color.FromArgb(220, 71, 85, 105));
                        link.Tag = fk;
                        _links.Add(link);
                    }
                }
            }
        }

		// 4. Arrange the diagram using auto-layout algorithms.
		ArrangeDiagram();
    }

    private void ArrangeDiagram()
    {
        var diagram = diagramView.Diagram;

		// 1. Arrange nodes inside each container using LayeredLayout
		foreach (var c in _containers)
		{
			var containerLayout = new LayeredLayout
			{
				Orientation = MindFusion.Diagramming.Avalonia.Layout.Orientation.Horizontal,
				LayerDistance = 80,
				NodeDistance = 180,
				Margins = new Size(25, 25)
			};
			c.AutoShrink = true;
			c.Arrange(containerLayout);
		}

		// 2. Arrange the whole diagram with LayeredLayout preserving containers
		var layout = new OrthogonalLayout
        {
            KeepGroupLayout = true,
            GrowToFit = true,
            Margins = new Size(50, 50),
            Padding = 50,
            MinLaneSize = 50,
            MinimizeLinkBends = true,
            Refine = true
        };

        layout.Arrange(diagram);

        // Ensure links are neatly routed orthogonally around nodes
        diagram.RouteAllLinks();

        // Adjust diagram extents
        var contentBounds = diagram.GetContentBounds(false, false);
        diagram.Bounds = new Rect(
            0,
            0,
            Math.Max(contentBounds.Right + 80, 1600),
            Math.Max(contentBounds.Bottom + 80, 1200)
        );

        diagramView.ZoomToFit();
        UpdateZoomStatus();
    }

    private void ShowTableDetails(TableSchema table)
    {
        txtInspectorTitle.Text = $"Table: {table.Name}";
        txtInspectorSubtitle.Text = $"Domain: {table.Domain} • {table.Columns.Count} Columns";

        var rows = new List<InspectorRowItem>();
        foreach (var col in table.Columns)
        {
            string keyTag = "";
            IBrush keyColor = _emptyBrush;

            if (col.IsPrimaryKey && col.IsForeignKey)
            {
                keyTag = "PK, FK";
                keyColor = _pkFkBrush;
            }
            else if (col.IsPrimaryKey)
            {
                keyTag = "PK";
                keyColor = _pkBrush;
            }
            else if (col.IsForeignKey)
            {
                keyTag = "FK";
                keyColor = _fkBrush;
            }

            rows.Add(new InspectorRowItem
            {
                KeyTag = keyTag,
                KeyColor = keyColor,
                ColumnName = col.Name,
                DataType = col.DataType + (col.IsForeignKey && col.RefTable != null ? $" -> {col.RefTable}" : "")
            });
        }
        icInspectorDetails.ItemsSource = rows;
    }

    private void ShowContainerDetails(DomainGroup domain)
    {
        txtInspectorTitle.Text = $"{domain.Name} Domain";
        txtInspectorSubtitle.Text = $"Container holding {domain.TableNames.Count} tables";

        var rows = new List<InspectorRowItem>();
        foreach (var tableName in domain.TableNames)
        {
            var t = _tables.Find(tbl => string.Equals(tbl.Name, tableName, StringComparison.OrdinalIgnoreCase));
            rows.Add(new InspectorRowItem
            {
                KeyTag = "TBL",
                KeyColor = new SolidColorBrush(domain.Color),
                ColumnName = tableName,
                DataType = $"{t?.Columns.Count ?? 0} cols"
            });
        }
        icInspectorDetails.ItemsSource = rows;
    }

    private void ShowLinkDetails(ForeignKeySchema fk)
    {
        txtInspectorTitle.Text = "Foreign Key Link";
        txtInspectorSubtitle.Text = $"{fk.FromTable}.{fk.FromColumn} -> {fk.ToTable}.{fk.ToColumn}";

        var rows = new List<InspectorRowItem>
        {
            new() { KeyTag = "FROM", KeyColor = _fkBrush, ColumnName = $"{fk.FromTable}.{fk.FromColumn}", DataType = "Foreign Key" },
            new() { KeyTag = "TO", KeyColor = _pkBrush, ColumnName = $"{fk.ToTable}.{fk.ToColumn}", DataType = "Primary Key" }
        };
        icInspectorDetails.ItemsSource = rows;
    }

    private void ClearInspector()
    {
        txtInspectorTitle.Text = "Click a table, domain, or link";
        txtInspectorSubtitle.Text = "Details will appear here";
        icInspectorDetails.ItemsSource = null;
    }

    private void UpdateZoomStatus()
    {
        txtStatusZoom.Text = $"Zoom: {(int)diagramView.ZoomFactor}%";
    }

    // --- Toolbar Event Handlers ---

    private void OnArrangeDiagram(object? sender, RoutedEventArgs e)
    {
        ArrangeDiagram();
    }

    private void OnRouteLinksClicked(object? sender, RoutedEventArgs e)
    {
        diagramView.Diagram.RouteAllLinks();
    }

    private void OnZoomToFitClicked(object? sender, RoutedEventArgs e)
    {
        diagramView.ZoomToFit();
        UpdateZoomStatus();
    }

    private void OnZoom100Clicked(object? sender, RoutedEventArgs e)
    {
        diagramView.ZoomFactor = 100;
        UpdateZoomStatus();
    }

    private void OnZoomInClicked(object? sender, RoutedEventArgs e)
    {
        diagramView.ZoomFactor = Math.Min(diagramView.ZoomFactor * 1.25, 500);
        UpdateZoomStatus();
    }

    private void OnZoomOutClicked(object? sender, RoutedEventArgs e)
    {
        diagramView.ZoomFactor = Math.Max(diagramView.ZoomFactor / 1.25, 20);
        UpdateZoomStatus();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        PopulateTableList(txtSearchTable.Text);
    }

    private void OnTableSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (lstTables.SelectedItem is TableListItem item && _tableNodes.TryGetValue(item.Name, out var tableNode))
        {
            var diagram = diagramView.Diagram;
            diagram.Selection.Change(tableNode);

            diagramView.ScrollTo(new Point(
                tableNode.Bounds.X + tableNode.Bounds.Width / 2 - diagramView.Bounds.Width / 2,
                tableNode.Bounds.Y + tableNode.Bounds.Height / 2 - diagramView.Bounds.Height / 2
            ));

            if (tableNode.Tag is TableSchema ts)
            {
                ShowTableDetails(ts);
            }
        }
    }
}
