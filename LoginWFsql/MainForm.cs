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
            Show_list_days(30);
            Show_Selected_Day();
        }

        /// <summary>
        /// Выводит список дней в лист (который слева) Принимает колво дней которое надо вывести
        /// </summary>
        /// <param name="count"> Колво дней которое надо вывести (до 30!) </param>
        private void Show_list_days(int count)
        {
            On_Of_btCrateNewDay(true);
            for (int i = 0; i < count; i++)
            {
                // Проверка на случай если будет меньше дней чем мы хочем вывести
                if (days.Length == i)
                    return;

                days[i].Create_New_Group_Box();
                mainContainer.Panel1.Controls.Add(days[i].GroupBox);
                days[i].is_show = true;
            }
        }

        /// <summary>
        ///  Выводит список дней выбраного месяца в лист, [Разница]: меняет положение дней
        /// </summary>
        private void Show_list_days_from_Select_Month()
        {
            On_Of_btCrateNewDay(false);
            for (int i = 0; i < days.Length; i++)
            {
                days[i].Create_New_Group_Box();
                days[i].GroupBox.Location = new Point(2, (65 * i + 25));
                mainContainer.Panel1.Controls.Add(days[i].GroupBox);
                days[i].is_show = true;
            }
        }

        /// <summary>
        /// Включает или отключаест видимость, верхнего GroupBox(кнопка для создания нового дня)
        /// </summary>
        /// <param name="On_Off"> true = on, false = off.</param>
        private void On_Of_btCrateNewDay(bool On_Off)
        {
            if(On_Off)
            {
                btCrateNewDay.Visible = true;
            }
            else
            {
                btCrateNewDay.Visible = false;
            }
        }

        /// <summary>
        /// Выводит информацию о выбраном дне на главую панель
        /// </summary>
        /// <param name="i"> индекс дня</param>
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

        /// <summary>
        /// Заполнение масива дней с Базы Даных
        /// </summary>
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

            // Ограничиваем считывание дней небольше 30
            if (count_days > 30)
                count_days = 30;

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

        /// <summary>
        /// Заполнение масива дней (определенным месяцем) с Базы Даных
        /// </summary>
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

        /// <summary>
        /// Чистит список дней от ранее выведеных
        /// </summary>
        private void Clear_list_days()
        {
            for (int i = 0; i < days.Length; i++)
            {
                // Для чего нужна ета проверка: 
                if (days[i].is_show)// бывает что в списке есть 30 дней но показано только 7-последних и он должен удалить только те 7 и не идти дальше
                    mainContainer.Panel1.Controls.Remove(days[i].GroupBox);
                else return;
            }
        }

        /// <summary>
        /// Заполняет конкретный день масива существующим днем из БД
        /// </summary>
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

        /// <summary>
        /// Заполняет конкретный день масива пустым днем
        /// </summary>
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

        /// <summary>
        /// Заполняет ComboBox месяцами из БД
        /// </summary>
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

        /// <summary>
        /// Заполняет верхнюю панель последними даными из БД
        /// </summary>
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

        /*______________Кнопки <7 - <30 и ComboBoxMonth___________________*/
        private void cbMonth_SelectionChangeCommited(object sender, EventArgs e)
        {
            string temp = cbMonth.Items[cbMonth.SelectedIndex].ToString();
            if (temp == " ")
            {
                Clear_list_days();
                Fill_Array_Days();
                Show_list_days(30);
                return;
            }

            Clear_list_days();
            Fill_Array_Days_from_Select_Month();
            Show_list_days_from_Select_Month();
        }

        private void lb_To_7_Click(object sender, EventArgs e)
        {
            Preparation_Panel_From_Show();
            Show_list_days(7);
        }

        private void lb_To_30_Click(object sender, EventArgs e)
        {
            Preparation_Panel_From_Show();
            Show_list_days(30);
        }
        private void Preparation_Panel_From_Show()
        {
            string temp;

            // проверка на то не стоит ли там стандартное значение (тоесть: ничего не выбрано)
            if (cbMonth.SelectedIndex == -1)
                temp = " ";
            else
                temp = cbMonth.Items[cbMonth.SelectedIndex].ToString();

            if (temp != " ")// Если раннее был выведен список дней выбраного месяца, ТО:
            {
                // 1) его надо почистить
                Clear_list_days();
                // 2) его надо заполнить просто всеми существующими днями последовательно
                Fill_Array_Days();
                // просто сбрасываем на стандартное значение
                cbMonth.SelectedIndex = -1;
            }
            else // иначе там (Fill_Array_Days) итак был простой последовательный список существующих дней
            {
                // и мы чистим панель
                Clear_list_days();
            }
        }

        private void lb_To_30_MouseEnter(object sender, EventArgs e)
        {
            Lighting_lb(lb_To_30);
        }

        private void lb_To_30_MouseLeave(object sender, EventArgs e)
        {
            Blackout_lb(lb_To_30);
        }

        private void lb_To_30_MouseDown(object sender, MouseEventArgs e)
        {
            Blackout_lb(lb_To_30);
        }

        private void lb_To_30_MouseUp(object sender, MouseEventArgs e)
        {
            Lighting_lb(lb_To_30);
        }

        private void lb_To_7_MouseEnter(object sender, EventArgs e)
        {
            Lighting_lb(lb_To_7);
        }

        private void lb_To_7_MouseLeave(object sender, EventArgs e)
        {
            Blackout_lb(lb_To_7);
        }

        private void lb_To_7_MouseDown(object sender, MouseEventArgs e)
        {
            Blackout_lb(lb_To_7);
        }

        private void lb_To_7_MouseUp(object sender, MouseEventArgs e)
        {
            Lighting_lb(lb_To_7);
        }
        
        /// <summary>
        /// Затемняет кнопку(в нашем случае кнопка ето лейбл)
        /// </summary>
        private void Blackout_lb (Label button)
        {
            int r = button.BackColor.R - 10;
            int g = button.BackColor.G - 10;
            int b = button.BackColor.B - 10;
            button.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Осветляет кнопку(в нашем случае кнопка ето лейбл)
        /// </summary>
        /// <param name="button"></param>
        private void Lighting_lb(Label button)
        {
            int r = button.BackColor.R + 10;
            int g = button.BackColor.G + 10;
            int b = button.BackColor.B + 10;
            button.BackColor = Color.FromArgb(r, g, b);
        }
        /*________________________________________________________________*/
    }
}
