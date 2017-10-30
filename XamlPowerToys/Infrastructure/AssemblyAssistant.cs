namespace XamlPowerToys.Infrastructure {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using XamlPowerToys.Model;
    using XamlPowerToys.UI.Infrastructure;
    using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    internal class AssemblyAssistant {

        internal static String GetAssemblyPath(Project vsProject) {
            var fullPath = Path.GetDirectoryName(vsProject.FullName);
            var outputPath = vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

            // ReSharper disable AssignNullToNotNullAttribute
            var outputDirectory = Path.Combine(fullPath, outputPath);

            // ReSharper restore AssignNullToNotNullAttribute
            var outputFileName = vsProject.Properties.Item("OutputFileName").Value.ToString();
            var assemblyPath = Path.Combine(outputDirectory, outputFileName);
            return assemblyPath;
        }

        internal static ProjectType GetProjectType(Project project) {
            var result = GetProjectTypeGuids(project).ToUpper();

            if (result.Contains("60DC8134-EBA5-43B8-BCC9-BB4BC16C2548")) {
                return ProjectType.Wpf;
            }
            if (result.Contains("786C830F-07A1-408B-BD7F-6EE04809D6DB")) {
                return ProjectType.Xamarin;
            }
            if (result.Contains("A5A43C5B-DE2A-4C0C-9213-0A381AF9435A")) {
                return ProjectType.Uwp;
            }
            if (result.Contains("A1591282-1198-4647-A2B1-27E5FF5F6F3B")) {
                return ProjectType.Silverlight;
            }


            return ProjectType.Unknown;
        }

        internal static String GetProjectFrameworkVersion(Project project) {
            try {
                return project.Properties.Item("TargetFrameworkMoniker").Value.ToString().Split(',')[1].Replace("Version=v", String.Empty);
            } catch (Exception ex) {
                DialogAssistant.ShowExceptionMessage(ex, "Checking Framework Version");
            }
            return String.Empty;
        }

        internal static String GetProjectTypeGuids(Project proj) {
            var ivsSolution = (IVsSolution)GetService(proj.DTE, typeof(IVsSolution));
            IVsHierarchy ivsHierarchy;
            Int32 result = ivsSolution.GetProjectOfUniqueName(proj.UniqueName, out ivsHierarchy);

            var projectTypeGuids = String.Empty;

            if (result == 0) {
                var ivsAggregatableProject = (IVsAggregatableProject)ivsHierarchy;
                ivsAggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);
            }

            return projectTypeGuids;
        }

        internal static Boolean SkipLoadingAssembly(String assemblyName) {
            assemblyName = assemblyName.ToLower();
            if (assemblyName.StartsWith("windows.")) {
                return true;
            }
            if (assemblyName.StartsWith("xamarin")) {
                return true;
            }
            if (assemblyName.StartsWith("microsoft")) {
                return true;
            }
            if (assemblyName.StartsWith("system")) {
                return true;
            }
            if (assemblyName.StartsWith("mscorlib")) {
                return true;
            }
            if (assemblyName.StartsWith("presentationframework")) {
                return true;
            }
            if (assemblyName.StartsWith("presentationcore")) {
                return true;
            }
            if (assemblyName.StartsWith("windowsbase")) {
                return true;
            }
            if (assemblyName.StartsWith("wpftoolkit")) {
                return true;
            }
            if (assemblyName.StartsWith("uiautomationprovider")) {
                return true;
            }
            if (assemblyName.StartsWith("mono")) {
                return true;
            }
            if (assemblyName.StartsWith("infragistics")) {
                return true;
            }
            if (assemblyName.StartsWith("telerik")) {
                return true;
            }
            if (assemblyName.StartsWith("devexpress")) {
                return true;
            }
            if (assemblyName.StartsWith("xuni")) {
                return true;
            }

            return false;
        }

        static Object GetService(Object serviceProvider, Guid guid) {
            Object service = null;
            IntPtr intPtr;
            var guidService = guid;
            var riidGuid = guidService;
            var iServiceProvider = (IServiceProvider)serviceProvider;
            var hr = iServiceProvider.QueryService(ref guidService, ref riidGuid, out intPtr);

            if (hr != 0) {
                Marshal.ThrowExceptionForHR(hr);
            } else if (!intPtr.Equals(IntPtr.Zero)) {
                service = Marshal.GetObjectForIUnknown(intPtr);
                Marshal.Release(intPtr);
            }

            return service;
        }

        static Object GetService(Object serviceProvider, Type type) {
            return GetService(serviceProvider, type.GUID);
        }

    }
}
