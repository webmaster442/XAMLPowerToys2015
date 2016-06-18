namespace XamlPowerToys.PackageCommands {
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using XamlPowerToys.Commands;
    using XamlPowerToys.Infrastructure;
    using XamlPowerToys.Model;

    internal sealed class XamarinXamlPowerToysCommand {

        public DTE2 Dte2 { get; }

        public static XamarinXamlPowerToysCommand Instance { get; private set; }

        public Package Package { get; }

        public IServiceProvider ServiceProvider => this.Package;

        public const Int32 CommandId = 256;

        public static readonly Guid CommandSet = new Guid("D309F791-903F-11D0-9EFC-00A0C911004F");

        public static void Initialize(Package package) {
            Instance = new XamarinXamlPowerToysCommand(package);
        }

        XamarinXamlPowerToysCommand(Package package) {
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
            // gets called from any code file editor.
            // will need to modify this after VS changes the editor for Xamarin XAML files.

            var result = AssemblyAssistant.GetProjectType(this.Dte2.ActiveDocument.ActiveWindow.Project);
            var cmd = (OleMenuCommand)sender;
            cmd.Visible = result == ProjectType.Xamarin && Path.GetExtension(this.Dte2.ActiveDocument.FullName) == ".xaml";
        }

        void MenuItemCallback(Object sender, EventArgs e) {
            var dataFormGenerator = new DataFormGenerator(this.Dte2, this.Dte2.ActiveDocument.ActiveWindow.Project);
            dataFormGenerator.Generate();
        }

    }
}
