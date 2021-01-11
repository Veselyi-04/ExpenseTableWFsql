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
        static public string main_command_str = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
            "FROM days WHERE days.id_user = @currentUserID " +
            "ORDER BY date DESC";

        static public string count_rows_from_main_command = "SELECT COUNT(*) FROM days WHERE days.id_user = @currentUserID";

        static public string select_month_command_str = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
            "FROM days WHERE days.id_user = @currentUserID " +
            "AND (date >= @beforeMonth AND date < @afterMonth) " +
            "ORDER BY date DESC";

        static public string fill_topBar_command_str = "SELECT users.login, days.cash, days.i_owe, days.saved, days.date " +
            "FROM users INNER JOIN days ON days.id_user = users.id " +
            "WHERE users.id = @currentUserID " +
            "ORDER BY date DESC";

            static public string fill_ComboBox_Month_str = "SELECT DISTINCT YEAR(date), MONTH(date) " +
            "FROM days WHERE days.id_user = @currentUserID";
    }
}
