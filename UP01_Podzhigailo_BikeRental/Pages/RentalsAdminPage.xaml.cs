
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class RentalsAdminPage : Page
    {
        public RentalsAdminPage()
        {
            InitializeComponent();
            LoadRentals();
        }

        private void LoadRentals()
        {
            try
            {
                string search = SearchTextBox.Text.Trim().ToLower();

                using (var db = Core.GetContext())
                {
                    var rentals = db.Rentals
                        .Select(r => new
                        {
                            r.RentalID,
                            UserLogin = r.Users.Login,
                            VehicleModel = r.Vehicles.Model,
                            r.StartTime,
                            r.EndTime,
                            r.TotalCost,
                            r.RentalStatus
                        })
                        .Where(r =>
                            search == "" ||
                            (r.UserLogin != null &&
                             r.UserLogin.ToLower().Contains(search)) ||
                            (r.VehicleModel != null &&
                             r.VehicleModel.ToLower().Contains(search)) ||
                            (r.RentalStatus != null &&
                             r.RentalStatus.ToLower().Contains(search)))
                        .OrderByDescending(r => r.RentalID)
                        .ToList();

                    RentalsGrid.ItemsSource = rentals;
                    CountTextBlock.Text =
                        "Количество аренд: " + rentals.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки аренд:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (IsInitialized)
                LoadRentals();
        }

        private void RefreshButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadRentals();
        }
    }
}
