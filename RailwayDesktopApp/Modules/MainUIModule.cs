using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp.Modules {
    public class MainUIModule : IModule {
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
            containerRegistry.RegisterForNavigation<AdminPassengerView>();
            containerRegistry.RegisterForNavigation<AdminSaleView>();
            containerRegistry.RegisterForNavigation<AdminCreateTicketsView>();
            containerRegistry.RegisterForNavigation<AdminTicketView>();
            containerRegistry.RegisterForNavigation<AdminTicketDetailsView>();
            containerRegistry.RegisterForNavigation<AdminTownView>();
            containerRegistry.RegisterForNavigation<AdminTrainView>();
            containerRegistry.RegisterForNavigation<AdminTypeWagonView>();
        }
    }
}