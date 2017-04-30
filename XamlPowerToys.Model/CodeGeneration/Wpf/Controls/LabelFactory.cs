namespace XamlPowerToys.Model.CodeGeneration.Wpf.Controls {
    using System;
    using System.Text;
    using XamlPowerToys.Model.CodeGeneration.Infrastructure;
    using XamlPowerToys.Model.Infrastructure;
    using XamlPowerToys.Model.Wpf;

    public class LabelFactory : IControlFactory {

        readonly ControlTemplateModel<LabelEditorProperties> _model;

        public LabelFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<LabelEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<Label ");
            if (!_model.IsNonBindingControl && !String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Content=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, BindingMode.OneWay, _model.StringFormatText, _model.BindingConverter));
            } else if (_model.IsNonBindingControl && !String.IsNullOrWhiteSpace(_model.EditorProperties.LabelText)) {
                sb.Append($"Content=\"{System.Security.SecurityElement.Escape(_model.EditorProperties.LabelText)}\" ");
            }

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(_model.WidthText);
            sb.Append(_model.HeightText);
            sb.Append(_model.HorizontalAlignmentText);
            sb.Append(_model.VerticalAlignmentText);
            sb.Append(_model.ControlNameText);
            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}