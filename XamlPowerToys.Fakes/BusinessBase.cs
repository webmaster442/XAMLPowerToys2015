namespace XamlPowerToys.Fakes {
    using System;

    public abstract class BusinessBase : ObservableObject {

        String _errors;
        Boolean _hasErrors;

        public String Errors {
            get { return _errors; }
            set {
                _errors = value;
                RaisePropertyChanged();
            }
        }

        public Boolean HasErrors {
            get { return _hasErrors; }
            private set {
                _hasErrors = value;
                RaisePropertyChanged();
            }
        }

    }
}
