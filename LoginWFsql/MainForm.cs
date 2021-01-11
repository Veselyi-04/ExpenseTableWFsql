using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LoginWFsql
{
    public partial class MainForm : Form
    {
        // Для работы с бд
        private readonly DataBase db = new DataBase();
        private MySqlCommand command;
        private MySqlDataReader reader;

        /*ID текущего пользователя*/
        private readonly int currentUserID;

        /*(основная)Скрытая логин форма, чтобы потом корректно завершить работу програмы */
        private readonly LoginForm LF;

        /*Масив с таблице дней (которая слева)*/
        public Day[] days;

        public MainForm(LoginForm lf, int id)
        {
            currentUserID = id;
            LF = lf;
            StartForm();
        }

        private void StartForm()
        {
            InitializeComponent();
            Fill_Top_Bar();
            Fill_ComboBox_Month();
            Fill_Array_Days();
            Show_list_days();
            Show_Selected_Day();
        }

        private void Show_list_days()
        {
            for (int i = 0; i < days.Length; i++)
            {
                mainContainer.Panel1.Controls.Add(days[i].Create_and_get_Group());
            }
        }

        private void Clear_list_days()
        {
            for (int i = 0; i < days.Length; i++)
            {
                mainContainer.Panel1.Controls.Remove(days[i].Get_GroupBox());
            }
        }

        private void Show_Selected_Day(int i = 0)
        {
            lbNameDay.Text = days[i].date.DayOfWeek.ToString();
            lbDate.Text = days[i].date.ToShortDateString();
            lbCash.Text = days[i].cash.ToString();
            lbCard.Text = days[i].card.ToString();
            lbIOwe.Text = days[i].i_owe.ToString();
            lbOweMe.Text = days[i].owe_me.ToString();
            lbSaved.Text = days[i].saved.ToString();
            lbWasted.Text = days[i].wasted.ToString();
            tbWasted.Text = days[i].str_wasted;
            lbIncome.Text = days[i].in_come.ToString();
            tbIncome.Text = days[i].str_income;
        }

        private void Fill_Array_Days()
        {
            db.openConnection();
            int count_days;
            // берем количество существующих дней для создания масива нужного размера
            {
                command = new MySqlCommand(SqlCommand.count_rows_from_main_command, db.getConnection());
                command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
                reader = command.ExecuteReader();
                reader.Read();
                count_days = reader.GetInt32(0);
                reader.Close();
            }

            command = new MySqlCommand(SqlCommand.main_command_str, db.getConnection());
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            reader = command.ExecuteReader();
            
            days = new Day[count_days];
            int i = 0;
            while (reader.Read() && i < days.Length)
            {
                Get_day_from_base(i);
                i++;
            }

            reader.Close();
            db.closeConnection();
        }

        private void Fill_Array_Days_from_Select_Month()
        {
            db.openConnection();
            int count_days;
            int year;
            string month;

            // записываем выбраную в cbMonth дату
            string selected = cbMonth.Items[cbMonth.SelectedIndex].ToString();

            // Разсоединяем и конвертируем ее в число
            year = int.Parse(selected.Substring(0, 4));
            month = selected.Substring(5);

            // Узнаем сколько дней в выбраном месяце
            count_days = DateTime.DaysInMonth(year, int.Parse(month));

            // Строки для сохранения прев.Месяца и след.Месяца
            DateTime beforeDate;
            DateTime afterDate;

            // Находим прев.Месяц и след.Месяц
            {
                beforeDate = new DateTime(year, int.Parse(month), 1);

                if (month == "12")
                    afterDate = new DateTime(year + 1, 1, 1);
                else
                    afterDate = beforeDate.AddMonths(1);
            }

            // создаем запрос
            command = new MySqlCommand(SqlCommand.select_month_command_str, db.getConnection());
            // заменяем заглушки на реальные значения
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@beforeMonth", MySqlDbType.DateTime).Value = beforeDate;
            command.Parameters.Add("@afterMonth", MySqlDbType.DateTime).Value = afterDate;

            // выполняем команду в Ридере
            reader = command.ExecuteReader();

            days = new Day[count_days];

            DateTime lastDay = new DateTime(year, int.Parse(month), count_days);

            for (int i = 0; i < days.Length; i++)
            {
                // читаем то что мы вытянули из БД
                while (reader.Read())
                {
                    // Узнаем дату дня который взяли из БД
                    DateTime current = reader.GetDateTime(9);

                    // Ищем для нее место начиная с конца месяца (при выборе даты отсортированы от 1->0)
                    while (current != lastDay)
                    {
                        // если несовпало, вставляем на ето место пустой день
                        Get_empty_day(i, lastDay);

                        // отнимаем день и снова проверяем current
                        lastDay = lastDay.AddDays(-1);
                        i++;
                    }

                    // если совпало то вставляем етот день в список(масив)
                    Get_day_from_base(i);

                    // отнимаем день и возвращаемся проверить reader.Read() есть ли там следущий день
                    lastDay = lastDay.AddDays(-1);
                    i++;
                }

                // если нет то до кона days.Length забиваем пустыми днями
                Get_empty_day(i, lastDay);

                // и все-же каждый раз отнимаем дату
                lastDay = lastDay.AddDays(-1);
            }

            reader.Close();
            db.closeConnection();
        }

        private void Get_day_from_base(int i)
        {
            days[i] = new Day(i);
            days[i].cash = reader.GetInt32(0);
            days[i].card = reader.GetInt32(1);
            days[i].i_owe = reader.GetInt32(2);
            days[i].owe_me = reader.GetInt32(3);
            days[i].saved = reader.GetInt32(4);
            days[i].wasted = reader.GetInt32(5);
            days[i].str_wasted = reader.GetString(6);
            days[i].in_come = reader.GetInt32(7);
            days[i].str_income = reader.GetString(8);
            days[i].date = reader.GetDateTime(9);
        }

        private void Get_empty_day(int i, DateTime date)
        {
            days[i] = new Day(i);
            days[i].cash = 0.0f;
            days[i].card = 0.0f;
            days[i].i_owe = 0.0f;
            days[i].owe_me = 0.0f;
            days[i].saved = 0.0f;
            days[i].wasted = 0.0f;
            days[i].str_wasted = "";
            days[i].in_come = 0.0f;
            days[i].str_income = "";
            days[i].date = date;
            days[i].is_empty = true;
        }

        private void Fill_ComboBox_Month()
        {
            db.openConnection();
            command = new MySqlCommand(SqlCommand.fill_ComboBox_Month_str, db.getConnection());
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            reader = command.ExecuteReader();

            while (reader.Read())
            {

                string year = reader.GetString(0);
                string month = reader.GetString(1);

                if (month.Length < 2)
                    month = "0" + month;

                cbMonth.Items.Insert(1, $"{year}.{month}");

            }
            reader.Close();
            db.closeConnection();
        }

        private void Fill_Top_Bar()
        {
            string userlogin;
            int cash;
            int i_owe;
            int saved;
            db.openConnection();
            {
                command = new MySqlCommand(SqlCommand.fill_topBar_command_str, db.getConnection());
                command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
                reader = command.ExecuteReader();

                reader.Read();
                {
                    userlogin = reader.GetString(0);
                    cash = reader.GetInt32(1);
                    i_owe = reader.GetInt32(2);
                    saved = reader.GetInt32(3);
                }
                reader.Close();

            }
            db.closeConnection();

            _lb_state_userlogin.Text = userlogin;
            _lb_state_cash.Text = cash.ToString();
            _lb_state_i_owe.Text = i_owe.ToString();
            _lb_state_saved.Text = saved.ToString();

        }

        /*_________Кнопки закрыть/свернуть и движение окна________________*/
        private void btClose_Click(object sender, EventArgs e)
        {
            LF.Close();
            this.Close();
        }

        private void btMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        Point lastpoint;
        private void panelTopStats_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastpoint.X;
                this.Top += e.Y - lastpoint.Y;
            }
        }

        private void panelTopStats_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastpoint.X = e.X;
                lastpoint.Y = e.Y;
            }
        }

        private void btClose_MouseEnter(object sender, EventArgs e)
        {
            this.btClose.Font = new Font("Crosterian", 17F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
            this.btClose.Location = new Point(658, 1);
            this.btClose.BackColor = Color.FromArgb(38, 56, 89);
        }

        private void btClose_MouseLeave(object sender, EventArgs e)
        {
            this.btClose.Font = new Font("Crosterian", 14.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
            this.btClose.Location = new Point(662, 3);
            this.btClose.BackColor = Color.FromArgb(23, 34, 59);
        }

        private void btMinimize_MouseEnter(object sender, EventArgs e)
        {
            this.btMinimize.Font = new Font("Crosterian", 22F, FontStyle.Bold);
            this.btMinimize.Location = new Point(615, -13);
            this.btMinimize.BackColor = Color.FromArgb(38, 56, 89);
        }

        private void btMinimize_MouseLeave(object sender, EventArgs e)
        {
            this.btMinimize.Font = new Font("Crosterian", 18F, FontStyle.Bold);
            this.btMinimize.Location = new Point(619, -8);
            this.btMinimize.BackColor = Color.FromArgb(23, 34,59);
        }
        /*________________________________________________________________*/
        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ff = cbMonth.Items[cbMonth.SelectedIndex].ToString();
            if (ff == " ")
            {
                Clear_list_days();
                Fill_Array_Days();
                Show_list_days();
                return;
            }
            Clear_list_days();
            Fill_Array_Days_from_Select_Month();
            Show_list_days();
        }
    }
}
