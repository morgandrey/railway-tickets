using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class AdminTownViewModel : BindableBase, INavigationAware {
        #region Properties
        private string town;
        public string Town {
            get => town;
            set {
                SetProperty(ref town, value);
                AddTownCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<TrainArrivalTown> towns;
        public ObservableCollection<TrainArrivalTown> Towns {
            get => towns;
            set => SetProperty(ref towns, value);
        }
        #endregion

        #region Commands
        private DelegateCommand addTownCommand;
        public DelegateCommand AddTownCommand =>
            addTownCommand ??= new DelegateCommand(ExecuteAddTownCommand, CanExecuteAddTownCommand);
        #endregion
        private void ExecuteAddTownCommand() {
            try {
                using var dbContext = new RailwaydbContext();
                using var transaction = dbContext.Database.BeginTransaction();
                var arrivalTown = new TrainArrivalTown {
                    TownName = Town
                };
                var departureTown = new TrainDepartureTown {
                    TownName = Town
                };
                dbContext.Add(arrivalTown);
                dbContext.Add(departureTown);
                dbContext.SaveChanges();
                transaction.Commit();
                LoadTowns();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CanExecuteAddTownCommand() {
            return !string.IsNullOrEmpty(Town);
        }
        private void LoadTowns() {
            try {
                using var dbContext = new RailwaydbContext();
                Towns = new ObservableCollection<TrainArrivalTown>(dbContext.TrainArrivalTown.ToList());
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            LoadTowns();
        }
        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}