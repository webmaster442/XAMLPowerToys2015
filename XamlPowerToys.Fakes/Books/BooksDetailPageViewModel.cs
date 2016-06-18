namespace XamlPowerToys.Fakes.Books {
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    public class BooksDetailPageViewModel : ObservableObject {

        public Book Book { get; set; }

        public IList<String> Formats { get; }

        public ICommand SaveCommand { get; }

    }
}
