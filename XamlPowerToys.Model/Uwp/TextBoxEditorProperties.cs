namespace XamlPowerToys.Model.Uwp {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Uwp.Controls;

    [Serializable]
    public class TextBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBoxEditorProperties() {
            this.TemplateResourceKey = "uwpTextBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
