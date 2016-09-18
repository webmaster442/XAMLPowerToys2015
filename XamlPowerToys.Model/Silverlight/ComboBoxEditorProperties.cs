namespace XamlPowerToys.Model.Silverlight {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Silverlight.Controls;

    [Serializable]
    public class ComboBoxEditorProperties : SelectorBase, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public ComboBoxEditorProperties() {
            this.TemplateResourceKey = "silverlightComboBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ComboBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
