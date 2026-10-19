
using System;
using System.Data.Entity;

namespace UP01_Podzhigailo_BikeRental.Data
{
    public static class Core
    {
        public static DB_BikeScooterRentalEntities GetContext()
        {
            return new DB_BikeScooterRentalEntities();
        }

        public static bool TestConnection(out string message)
        {
            try
            {
                using (var db = GetContext())
                {
                    db.Database.Connection.Open();

                    string databaseName =
                        db.Database.Connection.Database;

                    db.Database.Connection.Close();

                    message = "Подключение успешно!\nБаза данных: "
                        + databaseName;

                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Ошибка подключения к базе данных:\n"
                    + ex.GetBaseException().Message;

                return false;
            }
        }
    }
}
