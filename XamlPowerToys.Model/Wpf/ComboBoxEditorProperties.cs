namespace XamlPowerToys.Model.Wpf {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Wpf.Controls;

    [Serializable]
    public class ComboBoxEditorProperties : SelectorBase, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public ComboBoxEditorProperties() {
            this.TemplateResourceKey = "wpfComboBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ComboBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
