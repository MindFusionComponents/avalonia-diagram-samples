using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Microsoft.Data.Sqlite;

namespace MinApp.Models;

public record ColumnSchema(string Name, string DataType, bool IsPrimaryKey, bool IsForeignKey, string? RefTable = null, string? RefColumn = null);

public record ForeignKeySchema(string FromTable, string FromColumn, string ToTable, string ToColumn);

public record TableSchema(string Name, List<ColumnSchema> Columns, string Domain);

public record DomainGroup(string Name, Color Color, List<string> TableNames);

public static class DatabaseSchemaLoader
{
    public static string FindDatabasePath()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "shop.db"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "shop.db"),
            Path.Combine(Directory.GetCurrentDirectory(), "MinApp", "shop.db"),
            Path.Combine(Directory.GetCurrentDirectory(), "shop.db"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MinApp", "shop.db"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "shop.db"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "shop.db"),
            @"D:\llms\MinApp\shop.db",
            @"D:\llms\shop.db"
        };

        foreach (var c in candidates)
        {
            var full = Path.GetFullPath(c);
            if (File.Exists(full))
                return full;
        }

        return "shop.db";
    }

    public static (List<TableSchema> Tables, List<ForeignKeySchema> ForeignKeys, List<DomainGroup> Domains) Load(string dbPath)
    {
        var tables = new List<string>();
        var foreignKeys = new List<ForeignKeySchema>();
        var tableDict = new Dictionary<string, List<ColumnSchema>>(StringComparer.OrdinalIgnoreCase);

        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            // 1. Fetch tables
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }

            // 2. Fetch foreign keys for all tables
            foreach (var table in tables)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"PRAGMA foreign_key_list(\"{table}\");";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var toTable = reader.GetString(2);
                    var fromCol = reader.GetString(3);
                    var toCol = reader.GetString(4);
                    foreignKeys.Add(new ForeignKeySchema(table, fromCol, toTable, toCol));
                }
            }

            // 3. Fetch columns for each table
            foreach (var table in tables)
            {
                var cols = new List<ColumnSchema>();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"PRAGMA table_info(\"{table}\");";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var colName = reader.GetString(1);
                    var colType = reader.GetString(2);
                    var isPk = reader.GetInt32(5) > 0;

                    var matchingFk = foreignKeys.Find(fk =>
                        string.Equals(fk.FromTable, table, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(fk.FromColumn, colName, StringComparison.OrdinalIgnoreCase));

                    cols.Add(new ColumnSchema(
                        colName,
                        string.IsNullOrWhiteSpace(colType) ? "TEXT" : colType.ToUpperInvariant(),
                        isPk,
                        matchingFk != null,
                        matchingFk?.ToTable,
                        matchingFk?.ToColumn
                    ));
                }
                tableDict[table] = cols;
            }
        }

        // Domain classification
        var domains = new List<DomainGroup>
        {
            new("Sales", Color.FromRgb(16, 185, 129), new List<string> { "Orders", "OrderItems", "Payments", "Shipments" }),
            new("Customers", Color.FromRgb(59, 130, 246), new List<string> { "Users", "Roles", "UserRoles", "Addresses" }),
            new("Catalog", Color.FromRgb(245, 158, 11), new List<string> { "Products", "Categories", "Suppliers" }),
            new("Inventory", Color.FromRgb(139, 92, 246), new List<string> { "Warehouses", "StockLevels" })
        };

        var tableSchemas = new List<TableSchema>();
        foreach (var tableName in tables)
        {
            var domain = domains.Find(d => d.TableNames.Exists(t => string.Equals(t, tableName, StringComparison.OrdinalIgnoreCase)))?.Name ?? "General";
            tableSchemas.Add(new TableSchema(tableName, tableDict[tableName], domain));
        }

        return (tableSchemas, foreignKeys, domains);
    }
}
