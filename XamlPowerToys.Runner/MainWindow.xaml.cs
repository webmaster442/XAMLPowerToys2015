namespace XamlPowerToys.Runner {
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Mono.Cecil;
    using XamlPowerToys.Model;
    using XamlPowerToys.UI.DynamicForm;

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
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

        List<PropertyDefinition> GetAllPropertiesForType(AssemblyDefinition asy, TypeDefinition type) {
            var returnValue = new List<PropertyDefinition>();
            foreach (PropertyDefinition item in type.Properties) {
                returnValue.Add(item);
            }

            if (type.BaseType != null && !Object.ReferenceEquals(type.BaseType, type.Module.Import(typeof(Object))) && type.BaseType.Scope != null) {
                String baseTypeAssemblyName = null;

                var td = type.BaseType as TypeDefinition;
                if (td != null) {
                    var md = td.Scope as ModuleDefinition;
                    if (md != null) {
                        baseTypeAssemblyName = md.Name.ToLower();
                    }
                }

                if (baseTypeAssemblyName == null) {
                    var anr = type.BaseType.Scope as AssemblyNameReference;
                    if (anr != null) {
                        baseTypeAssemblyName = anr.Name.ToLower();
                    }
                }

                if (!String.IsNullOrWhiteSpace(baseTypeAssemblyName)) {
                    AssemblyDefinition asyTargetAssemblyDefinition = asy;

                    if (asyTargetAssemblyDefinition != null) {
                        foreach (TypeDefinition baseTypeDefinition in asyTargetAssemblyDefinition.MainModule.Types) {
                            if (baseTypeDefinition.IsClass && baseTypeDefinition.Name == type.BaseType.Name) {
                                returnValue.AddRange(GetAllPropertiesForType(asy, baseTypeDefinition));
                                break;
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        Boolean IsTypeNameEnumerable(String typeName) {
            return typeName.Contains("Collection") || typeName.Contains("Enumerable");
        }

        void LoadPropertyInformation(AssemblyDefinition assemblyDefinition, TypeDefinition typeDefinition, ClassEntity classEntity, String parentPropertyName = "") {
            foreach (var property in GetAllPropertiesForType(assemblyDefinition, typeDefinition)) {
                var isEnumerable = IsTypeNameEnumerable(property.PropertyType.FullName);
                if (!isEnumerable) {
                    var td = property.PropertyType as TypeDefinition;
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

                if (property.PropertyType is TypeDefinition) {
                    var td = (TypeDefinition)property.PropertyType;
                    if (td.HasProperties && td.IsPublic && td.IsClass && !td.IsAbstract) {
                        var childTypeDefinition = assemblyDefinition.MainModule.GetType(td.FullName);
                        pi.ClassEntity = new ClassEntity(assemblyDefinition, childTypeDefinition, classEntity.ProjectType, "");
                        LoadPropertyInformation(assemblyDefinition, childTypeDefinition, pi.ClassEntity, pi.Name);
                    }
                }
            }
        }

        void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            var path = @"C:\Dev\vs2015\xamlpowertoys2015\XamlPowerToys.Fakes\bin\Debug\XamlPowerToys.Fakes.dll";

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(path));

            var reader = new ReaderParameters {AssemblyResolver = resolver};

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(path, reader);

            var typeDefinition = assemblyDefinition.MainModule.GetType("XamlPowerToys.Fakes.People.PersonEditorViewModel");

            //var typeDefinition = assemblyDefinition.MainModule.GetType("XamlPowerToys.Fakes.Books.BooksDetailPageViewModel");

            var classEntity = new ClassEntity(assemblyDefinition, typeDefinition, ProjectType.Xamarin, "");

            LoadPropertyInformation(assemblyDefinition, typeDefinition, classEntity);

            var emptyConverterList = new List<String>();

            var vm = new CreateFormViewModel(classEntity, emptyConverterList);
            var view = new CreateFormView();
            this.DataContext = vm;
            this.rootGrid.Children.Add(view);
        }

    }
}
