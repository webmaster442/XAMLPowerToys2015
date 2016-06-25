namespace XamlPowerToys.Model.Uwp {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Uwp.Controls;

    [Serializable]
    public class DatePickerEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public DatePickerEditorProperties() {
            this.TemplateResourceKey = "uwpDatePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new DatePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
