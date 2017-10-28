using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NLog;
using System;

namespace BRClib
{
    /// <summary>
    /// A base class with <see cref="INotifyPropertyChanged"/> methods
    /// </summary>
    public class BindingBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected Logger Log { get; private set; }

        public BindingBase()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string pName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(pName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName]string pName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
            //Log.ConditionalDebug("{0} changed", pName);
        }
    }



}
