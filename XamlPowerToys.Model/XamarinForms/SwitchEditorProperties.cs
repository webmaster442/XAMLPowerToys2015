namespace XamlPowerToys.Model.XamarinForms {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Xamarin.Controls;

    [Serializable]
    public class SwitchEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public SwitchEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsSwitchEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new SwitcherFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
