
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class StationsAdminPage : Page
    {
        public StationsAdminPage()
        {
            InitializeComponent();
            LoadStations();
        }

        private void LoadStations()
        {
            try
            {
                string search = SearchTextBox.Text.Trim().ToLower();

                using (var db = Core.GetContext())
                {
                    StationsGrid.ItemsSource = db.Stations
                        .Where(s =>
                            search == "" ||
                            (s.Name != null && s.Name.ToLower().Contains(search)) ||
                            (s.Address != null && s.Address.ToLower().Contains(search)))
                        .OrderBy(s => s.StationID)
                        .ToList();

                    CountTextBlock.Text =
                        "Количество станций: " + StationsGrid.Items.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки станций:\n" + ex.Message);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsInitialized)
                LoadStations();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStations();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditStation(null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = StationsGrid.SelectedItem as Stations;

            if (selected == null)
            {
                MessageBox.Show("Выберите станцию.");
                return;
            }

            EditStation(selected.StationID);
        }

        private void EditStation(int? stationID)
        {
            using (var db = Core.GetContext())
            {
                var station = stationID.HasValue
                    ? db.Stations.FirstOrDefault(s => s.StationID == stationID.Value)
                    : new Stations { IsActive = true };

                if (station == null)
                {
                    MessageBox.Show("Станция не найдена.");
                    return;
                }

                var panel = new StackPanel { Margin = new Thickness(15) };

                panel.Children.Add(new TextBlock { Text = "Название станции" });
                var nameBox = new TextBox
                {
                    Text = station.Name ?? "",
                    Margin = new Thickness(0, 4, 0, 10),
                    Padding = new Thickness(6)
                };
                panel.Children.Add(nameBox);

                panel.Children.Add(new TextBlock { Text = "Адрес" });
                var addressBox = new TextBox
                {
                    Text = station.Address ?? "",
                    Margin = new Thickness(0, 4, 0, 10),
                    Padding = new Thickness(6)
                };
                panel.Children.Add(addressBox);

                var activeBox = new CheckBox
                {
                    Content = "Станция активна",
                    IsChecked = station.IsActive,
                    Margin = new Thickness(0, 4, 0, 10)
                };
                panel.Children.Add(activeBox);

                var window = new Window
                {
                    Title = stationID.HasValue ? "Редактирование станции" : "Добавление станции",
                    Content = panel,
                    Width = 350,
                    SizeToContent = SizeToContent.Height,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this)
                };

                var saveButton = new Button
                {
                    Content = "Сохранить",
                    Padding = new Thickness(12, 7, 12, 7),
                    Margin = new Thickness(0, 10, 0, 0)
                };

                saveButton.Click += (s, e) =>
                {
                    string name = nameBox.Text.Trim();
                    string address = addressBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(address))
                    {
                        MessageBox.Show("Заполните название и адрес.");
                        return;
                    }

                    bool duplicate = db.Stations.Any(x =>
                        x.Name == name && x.StationID != station.StationID);

                    if (duplicate)
                    {
                        MessageBox.Show("Станция с таким названием уже существует.");
                        return;
                    }

                    station.Name = name;
                    station.Address = address;
                    station.IsActive = activeBox.IsChecked == true;

                    if (!stationID.HasValue)
                        db.Stations.Add(station);

                    try
                    {
                        db.SaveChanges();
                        window.DialogResult = true;
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка сохранения:\n" + ex.Message);
                    }
                };

                panel.Children.Add(saveButton);

                if (window.ShowDialog() == true)
                    LoadStations();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = StationsGrid.SelectedItem as Stations;

            if (selected == null)
            {
                MessageBox.Show("Выберите станцию.");
                return;
            }

            if (MessageBox.Show(
                "Удалить станцию \"" + selected.Name + "\"?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = Core.GetContext())
                {
                    var station = db.Stations.FirstOrDefault(
                        s => s.StationID == selected.StationID);

                    if (station == null)
                        return;

                    // Не удаляем станцию, если на неё ссылается транспорт.
                    if (db.Vehicles.Any(v => v.StationID == station.StationID))
                    {
                        MessageBox.Show(
                            "На этой станции есть транспорт. " +
                            "Сначала переместите его на другую станцию.",
                            "Удаление невозможно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    db.Stations.Remove(station);
                    db.SaveChanges();
                }

                LoadStations();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления станции:\n" + ex.Message);
            }
        }
    }
}
