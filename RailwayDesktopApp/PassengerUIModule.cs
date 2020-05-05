using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp {
    public class PassengerUIModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(AuthorizationView));
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(NewPassengerView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<PassengerProfileView>();
            containerRegistry.RegisterForNavigation<PassengerTicketView>();
            containerRegistry.RegisterForNavigation<PassengerHistoryView>();
            containerRegistry.RegisterForNavigation<PassengerSellView>();
            containerRegistry.RegisterForNavigation<PassengerTicketDetailsView>();
        }
    }
}