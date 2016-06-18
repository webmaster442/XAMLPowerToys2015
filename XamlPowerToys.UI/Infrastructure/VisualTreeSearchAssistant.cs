namespace XamlPowerToys.UI.Infrastructure {
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents VisualTreeSearchAssistant
    /// </summary>
    public static class VisualTreeSearchAssistant {
        /// <summary>
        /// Initializes the <see cref="VisualTreeSearchAssistant"/> class.
        /// </summary>
        static VisualTreeSearchAssistant() {
        }

        /// <summary>
        /// Finds the ancestor.
        /// </summary>
        /// <typeparam name="TParentItem">Type of parent object to match.</typeparam>
        /// <param name="dependencyObject">The child dependency object to start the search up the visual tree from.</param>
        /// <returns>If parent object of type <typeparamref name="TParentItem"/> is found, that object will be returned; otherwise, null (default(TParentItem));</returns>
        public static TParentItem FindAncestor<TParentItem>(DependencyObject dependencyObject) where TParentItem : DependencyObject {
            while ((dependencyObject) != null) {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);

                if (dependencyObject is TParentItem) {
                    return dependencyObject as TParentItem;
                }
            }
            return default(TParentItem);
        }

        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="TChildItem">The type child object to match.</typeparam>
        /// <param name="dependencyObject">The parent dependency object to start the search down the visual tree from.</param>
        /// <returns>If child object of type <typeparamref name="TChildItem"/> is found, that object will be returned; otherwise, null (default(TChildItem));</returns>
        public static TChildItem FindVisualChild<TChildItem>(DependencyObject dependencyObject) where TChildItem : DependencyObject {
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++) {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                if (child is TChildItem) {
                    return (TChildItem)child;
                }
                var childOfChild = FindVisualChild<TChildItem>(child);

                if (childOfChild != null) {
                    return childOfChild;
                }
            }

            return default(TChildItem);
        }

        /// <summary>
        /// Finds the visual children.
        /// </summary>
        /// <typeparam name="T">The <c>Type</c> child object to match.</typeparam>
        /// <param name="dependencyObject">The parent dependency object to start the search down the visual tree from.</param>
        /// <returns>Returns IEnumerable of dependency objects of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject dependencyObject) where T : DependencyObject {
            if (dependencyObject != null) {
                for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
                    if (child is T) {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}