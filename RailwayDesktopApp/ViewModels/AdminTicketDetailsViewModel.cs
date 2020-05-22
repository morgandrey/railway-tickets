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

namespace RailwayDesktopApp.ViewModels {
    public class AdminTicketDetailsViewModel : BindableBase, INavigationAware {
        #region Properties
        private Ticket selectedTicket;
        public Ticket SelectedTicket {
            get => selectedTicket;
            set => SetProperty(ref selectedTicket, value);
        }

        private ObservableCollection<Ticket> tickets;
        public ObservableCollection<Ticket> Tickets {
            get => tickets;
            set => SetProperty(ref tickets, value);
        }
        #endregion
        #region Commands
        private DelegateCommand deleteTicketCommand;
        public DelegateCommand DeleteTicketCommand =>
            deleteTicketCommand ??= new DelegateCommand(ExecuteDeleteTicketCommand);
        #endregion

        private void LoadTrainDetails() {
            try {
                using var dbContext = new RailwaydbContext();
                Tickets = new ObservableCollection<Ticket>(dbContext.Ticket
                    .Include(type => type.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation)
                    .Include(train => train.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation)
                    .Include(departure => departure.IdTrainDepartureTownNavigation)
                    .Include(arrival => arrival.IdTrainArrivalTownNavigation)
                    .Where(x => x.IdTrainDepartureTownNavigation.TownName == AdminTicketViewModel.SelectedTrain.DepartureTown
                                && x.IdTrainArrivalTownNavigation.TownName == AdminTicketViewModel.SelectedTrain.ArrivalTown
                                && x.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation.TrainName == AdminTicketViewModel.SelectedTrain.TrainName
                                && x.TicketDate == AdminTicketViewModel.SelectedTrain.DepartureTime)
                    .OrderBy(x => x.IdSeatNavigation.IdWagonNavigation.WagonNumber).ThenBy(x => x.IdSeatNavigation.Seat1));
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExecuteDeleteTicketCommand() {
            try {
                using var dbContext = new RailwaydbContext();
                using var transaction1 = dbContext.Database.BeginTransaction();
                dbContext.Entry(SelectedTicket.IdSeatNavigation).State = EntityState.Deleted;
                dbContext.Entry(SelectedTicket).State = EntityState.Deleted;
                var seats = dbContext.Seat
                    .Where(x => x.IdWagon == SelectedTicket.IdSeatNavigation.IdWagonNavigation.IdWagon)
                    .ToList();
                if (seats.Count == 1) {
                    dbContext.Entry(SelectedTicket.IdSeatNavigation.IdWagonNavigation).State = EntityState.Deleted;
                    var wagons = dbContext.Wagon
                        .Where(x => x.IdTrainWagonNavigation.IdTrainWagon ==
                                    SelectedTicket.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation
                                        .IdTrainWagon)
                        .ToList();
                    if (wagons.Count == 1) {
                        dbContext.Entry(SelectedTicket.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation)
                                .State =
                            EntityState.Deleted;
                    }
                }
                dbContext.SaveChanges();
                transaction1.Commit();
                LoadTrainDetails();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            LoadTrainDetails();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}