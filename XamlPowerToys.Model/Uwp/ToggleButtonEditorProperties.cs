namespace XamlPowerToys.Model.Uwp {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Uwp.Controls;

    [Serializable]
    public class ToggleButtonEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _content;

        public String Content {
            get { return _content; }
            set {
                _content = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public ToggleButtonEditorProperties() {
            this.TemplateResourceKey = "uwpToggleButtonEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new CheckBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
