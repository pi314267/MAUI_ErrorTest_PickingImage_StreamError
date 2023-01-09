using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ErrorTest.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        bool isBusy = false;
        string title = string.Empty;


        public bool IsBusy
        {
            get { return this.isBusy; }
            set { SetProperty(ref this.isBusy, value); }
        }

        public string Title
        {
            get { return this.title; }
            set { SetProperty(ref this.title, value); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        ///*여기서 부터*/
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //    => OnPropertiesChanged(propertyName);

        //protected virtual void OnPropertiesChanged(params string[] propertiesNames)
        //{
        //    if (propertiesNames?.Length > 0)
        //        foreach (var name in propertiesNames)
        //            OnPropertyChanged(new PropertyChangedEventArgs(name));
        //}

        //protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        //    => MainThread.BeginInvokeOnMainThread(
        //            () => PropertyChanged?.Invoke(this, eventArgs));
        ///*여기까지*/


        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        public event Func<string, Task> DoDisplayAlert;

        public event Func<BaseViewModel, bool, Task> DoNavigate;

        protected Task DisplayAlertAsync(string message)
            => DoDisplayAlert?.Invoke(message) ?? Task.CompletedTask;

        protected Task NavigateAsync(BaseViewModel vm, bool showModal = false)
            => DoNavigate?.Invoke(vm, showModal) ?? Task.CompletedTask;
    }
}
