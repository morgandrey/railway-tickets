using System;
using System.Globalization;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace RailwayDesktopApp.ViewModels {
    public class PassengerShellViewModel : BindableBase {
        private static IRegionManager regionManager;

        private string timeNow;

        public string TimeNow {
            get => timeNow;
            set => SetProperty(ref timeNow, value);
        }


        private void StartClock() {
            var timer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += TickEvent;
            timer.Start();
        }

        private void TickEvent(object sender, EventArgs e) {
            TimeNow = DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("ru-RU"));
        }

        public DelegateCommand<string> NavigateCommand { get; set; }

        public PassengerShellViewModel(IRegionManager regionManager) {
            PassengerShellViewModel.regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
            StartClock();
        }

        public static void Navigate(string uri) {
            regionManager.RequestNavigate("ContentRegion", uri);
        }
    }
}
