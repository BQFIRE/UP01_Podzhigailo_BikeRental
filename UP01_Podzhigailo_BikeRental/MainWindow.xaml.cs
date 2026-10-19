
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace UP01_Podzhigailo_BikeRental
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Запуск часов после открытия окна
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

            DateTimeNow.Text = DateTime.Now.ToString(
                "dd.MM.yyyy HH:mm:ss");
        }

        // Возврат на предыдущую страницу
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

        // Подтверждение закрытия приложения
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
