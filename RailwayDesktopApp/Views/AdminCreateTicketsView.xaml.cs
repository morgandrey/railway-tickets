using System.Globalization;
using System.Windows.Controls;

namespace RailwayDesktopApp.Views {
    public partial class AdminCreateTicketsView : UserControl {
        public AdminCreateTicketsView() {
            InitializeComponent();
            datePicker.CultureInfo = new CultureInfo("ru-RU");
            timePicker.CultureInfo = new CultureInfo("ru-RU");
        }

        private void OnLoadingRow(object sender, DataGridRowEventArgs e) {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
