
using System.Windows;
using System.Windows.Controls;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }


        private void BtnUsers_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UsersAdminPage());
        }



        private void BtnVehicles_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavigationService?.Navigate(new VehiclesAdminPage());
        }


        private void BtnStations_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StationsAdminPage());
        }


        private void BtnRentals_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RentalsAdminPage());
        }

    }
}
