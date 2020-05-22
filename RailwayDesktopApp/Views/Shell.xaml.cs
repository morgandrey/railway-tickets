using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace RailwayDesktopApp.Views {
    public partial class Shell : Window {
        public Shell() {
            InitializeComponent();
            passengerStackPanel.Visibility = Visibility.Hidden;
            adminStackPanel.Visibility = Visibility.Hidden;
            LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}
