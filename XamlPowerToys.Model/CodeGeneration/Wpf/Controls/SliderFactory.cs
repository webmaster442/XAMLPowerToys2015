namespace XamlPowerToys.Model.CodeGeneration.Wpf.Controls {
    using System;
    using System.Text;
    using XamlPowerToys.Model.CodeGeneration.Infrastructure;
    using XamlPowerToys.Model.Infrastructure;
    using XamlPowerToys.Model.Wpf;

    public class SliderFactory : IControlFactory {

        readonly ControlTemplateModel<SliderEditorProperties> _model;

        public SliderFactory(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            _model = new ControlTemplateModel<SliderEditorProperties>(generateFormModel, propertyInformationViewModel);
        }

        public String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<Slider ");
            if (!String.IsNullOrWhiteSpace(_model.BindingPath)) {
                sb.AppendFormat("Value=\"{0}\" ", Helpers.ConstructBinding(_model.BindingPath, _model.BindingMode, String.Empty));
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.MinimumPathText)) {
                sb.AppendFormat("Minimum=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.MinimumPathText, _model.BindingMode, String.Empty));
            } else {
                sb.Append($"Minimum=\"{_model.EditorProperties.Minimum}\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.MaximumPathText)) {
                sb.AppendFormat("Maximum=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.MaximumPathText, _model.BindingMode, String.Empty));
            } else {
                sb.Append($"Maximum=\"{_model.EditorProperties.Maximum}\" ");
            }

            if (!String.IsNullOrWhiteSpace(_model.EditorProperties.TickFrequencyPathText)) {
                sb.AppendFormat("TickFrequency=\"{0}\" ", Helpers.ConstructBinding(_model.EditorProperties.TickFrequencyPathText, _model.BindingMode, String.Empty));
            } else {
                sb.Append($"TickFrequency=\"{_model.EditorProperties.TickFrequency}\" ");
            }

            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append("TickPlacement=\"BottomRight\" ");
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
