using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using MinApp.Models;
using MindFusion.Diagramming.Avalonia;
using MindFusion.Diagramming.Avalonia.Layout;

namespace MinApp;

public static class LayeredTest
{
    public static void Run()
    {
        var sb = new StringBuilder();
        try
        {
            Program.BuildAvaloniaApp().SetupWithoutStarting();

            var dbPath = DatabaseSchemaLoader.FindDatabasePath();
            var (tables, fks, domains) = DatabaseSchemaLoader.Load(dbPath);

            var diagram = new Diagram();
            diagram.EnableStyledText = true;

            var tableNodes = new Dictionary<string, TableNode>();
            foreach (var table in tables)
            {
                var tn = diagram.Factory.CreateTableNode(0, 0, 195, 32 + table.Columns.Count * 22, 3, table.Columns.Count);
                tn.Caption = table.Name;
                tn.ConnectionStyle = TableConnectionStyle.Rows;
                tableNodes[table.Name] = tn;
            }

            var containers = new List<ContainerNode>();
            foreach (var domain in domains)
            {
                var c = diagram.Factory.CreateContainerNode(0, 0, 300, 300, true);
                c.Caption = domain.Name;
                c.AutoGrow = true;
                foreach (var tName in domain.TableNames)
                {
                    if (tableNodes.TryGetValue(tName, out var tn))
                        c.Add(tn);
                }
                containers.Add(c);
            }

            var links = new List<DiagramLink>();
            foreach (var fk in fks)
            {
                if (tableNodes.TryGetValue(fk.FromTable, out var from) && tableNodes.TryGetValue(fk.ToTable, out var to))
                {
                    var fromT = tables.Find(tbl => tbl.Name == fk.FromTable)!;
                    var toT = tables.Find(tbl => tbl.Name == fk.ToTable)!;
                    int r1 = fromT.Columns.FindIndex(c => c.Name == fk.FromColumn);
                    int r2 = toT.Columns.FindIndex(c => c.Name == fk.ToColumn);
                    var link = from.AddRelation(r1, Relationship.ManyToOne, to, r2);
                    link.Shape = LinkShape.Cascading;
                    link.AutoRoute = true;
                    links.Add(link);
                }
            }

            sb.AppendLine("=== Arranging nodes inside each container with LayeredLayout (Horizontal) ===");
            foreach (var c in containers)
            {
                var containerLayout = new LayeredLayout
                {
                    Orientation = MindFusion.Diagramming.Avalonia.Layout.Orientation.Horizontal,
                    LayerDistance = 60,
                    NodeDistance = 180,
                    Margins = new Size(25, 25)
                };
                bool cRes = c.Arrange(containerLayout);
                c.ResizeToFitChildren(true, new Size(3,3));
                sb.AppendLine($"Container {c.Caption} Arrange result: {cRes}, Bounds: {c.Bounds}");
                foreach (var child in c.Children)
                {
                    if (child is TableNode tn)
                        sb.AppendLine($"  Table {tn.Caption}: {tn.Bounds}");
                }
            }

            sb.AppendLine("\n=== Arranging whole diagram with LayeredLayout (Horizontal) ===");
            var diagramLayout = new LayeredLayout
            {
                Orientation = MindFusion.Diagramming.Avalonia.Layout.Orientation.Horizontal,
                KeepGroupLayout = true,
                LayerDistance = 80,
                NodeDistance = 60,
                Margins = new Size(50, 50)
            };
            bool dRes = diagram.Arrange(diagramLayout);
            sb.AppendLine($"Diagram Arrange result: {dRes}");

            foreach (var c in containers) c.UpdateBounds();

            sb.AppendLine("\n=== FINAL BOUNDS ===");
            foreach (var c in containers)
            {
                sb.AppendLine($"Container {c.Caption}: {c.Bounds}");
            }
            foreach (var t in tableNodes)
            {
                sb.AppendLine($"Table {t.Key}: {t.Value.Bounds}");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"EXCEPTION: {ex}");
        }

        File.WriteAllText("inspect_layered.txt", sb.ToString());
    }
}
