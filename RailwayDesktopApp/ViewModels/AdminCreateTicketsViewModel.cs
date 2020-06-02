using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class AdminCreateTicketsViewModel : BindableBase, INavigationAware {
        #region Properties
        private ObservableCollection<Train> trains;
        public ObservableCollection<Train> Trains {
            get => trains;
            set => SetProperty(ref trains, value);
        }
        private Train selectedTrain;
        public Train SelectedTrain {
            get => selectedTrain;
            set {
                SetProperty(ref selectedTrain, value);
                ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<TrainDepartureTown> trainDepartureTowns;
        public ObservableCollection<TrainDepartureTown> TrainDepartureTowns {
            get => trainDepartureTowns;
            set => SetProperty(ref trainDepartureTowns, value);
        }
        private TrainDepartureTown selectedTrainDepartureTown;
        public TrainDepartureTown SelectedTrainDepartureTown {
            get => selectedTrainDepartureTown;
            set {
                SetProperty(ref selectedTrainDepartureTown, value);
                ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<TrainArrivalTown> trainArrivalTowns;
        public ObservableCollection<TrainArrivalTown> TrainArrivalTowns {
            get => trainArrivalTowns;
            set => SetProperty(ref trainArrivalTowns, value);
        }
        private TrainArrivalTown selectedTrainArrivalTown;
        public TrainArrivalTown SelectedTrainArrivalTown {
            get => selectedTrainArrivalTown;
            set {
                SetProperty(ref selectedTrainArrivalTown, value);
                ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<WagonType> wagonTypes;
        public ObservableCollection<WagonType> WagonTypes {
            get => wagonTypes;
            set => SetProperty(ref wagonTypes, value);
        }
        private WagonType selectedWagonType;
        public WagonType SelectedWagonType {
            get => selectedWagonType;
            set => SetProperty(ref selectedWagonType, value);
        }
        private ObservableCollection<AddWagon> wagons;
        public ObservableCollection<AddWagon> Wagons {
            get => wagons;
            set => SetProperty(ref wagons, value);
            }

        private AddWagon selectedWagon;
        public AddWagon SelectedWagon {
            get => selectedWagon;
            set => SetProperty(ref selectedWagon, value);
    }
        private DateTime departureTime = DateTime.Now;
        public DateTime DepartureTime {
            get => departureTime;
            set => SetProperty(ref departureTime, value);
        }
        private DateTime travelDuration;
        public DateTime TravelDuration {
            get => travelDuration;
            set {
                SetProperty(ref travelDuration, value);
                ContinueCommand.RaiseCanExecuteChanged();
            }
        }

        private int seatNumber = 1;
        public int SeatNumber {
            get => seatNumber;
            set {
                SetProperty(ref seatNumber, value);
                AddWagonCommand.RaiseCanExecuteChanged();
            }
        }

        private bool trainGridEnabled;
        public bool TrainGridEnabled {
            get => trainGridEnabled;
            set => SetProperty(ref trainGridEnabled, value);
        }
        private bool continueBtnEnabled;
        public bool ContinueBtnEnabled {
            get => continueBtnEnabled;
            set => SetProperty(ref continueBtnEnabled, value);
        }
        private Visibility confirmBtnVisibility;
        public Visibility ConfirmBtnVisibility {
            get => confirmBtnVisibility;
            set => SetProperty(ref confirmBtnVisibility, value);
        }
        private Visibility dataGridVisibility;
        public Visibility DataGridVisibility {
            get => dataGridVisibility;
            set => SetProperty(ref dataGridVisibility, value);
        }
        private Visibility addWagonVisibility;
        public Visibility AddWagonVisibility {
            get => addWagonVisibility;
            set => SetProperty(ref addWagonVisibility, value);
        }
        #endregion
        #region Commands
        private DelegateCommand continueCommand;
        public DelegateCommand ContinueCommand =>
            continueCommand ??= new DelegateCommand(ExecuteContinueCommand, CanExecuteContinueCommand);
        private DelegateCommand createTicketsCommand;
        public DelegateCommand CreateTicketsCommand =>
            createTicketsCommand ??= new DelegateCommand(ExecuteCreateTicketsCommand, CanExecuteCreateTicketsCommand);
        private DelegateCommand addWagonCommand;
        public DelegateCommand AddWagonCommand =>
            addWagonCommand ??= new DelegateCommand(ExecuteAddWagonCommand);
        private DelegateCommand deleteWagonCommand;
        public DelegateCommand DeleteWagonCommand =>
            deleteWagonCommand ??= new DelegateCommand(ExecuteDeleteWagonCommand);
        #endregion
        private void ExecuteAddWagonCommand() {
            Wagons.Add(
                new AddWagon {
                    WagonType = SelectedWagonType.WagonType1,
                    IdWagon = SelectedWagonType.IdWagonType,
                    SeatNumber = SeatNumber
                });
            CreateTicketsCommand.RaiseCanExecuteChanged();
        }
        private void ExecuteCreateTicketsCommand() {
            try {
                using var dbContext = new RailwaydbContext();
                using var transaction = dbContext.Database.BeginTransaction();
                var trainWagons = dbContext.TrainWagon
                    .Where(x => x.IdTrain == SelectedTrain.IdTrain)
                    .ToList();
                var nextTravelCount = 1;
                if (trainWagons.Count > 0) {
                    for (int i = 0; i < trainWagons.Count; i++) {
                        if (nextTravelCount == trainWagons[i].TrainTravelCount) {
                            nextTravelCount++;
                        }
                    }
                }
                var trainWagon = new TrainWagon {
                    IdTrain = SelectedTrain.IdTrain,
                    TrainTravelCount = nextTravelCount
                };
                dbContext.TrainWagon.Add(trainWagon);
                dbContext.SaveChanges();

                var wagonEntities = new List<Wagon>();
                for (int i = 0; i < Wagons.Count; i++) {
                    wagonEntities.Add(new Wagon {
                        IdTrainWagon = trainWagon.IdTrainWagon,
                        WagonNumber = int.Parse($"{i + 1}"),
                        IdWagonType = Wagons[i].IdWagon
                    });
                }
                dbContext.Wagon.AddRange(wagonEntities);
                dbContext.SaveChanges();

                var seats = new List<Seat>();
                for (int i = 0; i < wagonEntities.Count; i++) {
                    for (int j = 1; j <= Wagons[i].SeatNumber; j++) {
                        seats.Add(new Seat {
                            IdWagon = wagonEntities[i].IdWagon,
                            Seat1 = j,
                            SeatAvailability = true
                        });
                    }
                }
                dbContext.Seat.AddRange(seats);
                dbContext.SaveChanges();
                var time = DepartureTime.ToString("G");
                var tickets = new List<Ticket>();
                for (int i = 0; i < seats.Count; i++) {
                    tickets.Add(new Ticket {
                        IdSeat = seats[i].IdSeat,
                        IdTrainDepartureTown = SelectedTrainDepartureTown.IdTrainDepartureTown,
                        IdTrainArrivalTown = SelectedTrainArrivalTown.IdTrainArrivalTown,
                        TicketDate = DateTime.Parse(time),
                        TicketTravelTime = TravelDuration.TimeOfDay
                    });
                }
                dbContext.Ticket.AddRange(tickets);
                dbContext.SaveChanges();
                transaction.Commit();
                ShellViewModel.Navigate("AdminTicketView");
                MessageBox.Show("Новая поездка успешно добавлена!");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExecuteDeleteWagonCommand() {
            Wagons.Remove(SelectedWagon);
            CreateTicketsCommand.RaiseCanExecuteChanged();
        }
        private bool CanExecuteCreateTicketsCommand() {
            if (Wagons == null) {
                return false;
            }
            return Wagons.Count != 0;
        }
        private bool CanExecuteContinueCommand() {
            return SelectedTrainArrivalTown != null &&
                   SelectedTrainDepartureTown != null &&
                   SelectedTrain != null &&
                   TravelDuration.TimeOfDay != TimeSpan.MinValue &&
                   SelectedTrainArrivalTown.TownName != SelectedTrainDepartureTown.TownName;
        }
        private void ExecuteContinueCommand() {
            DataGridVisibility = Visibility.Visible;
            AddWagonVisibility = Visibility.Visible;
            ConfirmBtnVisibility = Visibility.Visible;
            ContinueBtnEnabled = false;
            TrainGridEnabled = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            DataGridVisibility = Visibility.Hidden;
            AddWagonVisibility = Visibility.Hidden;
            ConfirmBtnVisibility = Visibility.Hidden;
            ContinueBtnEnabled = true;
            TrainGridEnabled = true;
            LoadData();
        }

        private void LoadData() {
            try {
                using var dbContext = new RailwaydbContext();
                Wagons = new ObservableCollection<AddWagon>();
                Trains = new ObservableCollection<Train>(dbContext.Train.ToList());
                TrainDepartureTowns =
                    new ObservableCollection<TrainDepartureTown>(dbContext.TrainDepartureTown.ToList());
                TrainArrivalTowns = new ObservableCollection<TrainArrivalTown>(dbContext.TrainArrivalTown.ToList());
                WagonTypes = new ObservableCollection<WagonType>(dbContext.WagonType.ToList());
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}