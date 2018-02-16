using BitcoinLibrary;
using MVVMLibrary;
using System;
using System.Collections.Generic;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.ViewModels
{
    public class ImportViewModel : ViewModelBase
    {
        public ImportViewModel()
        {
            ImportCommand = new BindableCommand(CheckAndImport);
        }


        public string Note
        {
            get
            {
                return "Note: 1 address per line (seperated by a new line).";
            }
        }


        public List<BitcoinAddress> AddressList { get; set; }


        private string importText;
        public string ImportText
        {
            get { return importText; }
            set { SetField(ref importText, value); }
        }


        public BindableCommand ImportCommand { get; private set; }
        private void CheckAndImport()
        {
            Errors = string.Empty;
            if (string.IsNullOrWhiteSpace(ImportText))
            {
                Errors = "Input text can not be empty!";
            }
            else
            {
                List<BitcoinAddress> temp = new List<BitcoinAddress>();
                int lineNum = 0;
                foreach (var item in ImportText.SplitToLines())
                {
                    lineNum++;
                    VerificationResult vr = ValidateAddr(item);
                    if (vr.IsVerified)
                    {
                        temp.Add(new BitcoinAddress() { Address = item });
                    }
                    else
                    {
                        Errors = string.Format("Invalid address on line {0}: {1}", lineNum, vr.Error);
                        break;
                    }
                }
                if (string.IsNullOrEmpty(Errors))
                {
                    AddressList = temp;
                    OnClosingRequest();
                }
            }
        }
        private VerificationResult ValidateAddr(string addr)
        {
            VerificationResult vr = new VerificationResult();
            if (addr.StartsWith("bc1"))
            {
                vr = SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet);
            }
            else
            {
                vr = Base58.Verify(addr);
            }
            return vr;
        }

        public event EventHandler ClosingRequest;

        protected void OnClosingRequest()
        {
            if (ClosingRequest != null)
            {
                ClosingRequest(this, EventArgs.Empty);
            }
        }

    }
}
