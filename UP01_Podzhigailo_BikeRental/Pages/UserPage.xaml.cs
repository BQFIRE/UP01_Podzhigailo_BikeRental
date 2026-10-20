
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class UserPage : Page
    {
        private readonly int userID;

        public UserPage(int userID)
        {
            InitializeComponent();
            this.userID = userID;
            LoadVehicles();
            LoadActiveRental();
        }

        private void LoadVehicles()
        {
            try
            {
                using (var db = Core.GetContext())
                {
                    VehiclesGrid.ItemsSource = db.Vehicles
                        .Where(v => v.Status == "Available")
                        .OrderBy(v => v.VehicleID)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки транспорта:\n" + ex.Message);
            }
        }

        private void LoadActiveRental()
        {
            try
            {
                using (var db = Core.GetContext())
                {
                    var rental = db.Rentals
                        .Where(r => r.UserID == userID &&
                                    r.EndTime == null)
                        .OrderByDescending(r => r.StartTime)
                        .FirstOrDefault();

                    if (rental == null)
                    {
                        RentalInfoText.Text = "У вас нет активной аренды";
                    }
                    else
                    {
                        RentalInfoText.Text =
                            "Активная аренда: транспорт ID " +
                            rental.VehicleID +
                            ", начало: " +
                            rental.StartTime.ToString("dd.MM.yyyy HH:mm");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки аренды:\n" + ex.Message);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVehicles();
            LoadActiveRental();
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = VehiclesGrid.SelectedItem as Vehicles;

            if (selected == null)
            {
                MessageBox.Show("Выберите транспорт.");
                return;
            }

            try
            {
                using (var db = Core.GetContext())
                {
                    bool hasRental = db.Rentals.Any(r =>
                        r.UserID == userID && r.EndTime == null);

                    if (hasRental)
                    {
                        MessageBox.Show("Сначала завершите текущую аренду.");
                        return;
                    }

                    var vehicle = db.Vehicles.FirstOrDefault(v =>
                        v.VehicleID == selected.VehicleID &&
                        v.Status == "Available");

                    if (vehicle == null)
                    {
                        MessageBox.Show("Этот транспорт уже недоступен.");
                        LoadVehicles();
                        return;
                    }

                    db.Rentals.Add(new Rentals
                    {
                        UserID = userID,
                        VehicleID = vehicle.VehicleID,
                        StartTime = DateTime.Now,
                        TotalCost = 0,
                        RentalStatus = "Active"
                    });

                    vehicle.Status = "Rented";
                    db.SaveChanges();
                }

                MessageBox.Show("Аренда началась!");
                LoadVehicles();
                LoadActiveRental();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось начать аренду:\n" + ex.Message);
            }
        }

        private void FinishRentalButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = Core.GetContext())
                {
                    var rental = db.Rentals
                        .Where(r => r.UserID == userID && r.EndTime == null)
                        .OrderByDescending(r => r.StartTime)
                        .FirstOrDefault();

                    if (rental == null)
                    {
                        MessageBox.Show("У вас нет активной аренды.");
                        return;
                    }

                    var vehicle = db.Vehicles.FirstOrDefault(v =>
                        v.VehicleID == rental.VehicleID);

                    if (vehicle == null)
                    {
                        MessageBox.Show("Транспорт не найден.");
                        return;
                    }

                    DateTime endTime = DateTime.Now;
                    double hours = (endTime - rental.StartTime).TotalHours;

                    // Оплата рассчитывается за фактическое время,
                    // с округлением вверх до полного часа.
                    decimal cost = Math.Ceiling((decimal)hours)
                                   * vehicle.PricePerHour;

                    rental.EndTime = endTime;
                    rental.TotalCost = cost;
                    rental.RentalStatus = "Completed";
                    vehicle.Status = "Available";

                    db.SaveChanges();

                    MessageBox.Show(
                        "Аренда завершена.\nСтоимость: " +
                        cost.ToString("0.00"));
                }

                LoadVehicles();
                LoadActiveRental();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось завершить аренду:\n" + ex.Message);
            }
        }
    }
}
