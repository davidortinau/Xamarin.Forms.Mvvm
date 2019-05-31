using Xamarin.Forms;

namespace Xamarin.Forms.Mvvm
{
    public abstract class ContentPageBase : ContentPage
    {
        protected ViewModelBase ViewModel => BindingContext as ViewModelBase;

        protected override void OnAppearing() => ViewModel?.OnAppearing();

        protected override void OnDisappearing() => ViewModel?.OnDisappearing();

        public virtual void Initialize() { }
    }
}
