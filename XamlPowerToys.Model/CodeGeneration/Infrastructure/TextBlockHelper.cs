namespace XamlPowerToys.Model.CodeGeneration.Infrastructure {
    using System;
    using System.Text;
    using XamlPowerToys.Model.Infrastructure;

    public class TextBlockHelper {

        readonly ProjectType _projectType;

        public TextBlockHelper(ProjectType projectType) {
            _projectType = projectType;
        }

        public String MakeTag(String text, Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder();
            if (_projectType == ProjectType.Xamarin) {
                sb.Append($"<Label Text=\"{text}\" ");
            } else {
                sb.Append($"<TextBlock Text=\"{text}\" ");
            }
            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}
