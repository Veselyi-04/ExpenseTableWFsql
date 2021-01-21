using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace LoginWFsql
{
    static class SqlCommand
    {
        private static MySqlCommand command;

        /// <summary>
        /// Основной запрос, выбирает абсолютно все дни которые есть у текущего пользователя. Принимает только ID текущего пользователя и коннект.
        /// </summary>
        /// <param name="currentUserID">ID текущего пользователя</param>
        /// <param name="connection">Коннект к БД</param>
        /// <returns>Возвращает команду с готовым запросом.</returns>
        public static MySqlCommand Main_Сommand(int currentUserID, MySqlConnection connection)
        {
            command = new MySqlCommand(main_command, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            return command;
        }

        /// <summary>
        /// Запрос на количество дней которые есть у текущего пользователя. Принимает только ID текущего пользователя и коннект.
        /// </summary>
        /// <param name="currentUserID">ID текущего пользователя</param>
        /// <param name="connection">Коннект к БД</param>
        /// <returns>Возвращает команду с готовым запросом.</returns>
        public static MySqlCommand Сount_Days(int currentUserID, MySqlConnection connection)
        {
            command = new MySqlCommand(count_days, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            return command;
        }

        /// <summary>
        /// Запрос возвращает конкретный месяц. Принимает ID текущего пользователя, первый день месяца, последний день месяца и коннект.
        /// </summary>
        /// <returns>Возвращает 10 столбцов</returns>
        public static MySqlCommand Select_Month(int currentUserID, DateTime first_day_month, DateTime last_day_month, MySqlConnection connection)
        {
            command = new MySqlCommand(select_month, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@first_day_month", MySqlDbType.DateTime).Value = first_day_month;
            command.Parameters.Add("@last_day_month", MySqlDbType.DateTime).Value = last_day_month;

            return command;
        }

        /// <summary>
        /// Запрос возвращает последние актуальные даные для заполнения TopBar. Принимает только ID текущего пользователя и коннект.
        /// </summary>
        /// /// <returns>Возвращает 4 столбца</returns>
        public static MySqlCommand Fill_TopBar(int currentUserID, MySqlConnection connection)
        {
            command = new MySqlCommand(fill_topBar, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            return command;
        }

        /// <summary>
        /// Запрос возвращает существующие месяца(без повторений). Принимает только ID текущего пользователя и коннект.
        /// </summary>
        /// <returns></returns>
        public static MySqlCommand Fill_ComboBox_Month(int currentUserID, MySqlConnection connection)
        {
            command = new MySqlCommand(fill_comboBox_Month, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            return command;
        }

        /// <summary>
        /// Вставляет в список новый день. Принимает ID пользователя, коннект и параметры нового дня.
        /// </summary>
        /// <returns></returns>
        public static MySqlCommand Create_NewDay(int currentUserID, float cash, float card, float i_owe, float owe_me, float saved, float wasted, string str_wasted, float in_come, string str_in_come, DateTime date, MySqlConnection connection)
        {
            command = new MySqlCommand(create_newDay, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@cash", MySqlDbType.Float).Value = cash;
            command.Parameters.Add("@card", MySqlDbType.Float).Value = card;
            command.Parameters.Add("@i_owe", MySqlDbType.Float).Value = i_owe;
            command.Parameters.Add("@owe_me", MySqlDbType.Float).Value = owe_me;
            command.Parameters.Add("@saved", MySqlDbType.Float).Value = saved;
            command.Parameters.Add("@wasted", MySqlDbType.Float).Value = wasted;
            command.Parameters.Add("@str_wasted", MySqlDbType.String).Value = str_wasted;
            command.Parameters.Add("@in_come", MySqlDbType.Float).Value = in_come;
            command.Parameters.Add("@str_in_come", MySqlDbType.String).Value = str_in_come;
            command.Parameters.Add("@date", MySqlDbType.DateTime).Value = date;

            return command;
        }

        public static MySqlCommand Update_Day(int currentUserID, float cash, float card, float i_owe, float owe_me, float saved, float wasted, string str_wasted, float in_come, string str_in_come, DateTime date, MySqlConnection connection)
        {
            command = new MySqlCommand(update_day, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@cash", MySqlDbType.Float).Value = cash;
            command.Parameters.Add("@card", MySqlDbType.Float).Value = card;
            command.Parameters.Add("@i_owe", MySqlDbType.Float).Value = i_owe;
            command.Parameters.Add("@owe_me", MySqlDbType.Float).Value = owe_me;
            command.Parameters.Add("@saved", MySqlDbType.Float).Value = saved;
            command.Parameters.Add("@wasted", MySqlDbType.Float).Value = wasted;
            command.Parameters.Add("@str_wasted", MySqlDbType.String).Value = str_wasted;
            command.Parameters.Add("@in_come", MySqlDbType.Float).Value = in_come;
            command.Parameters.Add("@str_in_come", MySqlDbType.String).Value = str_in_come;
            command.Parameters.Add("@date", MySqlDbType.DateTime).Value = date;

            return command;
        }

        /// <summary>
        /// Запрос на удаление дня из списка. Принимает ID пользователя, дату дня и коннект.
        /// </summary>
        /// <returns></returns>
        public static MySqlCommand Delete_Day(int currentUserID, DateTime date, MySqlConnection connection)
        {
            command = new MySqlCommand(delete_day, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@date", MySqlDbType.DateTime).Value = date;
            return command;
        }

        /// <summary>
        /// Возвращает последний актуальный день для создания на его основе следущего дня
        /// </summary>
        public static MySqlCommand Select_LastDay(int currentUserID, DateTime SelectDate,  MySqlConnection connection)
        {
            command = new MySqlCommand(select_PrevDay, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@SelectDate", MySqlDbType.DateTime).Value = SelectDate;
            return command;
        }

        private static readonly string main_command = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
            "FROM days WHERE days.id_user = @currentUserID " +
            "ORDER BY date DESC";

        private static readonly string count_days = "SELECT COUNT(*) FROM days WHERE days.id_user = @currentUserID";

        private static readonly string select_month = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
            "FROM days WHERE days.id_user = @currentUserID " +
            "AND (date >= @first_day_month AND date <= @last_day_month) " +
            "ORDER BY date DESC";

        private static readonly string fill_topBar = "SELECT users.login, days.cash, days.i_owe, days.saved, days.date " +
            "FROM users INNER JOIN days ON days.id_user = users.id " +
            "WHERE users.id = @currentUserID " +
            "ORDER BY date DESC";

        private static readonly string fill_comboBox_Month = "SELECT DISTINCT YEAR(date), MONTH(date) " +
            "FROM days WHERE days.id_user = @currentUserID";

        private static readonly string create_newDay = "INSERT INTO `days` (`id_user`, `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date`) " +
            "VALUES (@currentUserID, @cash, @card, @i_owe, @owe_me, @saved, @wasted, @str_wasted, @in_come, @str_in_come, @date)";

        private static readonly string update_day = "UPDATE `days` SET `cash` = @cash, `card` = @card, `i_owe` = @i_owe, `owe_me` = @owe_me, " +
            "`saved` = @saved, `wasted` = @wasted, `str_wasted` = @str_wasted, `in_come` = @in_come, `str_in_come` = @str_in_come " +
            "WHERE days.date = @date AND days.id_user = @currentUserID";

        private static readonly string delete_day = "DELETE FROM `days` WHERE `days`.`date` = @date AND days.id_user = @currentUserID";

        private static readonly string select_PrevDay = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, date " +
            "FROM days WHERE(date <= @SelectDate) AND id_user = @currentUserID ORDER BY date DESC";

    }
}
