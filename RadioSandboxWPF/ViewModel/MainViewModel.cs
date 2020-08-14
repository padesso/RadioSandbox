using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace RadioSandboxWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _testString;

        public MainViewModel()
        {
            TestString = "this is a test";
        }

        public string TestString 
        { 
            get { return _testString; }
            set 
            { 
                _testString = value;
                RaisePropertyChanged("TestString");
            }
        }
    }
}
