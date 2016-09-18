namespace XamlPowerToys.Model.Silverlight {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Silverlight.Controls;

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
            this.TemplateResourceKey = "silverlightCheckBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new CheckBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
