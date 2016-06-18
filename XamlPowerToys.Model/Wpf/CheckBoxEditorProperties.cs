namespace XamlPowerToys.Model.Wpf {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Wpf.Controls;

    [Serializable]
    public class CheckBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public CheckBoxEditorProperties() {
            this.TemplateResourceKey = "wpfCheckBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new CheckBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
