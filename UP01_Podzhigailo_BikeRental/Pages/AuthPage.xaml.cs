
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class AuthPage : Page
    {
        // Счетчик неверных попыток авторизации
        private int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
        }

        // Метод хеширования пароля по методичке
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(
                    hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                        .Select(x => x.ToString("X2")));
            }
        }

        // Переключение между авторизацией и капчей
        public void CaptchaSwitch()
        {
            if (captcha.Visibility == Visibility.Visible)
            {
                LoginTextBox.Clear();
                PasswordInput.Clear();
                captchaInput.Clear();

                captcha.Visibility = Visibility.Collapsed;
                captchaInput.Visibility = Visibility.Collapsed;
                labelCaptcha.Visibility = Visibility.Collapsed;
                submitCaptcha.Visibility = Visibility.Collapsed;

                labelLogin.Visibility = Visibility.Visible;
                labelPass.Visibility = Visibility.Visible;
                LoginTextBox.Visibility = Visibility.Visible;
                txtHintLogin.Visibility = Visibility.Visible;
                PasswordInput.Visibility = Visibility.Visible;
                txtHintPass.Visibility = Visibility.Visible;

                ChangePasswordButton.Visibility = Visibility.Visible;
                LoginButton.Visibility = Visibility.Visible;
                RegisterButton.Visibility = Visibility.Visible;
            }
            else
            {
                CaptchaChange();
                captchaInput.Clear();

                captcha.Visibility = Visibility.Visible;
                captchaInput.Visibility = Visibility.Visible;
                labelCaptcha.Visibility = Visibility.Visible;
                submitCaptcha.Visibility = Visibility.Visible;

                labelLogin.Visibility = Visibility.Collapsed;
                labelPass.Visibility = Visibility.Collapsed;
                LoginTextBox.Visibility = Visibility.Collapsed;
                txtHintLogin.Visibility = Visibility.Collapsed;
                PasswordInput.Visibility = Visibility.Collapsed;
                txtHintPass.Visibility = Visibility.Collapsed;

                ChangePasswordButton.Visibility = Visibility.Collapsed;
                LoginButton.Visibility = Visibility.Collapsed;
                RegisterButton.Visibility = Visibility.Collapsed;
            }
        }

        // Генерация случайной капчи из 6 символов
        public void CaptchaChange()
        {
            string allowchar =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "1234567890";

            Random random = new Random();
            StringBuilder code = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                code.Append(allowchar[random.Next(allowchar.Length)]);
            }

            captcha.Text = code.ToString();
        }

        // Запрет копирования, вырезания и вставки капчи
        private void textBox_PreviewExecuted(
            object sender,
            ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        // Подтверждение ввода капчи
        private void submitCaptcha_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show(
                    "Неверно введена капча",
                    "Ошибка");

                CaptchaChange();
                captchaInput.Clear();
            }
            else
            {
                MessageBox.Show(
                    "Капча введена успешно, можете продолжить авторизацию",
                    "Успех");

                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        // Обработка нажатия кнопки авторизации
        private void LoginButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Введите логин и пароль",
                    "Ошибка");

                return;
            }

            try
            {
                // Хешируем введенный пароль перед сравнением с БД
                string hashedPassword = GetHash(password);

                using (var db = Core.GetContext())
                {
                    var user = db.Users
                        .AsNoTracking()
                        .FirstOrDefault(u =>
                            u.Login == login &&
                            u.PasswordHash == hashedPassword);

                    if (user == null)
                    {
                        failedAttempts++;

                        MessageBox.Show(
                            "Неверный логин или пароль.\n" +
                            "Количество неудачных попыток: " +
                            failedAttempts,
                            "Ошибка авторизации");

                        // После трех ошибок показываем капчу
                        if (failedAttempts >= 3)
                        {
                            CaptchaSwitch();
                        }

                        return;
                    }

                    if (user.Role != "User" &&
                        user.Role != "Admin")
                    {
                        MessageBox.Show(
                            "Учетная запись имеет неизвестную роль",
                            "Ошибка доступа");

                        return;
                    }


                    failedAttempts = 0;

                    MessageBox.Show(
                        "Добро пожаловать, " + user.FullName +
                        "!\nРоль: " + user.Role,
                        "Авторизация успешна",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Переход на страницу в зависимости от роли
                    if (user.Role == "User")
                    {
                        NavigationService?.Navigate(new UserPage(user.UserID));
                    }
                    else if (user.Role == "Admin")
                    {
                        NavigationService?.Navigate(new AdminPage());
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при работе с базой данных:\n" +
                    ex.Message,
                    "Ошибка");
            }
        }

        // Переход на страницу регистрации
        private void RegisterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage());
        }

        // Кнопка смены пароля
        private void ChangePasswordButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBox.Show(
                "Функция смены пароля будет реализована отдельно",
                "Смена пароля");
        }
    }
}
