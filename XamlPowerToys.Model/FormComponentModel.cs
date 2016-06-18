namespace XamlPowerToys.Model {
    public class FormComponentModel : FormComponentModelBase {

        public IControlFactory ControlFactory { get; }

        public FormComponentModel(GenerateFormModel generateFormModel, PropertyInformationViewModel viewModel)
            : base(generateFormModel, viewModel) {
            this.ControlFactory = viewModel.GetTemplateFactory().Make(generateFormModel, viewModel);
        }

    }
}
