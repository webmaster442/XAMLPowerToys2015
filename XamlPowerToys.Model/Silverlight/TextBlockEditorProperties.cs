namespace XamlPowerToys.Model.Silverlight {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Silverlight.Controls;

    [Serializable]
    public class TextBlockEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBlockEditorProperties() {
            this.TemplateResourceKey = "silverlightTextBlockEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBlockFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
