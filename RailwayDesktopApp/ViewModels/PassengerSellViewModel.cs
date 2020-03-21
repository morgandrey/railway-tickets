using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Models.Data;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp.ViewModels {
    public class PassengerSellViewModel : BindableBase, INavigationAware {

        #region Properties
        private ObservableCollection<Discount> discounts;
        public ObservableCollection<Discount> Discounts {
            get => discounts;
            set => SetProperty(ref discounts, value);
        }
        private Discount selectedDiscountItem;
        public Discount SelectedDiscountItem {
            get => selectedDiscountItem;
            set {
                SetProperty(ref selectedDiscountItem, value);
                ApplySaleCommand.RaiseCanExecuteChanged();
            }
        }

        private string ticketInformation;
        public string TicketInformation {
            get => ticketInformation;
            set => SetProperty(ref ticketInformation, value);
        }
        private string priceInformation;
        public string PriceInformation {
            get => priceInformation;
            set => SetProperty(ref priceInformation, value);
        }

        #endregion

        #region Commands
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand ApplySaleCommand { get; set; }
        public DelegateCommand DiscountSelectionChangedCommand { get; set; }
        #endregion

        public PassengerSellViewModel() {
            BackCommand = new DelegateCommand(ExecuteBackCommand, () => true);
            ApplySaleCommand = new DelegateCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            DiscountSelectionChangedCommand = new DelegateCommand(ExecuteDiscountSelectionChangedCommand, ()=> true);
        }

        private void ExecuteDiscountSelectionChangedCommand() {
            if (SelectedDiscountItem == null) {
                SelectedDiscountItem = Discounts[0];
            }
            PriceInformation = $"Цена(Скидка: { SelectedDiscountItem.DiscountName}): { PassengerTicketViewModel.currentTicket.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonPrice * SelectedDiscountItem.DiscountMultiply}";
        }

        private void LoadData() {
            using var dbContext = new RailwaydbContext();
            Discounts = new ObservableCollection<Discount>(dbContext.Discount.ToList());
            if (Discounts.Count != 0) {
                SelectedDiscountItem = Discounts[0];
            }
        }

        private async void ExecuteApplyCommand() {
            await using var dbContext = new RailwaydbContext();
            try {
                var saleItem = new Sale {
                    IdPassenger = PassengerProfileViewModel.idPassenger,
                    IdTicket = PassengerTicketViewModel.currentTicket.IdTicket,
                    IdDiscount = SelectedDiscountItem.IdDiscount,
                    SaleDate = DateTime.Now,
                    TotalPrice = SelectedDiscountItem.DiscountMultiply * PassengerTicketViewModel.currentTicket.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonPrice
                };
                await dbContext.AddAsync(saleItem);
                PassengerTicketViewModel.currentTicket.IdSeatNavigation.SeatAvailability = false; // Бронируем место в вагоне
                dbContext.Entry(PassengerTicketViewModel.currentTicket.IdSeatNavigation).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                ((PassengerShell) Application.Current.MainWindow).passengerGrid.Visibility = Visibility.Visible;
                PassengerShellViewModel.Navigate("PassengerHistoryView");
                MessageBox.Show("Билет забронирован!");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CanExecuteApplyCommand() {
            return SelectedDiscountItem != null;
        }

        private static void ExecuteBackCommand() {
            ((PassengerShell)Application.Current.MainWindow).passengerGrid.Visibility = Visibility.Visible;
            PassengerShellViewModel.Navigate("PassengerTicketView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            TicketInformation = PassengerTicketViewModel.ticketInformation;
            LoadData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}