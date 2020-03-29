using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp.ViewModels {
    public class PassengerTicketViewModel : BindableBase, INavigationAware {

        #region Properties
        private ObservableCollection<TrainDepartureTown> trainDepartureTowns;
        public ObservableCollection<TrainDepartureTown> TrainDepartureTowns {
            get => trainDepartureTowns;
            set => SetProperty(ref trainDepartureTowns, value);
        }
        private ObservableCollection<TrainArrivalTown> trainArrivalTowns;
        public ObservableCollection<TrainArrivalTown> TrainArrivalTowns {
            get => trainArrivalTowns;
            set => SetProperty(ref trainArrivalTowns, value);
        }
        private TrainDepartureTown selectedDepartureTownItem;
        public TrainDepartureTown SelectedDepartureTownItem {
            get => selectedDepartureTownItem;
            set => SetProperty(ref selectedDepartureTownItem, value);
        }
        private TrainArrivalTown selectedArrivalTownItem;
        public TrainArrivalTown SelectedArrivalTownItem {
            get => selectedArrivalTownItem;
            set => SetProperty(ref selectedArrivalTownItem, value);
        }
        private string ticketDate;
        public string TicketDate {
            get => ticketDate;
            set => SetProperty(ref ticketDate, value);
        }
        private ObservableCollection<Ticket> tickets;
        public ObservableCollection<Ticket> Tickets {
            get => tickets;
            set => SetProperty(ref tickets, value);
        }
        private Ticket selectedTicketItem;
        public Ticket SelectedTicketItem {
            get => selectedTicketItem;
            set => SetProperty(ref selectedTicketItem, value);
    }
        #endregion

        #region Commands
        public DelegateCommand FindTicketCommand { get; set; }
        public DelegateCommand SaleCommand { get; set; }
        #endregion

        public static Ticket currentTicket;
        public static string ticketInformation;

        public PassengerTicketViewModel() {
            FindTicketCommand = new DelegateCommand(ExecuteFindCommand, () => true);
            SaleCommand = new DelegateCommand(ExecuteSaleCommand, () => true);
            Tickets = new ObservableCollection<Ticket>();
        }
        private void LoadData() {
            using var dbContext = new RailwaydbContext();
            TrainDepartureTowns = new ObservableCollection<TrainDepartureTown>(dbContext.TrainDepartureTown.ToList());
            TrainArrivalTowns = new ObservableCollection<TrainArrivalTown>(dbContext.TrainArrivalTown.ToList());
            Tickets.Clear();
        }
        private void ExecuteFindCommand() {
            Tickets.Clear();
            if (DateTime.Parse(TicketDate) < DateTime.Now.Date ) {
                MessageBox.Show("Нет поездов на данный период");
                return;
            }
            using var dbContext = new RailwaydbContext();
            try {
                var items = dbContext.Ticket
                    .Include(s => s.IdTrainArrivalTownNavigation)
                    .Include(d => d.IdTrainDepartureTownNavigation)
                    .Include(seat => seat.IdSeatNavigation)
                    .Include(wag => wag.IdSeatNavigation.IdWagonNavigation)
                    .Include(wagtype => wagtype.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation)
                    .Include(train => train.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation)
                    .Where(ariv => ariv.IdTrainArrivalTown == SelectedArrivalTownItem.IdTrainArrivalTown)
                    .Where(depart => depart.IdTrainDepartureTown == SelectedDepartureTownItem.IdTrainDepartureTown)
                    .Where(availability => availability.IdSeatNavigation.SeatAvailability)
                    .Where(time => time.TicketDate >= DateTime.Parse(TicketDate)).ToList();
                Tickets.AddRange(items);
                if (Tickets.Count == 0) {
                    MessageBox.Show("Нет поездов на данный период");
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExecuteSaleCommand() {
            ((PassengerShell)Application.Current.MainWindow).passengerGrid.Visibility = Visibility.Hidden;
            currentTicket = SelectedTicketItem;
            ticketInformation = $"Дата: {SelectedTicketItem.TicketDate:g}\nВремя в пути: {SelectedTicketItem.TicketTravelTime:hh} ч. {SelectedTicketItem.TicketTravelTime:mm} мин.\nТип вагона: {SelectedTicketItem.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonType1}\nВагон: {SelectedTicketItem.IdSeatNavigation.IdWagonNavigation.WagonNumber} Место: {SelectedTicketItem.IdSeatNavigation.Seat1}\n{SelectedTicketItem.IdTrainDepartureTownNavigation.TownName} - {SelectedTicketItem.IdTrainArrivalTownNavigation.TownName}";
            PassengerShellViewModel.Navigate("PassengerSellView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            TicketDate = DateTime.Now.ToString("d");
            LoadData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}