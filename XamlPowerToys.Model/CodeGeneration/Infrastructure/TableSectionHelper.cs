namespace XamlPowerToys.Model.CodeGeneration.Infrastructure {
    using System;

    public class TableSectionHelper {

        public TableSectionHelper() {
        }

        public String EndTag() {
            return "</TableSection>";
        }

        public String StartTag(String title) {
            return $"<TableSection Title=\"{title}\">";
        }

    }
}
