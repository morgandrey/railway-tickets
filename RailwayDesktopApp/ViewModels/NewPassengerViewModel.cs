using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;
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

        #region Commands
        public DelegateCommand RegistrationCommand { get; set; }
        #endregion

        public NewPassengerViewModel() {
            RegistrationCommand = new DelegateCommand(Execute, CanExecute);
            using var dbContext = new RailwaydbContext();
            PassportType = new ObservableCollection<PassportType>(dbContext.PassportType.ToList());
            Birthday = string.Empty;
        }

        private bool CanExecute() {
            return !string.IsNullOrEmpty(Login)
                   && !string.IsNullOrEmpty(Password)
                   && !string.IsNullOrEmpty(FullName)
                   && SelectedPassportType != null
                   && !string.IsNullOrEmpty(Birthday)
                   && !string.IsNullOrEmpty(PassportData)
                   && long.TryParse(PassportData, out _)
                   && PassportData.Length >= 8
                   && PassportData.Length <= 12;
        }

        private async void Execute() {
            try {
                await using var dbContext = new RailwaydbContext();
                if (dbContext.User.FirstOrDefault(x => x.UserLogin == Login) != null) {
                    MessageBox.Show("Такой пользователь уже существует");
                    return;
                }
                await using var transaction = dbContext.Database.BeginTransaction();
                var salt = AuthorizationViewModel.CreateSalt();
                var hash = AuthorizationViewModel.GenerateSaltedHash(Encoding.UTF8.GetBytes(Password), salt);
                var user = new User {
                    UserLogin = Login,
                    UserHash = Convert.ToBase64String(hash),
                    UserSalt = Convert.ToBase64String(salt),
                    UserType = "passenger"
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
                transaction.Commit();
                ((Shell)Application.Current.MainWindow).passengerStackPanel.Visibility = Visibility.Visible;
                ((Shell)Application.Current.MainWindow).authStackPanel.Visibility = Visibility.Hidden;
                PassengerProfileViewModel.idPassenger = passenger.IdPassenger;
                ShellViewModel.Navigate("PassengerProfileView");
                MessageBox.Show("Пользователь зарегистрирован!");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}