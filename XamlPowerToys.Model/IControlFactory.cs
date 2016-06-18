namespace XamlPowerToys.Model {
    using System;

    public interface IControlFactory {

        String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null);

    }
}