namespace XamlPowerToys.UI.DragDrop {
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using XamlPowerToys.Model;
    using XamlPowerToys.Model.Infrastructure;
    using XamlPowerToys.UI.DynamicForm;

    public class DragDropHelper {

        DraggedAdorner _draggedAdorner;
        Object _draggedData;
        readonly DataFormat _format = DataFormats.GetDataFormat("DragDropItemsControl");
        Boolean _hasVerticalOrientation;
        Vector _initialMouseOffset;
        Point _initialMousePosition;
        InsertionAdorner _insertionAdorner;
        Int32 _insertionIndex;
        Boolean _isInFirstHalf;
        FrameworkElement _sourceItemContainer;
        ItemsControl _sourceItemsControl;
        FrameworkElement _targetItemContainer;
        ItemsControl _targetItemsControl;
        Window _topWindow;

        static DragDropHelper Instance => _instance ?? (_instance = new DragDropHelper());

        public static readonly DependencyProperty DragDropTemplateProperty = DependencyProperty.RegisterAttached("DragDropTemplate", typeof(DataTemplate), typeof(DragDropHelper), new UIPropertyMetadata(null));

        public static DataTemplate GetDragDropTemplate(DependencyObject obj) {
            return (DataTemplate)obj.GetValue(DragDropTemplateProperty);
        }

        public static Boolean GetIsDragSource(DependencyObject obj) {
            return (Boolean)obj.GetValue(IsDragSourceProperty);
        }

        public static Boolean GetIsDropTarget(DependencyObject obj) {
            return (Boolean)obj.GetValue(IsDropTargetProperty);
        }

        public static Boolean GetIsMasterListControl(DependencyObject element) {
            return (Boolean)element.GetValue(IsMasterListControlProperty);
        }

        public static Boolean GetIsNonBindingControlsListControl(DependencyObject element) {
            return (Boolean)element.GetValue(IsNonBindingControlsListControlProperty);
        }

        public static readonly DependencyProperty IsMasterListControlProperty = DependencyProperty.RegisterAttached("IsMasterListControl", typeof(Boolean), typeof(DragDropHelper), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsNonBindingControlsListControlProperty = DependencyProperty.RegisterAttached("IsNonBindingControlsListControl", typeof(Boolean), typeof(DragDropHelper), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(Boolean), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));
        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(Boolean), typeof(DragDropHelper), new UIPropertyMetadata(false, IsDropTargetChanged));

        public static void SetDragDropTemplate(DependencyObject obj, DataTemplate value) {
            obj.SetValue(DragDropTemplateProperty, value);
        }

        public static void SetIsDragSource(DependencyObject obj, Boolean value) {
            obj.SetValue(IsDragSourceProperty, value);
        }

        public static void SetIsDropTarget(DependencyObject obj, Boolean value) {
            obj.SetValue(IsDropTargetProperty, value);
        }

        public static void SetIsMasterListControl(DependencyObject element, Boolean value) {
            element.SetValue(IsMasterListControlProperty, value);
        }

        public static void SetIsNonBindingControlsListControl(DependencyObject element, Boolean value) {
            element.SetValue(IsNonBindingControlsListControlProperty, value);
        }

        static DragDropHelper _instance;

        void CreateInsertionAdorner() {
            if (this._targetItemContainer != null) {
                // Here, I need to get adorner layer from targetItemContainer and not targetItemsControl. 
                // This way I get the AdornerLayer within ScrollContentPresenter, and not the one under AdornerDecorator (Snoop is awesome).
                // If I used targetItemsControl, the adorner would hang out of ItemsControl when there's a horizontal scroll bar.
                var adornerLayer = AdornerLayer.GetAdornerLayer(this._targetItemContainer);
                this._insertionAdorner = new InsertionAdorner(this._hasVerticalOrientation, this._isInFirstHalf, this._targetItemContainer, adornerLayer);
            }
        }

