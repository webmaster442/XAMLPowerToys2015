namespace XamlPowerToys.UI.DynamicForm {
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public partial class StandardEditor : UserControl {

        public Object ControlSpecificContent {
            get { return GetValue(ControlSpecificContentProperty); }
            set { SetValue(ControlSpecificContentProperty, value); }
        }

        public StandardEditor() {
            InitializeComponent();
        }

        public static readonly DependencyProperty ControlSpecificContentProperty = DependencyProperty.Register("ControlSpecificContent", typeof(Object), typeof(StandardEditor), new UIPropertyMetadata(ControlSpecificContentChanged));

        static void ControlSpecificContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var standardEditor = (StandardEditor)d;
            standardEditor.controlSpecificContent.Content = e.NewValue;
        }

    }
}
