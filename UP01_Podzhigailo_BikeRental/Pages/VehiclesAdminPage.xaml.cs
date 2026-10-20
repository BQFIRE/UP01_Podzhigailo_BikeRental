using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class VehiclesAdminPage : Page
    {
        public VehiclesAdminPage()
        {
            InitializeComponent();
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            try
            {
                using (var db = Core.GetContext())
                {
                    var search = SearchTextBox.Text.Trim().ToLower();

                    var vehicles = db.Vehicles
                        .Where(v =>
                            search == "" ||
                            (v.Model != null && v.Model.ToLower().Contains(search)) ||
                            (v.SerialNumber != null && v.SerialNumber.ToLower().Contains(search)))
                        .OrderBy(v => v.VehicleID)
                        .ToList();

                    VehiclesGrid.ItemsSource = vehicles;
                    CountTextBlock.Text =
                        "Количество транспортных средств: " + vehicles.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsInitialized)
                LoadVehicles();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            LoadVehicles();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditVehicle(null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = VehiclesGrid.SelectedItem as Vehicles;

            if (selected == null)
            {
                MessageBox.Show("Выберите транспорт в таблице.");
                return;
            }

            EditVehicle(selected.VehicleID);
        }

        private void EditVehicle(int? vehicleID)
        {
            using (var db = Core.GetContext())
            {
                var vehicle = vehicleID.HasValue
                    ? db.Vehicles.FirstOrDefault(v => v.VehicleID == vehicleID.Value)
                    : new Vehicles();

                if (vehicle == null)
                {
                    MessageBox.Show("Транспорт не найден.");
                    return;
                }

                var types = db.VehicleTypes.OrderBy(t => t.Name).ToList();
                var stations = db.Stations
                    .Where(s => s.IsActive || s.StationID == vehicle.StationID)
                    .OrderBy(s => s.Name)
                    .ToList();

                if (types.Count == 0 || stations.Count == 0)
                {
                    MessageBox.Show("Добавьте тип транспорта и активную станцию.");
                    return;
                }

                var form = new StackPanel { Margin = new Thickness(15) };

                var model = AddField(form, "Модель", vehicle.Model);
                var serial = AddField(form, "Серийный номер", vehicle.SerialNumber);
                var status = AddField(form, "Статус", vehicle.Status ?? "Available");
                var price = AddField(form, "Цена за час",
                    vehicle.PricePerHour.ToString());
                var battery = AddField(form, "Заряд (0–100)",
                    vehicle.BatteryLevel.ToString());

                var typeBox = AddCombo(form, "Тип транспорта", types,
                    "TypeID", "Name", vehicleID.HasValue ? vehicle.TypeID : (int?)null);

                var stationBox = AddCombo(form, "Станция", stations,
                    "StationID", "Name", vehicleID.HasValue ? vehicle.StationID : (int?)null);

                var window = new Window
                {
                    Title = vehicleID.HasValue ? "Изменение транспорта" : "Добавление транспорта",
                    Content = form,
                    Width = 350,
                    SizeToContent = SizeToContent.Height,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this)
                };

                var save = new Button
                {
                    Content = "Сохранить",
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 12, 0, 0)
                };

                save.Click += (s, e) =>
                {
                    decimal priceValue;
                    byte batteryValue;

                    if (string.IsNullOrWhiteSpace(model.Text) ||
                        string.IsNullOrWhiteSpace(serial.Text) ||
                        string.IsNullOrWhiteSpace(status.Text))
                    {
                        MessageBox.Show("Заполните все поля.");
                        return;
                    }

                    if (!decimal.TryParse(price.Text, out priceValue) ||
                        priceValue < 0 ||
                        !byte.TryParse(battery.Text, out batteryValue) ||
                        batteryValue > 100)
                    {
                        MessageBox.Show("Проверьте цену и уровень заряда.");
                        return;
                    }

                    if (typeBox.SelectedValue == null ||
                        stationBox.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите тип и станцию.");
                        return;
                    }

                    if (db.Vehicles.Any(v =>
                        v.SerialNumber == serial.Text.Trim() &&
                        v.VehicleID != vehicle.VehicleID))
                    {
                        MessageBox.Show("Такой серийный номер уже существует.");
                        return;
                    }

                    vehicle.Model = model.Text.Trim();
                    vehicle.SerialNumber = serial.Text.Trim();
                    vehicle.Status = status.Text.Trim();
                    vehicle.PricePerHour = priceValue;
                    vehicle.BatteryLevel = batteryValue;
                    vehicle.TypeID = (int)typeBox.SelectedValue;
                    vehicle.StationID = (int)stationBox.SelectedValue;

                    if (!vehicleID.HasValue)
                        db.Vehicles.Add(vehicle);

                    try
                    {
                        db.SaveChanges();
                        window.DialogResult = true;
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка сохранения: " + ex.Message);
                    }
                };

                form.Children.Add(save);

                if (window.ShowDialog() == true)
                    LoadVehicles();
            }
        }

        private TextBox AddField(Panel panel, string label, string value)
        {
            panel.Children.Add(new TextBlock
            {
                Text = label,
                Margin = new Thickness(0, 6, 0, 3)
            });

            var box = new TextBox
            {
                Text = value ?? "",
                Padding = new Thickness(5)
            };

            panel.Children.Add(box);
            return box;
        }

        private ComboBox AddCombo<T>(
            Panel panel, string label, System.Collections.Generic.List<T> items,
            string valuePath, string displayPath, int? selectedID)
        {
            panel.Children.Add(new TextBlock
            {
                Text = label,
                Margin = new Thickness(0, 6, 0, 3)
            });

            var combo = new ComboBox
            {
                ItemsSource = items,
                SelectedValuePath = valuePath,
                DisplayMemberPath = displayPath,
                Padding = new Thickness(5)
            };

            if (selectedID.HasValue)
                combo.SelectedValue = selectedID.Value;
            else
                combo.SelectedIndex = 0;

            panel.Children.Add(combo);
            return combo;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = VehiclesGrid.SelectedItem as Vehicles;

            if (selected == null)
            {
                MessageBox.Show("Выберите транспорт в таблице.");
                return;
            }

            if (MessageBox.Show(
                "Удалить выбранный транспорт?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = Core.GetContext())
                {
                    var vehicle = db.Vehicles
                        .FirstOrDefault(v => v.VehicleID == selected.VehicleID);

                    if (vehicle != null)
                    {
                        db.Vehicles.Remove(vehicle);
                        db.SaveChanges();
                    }
                }

                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось удалить транспорт. Возможно, он участвует в аренде.\n" +
                    ex.Message);
            }
        }
    }
}
