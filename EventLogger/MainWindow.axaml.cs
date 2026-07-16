using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MindFusion.Diagramming.Avalonia;

namespace EventLogger
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var diagram = diagramView.Diagram;

			// Register DiagramView events
			diagramView.PointerPressed += diagram_MouseDown;
			diagramView.PointerReleased += diagram_MouseUp;
			diagramView.PointerMoved += diagram_MouseMove;
			diagramView.PointerEntered += diagram_MouseEnter;
			diagramView.PointerExited += diagram_MouseLeave;
			diagramView.PointerWheelChanged += diagram_MouseWheel;
			diagramView.DoubleTapped += diagram_MouseDoubleClick;
			diagramView.ZoomFactorChanged += diagram_ZoomFactorChanged;

			// Register Diagram events
			diagram.ActionRecorded += diagram_ActionRecorded;
			diagram.ActionRecording += diagram_ActionRecording;
			diagram.ActionRedone += diagram_ActionRedone;
			diagram.ActionUndone += diagram_ActionUndone;
			diagram.BeginLoad += diagram_BeginLoad;
			diagram.BoundsChanged += diagram_BoundsChanged;
			diagram.CellClicked += diagram_CellClicked;
			diagram.CellDoubleClicked += diagram_CellDoubleClicked;
			diagram.CellTextEdited += diagram_CellTextEdited;
			diagram.CellTextEditing += diagram_CellTextEditing;
			diagram.Clicked += diagram_Clicked;
			diagram.ContainerChildAdded += diagram_ContainerChildAdded;
			diagram.ContainerChildRemoved += diagram_ContainerChildRemoved;
			diagram.ContainerFolded += diagram_ContainerFolded;
			diagram.ContainerUnfolded += diagram_ContainerUnfolded;
			diagram.DefaultShapeChanged += diagram_DefaultShapeChanged;
			diagram.DeserializeTag += diagram_DeserializeTag;
			diagram.DestinationAnchorChanged += diagram_DestinationAnchorChanged;
			diagram.DirtyChanged += diagram_DirtyChanged;
			diagram.DoubleClicked += diagram_DoubleClicked;
			diagram.DrawAdjustmentHandles += diagram_DrawAdjustmentHandles;
			diagram.DrawAnchorPoint += diagram_DrawAnchorPoint;
			diagram.DrawCell += diagram_DrawCell;
			//diagram.DrawLaneHeader += diagram_DrawLaneHeader;
			diagram.DrawLink += diagram_DrawLink;
			diagram.DrawNode += diagram_DrawNode;
			diagram.EndLoad += diagram_EndLoad;
			diagram.EnterInplaceEditMode += diagram_EnterInplaceEditMode;
			diagram.ExpandButtonClicked += diagram_ExpandButtonClicked;
			diagram.GroupDestroyed += diagram_GroupDestroyed;
			diagram.HitTestAdjustmentHandles += diagram_HitTestAdjustmentHandles;
			diagram.InitializeLink += diagram_InitializeLink;
			diagram.InitializeNode += diagram_InitializeNode;
			diagram.ItemAdded += diagram_ItemAdded;
			diagram.ItemRemoved += diagram_ItemRemoved;
			diagram.LeaveInplaceEditMode += diagram_LeaveInplaceEditMode;
			diagram.LinkActivated += diagram_LinkActivated;
			diagram.LinkClicked += diagram_LinkClicked;
			diagram.LinkCreated += diagram_LinkCreated;
			diagram.LinkCreating += diagram_LinkCreating;
			diagram.LinkDeactivated += diagram_LinkDeactivated;
			diagram.LinkDeleted += diagram_LinkDeleted;
			diagram.LinkDeleting += diagram_LinkDeleting;
			diagram.LinkDeselected += diagram_LinkDeselected;
			diagram.LinkDoubleClicked += diagram_LinkDoubleClicked;
			diagram.LinkHovered += diagram_LinkHovered;
			diagram.LinkModified += diagram_LinkModified;
			diagram.LinkModifying += diagram_LinkModifying;
			diagram.LinkPasted += diagram_LinkPasted;
			diagram.LinkPointerEnter += diagram_LinkPointerEnter;
			diagram.LinkPointerLeave += diagram_LinkPointerLeave;
			diagram.LinkRouted += diagram_LinkRouted;
			diagram.LinkSelected += diagram_LinkSelected;
			diagram.LinkSelecting += diagram_LinkSelecting;
			diagram.LinkStartModifying += diagram_LinkStartModifying;
			diagram.LinkTextEdited += diagram_LinkTextEdited;
			diagram.LinkTextEditing += diagram_LinkTextEditing;
			diagram.MeasureUnitChanged += diagram_MeasureUnitChanged;
			diagram.NodeActivated += diagram_NodeActivated;
			diagram.NodeClicked += diagram_NodeClicked;
			diagram.NodeCreated += diagram_NodeCreated;
			diagram.NodeCreating += diagram_NodeCreating;
			diagram.NodeDeactivated += diagram_NodeDeactivated;
			diagram.NodeDeleted += diagram_NodeDeleted;
			diagram.NodeDeleting += diagram_NodeDeleting;
			diagram.NodeDeselected += diagram_NodeDeselected;
			diagram.NodeDoubleClicked += diagram_NodeDoubleClicked;
			diagram.NodeHovered += diagram_NodeHovered;
			diagram.NodeModified += diagram_NodeModified;
			diagram.NodeModifying += diagram_NodeModifying;
			diagram.NodePasted += diagram_NodePasted;
			diagram.NodePointerEnter += diagram_NodePointerEnter;
			diagram.NodePointerLeave += diagram_NodePointerLeave;
			diagram.NodeSelected += diagram_NodeSelected;
			diagram.NodeSelecting += diagram_NodeSelecting;
			diagram.NodeStartModifying += diagram_NodeStartModifying;
			diagram.NodeTextEdited += diagram_NodeTextEdited;
			diagram.NodeTextEditing += diagram_NodeTextEditing;
			diagram.OriginAnchorChanged += diagram_OriginAnchorChanged;
			diagram.SelectionChanged += diagram_SelectionChanged;
			diagram.SelectionModified += diagram_SelectionModified;
			diagram.SelectionModifying += diagram_SelectionModifying;
			diagram.SerializeTag += diagram_SerializeTag;
			diagram.TableSectionCollapsed += diagram_TableSectionCollapsed;
			diagram.TableSectionExpanded += diagram_TableSectionExpanded;
			diagram.TreeCollapsed += diagram_TreeCollapsed;
			diagram.TreeExpanded += diagram_TreeExpanded;
			diagram.TreeItemTextEditing += diagram_TreeItemTextEditing;
			diagram.ValidateAnchorPoint += diagram_ValidateAnchorPoint;
		}

		private void diagram_ActionRecorded(object sender, UndoEventArgs e)
		{
			Log("Diagram.ActionRecorded");
		}

		private void diagram_ActionRecording(object sender, UndoValidationEventArgs e)
		{
			Log("Diagram.ActionRecording");
		}

		private void diagram_ActionRedone(object sender, UndoEventArgs e)
		{
			Log("Diagram.ActionRedone");
		}

		private void diagram_ActionUndone(object sender, UndoEventArgs e)
		{
			Log("Diagram.ActionUndone");
		}

		private void diagram_BeginLoad(object sender, EventArgs e)
		{
			Log("Diagram.BeginLoad");
		}

		private void diagram_BoundsChanged(object sender, EventArgs e)
		{
			Log("Diagram.BoundsChanged");
		}

		private void diagram_CellClicked(object sender, CellEventArgs e)
		{
			Log("Diagram.CellClicked");
		}

		private void diagram_CellDoubleClicked(object sender, CellEventArgs e)
		{
			Log("Diagram.CellDoubleClicked");
		}

		private void diagram_CellTextEdited(object sender, EditCellTextEventArgs e)
		{
			Log("Diagram.CellTextEdited");
		}

		private void diagram_CellTextEditing(object sender, CellValidationEventArgs e)
		{
			Log("Diagram.CellTextEditing");
		}

		private void diagram_Clicked(object sender, DiagramEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("Diagram.Clicked");
		}

		private void diagram_ContainerChildAdded(object sender, ContainerChildEventArgs e)
		{
			Log("Diagram.ContainerChildAdded");
		}

		private void diagram_ContainerChildRemoved(object sender, ContainerChildEventArgs e)
		{
			Log("Diagram.ContainerChildRemoved");
		}

		private void diagram_ContainerFolded(object sender, NodeEventArgs e)
		{
			Log("Diagram.ContainerFolded");
		}

		private void diagram_ContainerUnfolded(object sender, NodeEventArgs e)
		{
			Log("Diagram.ContainerUnfolded");
		}

		private void diagram_DefaultShapeChanged(object sender, EventArgs e)
		{
			Log("Diagram.DefaultShapeChanged");
		}

		private void diagram_DeserializeTag(object sender, SerializeTagEventArgs e)
		{
			Log("Diagram.DeserializeTag");
		}

		private void diagram_DestinationAnchorChanged(object sender, AnchorPointEventArgs e)
		{
			Log("Diagram.DestinationAnchorChanged");
		}

		private void diagram_DirtyChanged(object sender, EventArgs e)
		{
			Log("Diagram.DirtyChanged");
		}

		private void diagram_DoubleClicked(object sender, DiagramEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("Diagram.DoubleClicked");
		}

		private void diagram_DrawAdjustmentHandles(object sender, DrawItemEventArgs e)
		{
			if (checkBox2.IsChecked == true)
				Log("Diagram.DrawAdjustmentHandles");
		}

		private void diagram_DrawAnchorPoint(object sender, DrawAnchorPointEventArgs e)
		{
			if (checkBox2.IsChecked == true)
				Log("Diagram.DrawAnchorPoint");
		}

		private void diagram_DrawCell(object sender, DrawCellEventArgs e)
		{
			if (checkBox2.IsChecked == true)
				Log("Diagram.DrawCell");
		}

		/*private void diagram_DrawLaneHeader(object sender, Lanes.DrawHeaderEventArgs e)
		{
			Log("Diagram.DrawLaneHeader");
		}*/

		private void diagram_DrawLink(object sender, DrawLinkEventArgs e)
		{
			if (checkBox2.IsChecked == true)
				Log("Diagram.DrawLink");
		}

		private void diagram_DrawNode(object sender, DrawNodeEventArgs e)
		{
			if (checkBox2.IsChecked == true)
				Log("Diagram.DrawNode");
		}

		private void diagram_EndLoad(object sender, EventArgs e)
		{
			Log("Diagram.EndLoad");
		}

		private void diagram_EnterInplaceEditMode(object sender, InPlaceEditEventArgs e)
		{
			Log("Diagram.EnterInplaceEditMode");
		}

		private void diagram_ExpandButtonClicked(object sender, NodeEventArgs e)
		{
			Log("Diagram.ExpandButtonClicked");
		}

		private void diagram_GroupDestroyed(object sender, GroupEventArgs e)
		{
			Log("Diagram.GroupDestroyed");
		}

		private void diagram_HitTestAdjustmentHandles(object sender, HitTestEventArgs e)
		{
			Log("Diagram.HitTestAdjustmentHandles");
		}

		private void diagram_InitializeLink(object sender, LinkEventArgs e)
		{
			Log("Diagram.InitializeLink");
		}

		private void diagram_InitializeNode(object sender, NodeEventArgs e)
		{
			Log("Diagram.InitializeNode");
		}

		private void diagram_ItemAdded(object sender, ItemEventArgs e)
		{
			Log("Diagram.ItemAdded");
		}

		private void diagram_ItemRemoved(object sender, ItemEventArgs e)
		{
			Log("Diagram.ItemRemoved");
		}

		private void diagram_LeaveInplaceEditMode(object sender, InPlaceEditEventArgs e)
		{
			Log("Diagram.LeaveInplaceEditMode");
		}

		private void diagram_LinkActivated(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkActivated");
		}

		private void diagram_LinkClicked(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkClicked");
		}

		private void diagram_LinkCreated(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkCreated");
		}

		private void diagram_LinkCreating(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.LinkCreating");
		}

		private void diagram_LinkDeactivated(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkDeactivated");
		}

		private void diagram_LinkDeleted(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkDeleted");
		}

		private void diagram_LinkDeleting(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.LinkDeleting");
		}

		private void diagram_LinkDeselected(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkDeselected");
		}

		private void diagram_LinkDoubleClicked(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkDoubleClicked");
		}

		private void diagram_LinkHovered(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkHovered");
		}

		private void diagram_LinkModified(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkModified");
		}

		private void diagram_LinkModifying(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.LinkModifying");
		}

		private void diagram_LinkPasted(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkPasted");
		}

		private void diagram_LinkPointerEnter(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkPointerEnter");
		}

		private void diagram_LinkPointerLeave(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkPointerLeave");
		}

		private void diagram_LinkRouted(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkRouted");
		}

		private void diagram_LinkSelected(object sender, LinkEventArgs e)
		{
			Log("Diagram.LinkSelected");
		}

		private void diagram_LinkSelecting(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.LinkSelecting");
		}

		private void diagram_LinkStartModifying(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.LinkStartModifying");
		}

		private void diagram_LinkTextEdited(object sender, EditLinkTextEventArgs e)
		{
			Log("Diagram.LinkTextEdited");
		}

		private void diagram_LinkTextEditing(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.LinkTextEditing");
		}

		private void diagram_MeasureUnitChanged(object sender, EventArgs e)
		{
			Log("Diagram.MeasureUnitChanged");
		}

		private void diagram_MouseDoubleClick(object sender, TappedEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseDoubleClick");
		}

		private void diagram_MouseDown(object sender, PointerPressedEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseDown");
		}

		private void diagram_MouseEnter(object sender, PointerEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseEnter");
		}

		private void diagram_MouseLeave(object sender, PointerEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseLeave");
		}

		private void diagram_MouseMove(object sender, PointerEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseMove");
		}

		private void diagram_MouseUp(object sender, PointerReleasedEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseUp");
		}

		private void diagram_MouseWheel(object sender, PointerWheelEventArgs e)
		{
			if (checkBox1.IsChecked == true)
				Log("DiagramView.MouseWheel");
		}

		private void diagram_NodeActivated(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeActivated");
		}

		private void diagram_NodeClicked(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeClicked");
		}

		private void diagram_NodeCreated(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeCreated");
		}

		private void diagram_NodeCreating(object sender, NodeValidationEventArgs e)
		{
			Log("Diagram.NodeCreating");
		}

		private void diagram_NodeDeactivated(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeDeactivated");
		}

		private void diagram_NodeDeleted(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeDeleted");
		}

		private void diagram_NodeDeleting(object sender, NodeValidationEventArgs e)
		{
			Log("Diagram.NodeDeleting");
		}

		private void diagram_NodeDeselected(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeDeselected");
		}

		private void diagram_NodeDoubleClicked(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeDoubleClicked");
		}

		private void diagram_NodeHovered(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeHovered");
		}

		private void diagram_NodeModified(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeModified");
		}

		private void diagram_NodeModifying(object sender, NodeValidationEventArgs e)
		{
			Log("Diagram.NodeModifying");
		}

		private void diagram_NodePasted(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodePasted");
		}

		private void diagram_NodePointerEnter(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodePointerEnter");
		}

		private void diagram_NodePointerLeave(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodePointerLeave");
		}

		private void diagram_NodeSelected(object sender, NodeEventArgs e)
		{
			Log("Diagram.NodeSelected");
		}

		private void diagram_NodeSelecting(object sender, NodeValidationEventArgs e)
		{
			Log("Diagram.NodeSelecting");
		}

		private void diagram_NodeStartModifying(object sender, NodeValidationEventArgs e)
		{
			Log("Diagram.NodeStartModifying");
		}

		private void diagram_NodeTextEdited(object sender, EditNodeTextEventArgs e)
		{
			Log("Diagram.NodeTextEdited");
		}

		private void diagram_NodeTextEditing(object sender, NodeValidationEventArgs e)
		{
			Log("Diagram.NodeTextEditing");
		}

		private void diagram_OriginAnchorChanged(object sender, AnchorPointEventArgs e)
		{
			Log("Diagram.OriginAnchorChanged");
		}

		private void diagram_SelectionChanged(object sender, EventArgs e)
		{
			Log("Diagram.SelectionChanged");
		}

		private void diagram_SelectionModified(object sender, EventArgs e)
		{
			Log("Diagram.SelectionModified");
		}

		private void diagram_SelectionModifying(object sender, SelectionValidationEventArgs e1)
		{
			Log("Diagram.SelectionModifying");
		}

		private void diagram_SerializeTag(object sender, SerializeTagEventArgs e)
		{
			Log("Diagram.SerializeTag");
		}

		private void diagram_TableSectionCollapsed(object sender, RowEventArgs e)
		{
			Log("Diagram.TableSectionCollapsed");
		}

		private void diagram_TableSectionExpanded(object sender, RowEventArgs e)
		{
			Log("Diagram.TableSectionExpanded");
		}		

		private void diagram_TreeCollapsed(object sender, NodeEventArgs e)
		{
			Log("Diagram.TreeCollapsed");
		}

		private void diagram_TreeExpanded(object sender, NodeEventArgs e)
		{
			Log("Diagram.TreeExpanded");
		}

		private void diagram_TreeItemTextEditing(object sender, TreeItemValidationEventArgs e)
		{
			Log("Diagram.TreeItemTextEditing");
		}

		private void diagram_ValidateAnchorPoint(object sender, LinkValidationEventArgs e)
		{
			Log("Diagram.ValidateAnchorPoint");
		}

		private void diagram_ZoomFactorChanged(object sender, EventArgs e)
		{
			Log("DiagramView.ZoomFactorChanged");
		}

		private void Log(string message)
		{
			ListBoxItem item = new ListBoxItem();
			item.Content = message;
			listBox1.Items.Add(item);
			listBox1.ScrollIntoView(item);
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			listBox1.Items.Clear();
		}

		private async void OnSave(object sender, RoutedEventArgs e)
		{
			var topLevel = TopLevel.GetTopLevel(this);
			if (topLevel != null)
			{
				var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
				{
					Title = "Save Diagram XML",
					DefaultExtension = "xml",
					ShowOverwritePrompt = true,
					FileTypeChoices = new[] { new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } } }
				});
				if (file != null)
				{
					try
					{
						diagramView.Diagram.SaveToXml(file.Path.LocalPath);
					}
					catch { }
				}
			}
		}

		private async void OnLoad(object sender, RoutedEventArgs e)
		{
			var topLevel = TopLevel.GetTopLevel(this);
			if (topLevel != null)
			{
				var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
				{
					Title = "Open Diagram XML",
					FileTypeFilter = new[] { new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } } }
				});
				if (files.Count > 0)
				{
					try
					{
						diagramView.Diagram.LoadFromXml(files[0].Path.LocalPath);
					}
					catch { }
				}
			}
		}
	}
}