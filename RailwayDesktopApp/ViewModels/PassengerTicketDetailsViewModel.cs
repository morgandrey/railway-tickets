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
    public class PassengerTicketDetailsViewModel : BindableBase {

        #region Properties
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

        public static string ticketInformation { get; set; }
        public static Ticket currentTicket { get; set; }
        #endregion

        #region Commands
        public DelegateCommand SaleCommand { get; set; }
        #endregion

        public PassengerTicketDetailsViewModel() {
            SaleCommand = new DelegateCommand(ExecuteSaleCommand, () => true);
            LoadTicketDetails();
        }

        private void LoadTicketDetails() {
            try {
                using var dbContext = new RailwaydbContext();
                Tickets = new ObservableCollection<Ticket>(
                    dbContext.Ticket
                        .Include(type => type.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation)
                        .Include(train => train.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation)
                        .Include(departure => departure.IdTrainDepartureTownNavigation)
                        .Include(arrival => arrival.IdTrainArrivalTownNavigation)
                        .Where(x => x.IdTrainDepartureTownNavigation.TownName == PassengerTicketViewModel.currentTicket.DepartureTown
                                    && x.IdTrainArrivalTownNavigation.TownName == PassengerTicketViewModel.currentTicket.ArrivalTown
                                    && x.IdSeatNavigation.IdWagonNavigation.IdTrainWagonNavigation.IdTrainNavigation.TrainName == PassengerTicketViewModel.currentTicket.TrainName
                                    && x.TicketDate == PassengerTicketViewModel.currentTicket.DepartureTime
                                    && x.IdSeatNavigation.SeatAvailability)
                        .OrderBy(x => x.IdSeatNavigation.IdWagonNavigation.WagonNumber).ThenBy(x => x.IdSeatNavigation.Seat1));
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExecuteSaleCommand() {
            currentTicket = SelectedTicketItem;
            ticketInformation = $"Дата: {SelectedTicketItem.TicketDate:g}" +
                                $"\nВремя в пути: {SelectedTicketItem.TicketTravelTime:hh} ч. {SelectedTicketItem.TicketTravelTime:mm} мин." +
                                $"\nТип вагона: {SelectedTicketItem.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonType1}" +
                                $"\nВагон: {SelectedTicketItem.IdSeatNavigation.IdWagonNavigation.WagonNumber} Место: {SelectedTicketItem.IdSeatNavigation.Seat1}" +
                                $"\n{SelectedTicketItem.IdTrainDepartureTownNavigation.TownName} - {SelectedTicketItem.IdTrainArrivalTownNavigation.TownName}";
            ShellViewModel.Navigate("PassengerSellView");
        }
    }
}
