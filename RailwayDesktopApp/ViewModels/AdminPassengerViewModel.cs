using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class AdminPassengerViewModel : BindableBase, INavigationAware {
        private ObservableCollection<Passenger> passengers;
        public ObservableCollection<Passenger> Passengers {
            get => passengers;
            set => SetProperty(ref passengers, value);
        }

        private void LoadPassengerData() {
            try {
                using var dbContext = new RailwaydbContext();
                Passengers = new ObservableCollection<Passenger>(dbContext.Passenger
                    .Include(user => user.IdUserNavigation)
                    .Include(type => type.IdPassengerPassportTypeNavigation)
                    .ToList());
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        public void OnNavigatedTo(NavigationContext navigationContext) {
            LoadPassengerData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}