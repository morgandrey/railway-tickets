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
    public class AdminTrainViewModel : BindableBase, INavigationAware {
        #region Properties
        private string train;
        public string Train {
            get => train;
            set {
                SetProperty(ref train, value);
                AddTrainCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<Train> trains;
        public ObservableCollection<Train> Trains {
            get => trains;
            set => SetProperty(ref trains, value);
        }
        #endregion

        #region Commands
        private DelegateCommand addTrainCommand;
        public DelegateCommand AddTrainCommand =>
            addTrainCommand ??= new DelegateCommand(ExecuteAddTrainCommand, CanExecuteAddTrainCommand);
        #endregion
        private void ExecuteAddTrainCommand() {
            try {
                using var dbContext = new RailwaydbContext();
                var trainItem = new Train {
                    TrainName = Train
                };
                dbContext.Add(trainItem);
                dbContext.SaveChanges();
                LoadTrains();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CanExecuteAddTrainCommand() {
            return !string.IsNullOrEmpty(Train);
        }
        private void LoadTrains() {
            try {
                using var dbContext = new RailwaydbContext();
                Trains = new ObservableCollection<Train>(dbContext.Train.ToList());
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            LoadTrains();
        }
        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}