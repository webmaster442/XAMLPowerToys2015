namespace XamlPowerToys {
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using XamlPowerToys.PackageCommands;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidXamlPowerToysCommandPackageString)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    public sealed class VSPackage : Package {

        public VSPackage() {
        }

        protected override void Initialize() {
            XamarinXamlPowerToysCommand.Initialize(this);
            XamlPowerToysCommand.Initialize(this);
            base.Initialize();

        }

    }
}
