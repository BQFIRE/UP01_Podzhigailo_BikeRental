
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Отображение текущей даты и времени
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += (s, args) =>
            {
                DateTimeNow.Text = DateTime.Now.ToString(
                    "dd.MM.yyyy HH:mm:ss");
            };

            timer.Start();

            MainFrame.Navigate(new Pages.AuthPage());

            bool connected = Core.TestConnection(out string message);

            MessageBox.Show(
                message,
                connected ? "Подключение к БД" : "Ошибка подключения",
                MessageBoxButton.OK,
                connected ? MessageBoxImage.Information : MessageBoxImage.Error);

            DateTimeNow.Text = DateTime.Now.ToString(
                "dd.MM.yyyy HH:mm:ss");
        }

        // Кнопка возврата
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
            else
            {
                MessageBox.Show(
                    "Предыдущая страница отсутствует.",
                    "Навигация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        // Переключение темы оформления
        private void ThemeSelector_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedIndex < 0)
                return;

            string dictionaryName =
                ThemeSelector.SelectedIndex == 0
                    ? "Dictionary.xaml"
                    : "darkDictionary.xaml";

            var uri = new Uri(
                dictionaryName,
                UriKind.Relative);

            var resourceDict =
                Application.LoadComponent(uri) as ResourceDictionary;

            if (resourceDict == null)
            {
                MessageBox.Show(
                    "Не удалось загрузить выбранную тему.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Application.Current.Resources.MergedDictionaries.Clear();

            Application.Current.Resources.MergedDictionaries.Add(
                resourceDict);
        }

        // Подтверждение выхода
        private void Window_Closing(
            object sender,
            CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
