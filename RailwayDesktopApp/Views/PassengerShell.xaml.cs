using System.Windows;

namespace RailwayDesktopApp.Views {
    public partial class PassengerShell : Window {
        public PassengerShell() {
            InitializeComponent();
            passengerGrid.Visibility = Visibility.Hidden;
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}
