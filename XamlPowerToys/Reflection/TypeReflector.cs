namespace XamlPowerToys.Reflection {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using Mono.Cecil;
    using VSLangProj;
    using XamlPowerToys.Infrastructure;
    using XamlPowerToys.Model;
    using XamlPowerToys.UI.Infrastructure;
    using XamlPowerToys.UI.SelectClassFromAssemblies;

    public class TypeReflector {

        public TypeReflectorResult SelectClassFromAllReferencedAssemblies(Project sourceProject, String xamlFileClassName, String sourceCommandName, ProjectType projectFrameworkType, String projectFrameworkVersion) {
            if (sourceProject == null) {
                throw new ArgumentNullException(nameof(sourceProject));
            }
            if (string.IsNullOrWhiteSpace(sourceCommandName)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceCommandName));
            }
            if (!Enum.IsDefined(typeof(ProjectType), projectFrameworkType)) {
                throw new InvalidEnumArgumentException(nameof(projectFrameworkType), (int)projectFrameworkType, typeof(ProjectType));
            }

            var assemblyPath = AssemblyAssistant.GetAssemblyPath(sourceProject);
            if (String.IsNullOrWhiteSpace(assemblyPath)) {
                DialogAssistant.ShowInformationMessage("The project associated with the selected file is either not vb, cs or is blacklisted.", "Invalid Project");
                return null;
            }

            if (!File.Exists(assemblyPath)) {
                DialogAssistant.ShowInformationMessage("Project assembly is missing, please 'build' your solution.", "Unbuilt Project");
                return null;
            }

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
            var reader = new ReaderParameters {AssemblyResolver = resolver};

            var classEntities = new ClassEntities();
            var sourceProjectPath = Path.GetDirectoryName(assemblyPath);
            var sourceAssemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath, reader);
            var assembliesToLoad = new Hashtable();

            //load the source assembly
            assembliesToLoad.Add(assemblyPath.ToLower(), String.Empty);

            //load up all referenced assemblies for above assemblyPath
            foreach (AssemblyNameReference assemblyReference in sourceAssemblyDefinition.MainModule.AssemblyReferences) {
                if (!AssemblyAssistant.SkipLoadingAssembly(assemblyReference.Name)) {
                    var assemblyFullPath = GetAssemblyFullPath(sourceProjectPath, assemblyReference.Name);
                    if (!String.IsNullOrWhiteSpace(assemblyFullPath) && !assembliesToLoad.ContainsKey(assemblyFullPath.ToLower())) {
                        assembliesToLoad.Add(assemblyFullPath.ToLower(), String.Empty);
                    }
                }
            }

            //load up all assemblies referenced in the project but that are not loaded yet.
            foreach (var projectReference in GetProjectReferences(sourceProject)) {
                if (!assembliesToLoad.ContainsKey(projectReference)) {
                    assembliesToLoad.Add(projectReference.ToLower(), String.Empty);
                }
            }

            ReflectClasses(sourceAssemblyDefinition, projectFrameworkType, projectFrameworkVersion, classEntities);
            foreach (var asyPath in assembliesToLoad.Keys) {
                var asyResolver = new DefaultAssemblyResolver();
                asyResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                var asyReader = new ReaderParameters {AssemblyResolver = asyResolver};

                ReflectClasses(AssemblyDefinition.ReadAssembly(asyPath.ToString(), asyReader), projectFrameworkType, projectFrameworkVersion, classEntities);
            }

            var listOfConverters = new List<String>();
            listOfConverters.AddRange(classEntities.Where(x => x.ClassName.ToLower().EndsWith("converter")).Select(n => n.ClassName).ToList());

            var view = new SelectClassFromAssembliesView(classEntities, sourceCommandName, xamlFileClassName);
            var result = view.ShowDialog();
            if (result.HasValue && result.Value && view.SelectedClassEntity != null) {
                LoadPropertyInformation(view.SelectedClassEntity.TypeDefinition, view.SelectedClassEntity, assembliesToLoad);
                return new TypeReflectorResult(view.SelectedClassEntity, listOfConverters);
            }

            return null;
        }

        Boolean CanWrite(PropertyDefinition property) {
            if (property.SetMethod == null) {
                return false;
            }
            if (property.SetMethod.IsPublic) {
                return true;
            }
            return false;
        }

        String FormatPropertyTypeName(PropertyDefinition property) {
            var name = property.PropertyType.Name;
            var fullName = property.PropertyType.FullName;

            if (!name.Contains("`")) {
                return name;
            }

            name = name.Remove(name.IndexOf("`", StringComparison.Ordinal), 2);

            if (property.PropertyType == null || !(property.PropertyType is GenericInstanceType) || fullName.IndexOf(">", StringComparison.Ordinal) == -1) {
                return name;
            }

            var sb = new System.Text.StringBuilder(512);
            sb.Append($"{name}(Of ");

            var obj = (GenericInstanceType)property.PropertyType;
            if (obj.HasGenericArguments) {
                foreach (TypeReference tr in obj.GenericArguments) {
                    sb.Append(tr.Name);
                    sb.Append(", ");
                }
            } else {
                return name;
            }

            sb.Length = sb.Length - 2;
            sb.Append(")");
            return sb.ToString();
        }

        List<PropertyDefinition> GetAllPropertiesForType(TypeDefinition type, Hashtable loadedAssemblies) {
            var returnValue = new List<PropertyDefinition>();
            foreach (PropertyDefinition item in type.Properties) {
                returnValue.Add(item);
            }

            if (type.BaseType != null && !Object.ReferenceEquals(type.BaseType, type.Module.Import(typeof(Object))) && type.BaseType.Scope != null) {
                String baseTypeAssemblyName = null;

                var td = type.BaseType as TypeDefinition;
                var md = td?.Scope as ModuleDefinition;
                if (md != null) {
                    // when base type is in the types assembly
                    if (md.Name == type.Module.Name) {
                        return returnValue;
                    }
                    baseTypeAssemblyName = md.Name.ToLower();
                }

                if (baseTypeAssemblyName == null) {
                    var anr = type.BaseType.Scope as AssemblyNameReference;
                    if (anr != null) {
                        baseTypeAssemblyName = anr.Name.ToLower();
                    }
                }

                if (!String.IsNullOrWhiteSpace(baseTypeAssemblyName)) {
                    AssemblyDefinition asyTargetAssemblyDefinition = null;

                    foreach (var asyName in loadedAssemblies.Keys) {
                        // ReSharper disable AssignNullToNotNullAttribute
                        if (asyName.ToString().IndexOf(baseTypeAssemblyName, StringComparison.Ordinal) > -1) {
                            // ReSharper restore AssignNullToNotNullAttribute
                            var resolver = new DefaultAssemblyResolver();
                            resolver.AddSearchDirectory(Path.GetDirectoryName(asyName.ToString()));
                            var reader = new ReaderParameters {AssemblyResolver = resolver};

                            asyTargetAssemblyDefinition = AssemblyDefinition.ReadAssembly(asyName.ToString(), reader);
                            break;
                        }
                    }

                    if (asyTargetAssemblyDefinition != null) {
                        foreach (TypeDefinition baseTypeDefinition in asyTargetAssemblyDefinition.MainModule.Types) {
                            if (baseTypeDefinition.IsClass && baseTypeDefinition.Name == type.BaseType.Name) {
                                returnValue.AddRange(GetAllPropertiesForType(baseTypeDefinition, loadedAssemblies));
                                break;
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        String GetAssemblyFullPath(String sourceProjectPath, String assemblyName) {
            if (File.Exists(Path.Combine(sourceProjectPath, assemblyName + ".dll"))) {
                return Path.Combine(sourceProjectPath, assemblyName + ".dll");
            }
            if (File.Exists(Path.Combine(sourceProjectPath, assemblyName + ".exe"))) {
                return Path.Combine(sourceProjectPath, assemblyName + ".exe");
            }
            return String.Empty;
        }

        IEnumerable<String> GetProjectReferences(Project sourceProject) {
            var list = new List<String>();
            var vsProject = (VSProject)sourceProject.Object;

            foreach (Reference reference in vsProject.References) {
                if (AssemblyAssistant.SkipLoadingAssembly(reference.Name)) {
                    continue;
                }
                if (String.IsNullOrEmpty(reference.Path)) {
                    DialogAssistant.ShowInformationMessage($"The {reference.Name} reference is broken or unresolved.  It will be ignored for now, but you need to correct it or removed the unused reference.", "Broken Reference Found");
                    continue;
                }
                list.Add(reference.Path);
            }
            return list;
        }

        Boolean IsTypeNameEnumerable(String typeName) {
            return typeName.Contains("Collection") || typeName.Contains("Enumerable");
        }

        void LoadPropertyInformation(TypeDefinition typeDefinition, ClassEntity classEntity, Hashtable assembliesToLoad, String parentPropertyName = "") {
            foreach (var property in GetAllPropertiesForType(typeDefinition, assembliesToLoad)) {
                TypeDefinition td = property.PropertyType as TypeDefinition;
                if (td == null) {
                    var tr = property.PropertyType;
                    if (tr != null) {
                        td = tr.Resolve();
                    }
                }

                var isEnumerable = IsTypeNameEnumerable(property.PropertyType.FullName);
                if (!isEnumerable) {
                    if (td?.BaseType != null) {
                        isEnumerable = IsTypeNameEnumerable(td.BaseType.FullName);
                    }
                }

                var pi = new PropertyInformationViewModel(CanWrite(property), property.Name, FormatPropertyTypeName(property), property.PropertyType.Namespace, classEntity.ProjectType, classEntity.ClassName, isEnumerable, false, parentPropertyName);

                if (property.PropertyType is Mono.Cecil.GenericInstanceType) {
                    var obj = (GenericInstanceType)property.PropertyType;
                    if (obj.HasGenericArguments) {
                        foreach (TypeReference genericTr in obj.GenericArguments) {
                            pi.GenericArguments.Add(genericTr.Name);
                            if (!genericTr.Namespace.Contains("System")) {

                                TypeDefinition genericTd = genericTr as TypeDefinition;
                                if (genericTd == null) {
                                    genericTd = genericTr.Resolve();
                                }

                                if (genericTd != null) {
                                    if (genericTd.HasProperties && genericTd.IsPublic && genericTd.IsClass && !genericTd.IsAbstract && !genericTd.Namespace.Contains("System")) {
                                        foreach (var prop in genericTd.Properties) {
                                            pi.GenericCollectionClassPropertyNames.Add(prop.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (property.HasParameters) {
                    foreach (var pd in property.Parameters) {
                        pi.PropertyParameters.Add(new PropertyParameter(pd.Name, pd.ParameterType.Name));
                    }
                }
                classEntity.PropertyInformationCollection.Add(pi);

                if (td != null) {
                    if (td.HasProperties && td.IsPublic && td.IsClass && !td.IsAbstract && !td.Namespace.Contains("System")) {
                        var childTypeDefinition = td;
                        var childAssemblyDefinition = td.Module.Assembly;
                        pi.ClassEntity = new ClassEntity(childAssemblyDefinition, childTypeDefinition, classEntity.ProjectType, "");
                        LoadPropertyInformation(childTypeDefinition, pi.ClassEntity, assembliesToLoad, pi.Name);
                    }
                }
            }
        }

        void ReflectClasses(AssemblyDefinition asy, ProjectType projectFrameworkType, String projectFrameworkVersion, ClassEntities classEntities) {
            if (asy == null) {
                throw new ArgumentNullException(nameof(asy));
            }
            if (classEntities == null) {
                throw new ArgumentNullException(nameof(classEntities));
            }
            if (!Enum.IsDefined(typeof(ProjectType), projectFrameworkType)) {
                throw new InvalidEnumArgumentException(nameof(projectFrameworkType), (int)projectFrameworkType, typeof(ProjectType));
            }
            if (string.IsNullOrWhiteSpace(projectFrameworkVersion)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectFrameworkVersion));
            }

            foreach (TypeDefinition type in asy.MainModule.GetTypes()) {
                if (type.IsPublic && type.IsClass && !type.IsAbstract && !type.Name.StartsWith("<") && !type.Name.Contains("AnonymousType") && type.Name != "AssemblyInfo" && !type.Name.StartsWith("__")) {
                    var previouslyLoaded = false;

                    foreach (var classEntity in classEntities) {
                        if (type.Name == classEntity.ClassName && type.Namespace == classEntity.NamespaceName && asy.Name.Name == classEntity.AssemblyName) {
                            previouslyLoaded = true;
                            break;
                        }
                    }

                    if (!previouslyLoaded) {
                        if (type.BaseType == null || type.BaseType.Name != "MulticastDelegate") {
                            classEntities.Add(new ClassEntity(asy, type, projectFrameworkType, projectFrameworkVersion));
                        }
                    }
                }
            }
        }

    }
}
