using System;
using System.Globalization;
using System.IO;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class ShellViewModel : BindableBase {
        private static IRegionManager regionManager;
        public static string FONT = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Resources/Fonts/arial.ttf";
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

        public ShellViewModel(IRegionManager regionManager) {
            ShellViewModel.regionManager = regionManager;
            SampleData.Initialize();
            NavigateCommand = new DelegateCommand<string>(Navigate);
            StartClock();
        }

        public static void Navigate(string uri) {
            regionManager.RequestNavigate("ContentRegion", uri);
        }
    }
}
