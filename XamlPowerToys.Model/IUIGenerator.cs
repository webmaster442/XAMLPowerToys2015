namespace XamlPowerToys.Model {
    using System;
    using System.CodeDom.Compiler;

    public interface IUIGenerator {

        CompilerErrorCollection Errors { get; }

        String TransformText();

    }
}
