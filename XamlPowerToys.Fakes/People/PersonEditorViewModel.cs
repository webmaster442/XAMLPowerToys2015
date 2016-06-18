namespace XamlPowerToys.Fakes.People {
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    public class PersonEditorViewModel : ObservableObject {

        IList<Country> _countries;
        Person _person;
        IList<String> _states;

        public IList<Country> Countries {
            get { return _countries; }
            set {
                _countries = value;
                RaisePropertyChanged();
            }
        }

        public DateTime MaximumBirthdayValue { get; set; }

        public DateTime MinimumBirthdayValue { get; set; }

        public Person Person {
            get { return _person; }
            set {
                _person = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SaveCommand { get; set; }

        public IList<String> States {
            get { return _states; }
            set {
                _states = value;
                RaisePropertyChanged();
            }
        }

        public PersonEditorViewModel() {
            var person = new Person();
            person.LastName = "Shifflett";
            person.Address = "2 Commerce Drive";
            person.City = "Cranbury";
            person.Country = "USA";
            person.State = "NJ";
            person.Id = 100;
            person.FirstName = "Karl";
            person.Phone = "800-555-1212";
            person.ZipCode = "08512";
            this.Person = person;

            this.States = new List<String> {"NC", "NJ", "NY"};
            this.Countries = new List<Country>();
            this.Countries.Add(new Country {Abbreviation = "BGR", Name = "Bulgaria"});
            this.Countries.Add(new Country {Abbreviation = "ROU", Name = "Romania"});
            this.Countries.Add(new Country {Abbreviation = "RUS", Name = "Russian Federation"});
            this.Countries.Add(new Country {Abbreviation = "USA", Name = "United States"});
        }

    }
}