        void DecideDropTarget(DragEventArgs e) {
            var targetItemsControlCount = this._targetItemsControl.Items.Count;
            var draggedItem = e.Data.GetData(this._format.Name);

            var propertyInformation = (PropertyInformationViewModel)draggedItem;
            var originalSourceListBox = (ListBox)_sourceItemsControl;
            var dropSourceListBox = (ListBox)e.Source;

            var isOriginalSourceMasterListControl = (Boolean)originalSourceListBox.GetValue(DragDropHelper.IsMasterListControlProperty);
            var isDropSourceMasterListControl = (Boolean)dropSourceListBox.GetValue(DragDropHelper.IsMasterListControlProperty);
            var createFormViewModel = (CreateFormViewModel)dropSourceListBox.DataContext;
            Boolean allowDrop;

            if (isOriginalSourceMasterListControl && isDropSourceMasterListControl) {
                allowDrop = false;
            } else if (isDropSourceMasterListControl && createFormViewModel.ClassName != propertyInformation.ParentClassName) {
                allowDrop = false;
            } else {
                allowDrop = true;
            }

            if (allowDrop && IsDropDataTypeAllowed(draggedItem)) {
                if (targetItemsControlCount > 0) {
                    this._hasVerticalOrientation = Utilities.HasVerticalOrientation(this._targetItemsControl.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement);
                    this._targetItemContainer = _targetItemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;

                    if (this._targetItemContainer != null) {
                        Point positionRelativeToItemContainer = e.GetPosition(this._targetItemContainer);
                        this._isInFirstHalf = Utilities.IsInFirstHalf(this._targetItemContainer, positionRelativeToItemContainer, this._hasVerticalOrientation);
                        this._insertionIndex = this._targetItemsControl.ItemContainerGenerator.IndexFromContainer(this._targetItemContainer);

                        if (!this._isInFirstHalf) {
                            this._insertionIndex++;
                        }
                    } else {
                        this._targetItemContainer = this._targetItemsControl.ItemContainerGenerator.ContainerFromIndex(targetItemsControlCount - 1) as FrameworkElement;
                        this._isInFirstHalf = false;
                        this._insertionIndex = targetItemsControlCount;
                    }
                } else {
                    this._targetItemContainer = null;
                    this._insertionIndex = 0;
                }
            } else {
                this._targetItemContainer = null;
                this._insertionIndex = -1;
                e.Effects = DragDropEffects.None;
            }
        }

