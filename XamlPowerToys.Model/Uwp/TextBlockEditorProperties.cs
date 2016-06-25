namespace XamlPowerToys.Model.Uwp {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Uwp.Controls;

    [Serializable]
    public class TextBlockEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBlockEditorProperties() {
            this.TemplateResourceKey = "uwpTextBlockEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBlockFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
