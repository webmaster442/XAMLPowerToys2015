namespace XamlPowerToys.Model.Wpf {
    using System;
    using XamlPowerToys.Model.CodeGeneration.Wpf.Controls;

    [Serializable]
    public class ImageEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _source;

        public String Source {
            get { return _source; }
            set {
                _source = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public ImageEditorProperties() {
            this.TemplateResourceKey = "wpfImageEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ImageFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
