namespace XamlPowerToys.UI.DragDrop {
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using XamlPowerToys.Model;

    /// <summary>
    /// Represents a Utilities
    /// </summary>
    public static class Utilities {

        // Finds the orientation of the panel of the ItemsControl that contains the itemContainer passed as a parameter.
        // The orientation is needed to figure out where to draw the adorner that indicates where the item will be dropped.
        /// <summary>
        /// Determines whether [has vertical orientation] [the specified item container].
        /// </summary>
        /// <param name="itemContainer">The item container.</param>
        /// <returns><c>true</c> if [has vertical orientation] [the specified item container]; otherwise, <c>false</c>.</returns>
        public static Boolean HasVerticalOrientation(FrameworkElement itemContainer) {
            var hasVerticalOrientation = true;
            if (itemContainer != null) {
                var panel = VisualTreeHelper.GetParent(itemContainer) as Panel;
                StackPanel stackPanel;
                WrapPanel wrapPanel;

                if ((stackPanel = panel as StackPanel) != null) {
                    hasVerticalOrientation = (stackPanel.Orientation == Orientation.Vertical);
                } else if ((wrapPanel = panel as WrapPanel) != null) {
                    hasVerticalOrientation = (wrapPanel.Orientation == Orientation.Vertical);
                }

                // You can add support for more panel types here.
            }
            return hasVerticalOrientation;
        }

        /// <summary>
        /// Inserts the item in items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemToInsert">The item to insert.</param>
        /// <param name="insertionIndex">Index of the insertion.</param>
        public static void InsertItemInItemsControl(ItemsControl itemsControl, PropertyInformationViewModel itemToInsert, Int32 insertionIndex) {
            if (itemToInsert != null) {
                var itemsSource = itemsControl.ItemsSource;

                if (itemsSource == null) {
                    if (itemsControl.Items.Contains(itemToInsert)) {
                        return;
                    }

                    itemsControl.Items.Insert(insertionIndex, itemToInsert);
                }

                // Is the ItemsSource IList or IList<T>? If so, insert the dragged item in the list.
                else if (itemsSource is IList) {
                    var list = (IList)itemsSource;
                    if (list.Contains(itemToInsert)) {
                        return;
                    }
                    ((IList)itemsSource).Insert(insertionIndex, itemToInsert);
                } else {
                    var type = itemsSource.GetType();
                    var genericIListType = type.GetInterface("IList`1");
                    if (genericIListType != null) {
                        var result = type.GetMethod("Contains").Invoke(itemsSource, new[] {(Object)itemToInsert});
                        if ((Boolean)result) {
                            return;
                        }

                        type.GetMethod("Insert").Invoke(itemsSource, new[] {insertionIndex, (Object)itemToInsert});
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is in first half] [the specified container].
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="clickedPoint">The clicked point.</param>
        /// <param name="hasVerticalOrientation">if set to <c>true</c> [has vertical orientation].</param>
        /// <returns><c>true</c> if [is in first half] [the specified container]; otherwise, <c>false</c>.</returns>
        public static Boolean IsInFirstHalf(FrameworkElement container, Point clickedPoint, Boolean hasVerticalOrientation) {
            if (hasVerticalOrientation) {
                return clickedPoint.Y < container.ActualHeight / 2;
            }
            return clickedPoint.X < container.ActualWidth / 2;
        }

        /// <summary>
        /// Determines whether [is movement big enough] [the specified initial mouse position].
        /// </summary>
        /// <param name="initialMousePosition">The initial mouse position.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <returns><c>true</c> if [is movement big enough] [the specified initial mouse position]; otherwise, <c>false</c>.</returns>
        public static Boolean IsMovementBigEnough(Point initialMousePosition, Point currentPosition) {
            return Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                   Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance;
        }

        /// <summary>
        /// Removes the item from items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemToRemove">The item to remove.</param>
        /// <returns>Index of item to be removed</returns>
        public static Int32 RemoveItemFromItemsControl(ItemsControl itemsControl, Object itemToRemove) {
            var indexToBeRemoved = -1;
            if (itemToRemove != null) {
                indexToBeRemoved = itemsControl.Items.IndexOf(itemToRemove);

                if (indexToBeRemoved != -1) {
                    var itemsSource = itemsControl.ItemsSource;
                    if (itemsSource == null) {
                        itemsControl.Items.RemoveAt(indexToBeRemoved);
                    }

                    // Is the ItemsSource IList or IList<T>? If so, remove the item from the list.
                    else if (itemsSource is IList) {
                        ((IList)itemsSource).RemoveAt(indexToBeRemoved);
                    } else {
                        var type = itemsSource.GetType();
                        var genericIListType = type.GetInterface("IList`1");
                        if (genericIListType != null) {
                            type.GetMethod("RemoveAt").Invoke(itemsSource, new Object[] {indexToBeRemoved});
                        }
                    }
                }
            }
            return indexToBeRemoved;
        }

    }
}
