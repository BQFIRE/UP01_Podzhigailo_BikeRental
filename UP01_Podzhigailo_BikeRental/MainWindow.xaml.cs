
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
        private bool isChangingTheme;

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

            // При первом запуске устанавливаем светлую тему
            ApplyTheme(0);

            MainFrame.Navigate(new Pages.AuthPage());

            DateTimeNow.Text = DateTime.Now.ToString(
                "dd.MM.yyyy HH:mm:ss");
        }

        // Применение выбранной темы
        private void ApplyTheme(int selectedIndex)
        {
            string dictionaryName = selectedIndex == 0
                ? "Dictionary.xaml"
                : "darkDictionary.xaml";

            try
            {
                Uri uri = new Uri(
                    dictionaryName,
                    UriKind.Relative);

                ResourceDictionary resourceDict =
                    Application.LoadComponent(uri) as ResourceDictionary;

                if (resourceDict == null)
                {
                    MessageBox.Show(
                        "Не удалось загрузить словарь темы.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                Application.Current.Resources.MergedDictionaries.Clear();

                Application.Current.Resources.MergedDictionaries.Add(
                    resourceDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки темы:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Обработчик переключения темы
        private void ThemeSelector_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ThemeSelector == null ||
                ThemeSelector.SelectedIndex < 0 ||
                isChangingTheme)
            {
                return;
            }

            ApplyTheme(ThemeSelector.SelectedIndex);
        }

        // Кнопка возврата
        private void BackButton_Click(
            object sender,
            RoutedEventArgs e)
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

            if (!e.Cancel && timer != null)
            {
                timer.Stop();
            }
        }
    }
}
