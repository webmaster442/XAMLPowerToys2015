namespace XamlPowerToys.UI.DynamicForm {
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public partial class StandardEditorHeader : UserControl {

        public String EditorBindingTargetProperty {
            get { return (String)GetValue(EditorBindingTargetPropertyProperty); }
            set { SetValue(EditorBindingTargetPropertyProperty, value); }
        }

        public StandardEditorHeader() {
            InitializeComponent();
        }

        public static readonly DependencyProperty EditorBindingTargetPropertyProperty = DependencyProperty.Register("EditorBindingTargetProperty", typeof(String), typeof(StandardEditorHeader), new UIPropertyMetadata(EditorBindingTargetPropertyChanged));

        static void EditorBindingTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var uc = (StandardEditorHeader)d;
            uc.tbEditorTargetProperty.Text = $"(Binds to the {e.NewValue} property.)";
        }

    }
}
