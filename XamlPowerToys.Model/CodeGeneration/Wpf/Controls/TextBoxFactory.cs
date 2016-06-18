namespace XamlPowerToys.Model.CodeGeneration.Wpf.Controls {
    using System;
    using System.Text;
    using XamlPowerToys.Model.CodeGeneration.Infrastructure;
    using XamlPowerToys.Model.Infrastructure;
    using XamlPowerToys.Model.Wpf;

    public class TextBoxFactory : IControlFactory {

        readonly ControlTemplateModel<TextBoxEditorProperties> _model;

        public TextBoxFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<TextBoxEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<TextBox ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Text=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, _model.StringFormatText, IncludeValidationAttributes.Yes));
            }

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(_model.WidthText);
            sb.Append(_model.HeightText);
            sb.Append(_model.ControlNameText);
            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}
