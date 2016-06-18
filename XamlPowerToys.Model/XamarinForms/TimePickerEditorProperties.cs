namespace XamlPowerToys.Model.XamarinForms {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Xamarin.Controls;

    [Serializable]
    public class TimePickerEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TimePickerEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsTimePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TimePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
