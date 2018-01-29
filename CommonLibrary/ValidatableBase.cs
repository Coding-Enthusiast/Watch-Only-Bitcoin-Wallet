using BitcoinLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MVVMLibrary
{
    public class ValidatableBase : CommonBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName))
            {
                return null;
            }
            else
            {
                return errors[propertyName];
            }
        }

        public bool HasErrors
        {
            get
            {
                return errors.Count > 0;
            }
        }


        public void AddError(string propertyName, string error)
        {
            if (!errors.ContainsKey(propertyName))
            {
                errors[propertyName] = new List<string>();
            }
            if (!errors[propertyName].Contains(error))
            {
                errors[propertyName].Add(error);
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RemoveError(string propertyName, string error)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }


        public void Validate(string address)
        {
            VerificationResult r = new VerificationResult();
            if (address.StartsWith("bc1"))
            {
                r = SegWitAddress.Verify(address, SegWitAddress.NetworkType.MainNet);
            }
            else
            {
                r = Base58.Verify(address);
            }

            if (r.IsVerified)
            {
                RemoveError("Address", "");
            }
            else
            {
                AddError("Address", r.Error);
            }
        }

    }
}
