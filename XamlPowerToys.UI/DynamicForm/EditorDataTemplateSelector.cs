namespace XamlPowerToys.UI.DynamicForm {
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using XamlPowerToys.Model;
    using XamlPowerToys.UI.Infrastructure;

    public class EditorDataTemplateSelector : DataTemplateSelector {

        public override DataTemplate SelectTemplate(Object item, DependencyObject container) {
            var fwe = container as FrameworkElement;

            if (fwe != null) {
                var lbi = VisualTreeSearchAssistant.FindAncestor<ListBoxItem>(fwe);

                if (lbi != null) {
                    var pi = (PropertyInformationViewModel)lbi.DataContext;

                    PropertyChangedEventHandler lambda = null;
                    lambda = (s, args) => {
                        if (args.PropertyName == "ControlSpecificProperties") {
                            pi.PropertyChanged -= lambda;

                            var cp = (ContentPresenter)container;

                            try {
                                cp.ContentTemplateSelector = null;
                                cp.ContentTemplateSelector = this;
                            } catch {
                                cp.ContentTemplateSelector = this;
                            }
                        }
                    };

                    pi.PropertyChanged += lambda;

                    var editEditor = (IEditEditor)pi.ControlSpecificProperties;
                    return fwe.FindResource(editEditor.TemplateResourceKey) as DataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

    }
}
