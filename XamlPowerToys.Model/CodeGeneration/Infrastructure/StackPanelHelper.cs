namespace XamlPowerToys.Model.CodeGeneration.Infrastructure {
    using System;
    using System.Text;
    using System.Windows.Controls;

    public class StackPanelHelper {

        readonly String _elementType = "StackPanel";

        public StackPanelHelper(ProjectType projectType) {
            if (projectType == ProjectType.Xamarin) {
                _elementType = "StackLayout";
            }
        }

        public String EndTag() {
            return $"</{_elementType}>";
        }

        public String StartTag(Orientation orientation, Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder($"<{_elementType} Orientation=\"{orientation}\" ");
            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }
            sb.AppendLine(">");

            return sb.ToString();
        }

    }
}
