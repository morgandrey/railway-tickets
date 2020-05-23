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
    public class AdminTypeWagonViewModel : BindableBase, INavigationAware {
        #region Properties
        private string wagonType;
        public string WagonType {
            get => wagonType;
            set {
                SetProperty(ref wagonType, value);
                AddWagonTypeCommand.RaiseCanExecuteChanged();
            }
        }
        private double wagonPrice = 1.0;
        public double WagonPrice {
            get => wagonPrice;
            set {
                SetProperty(ref wagonPrice, value);
                AddWagonTypeCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<WagonType> wagonTypes;
        public ObservableCollection<WagonType> WagonTypes {
            get => wagonTypes;
            set => SetProperty(ref wagonTypes, value);
        }
        #endregion

        #region Commands
        private DelegateCommand addWagonTypeCommand;
        public DelegateCommand AddWagonTypeCommand =>
            addWagonTypeCommand ??= new DelegateCommand(ExecuteAddWagonTypeCommand, CanExecuteAddWagonTypeCommand);
        #endregion
        private void ExecuteAddWagonTypeCommand() {
            try {
                using var dbContext = new RailwaydbContext();
                var wagonTypeItem = new WagonType {
                    WagonType1 = WagonType,
                    WagonPrice = WagonPrice
                };
                dbContext.Add(wagonTypeItem);
                dbContext.SaveChanges();
                LoadWagonTypes();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CanExecuteAddWagonTypeCommand() {
            return !string.IsNullOrEmpty(WagonType);
        }
        private void LoadWagonTypes() {
            try {
                using var dbContext = new RailwaydbContext();
                WagonTypes = new ObservableCollection<WagonType>(dbContext.WagonType.ToList());
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            LoadWagonTypes();
        }
        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}