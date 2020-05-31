using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Views;

namespace RailwayDesktopApp.ViewModels {
    public class AuthorizationViewModel : BindableBase {
        #region Properties
        private string login;
        public string Login {
            get => login;
            set {
                SetProperty(ref login, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        private string password;
        public string Password {
            get => password;
            set {
                SetProperty(ref password, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get; set; }
        #endregion

        public AuthorizationViewModel() {
            LoginCommand = new DelegateCommand(Execute, CanExecute);
        }

        private void Execute() {
            using var dbContext = new RailwaydbContext();
            var item = dbContext.User.FirstOrDefault(x => x.UserLogin == Login);
            if (item != null) {
                var salt = Convert.FromBase64String(item.UserSalt);
                var saltedHash = GenerateSaltedHash(Encoding.UTF8.GetBytes(Password), salt);
                var hash = Convert.FromBase64String(item.UserHash);
                if (!CompareByteArrays(saltedHash, hash)) {
                    MessageBox.Show("Неправильный пароль");
                    return;
                }
                if (item.UserType == "admin") {
                    ((Shell)Application.Current.MainWindow).adminStackPanel.Visibility = Visibility.Visible;
                    ((Shell)Application.Current.MainWindow).authStackPanel.Visibility = Visibility.Hidden;
                    ShellViewModel.Navigate("AdminPassengerView");
                } else {
                    ((Shell)Application.Current.MainWindow).passengerStackPanel.Visibility = Visibility.Visible;
                    ((Shell)Application.Current.MainWindow).authStackPanel.Visibility = Visibility.Hidden;
                    var passenger = dbContext.Passenger
                        .Include(x => x.IdUserNavigation)
                        .FirstOrDefault(x => x.IdUser == item.IdUser);
                    PassengerProfileViewModel.idPassenger = passenger.IdPassenger;
                    ShellViewModel.Navigate("PassengerProfileView");
                }
                MessageBox.Show($"Добро пожаловать, {Login}");
            } else {
                MessageBox.Show("Данного пользователя не существует!");
            }
        }

        public bool CanExecute() {
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        }

        public static byte[] CreateSalt() {
            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
        public static byte[] GenerateSaltedHash(byte[] password, byte[] salt) {
            var algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[password.Length + salt.Length];

            for (int i = 0; i < password.Length; i++) {
                plainTextWithSaltBytes[i] = password[i];
            }
            for (int i = 0; i < salt.Length; i++) {
                plainTextWithSaltBytes[password.Length + i] = salt[i];
            }
            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
        public static bool CompareByteArrays(byte[] array1, byte[] array2) {
            if (array1.Length != array2.Length) {
                return false;
            }

            for (int i = 0; i < array1.Length; i++) {
                if (array1[i] != array2[i]) {
                    return false;
                }
            }
            return true;
        }
    }
}