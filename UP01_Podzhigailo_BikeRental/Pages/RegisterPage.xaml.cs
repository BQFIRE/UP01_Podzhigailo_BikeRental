
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        // Хеширование пароля по методичке: SHA-1
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(
                    hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                        .Select(x => x.ToString("X2")));
            }
        }

        // Регистрация нового пользователя
        private void RegisterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordInput.Password;
            string confirmPassword = ConfirmPasswordInput.Password;

            // Проверка заполнения полей
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show(
                    "Заполните все поля.",
                    "Ошибка регистрации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                MessageBox.Show(
                    "Пароли не совпадают.",
                    "Ошибка регистрации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                using (var db = Core.GetContext())
                {
                    // Проверка, существует ли такой логин
                    bool loginExists = db.Users.Any(
                        u => u.Login == login);

                    if (loginExists)
                    {
                        MessageBox.Show(
                            "Пользователь с таким логином уже существует.",
                            "Ошибка регистрации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    // Создаем пользователя и хешируем пароль
                    var user = new Users
                    {
                        FullName = fullName,
                        Login = login,
                        PasswordHash = GetHash(password),
                        Role = "User",
                        Photo = null
                    };

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                MessageBox.Show(
                    "Регистрация прошла успешно!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                NavigationService?.Navigate(new AuthPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось зарегистрировать пользователя:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Возврат к авторизации
        private void BackButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }
    }
}
