
using System.Windows;
using System.Windows.Controls;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordInput.Password))
            {
                MessageBox.Show(
                    "Введите логин и пароль.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                "Форма заполнена. Подключение к базе данных реализуем на следующем этапе.",
                "Авторизация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void RegisterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Страницу регистрации создадим следующим этапом.",
                "Регистрация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ChangePasswordButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Страницу смены пароля создадим отдельным этапом.",
                "Смена пароля",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
