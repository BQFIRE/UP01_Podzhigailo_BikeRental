
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP01_Podzhigailo_BikeRental.Data;

namespace UP01_Podzhigailo_BikeRental.Pages
{
    public partial class UsersAdminPage : Page
    {
        public UsersAdminPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                string search = SearchTextBox.Text.Trim().ToLower();

                using (var db = Core.GetContext())
                {
                    var users = db.Users
                        .Where(u =>
                            search == "" ||
                            (u.Login != null &&
                             u.Login.ToLower().Contains(search)) ||
                            (u.FullName != null &&
                             u.FullName.ToLower().Contains(search)))
                        .OrderBy(u => u.UserID)
                        .Select(u => new
                        {
                            u.UserID,
                            u.Login,
                            u.FullName,
                            u.Role
                        })
                        .ToList();

                    UsersGrid.ItemsSource = users;
                    CountTextBlock.Text =
                        "Количество пользователей: " + users.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки пользователей:\n" + ex.Message,
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
                LoadUsers();
        }

        private void RefreshButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void ChangeRoleButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            dynamic selected = UsersGrid.SelectedItem;
            int userID = selected.UserID;
            string currentRole = selected.Role;

            string newRole = currentRole == "Admin" ? "User" : "Admin";

            var result = MessageBox.Show(
                "Изменить роль пользователя " + selected.Login +
                " с \"" + currentRole + "\" на \"" + newRole + "\"?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = Core.GetContext())
                {
                    var user = db.Users.FirstOrDefault(
                        u => u.UserID == userID);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден.");
                        LoadUsers();
                        return;
                    }

                    user.Role = newRole;
                    db.SaveChanges();
                }

                LoadUsers();

                MessageBox.Show(
                    "Роль изменена на " + newRole + ".",
                    "Готово",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось изменить роль:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
