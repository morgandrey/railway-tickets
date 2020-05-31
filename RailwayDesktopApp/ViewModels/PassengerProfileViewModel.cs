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
    public class PassengerProfileViewModel : BindableBase, INavigationAware {

        #region Properties
        private string login;
        public string Login {
            get => login;
            set {
                SetProperty(ref login, value);
                ChangeProfileCommand.RaiseCanExecuteChanged();
            }
        }
        private string fullName;
        public string FullName {
            get => fullName;
            set {
                SetProperty(ref fullName, value);
                ChangeProfileCommand.RaiseCanExecuteChanged();
            }
        }

        private string birthday;
        public string Birthday {
            get => birthday;
            set => SetProperty(ref birthday, value);
        }
        private ObservableCollection<PassportType> passportType;
        public ObservableCollection<PassportType> PassportType {
            get => passportType;
            set => SetProperty(ref passportType, value);
        }
        private PassportType selectedPassportType;
        public PassportType SelectedPassportType {
            get => selectedPassportType;
            set => SetProperty(ref selectedPassportType, value);
        }
        private string passportData;
        public string PassportData {
            get => passportData;
            set {
                SetProperty(ref passportData, value);
                ChangeProfileCommand.RaiseCanExecuteChanged();
            }
        }

        public static int idPassenger;
        private Passenger currentPassenger;
        #endregion

        #region Commands
        public DelegateCommand ChangeProfileCommand { get; set; }
        public DelegateCommand ChangePassengerCommand { get; set; }
        #endregion

        public PassengerProfileViewModel() {
            ChangeProfileCommand = new DelegateCommand(Execute, CanExecute);
            ChangePassengerCommand = new DelegateCommand(NewPassengerExecute, () => true);
        }

        private static void NewPassengerExecute() {
            ((Shell)Application.Current.MainWindow).passengerStackPanel.Visibility = Visibility.Hidden;
            ((Shell)Application.Current.MainWindow).authStackPanel.Visibility = Visibility.Visible;
            ShellViewModel.Navigate("AuthorizationView");
        }

        private void LoadProfileData() {
            using var dbContext = new RailwaydbContext();
            PassportType = new ObservableCollection<PassportType>(dbContext.PassportType.ToList());
            Login = currentPassenger.IdUserNavigation.UserLogin;
            FullName = currentPassenger.PassengerFullName;
            Birthday = currentPassenger.PassengerBirthday.ToString("d");
            SelectedPassportType = PassportType.First(x => x.IdPassportType == currentPassenger.IdPassengerPassportType);
            PassportData = currentPassenger.PassengerPassport;
        }

        private bool CanExecute() {
            return !string.IsNullOrEmpty(Login)
                   && !string.IsNullOrEmpty(FullName)
                   && !string.IsNullOrEmpty(PassportData)
                   && long.TryParse(PassportData, out _)
                   && PassportData.Length >= 8
                   && PassportData.Length <= 12;
        }

        private void Execute() {
            try {
                using var dbContext = new RailwaydbContext();
                currentPassenger.IdUserNavigation.UserLogin = Login;
                currentPassenger.PassengerFullName = FullName;
                currentPassenger.PassengerBirthday = DateTime.Parse(Birthday);
                currentPassenger.IdPassengerPassportType = SelectedPassportType.IdPassportType;
                currentPassenger.PassengerPassport = PassportData;
                dbContext.Entry(currentPassenger.IdUserNavigation).State = EntityState.Modified;
                dbContext.Entry(currentPassenger).State = EntityState.Modified;
                dbContext.SaveChanges();
                MessageBox.Show("Данные изменены!");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            using var dbContext = new RailwaydbContext();
            currentPassenger = dbContext.Passenger
                .Include(usr => usr.IdUserNavigation)
                .First(x => x.IdPassenger == idPassenger);
            LoadProfileData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}