        void DragSource_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e) {
            this._sourceItemsControl = (ItemsControl)sender;
            var visual = (Visual)e.OriginalSource;

            this._topWindow = Window.GetWindow(this._sourceItemsControl);
            this._initialMousePosition = e.GetPosition(this._topWindow);

            this._sourceItemContainer = _sourceItemsControl.ContainerFromElement(visual) as FrameworkElement;

            if (this._sourceItemContainer != null) {
                this._draggedData = this._sourceItemContainer.DataContext;
            }
        }

        void DragSource_PreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs e) {
            this._draggedData = null;
        }

        void DragSource_PreviewMouseMove(Object sender, MouseEventArgs e) {
            if (e.OriginalSource is TextBox || e.OriginalSource is TextBlock || e.OriginalSource is CheckBox || e.OriginalSource is Label || e.OriginalSource is RadioButton) {
                return;
            }
            if (this._draggedData != null) {
                // Only drag when user moved the mouse by a reasonable amount.
                if (Utilities.IsMovementBigEnough(this._initialMousePosition, e.GetPosition(this._topWindow))) {
                    this._initialMouseOffset = this._initialMousePosition - this._sourceItemContainer.TranslatePoint(new Point(0, 0), this._topWindow);

                    var data = new DataObject(this._format.Name, this._draggedData);

                    // Adding events to the window to make sure dragged adorner comes up when mouse is not over a drop target.
                    var previousAllowDrop = this._topWindow.AllowDrop;
                    this._topWindow.AllowDrop = true;
                    this._topWindow.DragEnter += TopWindow_DragEnter;
                    this._topWindow.DragOver += TopWindow_DragOver;
                    this._topWindow.DragLeave += TopWindow_DragLeave;

                    DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);

                    // Without this call, there would be a bug in the following scenario: Click on a data item, and drag
                    // the mouse very fast outside of the window. When doing this really fast, for some reason I don't get 
                    // the Window leave event, and the dragged adorner is left behind.
                    // With this call, the dragged adorner will disappear when we release the mouse outside of the window,
                    // which is when the DoDragDrop synchronous method returns.
                    RemoveDraggedAdorner();

                    this._topWindow.AllowDrop = previousAllowDrop;
                    this._topWindow.DragEnter -= TopWindow_DragEnter;
                    this._topWindow.DragOver -= TopWindow_DragOver;
                    this._topWindow.DragLeave -= TopWindow_DragLeave;

                    this._draggedData = null;
                }
            }
        }

        void DropTarget_PreviewDragEnter(Object sender, DragEventArgs e) {
            this._targetItemsControl = (ItemsControl)sender;
            var draggedItem = e.Data.GetData(this._format.Name);

            DecideDropTarget(e);
            if (draggedItem != null) {
                // Dragged Adorner is created on the first enter only.
                ShowDraggedAdorner(e.GetPosition(this._topWindow));
                CreateInsertionAdorner();
            }
            e.Handled = true;
        }

        void DropTarget_PreviewDragLeave(Object sender, DragEventArgs e) {
            // Dragged Adorner is only created once on DragEnter + every time we enter the window. 
            // It's only removed once on the DragDrop, and every time we leave the window. (so no need to remove it here)
            var draggedItem = e.Data.GetData(this._format.Name);

            if (draggedItem != null) {
                RemoveInsertionAdorner();
            }
            e.Handled = true;
        }

        void DropTarget_PreviewDragOver(Object sender, DragEventArgs e) {
            var draggedItem = e.Data.GetData(this._format.Name);

            DecideDropTarget(e);
            if (draggedItem != null) {
                // Dragged Adorner is only updated here - it has already been created in DragEnter.
                ShowDraggedAdorner(e.GetPosition(this._topWindow));
                UpdateInsertionAdornerPosition();
            }
            e.Handled = true;
        }

        void DropTarget_PreviewDrop(Object sender, DragEventArgs e) {
            var draggedItem = e.Data.GetData(this._format.Name);
            var indexRemoved = -1;

            var dropSourceListBox = (ListBox)e.Source;

            var isDropSourceMasterListControl = (Boolean)dropSourceListBox.GetValue(DragDropHelper.IsMasterListControlProperty);
            var isNonBindingControlsListControl = (Boolean)dropSourceListBox.GetValue(DragDropHelper.IsNonBindingControlsListControlProperty);

            var propertyInformation = (PropertyInformationViewModel)draggedItem;
            if (propertyInformation.IsNonBindingControl) {
                propertyInformation = Cloner.DeepCopy<PropertyInformationViewModel>((PropertyInformationViewModel)draggedItem);
            }

            if (draggedItem != null) {
                if ((e.Effects & DragDropEffects.Move) != 0) {
                    if (isNonBindingControlsListControl || isDropSourceMasterListControl || !propertyInformation.IsNonBindingControl) {
                        indexRemoved = Utilities.RemoveItemFromItemsControl(this._sourceItemsControl, draggedItem);
                    }
                }

                // This happens when we drag an item to a later position within the same ItemsControl.
                if (indexRemoved != -1 && this._sourceItemsControl == this._targetItemsControl && indexRemoved < this._insertionIndex) {
                    this._insertionIndex--;
                }

                if (!isNonBindingControlsListControl) {
                    Utilities.InsertItemInItemsControl(this._targetItemsControl, propertyInformation, this._insertionIndex);

                    if (!isDropSourceMasterListControl) {
                    } else {
                        propertyInformation.ResetUserEnteredValues();
                    }
                }

                RemoveDraggedAdorner();
                RemoveInsertionAdorner();
            }
            e.Handled = true;
        }

        static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var dragSource = obj as ItemsControl;
            if (dragSource != null) {
                if (Object.Equals(e.NewValue, true)) {
                    dragSource.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;
                } else {
                    dragSource.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                    dragSource.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                    dragSource.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
                }
            }
        }

        Boolean IsDropDataTypeAllowed(Object draggedItem) {
            // Can the dragged data be added to the destination collection?
            // It can if destination is bound to IList<allowed type>, IList or not data bound.

            Boolean isDropDataTypeAllowed;
            var collectionSource = this._targetItemsControl.ItemsSource;
            if (draggedItem != null) {
                if (collectionSource != null) {
                    var draggedType = draggedItem.GetType();
                    var collectionType = collectionSource.GetType();

                    var genericIListType = collectionType.GetInterface("IList`1");
                    if (genericIListType != null) {
                        var genericArguments = genericIListType.GetGenericArguments();
                        isDropDataTypeAllowed = genericArguments[0].IsAssignableFrom(draggedType);
                    } else if (typeof(IList).IsAssignableFrom(collectionType)) {
                        isDropDataTypeAllowed = true;
                    } else {
                        isDropDataTypeAllowed = false;
                    }
                } else // the ItemsControl's ItemsSource is not data bound.
                {
                    isDropDataTypeAllowed = true;
                }
            } else {
                isDropDataTypeAllowed = false;
            }
            return isDropDataTypeAllowed;
        }

        static void IsDropTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var dropTarget = obj as ItemsControl;
            if (dropTarget != null) {
                if (Object.Equals(e.NewValue, true)) {
                    dropTarget.AllowDrop = true;
                    dropTarget.PreviewDrop += Instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter += Instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver += Instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave += Instance.DropTarget_PreviewDragLeave;
                } else {
                    dropTarget.AllowDrop = false;
                    dropTarget.PreviewDrop -= Instance.DropTarget_PreviewDrop;
                    dropTarget.PreviewDragEnter -= Instance.DropTarget_PreviewDragEnter;
                    dropTarget.PreviewDragOver -= Instance.DropTarget_PreviewDragOver;
                    dropTarget.PreviewDragLeave -= Instance.DropTarget_PreviewDragLeave;
                }
            }
        }

        void RemoveDraggedAdorner() {
            if (this._draggedAdorner != null) {
                this._draggedAdorner.Detach();
                this._draggedAdorner = null;
            }
        }

        void RemoveInsertionAdorner() {
            if (this._insertionAdorner != null) {
                this._insertionAdorner.Detach();
                this._insertionAdorner = null;
            }
        }

        void ShowDraggedAdorner(Point currentPosition) {
            if (this._draggedAdorner == null) {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this._sourceItemsControl);
                this._draggedAdorner = new DraggedAdorner(this._draggedData, GetDragDropTemplate(this._sourceItemsControl), this._sourceItemContainer, adornerLayer);
            }
            this._draggedAdorner.SetPosition(currentPosition.X - this._initialMousePosition.X + this._initialMouseOffset.X, currentPosition.Y - this._initialMousePosition.Y + this._initialMouseOffset.Y);
        }

        void TopWindow_DragEnter(Object sender, DragEventArgs e) {
            ShowDraggedAdorner(e.GetPosition(this._topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        void TopWindow_DragLeave(Object sender, DragEventArgs e) {
            RemoveDraggedAdorner();
            e.Handled = true;
        }

        void TopWindow_DragOver(Object sender, DragEventArgs e) {
            ShowDraggedAdorner(e.GetPosition(this._topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        void UpdateInsertionAdornerPosition() {
            if (this._insertionAdorner != null) {
                this._insertionAdorner.IsInFirstHalf = this._isInFirstHalf;
                this._insertionAdorner.InvalidateVisual();
            }
        }

    }
}
