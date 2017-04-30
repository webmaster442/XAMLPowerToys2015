namespace XamlPowerToys.UI.DynamicForm {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using XamlPowerToys.Model;
    using XamlPowerToys.UI.DragDrop;
    using XamlPowerToys.UI.Infrastructure;

    public partial class CreateFormView : UserControl {

        Int32 _currentColumnGroupCount;
        CreateFormViewModel _viewModel;

        public CreateFormView() {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler(TextBoxGotFocus), true);
        }

        void AddNewColumns(Int32 requestedColumnGroupCount) {
            for (Int32 i = _currentColumnGroupCount; i < requestedColumnGroupCount; i++) {
                if (_currentColumnGroupCount == 0) {
                    this.gridColumnGroupsContainer.ColumnDefinitions.Insert(0, new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star), MinWidth = 50});
                    this.gridColumnGroupsContainer.Children.Add(ListBoxFactory(0));
                } else {
                    this.gridColumnGroupsContainer.ColumnDefinitions.Insert(this.gridColumnGroupsContainer.ColumnDefinitions.Count, new ColumnDefinition {Width = new GridLength(8)});
                    this.gridColumnGroupsContainer.ColumnDefinitions.Insert(this.gridColumnGroupsContainer.ColumnDefinitions.Count, new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star), MinWidth = 50});
                    this.gridColumnGroupsContainer.Children.Add(GridSplitterFactory(this.gridColumnGroupsContainer.ColumnDefinitions.Count - 2));
                    this.gridColumnGroupsContainer.Children.Add(ListBoxFactory(this.gridColumnGroupsContainer.ColumnDefinitions.Count - 1));
                }
                _currentColumnGroupCount += 1;
            }
        }

        void BtnApplyUI_OnClick(Object sender, RoutedEventArgs e) {
            GenerateUI(SelectedAction.Apply);
        }

        void BtnCancel_OnClick(Object sender, RoutedEventArgs e) {
            Close();
        }

        void BtnGenerateUI_OnClick(Object sender, RoutedEventArgs e) {
            GenerateUI(SelectedAction.Generate);
        }

        void Close() {
            var win = VisualTreeSearchAssistant.FindAncestor<Window>(this);
            win?.Close();
        }

        void CloseWindow(Object sender, EventArgs eventArgs) {
            Close();
        }

        void ConfigureColumnGroups(Int32 requestedColumnGroupCount) {
            if (requestedColumnGroupCount > _currentColumnGroupCount) {
                AddNewColumns(requestedColumnGroupCount);
            } else if (requestedColumnGroupCount < _currentColumnGroupCount) {
                RemoveColumns(requestedColumnGroupCount);
            }
        }

        void CreateFormView_OnLoaded(Object sender, RoutedEventArgs e) {
            _viewModel = (CreateFormViewModel)this.DataContext;
            _viewModel.CloseWindow += CloseWindow;
            ConfigureColumnGroups(1);
            Keyboard.Focus(this);
            this.btnApplyUI.Visibility = _viewModel.IsXamarinFormsProject ? Visibility.Visible : Visibility.Collapsed;
        }

        void GenerateUI(SelectedAction selectedAction) {
            var listBoxes = new List<ListBox>();
            foreach (var child in this.gridColumnGroupsContainer.Children) {
                var listBox = child as ListBox;
                if (listBox != null) {
                    listBoxes.Add(listBox);
                }
            }

            List<PropertyInformationViewModel> columnZeroItems = null;
            List<PropertyInformationViewModel> columnOneItems = null;
            List<PropertyInformationViewModel> columnTwoItems = null;

            if (listBoxes.Count > 0) {
                columnZeroItems = listBoxes[0].Items.OfType<PropertyInformationViewModel>().ToList();
            }
            if (listBoxes.Count > 1) {
                columnOneItems = listBoxes[1].Items.OfType<PropertyInformationViewModel>().ToList();
            }
            if (listBoxes.Count > 2) {
                columnTwoItems = listBoxes[2].Items.OfType<PropertyInformationViewModel>().ToList();
            }

            if (columnZeroItems == null || columnZeroItems.Count == 0) {
                MessageBox.Show("You need to add at least one control to the design surface.", "No Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _viewModel.GenerateUI(selectedAction, columnZeroItems, columnOneItems, columnTwoItems);
        }

        GridSplitter GridSplitterFactory(Int32 gridColumnIndex) {
            var gs = new GridSplitter();
            gs.SetValue(Grid.ColumnProperty, gridColumnIndex);
            return gs;
        }

        ListBox ListBoxFactory(Int32 gridColumnIndex) {
            var lb = new ListBox {HorizontalContentAlignment = HorizontalAlignment.Stretch, BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.LightGray), Margin = new Thickness(3)};
            lb.SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Continue);
            lb.SetValue(DragDropHelper.IsDragSourceProperty, true);
            lb.SetValue(DragDropHelper.IsDropTargetProperty, true);
            lb.SetValue(DragDropHelper.IsMasterListControlProperty, false);
            lb.Tag = "dropUnboundControlsHere";
            lb.SetValue(DragDropHelper.DragDropTemplateProperty, this.FindResource("fieldsListDragDropDataTemplate"));
            lb.SetValue(ItemsControl.ItemTemplateSelectorProperty, this.FindResource("editorDataTemplateSelector"));
            lb.SetValue(Grid.ColumnProperty, gridColumnIndex);
            return lb;
        }

        void RemoveColumns(Int32 requestedColumnGroupCount) {
            var lastColumnIndexToKeep = (requestedColumnGroupCount * 2) - 1;
            var listOfGridSplittersToRemove = new List<GridSplitter>();
            var listOfListBoxesToRemove = new List<ListBox>();

            foreach (var child in this.gridColumnGroupsContainer.Children) {
                if (child is GridSplitter) {
                    var gs = (GridSplitter)child;
                    var column = (Int32)gs.GetValue(Grid.ColumnProperty);
                    if (column >= lastColumnIndexToKeep) {
                        listOfGridSplittersToRemove.Add(gs);
                    }
                } else if (child is ListBox) {
                    var lb = (ListBox)child;
                    var column = (Int32)lb.GetValue(Grid.ColumnProperty);
                    if (column > lastColumnIndexToKeep) {
                        if (lb.Items != null && lb.Items.Count > 0) {
                            MessageBox.Show("Can't remove a column with added controls.  Please remove the controls and retry.", "Invalid Action", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        listOfListBoxesToRemove.Add(lb);
                    }
                }
            }

            foreach (var gridSplitter in listOfGridSplittersToRemove) {
                this.gridColumnGroupsContainer.Children.Remove(gridSplitter);
            }

            foreach (var listBox in listOfListBoxesToRemove) {
                this.gridColumnGroupsContainer.Children.Remove(listBox);
            }

            for (var i = this.gridColumnGroupsContainer.ColumnDefinitions.Count - 1; i >= lastColumnIndexToKeep; i--) {
                this.gridColumnGroupsContainer.ColumnDefinitions.RemoveAt(i);
            }

            _currentColumnGroupCount = requestedColumnGroupCount;
        }

        void TextBoxGotFocus(Object sender, RoutedEventArgs e) {
            var tb = (TextBox)sender;
            tb.SelectAll();
        }

        void TxtColumnCount_OnKeyDown(Object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab) {
                ValidateColumnCountAndConfigureColumns();
            }
        }

        void ValidateColumnCountAndConfigureColumns() {
            if (String.IsNullOrWhiteSpace(this.txtColumnCount.Text)) {
                return;
            }
            Int32 count;
            if (Int32.TryParse(this.txtColumnCount.Text, out count) && count > 0 && count < 4) {
                ConfigureColumnGroups(count);
            } else {
                MessageBox.Show("Must enter an integer value between 1 and 3", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

    }
}
