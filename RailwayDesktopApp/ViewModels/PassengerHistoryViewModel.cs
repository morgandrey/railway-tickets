using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class PassengerHistoryViewModel : BindableBase, INavigationAware {
        #region Properties
        private string timeFrom;
        public string TimeFrom {
            get => timeFrom;
            set => SetProperty(ref timeFrom, value);
        }
        private string timeTo;
        public string TimeTo {
            get => timeTo;
            set => SetProperty(ref timeTo, value);
        }

        private ObservableCollection<Sale> sales;
        public ObservableCollection<Sale> Sales {
            get => sales; 
            set => SetProperty(ref sales, value);
        }
        private Sale selectedSaleItem;
        public Sale SelectedSaleItem {
            get => selectedSaleItem;
            set => SetProperty(ref selectedSaleItem, value);
        }
        #endregion

        #region Commands
        private DelegateCommand ticketReportCommand;
        public DelegateCommand TicketReportCommand =>
            ticketReportCommand ??= new DelegateCommand(ExecuteTicketReportCommand);
        private DelegateCommand findSalesCommand;
        public DelegateCommand FindSalesCommand =>
            findSalesCommand ??= new DelegateCommand(ExecuteFindSalesCommand);
        #endregion

        private void ExecuteTicketReportCommand() {
            CreateTicketReport(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"/Билет №{SelectedSaleItem.IdSale}.pdf", SelectedSaleItem);
        }

        private void ExecuteFindSalesCommand() {
            Sales.Clear();
            LoadSaleData();
        }

        public void CreateTicketReport(string dest, Sale currentSale) {
            try {
                var file = new FileInfo(dest);
                file.Directory.Create();
                var writer = new PdfWriter(dest);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                var font = PdfFontFactory.CreateFont(ShellViewModel.FONT, PdfEncodings.IDENTITY_H);
                document.Add(new Paragraph($"Билет №{currentSale.IdSale}:").SetFont(font));
                var ticketList = new List().SetSymbolIndent(12).SetListSymbol("\u2022").SetFont(font);
                ticketList.Add(new ListItem($"Дата заказа: {currentSale.SaleDate.ToString("f", CultureInfo.GetCultureInfo("ru-RU"))}"))
                    .Add(new ListItem($"Отправление: {currentSale.IdTicketNavigation.IdTrainDepartureTownNavigation.TownName}"))
                    .Add(new ListItem($"Прибытие: {currentSale.IdTicketNavigation.IdTrainArrivalTownNavigation.TownName}"))
                    .Add(new ListItem($"Дата отправления: {currentSale.IdTicketNavigation.TicketDate.ToString("F", CultureInfo.GetCultureInfo("ru-RU"))}"))
                    .Add(new ListItem($"Вагон: {currentSale.IdTicketNavigation.IdSeatNavigation.IdWagonNavigation.WagonNumber}"))
                    .Add(new ListItem($"Место: {currentSale.IdTicketNavigation.IdSeatNavigation.Seat1}"))
                    .Add(new ListItem($"Продолжительность поездки: {currentSale.IdTicketNavigation.TicketTravelTime.Hours} ч. {currentSale.IdTicketNavigation.TicketTravelTime.Minutes} мин."))
                    .Add(new ListItem($"Стоимость: {currentSale.TotalPrice}"));
                document.Add(ticketList);

                document.Add(new Paragraph("Информация о пассажире:").SetFont(font));
                var passengerList = new List().SetSymbolIndent(12).SetListSymbol("\u2022").SetFont(font);
                passengerList.Add(new ListItem($"ФИО: {currentSale.IdPassengerNavigation.PassengerFullName}"))
                    .Add(new ListItem($"День рождения: {currentSale.IdPassengerNavigation.PassengerBirthday:d}"))
                    .Add(new ListItem($"Тип паспорта: {currentSale.IdPassengerNavigation.IdPassengerPassportTypeNavigation.PassportType1}"))
                    .Add(new ListItem($"Паспортные данные: {currentSale.IdPassengerNavigation.PassengerPassport}"));
                document.Add(passengerList);
                document.Close();
                var process = new Process {
                    StartInfo = new ProcessStartInfo(dest) {
                        UseShellExecute = true
                    }
                };
                process.Start();
                MessageBox.Show("Билет сохранен в документах");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadSaleData() {
            using var dbContext = new RailwaydbContext();
            Sales = new ObservableCollection<Sale>(dbContext.Sale
                .Include(tic => tic.IdTicketNavigation)
                .Include(arrival => arrival.IdTicketNavigation.IdTrainArrivalTownNavigation)
                .Include(departure => departure.IdTicketNavigation.IdTrainDepartureTownNavigation)
                .Include(seat => seat.IdTicketNavigation.IdSeatNavigation)
                .Include(wagon => wagon.IdTicketNavigation.IdSeatNavigation.IdWagonNavigation)
                .Include(passenger => passenger.IdPassengerNavigation.IdPassengerPassportTypeNavigation)
                .Where(x => x.IdTicketNavigation.TicketDate >= DateTime.Parse(TimeFrom) && x.IdTicketNavigation.TicketDate <= DateTime.Parse(TimeTo) && x.IdPassenger == PassengerProfileViewModel.idPassenger));
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            TimeTo = DateTime.Now.AddDays(31).ToString("d");
            TimeFrom = DateTime.Now.AddDays(-7).ToString("d");
            LoadSaleData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}