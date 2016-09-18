namespace XamlPowerToys.Model.Silverlight {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Silverlight.Controls;

    [Serializable]
    public class TextBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBoxEditorProperties() {
            this.TemplateResourceKey = "silverlightTextBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
