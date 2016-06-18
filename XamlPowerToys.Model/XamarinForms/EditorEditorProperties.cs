namespace XamlPowerToys.Model.XamarinForms {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Xamarin.Controls;

    [Serializable]
    public class EditorEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public EditorEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsEditorEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new EditorFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
