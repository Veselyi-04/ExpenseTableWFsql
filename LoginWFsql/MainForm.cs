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
        private Day[] days;

        // Для того чтобы выводить ранее выбраный день полсе отмены редактирования
        /// <summary>
        /// Запоминает последний день который мы выводили в MainPanel
        /// </summary>
        private int Id_selected_day;

        // Переменные для ля работы с даными из интерфейса
        Buttons_Push buttons_Push = Buttons_Push.NULL;
        CellState cellState1 = CellState.NULL;
        CellState cellState2 = CellState.NULL;
        CellState cellState3 = CellState.NULL;

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
            Show_Selected_Day(0);
        }

        /// <summary>
        /// Выводит список дней в лист (который слева) Принимает колво дней которое надо вывести
        /// </summary>
        /// <param name="count"> Колво дней которое надо вывести (до 30!) </param>
        private void Show_list_days(int count)
        {
            On_Of_btCrateNewDay(true);
            Set_state_btDelete();

            for (int i = 0; i < count; i++)
            {
                // Проверка на случай если будет меньше дней чем мы хочем вывести
                if (days.Length == i)
                    return;

                days[i].Create_New_Group_Box();
                days[i].Click_Button += new DayEventHandler (btShow_click_Handler);
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
            Set_state_btDelete();
            for (int i = 0; i < days.Length; i++)
            {
                days[i].Create_New_Group_Box();
                days[i].Click_Button += new DayEventHandler(btShow_click_Handler);
                days[i].GroupBox.Location = new Point(2, (65 * i + 25));
                mainContainer.Panel1.Controls.Add(days[i].GroupBox);
                days[i].is_show = true;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки SHOW\/Принимает индекс дня
        /// </summary>
        /// <param name="INDEX">Индекс дня, на котором мы нажали кнопку</param>
        private void btShow_click_Handler(int INDEX)
        {
            if (Check_state_edit())
                return;
            // Тут определяем етот день пустой или нет
            if (days[INDEX].is_empty)
                return; //Create_New_Day_Handler(INDEX); // Да: ТОгда ето кнопка создания дня.
            else
                Show_Selected_Day(INDEX); // Нет: тогда ето кнопка SHOW и мы показываем етот день
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
        private void Show_Selected_Day(int i)
        {
            lbNameDay.Text = days[i].date.DayOfWeek.ToString();
            lbDate.Text = days[i].date.ToShortDateString();
            lbCash.Text = days[i].cash.ToString();
            lbCard.Text = days[i].card.ToString();
            lbIOwe.Text = days[i].i_owe.ToString();
            lbOweMe.Text = days[i].owe_me.ToString();
            lbSaved.Text = days[i].saved.ToString();
            lbWasted.Text = days[i].wasted.ToString();
            lb_Wasted_Str.Text = days[i].str_wasted;
            lbIncome.Text = days[i].in_come.ToString();
            lb_In_Come_Str.Text = days[i].str_income;
            Id_selected_day = i;
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
                command = SqlCommand.Сount_Days(currentUserID, db.getConnection());
                reader = command.ExecuteReader();
                reader.Read();
                count_days = reader.GetInt32(0);
                reader.Close();
            }

            command = SqlCommand.Main_Сommand(currentUserID, db.getConnection());

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
            DateTime first_day_month;
            DateTime last_day_month;

            // Находим прев.Месяц и след.Месяц
            {
                first_day_month = new DateTime(year, int.Parse(month), 1);
                last_day_month = new DateTime(year, int.Parse(month), count_days);

                if (last_day_month > DateTime.Now)
                {
                    last_day_month = DateTime.Now;
                    count_days = DateTime.Now.Day;
                }
            }

            // создаем запрос
            command = SqlCommand.Select_Month(currentUserID, first_day_month, last_day_month, db.getConnection());

            // выполняем команду в Ридере
            reader = command.ExecuteReader();

            days = new Day[count_days];

            DateTime lastDay = new DateTime(year, int.Parse(month), count_days);

            int i = 0;
            // читаем то что мы вытянули из БД
            while (reader.Read())
            {
                DateTime currentDate = reader.GetDateTime(9);

                while(currentDate != lastDay)
                {
                    Get_empty_day(i, lastDay);

                    lastDay = lastDay.AddDays(-1);
                    i++;
                }

                Get_day_from_base(i);
                lastDay = lastDay.AddDays(-1);
                i++;
            }

            while(i < count_days)
            {
                Get_empty_day(i, lastDay);
                lastDay = lastDay.AddDays(-1);
                i++;
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
            days[i].cash = reader.GetFloat(0);
            days[i].card = reader.GetFloat(1);
            days[i].i_owe = reader.GetFloat(2);
            days[i].owe_me = reader.GetFloat(3);
            days[i].saved = reader.GetFloat(4);
            days[i].wasted = reader.GetFloat(5);
            days[i].str_wasted = reader.GetString(6);
            days[i].in_come = reader.GetFloat(7);
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
            cbMonth.Items.Clear();
            cbMonth.Items.Add(" ");

            db.openConnection();

            command = SqlCommand.Fill_ComboBox_Month(currentUserID, db.getConnection());

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
                command = SqlCommand.Fill_TopBar(currentUserID, db.getConnection());

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

        private void Delete_Day()
        {
            command = SqlCommand.Delete_Day(currentUserID, date: days[Id_selected_day].date, db.getConnection()); ;
                
            db.openConnection();
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Неизвесная ошибка\n {ex.Message}");
                    return;
                }
            }
            db.closeConnection();

            Clear_list_days();
            Fill_Top_Bar();

            if (cbMonth.SelectedIndex != -1)
            {
                string temporary = cbMonth.Text;
                Fill_ComboBox_Month();

                for (int i = 0; i < cbMonth.Items.Count; i++)
                {
                    if (temporary == cbMonth.Items[i].ToString())
                    {
                        cbMonth.SelectedIndex = i;
                        Fill_Array_Days_from_Select_Month();
                        Show_list_days_from_Select_Month();
                        Show_Selected_Day(0);
                        return;
                    }
                }
            }

            Fill_ComboBox_Month();
            Fill_Array_Days();
            Show_list_days(6);
            Show_Selected_Day(0);
        }

        #region Кнопки закрыть/свернуть и движение окна [TopPanel]
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
        #endregion

        #region Кнопки <7 - <30, ComboBoxMonth, CrateNewDay -[LeftPanel]
        /*______________Кнопки <7 - <30, ComboBoxMonth, CrateNewDay___________________*/
        private void cbMonth_SelectionChangeCommited(object sender, EventArgs e)
        {
            string temp = cbMonth.Items[cbMonth.SelectedIndex].ToString();
            if (temp == " ")
            {
                cbMonth.SelectedIndex = -1;
                Clear_list_days();
                Fill_Array_Days();
                Show_list_days(30);
                return;
            }

            Clear_list_days();
            Fill_Array_Days_from_Select_Month();
            Show_list_days_from_Select_Month();
        }

        /// <summary>
        /// Кнопка CrateNewDay над листом дней
        /// </summary>
        private void btCrateNewDay_Click(object sender, EventArgs e)
        {
            if (Check_state_edit())
                return;
           //Create_New_Day_Handler(-1);
        }

        private void lb_To_7_Click(object sender, EventArgs e)
        {
            Preparation_Panel_From_Show();
            Show_list_days(6);
            Show_Selected_Day(0);
        }

        private void lb_To_30_Click(object sender, EventArgs e)
        {
            Preparation_Panel_From_Show();
            Show_list_days(29);
            Show_Selected_Day(0);
        }
        private void Preparation_Panel_From_Show()
        {
            if (cbMonth.SelectedIndex != -1)// Если раннее был выведен список дней выбраного месяца, ТО:
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
        #endregion

        #region Кнопки [Save][Delete][COMBO_BOX] [MainPanel]

        private void btSave_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(lbDate.Text);

            if (!check_valid_data())
                return;

            realization();

            command = SqlCommand.Update_Day(currentUserID, float.Parse(lbCash.Text), float.Parse(lbCard.Text), float.Parse(lbIOwe.Text), float.Parse(lbOweMe.Text),
           float.Parse(lbSaved.Text), float.Parse(lbWasted.Text), lb_Wasted_Str.Text, float.Parse(lbIncome.Text), lb_In_Come_Str.Text, date, db.getConnection());


            db.openConnection();
            command.ExecuteNonQuery();
            db.closeConnection();

            cancel();
            End_Edit();
        }

        private void realization()
        {
            float quantity = float.Parse(tb_quantity.Text);
            float cash = float.Parse(lbCash.Text);
            float card = float.Parse(lbCard.Text);
            float saved = float.Parse(lbSaved.Text);
            float owe_me = float.Parse(lbOweMe.Text);
            float i_owe = float.Parse(lbIOwe.Text);

            switch (buttons_Push)
            {
                case Buttons_Push.WASTED:
                    {
                        if (cellState2 == CellState.Cash)
                        {
                            lbCash.Text = (cash - quantity).ToString();
                        }
                        else if (cellState2 == CellState.Card)
                        {
                            lbCard.Text = (card - quantity).ToString();
                        }
                        lb_Wasted_Str.Text += $"[-{tb_quantity.Text}]   -{tb_comment.Text}\n";
                    }
                    break;
                case Buttons_Push.OWE_ME: // Дал в долг
                    {
                        if (cellState2 == CellState.Cash)
                        {
                            lbCash.Text = (cash - quantity).ToString();
                        }
                        else if (cellState2 == CellState.Card)
                        {
                            lbCard.Text = (card - quantity).ToString();
                        }
                        else if (cellState2 == CellState.Saved)
                        {
                            lbSaved.Text = (saved - quantity).ToString();
                        }
                        lbOweMe.Text = (owe_me + quantity).ToString();
                        lb_Wasted_Str.Text += $"[-{tb_quantity.Text}]   -{tb_comment.Text}\n";
                    }
                    break;
                case Buttons_Push.TRANSFER:
                    {
                        if (cellState1 == CellState.Cash && cellState3 == CellState.Card)
                        {
                            lbCash.Text = (cash - quantity).ToString();
                            lbCard.Text = (card + quantity).ToString();
                        }
                        else if (cellState1 == CellState.Cash && cellState3 == CellState.Saved)
                        {
                            lbCash.Text = (cash - quantity).ToString();
                            lbSaved.Text = (saved + quantity).ToString();
                        }
                        else if (cellState1 == CellState.Cash && cellState3 == CellState.I_OWE)
                        {
                            lbCash.Text = (cash - quantity).ToString();
                            lbIOwe.Text = (i_owe - quantity).ToString();
                        }
                        else if (cellState1 == CellState.Card && cellState3 == CellState.Cash)
                        {
                            lbCard.Text = (card - quantity).ToString();
                            lbCash.Text = (cash + quantity).ToString();
                        }
                        else if (cellState1 == CellState.Card && cellState3 == CellState.Saved)
                        {
                            lbCard.Text = (card - quantity).ToString();
                            lbSaved.Text = (saved + quantity).ToString();
                        }
                        else if (cellState1 == CellState.Card && cellState3 == CellState.I_OWE)
                        {
                            lbCard.Text = (card - quantity).ToString();
                            lbIOwe.Text = (i_owe - quantity).ToString();
                        }
                        else if (cellState1 == CellState.Saved && cellState3 == CellState.Cash)
                        {
                            lbSaved.Text = (saved - quantity).ToString();
                            lbCash.Text = (cash + quantity).ToString();
                        }
                        else if (cellState1 == CellState.Saved && cellState3 == CellState.Card)
                        {
                            lbSaved.Text = (saved - quantity).ToString();
                            lbCard.Text = (card + quantity).ToString();
                        }
                        else if (cellState1 == CellState.Saved && cellState3 == CellState.I_OWE)
                        {
                            lbSaved.Text = (saved - quantity).ToString();
                            lbIOwe.Text = (i_owe - quantity).ToString();
                        }
                        else if (cellState1 == CellState.OWE_ME && cellState3 == CellState.Cash)
                        {
                            lbOweMe.Text = (owe_me - quantity).ToString();
                            lbCash.Text = (cash + quantity).ToString();
                        }
                        else if (cellState1 == CellState.OWE_ME && cellState3 == CellState.Card)
                        {
                            lbOweMe.Text = (owe_me - quantity).ToString();
                            lbCard.Text = (card + quantity).ToString();
                        }
                        else if (cellState1 == CellState.OWE_ME && cellState3 == CellState.Saved)
                        {
                            lbOweMe.Text = (owe_me - quantity).ToString();
                            lbSaved.Text = (saved + quantity).ToString();
                        }
                        lb_Wasted_Str.Text += $"[-{tb_quantity.Text}]   -{tb_comment.Text}\n";
                        lb_In_Come_Str.Text += $"[+{tb_quantity.Text}]   -{tb_comment.Text}\n";
                    }
                    break;
                case Buttons_Push.I_OWE:// Беру в долг
                    { 
                    if (cellState2 == CellState.Cash)
                    {
                        lbCash.Text = (cash + quantity).ToString();
                    }
                    else if(cellState2 == CellState.Card)
                    {
                        lbCard.Text = (card + quantity).ToString();
                    }
                    lbIOwe.Text =(i_owe + quantity).ToString();
                    lb_In_Come_Str.Text += $"[+{tb_quantity.Text}]   -{tb_comment.Text}\n";
                    }
                    break;
                case Buttons_Push.IN_COME:
                    {
                        if (cellState2 == CellState.Cash)
                        {
                            lbCash.Text = (cash + quantity).ToString();
                        }
                        else if (cellState2 == CellState.Card)
                        {
                            lbCard.Text = (card + quantity).ToString();
                        }
                        else if (cellState2 == CellState.Saved)
                        {
                            lbSaved.Text = (saved + quantity).ToString();
                        }
                        lb_In_Come_Str.Text += $"[-{tb_quantity.Text}]   -{tb_comment.Text}\n";
                    }
                    break;
            }
        }

        private bool check_valid_data()
        {
            float quantity = float.Parse(tb_quantity.Text);
            float cash = float.Parse(lbCash.Text);
            float saved = float.Parse(lbSaved.Text);
            float i_owe = float.Parse(lbIOwe.Text);
            float owe_me = float.Parse(lbOweMe.Text);

            if (cellState2 == CellState.NULL)
            {
                MessageBox.Show("Выберите ячейку.");
                return false;
            }
            

            if (quantity <= 0)
            {
                MessageBox.Show("Сумма не должна быть меньше или равна Нулю.");
                return false;
            }

            switch (buttons_Push)
            {
                case Buttons_Push.WASTED:
                    {
                        if (cellState2 == CellState.Cash && quantity > cash)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем есть в Наличке");
                            return false;
                        }
                    }
                    break;
                case Buttons_Push.TRANSFER:
                    {
                        if (cellState1 == CellState.NULL || cellState3 == CellState.NULL)
                        {
                            MessageBox.Show("Выберите ячейку.");
                            return false;
                        }

                        if (cellState1 == CellState.Cash && quantity > cash)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем есть в Наличке");
                            return false;
                        }
                        else if (cellState1 == CellState.Saved && quantity > saved)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем есть в Отложено");
                            return false;
                        }
                        else if (cellState3 == CellState.I_OWE && quantity > i_owe)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем Я должен");
                            return false;
                        }
                        else if (cellState1 == CellState.OWE_ME && quantity > owe_me)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем мне должны");
                            return false;
                        }
                    }
                    break;
                case Buttons_Push.OWE_ME:
                    {
                        if (cellState1 == CellState.Cash && quantity > cash)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем есть в Наличке");
                            return false;
                        }
                        else if (cellState1 == CellState.Saved && quantity > saved)
                        {
                            MessageBox.Show("Сумма не должна быть больше чем есть в Отложено");
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
           DialogResult result = MessageBox.Show("Вы действительно хотите удалить етот день?\n" +
                $"{days[Id_selected_day].date.DayOfWeek} [{days[Id_selected_day].date.ToShortDateString()}]",
                "Удаление дня...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            
            if (result == DialogResult.OK)
                Delete_Day();
        }

        /// <summary>
        /// Завершивание редактирования\создания дня
        /// </summary>
        private void End_Edit()
        {            
            Clear_list_days();
            Fill_Top_Bar();
            if (cbMonth.SelectedIndex == -1)
            {
                Fill_Array_Days();
                Show_list_days(6);
                Fill_ComboBox_Month();
            }
            else
            {
                Fill_Array_Days_from_Select_Month();
                Show_list_days_from_Select_Month();
            }

            Show_Selected_Day(Id_selected_day);
        }

        private void Set_state_btDelete()
        {
            if (cbMonth.SelectedIndex == -1 && days.Length == 1)
                bt_Delete.Enabled = false;
            else if (cbMonth.SelectedIndex != -1)
            {
                command = SqlCommand.Сount_Days(currentUserID, db.getConnection());
                
                db.openConnection();
                {
                    reader = command.ExecuteReader();
                    reader.Read();

                    if (reader.GetInt32(0) > 1)
                        bt_Delete.Enabled = true;
                    else
                        bt_Delete.Enabled = false;

                    reader.Close();
                }
                db.closeConnection();
            }
            else
                bt_Delete.Enabled = true;
        }

        /// <summary>
        /// При создании дня - lbNameDay будет менятся в зависиости от выбраного дня в DateTimePicker
        /// </summary>
        private void _dateTime_ValueChanged(object sender, EventArgs e)
        {
            lbNameDay.Text = _dateTime.Value.DayOfWeek.ToString();
        }

        /// <summary>
        /// Проверка находимся ли мы сейчас в состояние редагирования\создания или нет.
        /// </summary>
        /// <returns>[True]-[False]</returns>
        private bool Check_state_edit()
        {
            /*if (state_edit)
            {
                MessageBox.Show("Завершите редактирование", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }*/
            return false;
        }
        #endregion

        DateTimePicker _dateTime;
        
        private void bt_Wasted_Click(object sender, EventArgs e)
        {
            if (buttons_Push == Buttons_Push.WASTED)
            {
                cancel();
                return;
            }
            buttons_Push = Buttons_Push.WASTED;
            bt_Owe_Me.Enabled = false;
            bt_Transfer.Enabled = false;
            bt_I_Owe.Enabled = false;
            bt_In_Come.Enabled = false;

            lbSavedText.Enabled = false;
            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            lbCashText.Cursor = Cursors.Hand;
            lbCashText.Click += LbCashText_Click;
            lbCardText.Cursor = Cursors.Hand;
            lbCardText.Click += LbCardText_Click;

            lbSymbol.Text = "-";
            bt_Wasted.Text = "Отмена";
            btSave.Enabled = true;

            lb_select_cell.ForeColor = Color.IndianRed;
        }

        private void bt_Owe_Me_Click(object sender, EventArgs e)
        {
            if (buttons_Push == Buttons_Push.OWE_ME)
            {
                cancel();
                return;
            }

            buttons_Push = Buttons_Push.OWE_ME;
            bt_Wasted.Enabled = false;
            bt_Transfer.Enabled = false;
            bt_I_Owe.Enabled = false;
            bt_In_Come.Enabled = false;

            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            lbCashText.Cursor = Cursors.Hand;
            lbCashText.Click += LbCashText_Click;
            lbCardText.Cursor = Cursors.Hand;
            lbCardText.Click += LbCardText_Click;
            lbSavedText.Cursor = Cursors.Hand;
            lbSavedText.Click += lbSavedText_Click;

            lbSymbol.Text = "-";
            bt_Owe_Me.Text = "Отмена";
            btSave.Enabled = true;

            lb_select_cell.ForeColor = Color.IndianRed;
        }

        private void bt_Transfer_Click(object sender, EventArgs e)
        {
            if (buttons_Push == Buttons_Push.TRANSFER)
            {
                cancel();
                return;
            }

            buttons_Push = Buttons_Push.TRANSFER;
            bt_Wasted.Enabled = false;
            bt_Owe_Me.Enabled = false;
            bt_I_Owe.Enabled = false;
            bt_In_Come.Enabled = false;

            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            lbCashText.Cursor = Cursors.Hand;
            lbCashText.Click += LbCashText_Click;
            lbCardText.Cursor = Cursors.Hand;
            lbCardText.Click += LbCardText_Click;
            lbSavedText.Cursor = Cursors.Hand;
            lbSavedText.Click += lbSavedText_Click;

            lbSymbol.Text = "-";
            bt_Transfer.Text = "Отмена";
            btSave.Enabled = true;

            lb_select_cell.ForeColor = Color.IndianRed;
        }

        private void cancel()
        {
            // Clear. End EDIT cancel
            buttons_Push = Buttons_Push.NULL;
            // Включаю все кнопки
            bt_Wasted.Enabled = true;
            bt_Owe_Me.Enabled = true;
            bt_Transfer.Enabled = true;
            bt_I_Owe.Enabled = true;
            bt_In_Come.Enabled = true;
            // Включаю все label
            lbCashText.Enabled = true;
            lbCardText.Enabled = true;
            lbSavedText.Enabled = true;
            lbIOweText.Enabled = true;
            lbOweMeText.Enabled = true;

            // Отключаю для label обработчик нажатия
            lbCashText.Cursor = Cursors.Default;
            lbCashText.Click -= LbCashText_Click;

            lbCardText.Cursor = Cursors.Default;
            lbCardText.Click -= LbCardText_Click;

            lbSavedText.Cursor = Cursors.Default;
            lbSavedText.Click -= lbSavedText_Click;

            lbIOweText.Cursor = Cursors.Default;
            //lbIOweText.Click -= ;

            lbOweMeText.Cursor = Cursors.Default;
            //lbOweMeText.Click -= ;

            // Чищу выбраные ячейки
            picture1.Image = null;
            picture2.Image = null;
            picture3.Image = null;
            cellState1 = CellState.NULL;
            cellState2 = CellState.NULL;
            cellState3 = CellState.NULL;

            // Удаляю все из текстовых полей
            tb_comment.Text = "";
            tb_quantity.Text = "";
            lbSymbol.Text = "";

            // Кнопкам возвращаю преведущее название
            bt_Wasted.Text = "Потратил";
            bt_Owe_Me.Text = "Дал в долг";

            
            btSave.Enabled = false;
            //Делит должен ставиться в тру если чек делит позволяет
            lb_select_cell.ForeColor = Color.Gainsboro;
        }

        private void lbSavedText_Click(object sender, EventArgs e)
        {
            picture2.Image = pbSaved.Image;
            if (cellState2 != CellState.TRANSFER)
                cellState2 = CellState.Saved;
        }

        private void LbCardText_Click(object sender, EventArgs e)
        {
            picture2.Image = pbCard.Image;
            if(cellState2 != CellState.TRANSFER)
                cellState2 = CellState.Card;
        }

        private void LbCashText_Click(object sender, EventArgs e)
        {
            picture2.Image = pbCash.Image;
            if (cellState2 != CellState.TRANSFER)
                cellState2 = CellState.Cash;
        }

        private enum Buttons_Push
        { 
            NULL,
            WASTED,
            I_OWE,// Беру в долг (Я должен)
            TRANSFER,
            OWE_ME,// Даю в долг (Мне должны)
            IN_COME
        }
        private enum CellState
        {
            NULL,
            Cash,
            Card,
            Saved,
            I_OWE,
            OWE_ME,
            TRANSFER
        }

        private void tb_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != 44)
            {
                e.Handled = true;
            }
        }

    }
}
