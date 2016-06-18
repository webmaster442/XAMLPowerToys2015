namespace XamlPowerToys.Model.CodeGeneration {
    using System;

    public partial class UIFormTemplate : IUIGenerator {

        public FormTemplateModel Model { get; }

        public UIFormTemplate(GenerateFormModel generateFormModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }

            this.Model = new FormTemplateModel(generateFormModel);
        }

    }
}
