namespace XamlPowerToys.Model {
    public interface IConstructControlFactory {

        IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel);

    }
}