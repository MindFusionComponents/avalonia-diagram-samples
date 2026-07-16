using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using MindFusion.Diagramming.Avalonia;

namespace SpanningCells
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;
			diagram.LinkHeadShapeSize = 8;

			diagram.BackBrush = new SolidColorBrush(Color.Parse("#e0e9e9"));
			diagram.FontFamily = "Open Sans";
			diagram.FontSize = 10;

			// create first table
			var table1 = diagram.Factory.CreateTableNode(80, 80, 240, 160);
			table1.RedimTable(2, 6);
			table1.Columns[1].ColumnStyle = ColumnStyle.AutoWidth;
			table1.Text = "Preparation";
			table1.CaptionBackBrush = new SolidColorBrush(Color.Parse("#9caac6"));

			var cell = table1[0, 0];
			cell.ColumnSpan = 2;
			cell.Text = "Calculate Price";
			cell.TextAlignment = TextAlignment.Center;
			cell.TextBrush = new SolidColorBrush(Color.Parse("#2d3956"));
			cell.Brush = new SolidColorBrush(Color.Parse("#9caac6"));

			cell = table1[0, 3];
			cell.ColumnSpan = 2;
			cell.Text = "Print Papers";
			cell.TextAlignment = TextAlignment.Center;
			cell.TextBrush = new SolidColorBrush(Color.Parse("#2d3956"));
			cell.Brush = new SolidColorBrush(Color.Parse("#9caac6"));

			cell = table1[0, 1];
			cell.RowSpan = 2;
			cell.Image = LoadImage("icon_calculator.png");
			cell = table1[0, 4];
			cell.RowSpan = 2;
			cell.Image = LoadImage("icon_print.png");

			string[] preparation_tasks = new string[] { "Distance", "Cargo Weight", "Invoice", "Track Number" };

			for (var i = 1; i <= 4; i++)
			{
				var row = i <= 2 ? i : i + 1;
				cell = table1[1, row];
				cell.Text = preparation_tasks[i - 1];
				cell.TextAlignment = TextAlignment.Right;
			}

			// create second table
			var table2 = diagram.Factory.CreateTableNode(460, 140, 210, 120);
			table2.RedimTable(3, 4);
			table2.Columns[2].ColumnStyle = ColumnStyle.AutoWidth;
			table2.Text = "Spedition";
			table2.CaptionBackBrush = new SolidColorBrush(Colors.Black);
			table2.TextBrush = new SolidColorBrush(Colors.White);

			cell = table2[1, 0];
			cell.RowSpan = 3;
			cell.ColumnSpan = 2;
			cell.Image = LoadImage("icon_delivery.png");

			string[] delivery_tasks = new string[] { "route", "fuel", "driver_id", "" };
			for (var i = 0; i < 4; i++)
			{
				cell = table2[0, i];
				cell.Text = delivery_tasks[i];
				cell.TextBrush = new SolidColorBrush(Color.Parse("#003466"));
			}

			string[] colors = new string[] { "#000063", "#ce0000", "#5a79a5" };
			for (var i = 1; i < 4; i++)
			{
				cell = table2[i - 1, 3];
				cell.Text = "District " + i;
				cell.TextBrush = new SolidColorBrush(Color.Parse("#e0e9e9"));
				cell.Brush = new SolidColorBrush(Color.Parse(colors[i - 1]));
			}

			// create third table
			var table3 = diagram.Factory.CreateTableNode(100, 300, 280, 120);
			table3.RedimTable(4, 4);
			table3.Text = "Delivery";
			table3.CaptionBackBrush = new SolidColorBrush(Color.Parse("#ce0000"));
			table3.TextBrush = new SolidColorBrush(Color.Parse("#e0e9e9"));

			cell = table3[0, 0];
			cell.ColumnSpan = 2;
			cell.Text = "Destination";
			cell.FontWeight = FontWeight.Bold;
			cell.TextAlignment = TextAlignment.Center;
			cell.TextBrush = new SolidColorBrush(Color.Parse("#000063"));
			cell.Brush = new SolidColorBrush(Color.Parse("#c0c0c0"));

			cell = table3[0, 1];
			cell.RowSpan = 3;
			cell.Image = LoadImage("icon_home.png");

			delivery_tasks = new string[] { "address", "to door", "" };
			for (var i = 1; i < 3; i++)
			{
				cell = table3[1, i];
				cell.Text = delivery_tasks[i - 1];
				cell.TextBrush = new SolidColorBrush(Color.Parse("#003466"));

				if (i == 2)
					cell.RowSpan = 2;
			}

			cell = table3[2, 0];
			cell.ColumnSpan = 2;
			cell.Text = "Arrival";
			cell.FontWeight = FontWeight.Bold;
			cell.TextAlignment = TextAlignment.Center;
			cell.TextBrush = new SolidColorBrush(Color.Parse("#e0e9e9"));
			cell.Brush = new SolidColorBrush(Color.Parse("#003466"));

			cell = table3[2, 1];
			cell.RowSpan = 3;
			cell.Brush = new SolidColorBrush(Color.Parse("#9caac6"));
			cell.Image = LoadImage("icon_time.png");

			delivery_tasks = new string[] { "time", "receiver", "charges" };
			for (var i = 1; i < 4; i++)
			{
				cell = table3[3, i];
				cell.Text = delivery_tasks[i - 1];
				cell.TextBrush = new SolidColorBrush(Color.Parse("#003466"));
				cell.Brush = new SolidColorBrush(Color.Parse("#9caac6"));
			}

			// draw link from "item 1" in first table to "row 0" in second table
			var link = diagram.Factory.CreateDiagramLink(table1, table2);
			link.OriginIndex = 1;
			link.DestinationIndex = 0;

			link = diagram.Factory.CreateDiagramLink(table2, table3);
			link.OriginIndex = 2;
			link.Dynamic = true;
			link.DestinationIndex = 1;
		}

		private IImage? LoadImage(string imageName)
		{
			try
			{
				var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", imageName);
				if (System.IO.File.Exists(path))
				{
					return new Avalonia.Media.Imaging.Bitmap(path);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}