namespace XamlPowerToys.Model.CodeGeneration.Infrastructure {
    using System;
    using System.Text;
    using XamlPowerToys.Model.Infrastructure;

    public class LabelHelper {

        readonly ProjectType _projectType;

        public LabelHelper(ProjectType projectType) {
            _projectType = projectType;
        }

        public String MakeTag(String text, String labelWidthText, Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            if (String.IsNullOrWhiteSpace(text)) {
                return String.Empty;
            }

            var sb = new StringBuilder();
            if (_projectType == ProjectType.Xamarin) {
                sb.Append($"<Label Text=\"{text}\" ");
            } else if (_projectType == ProjectType.Uwp) {
                sb.Append($"<TextBlock Text=\"{text}\" ");
            } else {
                sb.Append($"<Label Content=\"{text}\" ");
            }

            if (parentGridColumn != null && parentGridColumn != 0) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null && parentGridRow != 0) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(labelWidthText);

            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}
