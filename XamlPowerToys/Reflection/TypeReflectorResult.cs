namespace XamlPowerToys.Reflection {
    using System;
    using System.Collections.Generic;
    using XamlPowerToys.Model;

    public class TypeReflectorResult {

        public IEnumerable<String> AvailableConverters { get; }

        public ClassEntity ClassEntity { get; }

        public TypeReflectorResult(ClassEntity classEntity, IEnumerable<String> availableConverters) {
            if (classEntity == null) {
                throw new ArgumentNullException(nameof(classEntity));
            }
            if (availableConverters == null) {
                throw new ArgumentNullException(nameof(availableConverters));
            }

            this.ClassEntity = classEntity;
            this.AvailableConverters = availableConverters;

        }

    }
}
