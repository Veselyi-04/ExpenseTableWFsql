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

        private static Currency currency;

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
        public static MySqlCommand Fill_TopBar(Currency currency ,int currentUserID, MySqlConnection connection)
        {
            string purse = fill_topBar_UAH;
            switch (currency)
            {
                case Currency.UAH:
                    purse = fill_topBar_UAH;
                    break;
                case Currency.EUR:
                    purse = fill_topBar_EUR;
                    break;
            }
            command = new MySqlCommand(purse, connection);
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
        public static MySqlCommand Create_NewDay(int currentUserID, DateTime date, 
            float cash_uah, float card_uah, float i_owe_uah, float owe_me_uah, float saved_uah, 
            float cash_eur, float card_eur, float i_owe_eur, float owe_me_eur, float saved_eur, MySqlConnection connection)
        {
            command = new MySqlCommand(create_newDay, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@date", MySqlDbType.DateTime).Value = date;

            command.Parameters.Add("@cash_uah", MySqlDbType.Float).Value = cash_uah;
            command.Parameters.Add("@card_uah", MySqlDbType.Float).Value = card_uah;
            command.Parameters.Add("@i_owe_uah", MySqlDbType.Float).Value = i_owe_uah;
            command.Parameters.Add("@owe_me_uah", MySqlDbType.Float).Value = owe_me_uah;
            command.Parameters.Add("@saved_uah", MySqlDbType.Float).Value = saved_uah;

            command.Parameters.Add("@cash_eur", MySqlDbType.Float).Value = cash_eur;
            command.Parameters.Add("@card_eur", MySqlDbType.Float).Value = card_eur;
            command.Parameters.Add("@i_owe_eur", MySqlDbType.Float).Value = i_owe_eur;
            command.Parameters.Add("@owe_me_eur", MySqlDbType.Float).Value = owe_me_eur;
            command.Parameters.Add("@saved_eur", MySqlDbType.Float).Value = saved_eur;

            return command;
        }

        public static MySqlCommand Update_Day(Currency currency, int currentUserID, float cash, float card, float i_owe, float owe_me, float saved, float wasted, string str_wasted, float in_come, string str_in_come, DateTime date, MySqlConnection connection)
        {
            string purse = upd_purse_UAH;
            switch (currency)
            {
                case Currency.UAH:
                    purse = upd_purse_UAH;
                    break;
                case Currency.EUR:
                    purse = upd_purse_EUR;
                    break;
            }

            command = new MySqlCommand(update_day + purse, connection);
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
        public static MySqlCommand Select_PrevDay(Currency currency, int currentUserID, DateTime SelectDate,  MySqlConnection connection)
        {
            string purse = select_PrevDay_UAH;
            switch (currency)
            {
                case Currency.UAH:
                    purse = select_PrevDay_UAH;
                    break;
                case Currency.EUR:
                    purse = select_PrevDay_EUR;
                    break;
                default:
                    purse = select_PrevDay;
                    break;
            }

            command = new MySqlCommand(purse, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@SelectDate", MySqlDbType.DateTime).Value = SelectDate;
            return command;
        }

        public static MySqlCommand Select_Login(int currentUserID, MySqlConnection connection)
        {
            command = new MySqlCommand(select_login, connection);
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            return command;
        }

        private static readonly string main_command = "SELECT day.str_wasted, day.str_in_come, day.date, " +
            "UAH.cash, UAH.card, UAH.i_owe, UAH.owe_me, UAH.saved, UAH.wasted, UAH.in_come, " +
            "EUR.cash, EUR.card, EUR.i_owe, EUR.owe_me, EUR.saved, EUR.wasted, EUR.in_come " +
            "FROM day " +
            "INNER JOIN purse_uah AS UAH ON UAH.id_day = day.id_day " +
            "INNER JOIN purse_eur AS EUR ON EUR.id_day = day.id_day " +
            "WHERE day.id_user = @currentUserID ORDER BY day.date DESC";
        // переделать запись в масив дней
        private static readonly string count_days = "SELECT COUNT(*) FROM day WHERE day.id_user = @currentUserID";

        private static readonly string select_month = "SELECT day.str_wasted, day.str_in_come, day.date, " +
            "UAH.cash, UAH.card, UAH.i_owe, UAH.owe_me, UAH.saved, UAH.wasted, UAH.in_come, " +
            "EUR.cash, EUR.card, EUR.i_owe, EUR.owe_me, EUR.saved, EUR.wasted, EUR.in_come " +
            "FROM day " +
            "INNER JOIN purse_uah AS UAH ON UAH.id_day = day.id_day " +
            "INNER JOIN purse_eur AS EUR ON EUR.id_day = day.id_day " +
            "WHERE day.id_user = @currentUserID AND (date BETWEEN @first_day_month AND @last_day_month) ORDER BY date DESC";

        private static readonly string fill_topBar_UAH = "SELECT purse_uah.cash, purse_uah.i_owe, purse_uah.saved " +
            "FROM purse_uah " +
            "WHERE purse_uah.id_day = (SELECT day.id_day FROM day WHERE day.id_user = @currentUserID ORDER BY date DESC LIMIT 1)";

        private static readonly string fill_topBar_EUR = "SELECT purse_eur.cash, purse_eur.i_owe, purse_eur.saved " +
            "FROM purse_eur " +
            "WHERE purse_eur.id_day = (SELECT day.id_day FROM day WHERE day.id_user = @currentUserID ORDER BY date DESC LIMIT 1)";

        private static readonly string select_login = "SELECT users.login " +
            "FROM users WHERE users.id = @currentUserID";

        private static readonly string fill_comboBox_Month = "SELECT DISTINCT YEAR(date), MONTH(date) " +
            "FROM day WHERE day.id_user = @currentUserID";

        private static readonly string create_newDay = "INSERT INTO day (id_user, date) " +
            "VALUES (@currentUserID, @date);" +
            "INSERT INTO `purse_uah` (`id_day`, `cash`, `card`, `i_owe`, `owe_me`, `saved`) " +
            "VALUES((SELECT day.id_day FROM day WHERE day.date = @date AND day.id_user = @currentUserID), @cash_uah, @card_uah, @i_owe_uah, @owe_me_uah, @saved_uah);" +
            "INSERT INTO `purse_eur` (`id_day`, `cash`, `card`, `i_owe`, `owe_me`, `saved`) " +
            "VALUES((SELECT day.id_day FROM day WHERE day.date = @date AND day.id_user = @currentUserID), @cash_eur, @card_eur, @i_owe_eur, @owe_me_eur, @saved_eur);";

        private static readonly string update_day = "UPDATE day SET day.str_wasted = @str_wasted, day.str_in_come = @str_in_come " +
            "WHERE day.date = @date AND day.id_user = @currentUserID; ";
            
        private static readonly string upd_purse_UAH = "UPDATE purse_uah AS UAH SET UAH.cash = @cash, UAH.card = @card, UAH.i_owe = @i_owe, " +
            "UAH.owe_me = @owe_me, UAH.saved = @saved, UAH.wasted = @wasted, UAH.in_come = @in_come WHERE UAH.id_day = (SELECT id_day FROM day " +
            "WHERE day.date = @date AND day.id_user = @currentUserID);";

        private static readonly string upd_purse_EUR = "UPDATE purse_eur AS EUR SET EUR.cash = @cash, EUR.card = @card, EUR.i_owe = @i_owe, " +
            "EUR.owe_me = @owe_me, EUR.saved = @saved, EUR.wasted = @wasted, EUR.in_come = @in_come WHERE EUR.id_day = (SELECT id_day FROM day " +
            "WHERE day.date = @date AND day.id_user = @currentUserID);";

        private static readonly string delete_day = "DELETE FROM day WHERE day.date = @date AND day.id_user = @currentUserID";

        // Переделал, надо интегрировать
        private static readonly string select_PrevDay_UAH = "SELECT cash, card, i_owe, owe_me, saved " +
            "FROM purse_uah " +
            "WHERE purse_uah.id_day = " +
            "(SELECT day.id_day FROM day WHERE day.date < @SelectDate AND day.id_user = @currentUserID ORDER BY date DESC LIMIT 1)";

        private static readonly string select_PrevDay_EUR = "SELECT cash, card, i_owe, owe_me, saved " +
            "FROM purse_eur " +
            "WHERE purse_eur.id_day = " +
            "(SELECT day.id_day FROM day WHERE day.date < @SelectDate AND day.id_user = @currentUserID ORDER BY date DESC LIMIT 1)";

        private static readonly string select_PrevDay = "SELECT UAH.cash, UAH.card, UAH.i_owe, UAH.owe_me, UAH.saved, " +
            "EUR.cash, EUR.card, EUR.i_owe, EUR.owe_me, EUR.saved " +
            "FROM purse_uah AS UAH " +
            "INNER JOIN purse_eur AS EUR ON UAH.id_day = EUR.id_day " +
            "WHERE UAH.id_day = " +
            "(SELECT day.id_day FROM day WHERE day.date<@SelectDate AND day.id_user = @currentUserID ORDER BY date DESC LIMIT 1);";
    }
}
