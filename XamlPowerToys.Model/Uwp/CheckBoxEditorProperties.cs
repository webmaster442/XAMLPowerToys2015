namespace XamlPowerToys.Model.Uwp {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Uwp.Controls;

    [Serializable]
    public class CheckBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _content;

        public String Content {
            get { return _content; }
            set {
                _content = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public CheckBoxEditorProperties() {
            this.TemplateResourceKey = "uwpCheckBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new CheckBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
