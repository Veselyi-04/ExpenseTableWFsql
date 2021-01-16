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
        static public string main_command = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
            "FROM days WHERE days.id_user = @currentUserID " +
            "ORDER BY date DESC";

        static public string count_rows_from_main_command = "SELECT COUNT(*) FROM days WHERE days.id_user = @currentUserID";

        static public string select_month_command = "SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
            "FROM days WHERE days.id_user = @currentUserID " +
            "AND (date >= @beforeMonth AND date <= @afterMonth) " +
            "ORDER BY date DESC";

        static public string fill_topBar_command = "SELECT users.login, days.cash, days.i_owe, days.saved, days.date " +
            "FROM users INNER JOIN days ON days.id_user = users.id " +
            "WHERE users.id = @currentUserID " +
            "ORDER BY date DESC";

        static public string fill_ComboBox_Month = "SELECT DISTINCT YEAR(date), MONTH(date) " +
            "FROM days WHERE days.id_user = @currentUserID";

        static public string Save_NewDay = "INSERT INTO `days` (`id_user`, `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date`) " +
            "VALUES (@currentUserID, @cash, @card, @i_owe, @owe_me, @saved, @wasted, @str_wasted, @in_come, @str_in_come, @date)";

        static public string Update_Day = "UPDATE `days` SET `cash` = @cash, `card` = @card, `i_owe` = @i_owe, `owe_me` = @owe_me, " +
            "`saved` = @saved, `wasted` = @wasted, `str_wasted` = @str_wasted, `in_come` = @in_come, `str_in_come` = @str_in_come " +
            "WHERE days.date = @date AND days.id_user = @currentUserID";

        static public string Delete_Day = "DELETE FROM `days` WHERE `days`.`date` = @date AND days.id_user = @currentUserID";
    }
}
