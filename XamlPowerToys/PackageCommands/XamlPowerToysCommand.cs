namespace XamlPowerToys.PackageCommands {
    using System;
    using System.ComponentModel.Design;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using XamlPowerToys.Commands;
    using XamlPowerToys.Infrastructure;
    using XamlPowerToys.Model;

    internal sealed class XamlPowerToysCommand {

        public DTE2 Dte2 { get; }

        public static XamlPowerToysCommand Instance { get; private set; }

        public Package Package { get; }

        public IServiceProvider ServiceProvider => this.Package;

        public const Int32 CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("4C87B692-1202-46AA-B64C-EF01FAEC53DA");

        public static void Initialize(Package package) {
            Instance = new XamlPowerToysCommand(package);
        }

        XamlPowerToysCommand(Package package) {
            if (package == null) {
                throw new ArgumentNullException(nameof(package));
            }
            this.Package = package;
            this.Dte2 = (DTE2)this.ServiceProvider.GetService(typeof(DTE));

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null) {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatusCallback;
                commandService.AddCommand(menuItem);
            }
        }

        void BeforeQueryStatusCallback(Object sender, EventArgs e) {
            // gets called from a XAML file being edited by the WPF Designer editor.
            var result = AssemblyAssistant.GetProjectType(this.Dte2.ActiveDocument.ActiveWindow.Project);
            var cmd = (OleMenuCommand)sender;

            // future proofing when Visual Studio changes the editor for Xamarin
            cmd.Visible = result == ProjectType.Wpf || result == ProjectType.Xamarin || result == ProjectType.Uwp;
        }

        void MenuItemCallback(Object sender, EventArgs e) {
            var dataFormGenerator = new DataFormGenerator(this.Dte2, this.Dte2.ActiveDocument.ActiveWindow.Project);
            dataFormGenerator.Generate();
        }

    }
}
