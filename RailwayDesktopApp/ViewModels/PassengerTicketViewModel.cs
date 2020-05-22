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
        private DateTime dateStart;
        public DateTime DateStart {
            get => dateStart;
            set => SetProperty(ref dateStart, value);
        }
        private ObservableCollection<FindTrain> tickets;
        public ObservableCollection<FindTrain> Tickets {
            get => tickets;
            set => SetProperty(ref tickets, value);
        }
        private FindTrain selectedTicketItem;
        public FindTrain SelectedTicketItem {
            get => selectedTicketItem;
            set => SetProperty(ref selectedTicketItem, value);
    }
        #endregion

        #region Commands
        public DelegateCommand FindTicketCommand { get; set; }
        public DelegateCommand DetailsCommand { get; set; }
        #endregion

        public static FindTrain currentTicket;

        public PassengerTicketViewModel() {
            FindTicketCommand = new DelegateCommand(ExecuteFindCommand, () => true);
            DetailsCommand = new DelegateCommand(ExecuteDetailsCommand, () => true);
            Tickets = new ObservableCollection<FindTrain>();
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
                var ticketItems = dbContext.Ticket
                    .Include(departure => departure.IdTrainDepartureTownNavigation)
                    .Include(arrival => arrival.IdTrainArrivalTownNavigation)
                    .Include(train => train.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation)
                    .Where(x => x.IdTrainArrivalTown == SelectedArrivalTownItem.IdTrainArrivalTown)
                    .Where(x => x.IdTrainDepartureTown == SelectedDepartureTownItem.IdTrainDepartureTown)
                    .Where(availability => availability.IdSeatNavigation.SeatAvailability)
                    .Where(time => time.TicketDate >= DateTime.Parse(TicketDate))
                    .GroupBy(x => new {
                        ArrivalTown = x.IdTrainArrivalTownNavigation.TownName,
                        DepartureTown = x.IdTrainDepartureTownNavigation.TownName,
                        x.TicketDate,
                        x.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation.TrainName,
                        x.TicketTravelTime
                    })
                    .Select(x => new {
                        x.Key.DepartureTown,
                        x.Key.ArrivalTown,
                        x.Key.TicketDate,
                        x.Key.TrainName,
                        x.Key.TicketTravelTime
                    })
                    .OrderBy(x => x.TicketDate)
                    .ToList();
                foreach (var ticket in ticketItems) {
                    Tickets.Add(
                        new FindTrain {
                            TrainName = ticket.TrainName,
                            DepartureTown = ticket.DepartureTown,
                            ArrivalTown = ticket.ArrivalTown,
                            DepartureTime = ticket.TicketDate,
                            TravelDuration = ticket.TicketTravelTime
                        });
                }
                if (Tickets.Count == 0) {
                    MessageBox.Show("Нет поездов на данный период");
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExecuteDetailsCommand() {
            currentTicket = SelectedTicketItem;
            ShellViewModel.Navigate("PassengerTicketDetailsView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            TicketDate = DateTime.Now.ToString("d");
            DateStart = DateTime.Now;
            LoadData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}