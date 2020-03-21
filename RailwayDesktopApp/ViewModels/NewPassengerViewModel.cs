using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using RailwayDesktopApp.Models.Data;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp.ViewModels {
    public class NewPassengerViewModel : BindableBase {

        #region Properties
        private string login;
        public string Login {
            get => login;
            set {
                SetProperty(ref login, value);
                RegistrationCommand.RaiseCanExecuteChanged();
            }
        }
        private string password;
        public string Password {
            get => password;
            set {
                SetProperty(ref password, value);
                RegistrationCommand.RaiseCanExecuteChanged();
            }
        }
        private string fullName;
        public string FullName {
            get => fullName;
            set {
                SetProperty(ref fullName, value);
                RegistrationCommand.RaiseCanExecuteChanged();
            }
        }

        private string birthday;
        public string Birthday {
            get => birthday;
            set {
                SetProperty(ref birthday, value);
                RegistrationCommand.RaiseCanExecuteChanged();
            }
        }
        private ObservableCollection<PassportType> passportType;
        public ObservableCollection<PassportType> PassportType {
            get => passportType;
            set => SetProperty(ref passportType, value);
        }
        private PassportType selectedPassportType;
        public PassportType SelectedPassportType {
            get => selectedPassportType;
            set {
                SetProperty(ref selectedPassportType, value);
                RegistrationCommand.RaiseCanExecuteChanged();
            }
        }
        private string passportData;
        public string PassportData {
            get => passportData;
            set {
                SetProperty(ref passportData, value);
                RegistrationCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        public DelegateCommand RegistrationCommand { get; set; }
        public NewPassengerViewModel() {
            RegistrationCommand = new DelegateCommand(Execute, CanExecute);
            using var dbContext = new RailwaydbContext();
            PassportType = new ObservableCollection<PassportType>(dbContext.PassportType.ToList());
            Birthday = string.Empty;
        }

        private bool CanExecute() {
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(FullName) &&
                   SelectedPassportType != null &&
                   !string.IsNullOrEmpty(Birthday) && !string.IsNullOrEmpty(PassportData);
        }

        private async void Execute() {
            await using var dbContext = new RailwaydbContext();
            var salt = AuthorizationViewModel.CreateSalt();
            var hash = AuthorizationViewModel.GenerateSaltedHash(Encoding.UTF8.GetBytes(Password), salt);
            try {
                var user = new User {
                    UserLogin = Login,
                    UserHash = Convert.ToBase64String(hash),
                    UserSalt = Convert.ToBase64String(salt),
                    UserType = true
                };
                await dbContext.AddAsync(user);
                await dbContext.SaveChangesAsync();
                var passenger = new Passenger {
                    PassengerFullName = FullName,
                    PassengerBirthday = DateTime.Parse(Birthday),
                    IdPassengerPassportType = SelectedPassportType.IdPassportType,
                    PassengerPassport = PassportData,
                    IdUser = user.IdUser
                };
                await dbContext.AddAsync(passenger);
                await dbContext.SaveChangesAsync();
                ((PassengerShell)Application.Current.MainWindow).passengerGrid.Visibility = Visibility.Visible;
                ((PassengerShell)Application.Current.MainWindow).authGrid.Visibility = Visibility.Hidden;
                PassengerProfileViewModel.idPassenger = passenger.IdPassenger;
                PassengerShellViewModel.Navigate("PassengerProfileView");
                MessageBox.Show("Пользователь зарегистрирован!");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}