namespace XamlPowerToys.Model.XamarinForms {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Xamarin.Controls;

    [Serializable]
    public class BindablePickerEditorProperties : SelectorBase, IEditEditor, IConstructControlFactory {

        String _title = String.Empty;

        public String TemplateResourceKey { get; }

        public String Title {
            get { return _title; }
            set {
                _title = value;
                RaisePropertyChanged();
            }
        }

        public String TitleText {
            get {
                if (String.IsNullOrWhiteSpace(this.Title)) {
                    return String.Empty;
                }
                return $"Title=\"{this.Title}\" ";
            }
        }

        public BindablePickerEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsBindablePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new BindablePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
