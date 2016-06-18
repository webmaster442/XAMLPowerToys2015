namespace XamlPowerToys.Commands {
    using System;
    using System.IO;
    using EnvDTE;
    using EnvDTE80;
    using XamlPowerToys.Infrastructure;
    using XamlPowerToys.Model;
    using XamlPowerToys.Reflection;
    using XamlPowerToys.UI.DynamicForm;
    using XamlPowerToys.UI.Infrastructure;

    internal class DataFormGenerator {

        readonly Project _activeProject;
        readonly DTE2 _dte2;
        readonly String _projectFrameworkVersion;
        readonly ProjectType _projectType;

        public DataFormGenerator(DTE2 dte2, Project activeProject) {
            _dte2 = dte2;
            _activeProject = activeProject;
            if (dte2 == null) {
                throw new ArgumentNullException(nameof(dte2));
            }
            if (activeProject == null) {
                throw new ArgumentNullException(nameof(activeProject));
            }
            _projectType = AssemblyAssistant.GetProjectType(dte2.ActiveDocument.ActiveWindow.Project);
            _projectFrameworkVersion = AssemblyAssistant.GetProjectFrameworkVersion(dte2.ActiveDocument.ActiveWindow.Project);
        }

        public void Generate() {
            try {
                var xamlFileClassName = Path.GetFileNameWithoutExtension(_dte2.ActiveDocument.Name);
                var typeReflector = new TypeReflector();
                var selectedClassEntity = typeReflector.SelectClassFromAllReferencedAssemblies(_activeProject, xamlFileClassName, "Data Form Generator", _projectType, _projectFrameworkVersion);
                if (selectedClassEntity != null) {
                    var win = new XamlPowerToysWindow();
                    var vm = new CreateFormViewModel(selectedClassEntity);
                    var view = new CreateFormView();
                    win.DataContext = vm;
                    win.rootGrid.Children.Add(view);
                    win.ShowDialog();
                    var ts = (TextSelection)_dte2.ActiveDocument.Selection;
                    ts.Insert(vm.ResultXaml);
                    _dte2.ExecuteCommand("Edit.FormatDocument");
                }
            } catch (Exception ex) {
                DialogAssistant.ShowExceptionMessage(ex);
            }
        }

    }
}
