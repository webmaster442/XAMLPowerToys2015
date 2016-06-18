namespace XamlPowerToys.UI.Infrastructure {
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class ObservableCollectionExtensions {

        public static void Sort<T>(this ObservableCollection<T> observable) where T : IComparable<T>, IEquatable<T> {
            var sorted = observable.OrderBy(x => x).ToList();

            Int32 ptr = 0;
            while (ptr < sorted.Count) {
                if (!observable[ptr].Equals(sorted[ptr])) {
                    T t = observable[ptr];
                    observable.RemoveAt(ptr);
                    observable.Insert(sorted.IndexOf(t), t);
                } else {
                    ptr++;
                }
            }
        }

    }
}
