using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class AdminTicketViewModel : BindableBase {

        #region Properties
        private DateTime timeFrom;
        public DateTime TimeFrom {
            get => timeFrom;
            set => SetProperty(ref timeFrom, value);
        }
        private DateTime timeTo;
        public DateTime TimeTo {
            get => timeTo;
            set => SetProperty(ref timeTo, value);
        }
        private ObservableCollection<FindTrain> ticketModel;
        public ObservableCollection<FindTrain> TicketModel {
            get => ticketModel;
            set => SetProperty(ref ticketModel, value);
        }
        public static FindTrain SelectedTrain { get; set; }
        #endregion

        #region Commands
        private DelegateCommand findTrainCommand;
        public DelegateCommand FindTrainCommand =>
            findTrainCommand ??= new DelegateCommand(ExecuteFindTrainCommand);
        private DelegateCommand createTicketCommand;
        public DelegateCommand CreateTicketCommand =>
            createTicketCommand ??= new DelegateCommand(ExecuteCreateTicketCommand);
        private DelegateCommand trainDetails;
        public DelegateCommand TrainDetails =>
            trainDetails ??= new DelegateCommand(ExecuteTrainDetails);
        #endregion

        public AdminTicketViewModel() {
            TimeFrom = DateTime.Now.AddDays(-7);
            TimeTo = DateTime.Now.AddDays(10);
        }

        private void LoadTrains(DateTime from, DateTime to) {
            try {
                using var dbContext = new RailwaydbContext();
                var trains = dbContext.Ticket
                    .Include(departure => departure.IdTrainDepartureTownNavigation)
                    .Include(arrival => arrival.IdTrainArrivalTownNavigation)
                    .Include(train => train.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation)
                    .Where(time => time.TicketDate >= from && time.TicketDate <= to)
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
                if (trains.Count == 0) {
                    MessageBox.Show("Нет поездов на данный период");
                }
                TicketModel = new ObservableCollection<FindTrain>();
                foreach (var ticket in trains) {
                    TicketModel.Add(
                        new FindTrain {
                            TrainName = ticket.TrainName,
                            DepartureTown = ticket.DepartureTown,
                            ArrivalTown = ticket.ArrivalTown,
                            DepartureTime = ticket.TicketDate,
                            TravelDuration = ticket.TicketTravelTime
                        });
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExecuteCreateTicketCommand() {
            ShellViewModel.Navigate("AdminCreateTicketsView");
        }
        private void ExecuteFindTrainCommand() {
            LoadTrains(TimeFrom, TimeTo);
        }
        private void ExecuteTrainDetails() {
            ShellViewModel.Navigate("AdminTicketDetailsView");
        }
    }
}