namespace XamlPowerToys.Model.Infrastructure {
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class Cloner {

        /// <summary>
        /// Deep copy serializable object.
        /// </summary>
        /// <typeparam name="T">Type to create deep copy of.</typeparam>
        /// <param name="toClone">The object instance to clone.</param>
        /// <returns>Deep copy instance of <typeparamref name="T"/>.</returns>
        public static T DeepCopy<T>(Object toClone) {
            using (var ms = new MemoryStream()) {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, toClone);
                ms.Position = 0;
                return (T)bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// Initializes the <see cref="Cloner"/> class.
        /// </summary>
        static Cloner() {
        }

    }
}
