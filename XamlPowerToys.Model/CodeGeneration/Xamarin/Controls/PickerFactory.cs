namespace XamlPowerToys.Model.CodeGeneration.Xamarin.Controls {
    using System;
    using System.Text;
    using XamlPowerToys.Model.CodeGeneration.Infrastructure;
    using XamlPowerToys.Model.Infrastructure;
    using XamlPowerToys.Model.XamarinForms;

    public class PickerFactory : IControlFactory {

        readonly ControlTemplateModel<PickerEditorProperties> _model;

        public PickerFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<PickerEditorProperties>(generateFormModel, propertyInformationViewModel);
        }


        public string MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<Picker ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("SelectedIndex=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, _model.StringFormatText));
            }

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append(_model.EditorProperties.TitleText);
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