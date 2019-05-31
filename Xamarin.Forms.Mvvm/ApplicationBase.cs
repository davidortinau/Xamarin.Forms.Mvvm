using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using Xamarin.Forms;

namespace Xamarin.Forms.Mvvm
{
    public abstract class ApplicationBase : Application
    {
        private Lazy<UnityContainer> _container = new Lazy<UnityContainer>(() => new UnityContainer());
        public UnityContainer Container => _container.Value;

        public void Init(Action<UnityContainer> platformInitializeContainer = null)
        {
            platformInitializeContainer?.Invoke(Container);
            InitializeContainer();

            Page mainPage = null;
            Task.Run(async () => mainPage = await CreateMainPage()).Wait();
            
            MainPage = UseRootNavigationPage ? new NavigationPage(mainPage) : mainPage;
        }

        protected abstract void InitializeContainer();
        protected abstract bool UseRootNavigationPage { get; }
        protected abstract Task<Page> CreateMainPage();

        public async Task<Page> CreatePage<TPage, TViewModel>(Dictionary<string, object> navigationParams = null)
            where TPage : ContentPageBase
            where TViewModel : ViewModelBase
        {
            var vm = Container.Resolve<TViewModel>();
            var page = Container.Resolve<TPage>();

            vm.SetWeakPage(page);
            await vm.Initialize(navigationParams);

            page.BindingContext = vm;
            page.Initialize();

            return page;
        }

        public async Task<Page> CreateMasterDetailPage<TPage>(Dictionary<string, object> navigationParams = null)
            where TPage : MasterDetailPageBase
        {
            var masterDetailPage = Container.Resolve<TPage>();
            await masterDetailPage.Initialize(navigationParams);

            Page masterPage = await masterDetailPage.CreateMasterPage();
            masterDetailPage.Master = masterPage;
            (masterPage.BindingContext as ViewModelBase)?.SetWeakMasterDetailpage(masterDetailPage);

            Page detailPage = await masterDetailPage.CreateDetailPage();
            masterDetailPage.Detail = masterDetailPage.UseDetailNavigationPage ? new NavigationPage(detailPage) : detailPage;
            (detailPage.BindingContext as ViewModelBase)?.SetWeakMasterDetailpage(masterDetailPage);

            return masterDetailPage;
        }
    }
}
