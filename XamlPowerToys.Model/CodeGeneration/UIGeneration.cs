namespace XamlPowerToys.Model.CodeGeneration {
    using System;

    public class UIGeneration {

        /// <summary>
        /// Generates the specified XAML UI from the data.
        /// </summary>
        /// <param name="generateFormModel">The code generation data.</param>
        /// <returns>String of XAML</returns>
        /// <exception cref="System.InvalidOperationException">$Unable to complete code generation: {formTemplate.Errors}</exception>
        public String Generate(GenerateFormModel generateFormModel) {
            generateFormModel.PreGenerateConfiguration();
            var formTemplate = new UIFormTemplate(generateFormModel);
            var xaml = formTemplate.TransformText();
            if (formTemplate.Errors != null && formTemplate.Errors.HasErrors) {
                throw new InvalidOperationException($"Unable to complete code generation: {formTemplate.Errors}");
            }
            return xaml;
        }

    }
}
