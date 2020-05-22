using System;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using RailwayDesktopApp.Modules;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp {
    public partial class App {
        protected override Window CreateShell() {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<MainUIModule>();
        }
    }
}
