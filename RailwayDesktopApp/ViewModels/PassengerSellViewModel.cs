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
            PriceInformation = $"Цена(Скидка: { SelectedDiscountItem.DiscountName}): { PassengerTicketDetailsViewModel.currentTicket.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonPrice * SelectedDiscountItem.DiscountMultiply}";
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
                    IdTicket = PassengerTicketDetailsViewModel.currentTicket.IdTicket,
                    IdDiscount = SelectedDiscountItem.IdDiscount,
                    SaleDate = DateTime.Now,
                    TotalPrice = SelectedDiscountItem.DiscountMultiply * PassengerTicketDetailsViewModel.currentTicket.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonPrice
                };
                await dbContext.AddAsync(saleItem);
                PassengerTicketDetailsViewModel.currentTicket.IdSeatNavigation.SeatAvailability = false; // Бронируем место в вагоне
                dbContext.Entry(PassengerTicketDetailsViewModel.currentTicket.IdSeatNavigation).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                ShellViewModel.Navigate("PassengerHistoryView");
                MessageBox.Show("Билет забронирован!");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CanExecuteApplyCommand() {
            return SelectedDiscountItem != null;
        }

        private static void ExecuteBackCommand() {
            ShellViewModel.Navigate("PassengerTicketView");
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            LoadData();
            TicketInformation = PassengerTicketDetailsViewModel.ticketInformation;
            PriceInformation = $"Цена(Скидка: { SelectedDiscountItem.DiscountName}): { PassengerTicketDetailsViewModel.currentTicket.IdSeatNavigation.IdWagonNavigation.IdWagonTypeNavigation.WagonPrice * SelectedDiscountItem.DiscountMultiply}";
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}