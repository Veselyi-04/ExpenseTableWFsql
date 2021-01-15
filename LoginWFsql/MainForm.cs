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

        // Для проверки находимся ли мы сейчас в редакторе
        private bool state_edit;

        // Для того чтобы выводить ранее выбраный день полсе отмены редактирования
        private int Id_selected_day;

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
            for (int i = 0; i < count; i++)
            {
                // Проверка на случай если будет меньше дней чем мы хочем вывести
                if (days.Length == i)
                    return;

                days[i].Create_New_Group_Box();
                days[i].Click_Button += new DayEventHandler (Day_click_Handler);
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
                days[i].Click_Button += new DayEventHandler(Day_click_Handler);
                days[i].GroupBox.Location = new Point(2, (65 * i + 25));
                mainContainer.Panel1.Controls.Add(days[i].GroupBox);
                days[i].is_show = true;
            }
        }

        private void Day_click_Handler(int INDEX)
        {
            if (Check_state_edit())
                return;
            if (days[INDEX].is_empty)
                Create_New_Day_Handler(INDEX);
            else
                Show_Selected_Day(INDEX);
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
                command = new MySqlCommand(SqlCommand.count_rows_from_main_command, db.getConnection());
                command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
                reader = command.ExecuteReader();
                reader.Read();
                count_days = reader.GetInt32(0);
                reader.Close();
            }

            command = new MySqlCommand(SqlCommand.main_command, db.getConnection());
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
            command = new MySqlCommand(SqlCommand.select_month_command, db.getConnection());
            // заменяем заглушки на реальные значения
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@beforeMonth", MySqlDbType.DateTime).Value = beforeDate;
            command.Parameters.Add("@afterMonth", MySqlDbType.DateTime).Value = afterDate;

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
            cbMonth.Items.Clear();
            cbMonth.Items.Add(" ");

            db.openConnection();
            command = new MySqlCommand(SqlCommand.fill_ComboBox_Month, db.getConnection());
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
                command = new MySqlCommand(SqlCommand.fill_topBar_command, db.getConnection());
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

        private void Create_New_Day_Handler(int i = -1)
        {
            Initialize_Text_Box();
            state_edit = true;

            if (i == -1)
            {
                create_button_Chanel_Save(is_edit: false);
                _dateTime = new DateTimePicker
                {
                    Location = new Point(lbDate.Location.X, lbDate.Location.Y - 5),
                    Value = DateTime.Parse(lbDate.Text),
                    Width = 150,
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Now,
                    Font = lbDate.Font
                };
                _dateTime.ValueChanged += _dateTime_ValueChanged;
                mainContainer.Panel2.Controls.Add(_dateTime);
                _dateTime.BringToFront();
                lbDate.Visible = false;
            }
            else
            {
                create_button_Chanel_Save(is_edit: true);
                lbDate.Text = days[i].date.ToShortDateString();
                lbNameDay.Text = days[i].date.DayOfWeek.ToString();
            }

            tb_Cash.Text = "0";
            tb_Card.Text = "0";
            tb_OweMe.Text = "0";
            tb_IOwe.Text = "0";
            tb_Saved.Text = "0";
            tb_Wasted.Text = "0";
            tb_Income.Text = "0";
            tb_Wasted.Text = "0";
            tb_Income.Text = "0";
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
                Clear_list_days();
                Fill_Array_Days();
                Show_list_days(30);
                return;
            }

            Clear_list_days();
            Fill_Array_Days_from_Select_Month();
            Show_list_days_from_Select_Month();
        }

        private void btCrateNewDay_Click(object sender, EventArgs e)
        {
            if (Check_state_edit())
                return;
            Create_New_Day_Handler(-1);
        }

        private void lb_To_7_Click(object sender, EventArgs e)
        {
            Preparation_Panel_From_Show();
            Show_list_days(6);
        }

        private void lb_To_30_Click(object sender, EventArgs e)
        {
            Preparation_Panel_From_Show();
            Show_list_days(29);
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
        #endregion

        #region Кнопки [Edit][Save][Create] [MainPanel]
        private void btEdit_Click(object sender, EventArgs e)
        {
            Initialize_Text_Box();
            create_button_Chanel_Save(is_edit: true);
            state_edit = true;
            tb_Cash.Text = lbCash.Text;
            tb_Card.Text = lbCard.Text;
            tb_OweMe.Text = lbOweMe.Text;
            tb_IOwe.Text = lbIOwe.Text;
            tb_Saved.Text = lbSaved.Text;
            tb_Wasted.Text = lbWasted.Text;
            tb_Income.Text = lbIncome.Text;
            tb_Wasted.Text = lbWasted.Text;
            tb_Income.Text = lbIncome.Text;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            command = new MySqlCommand(SqlCommand.Update_Day, db.getConnection());
            command.Parameters.Add("@cash", MySqlDbType.String).Value = tb_Cash.Text;
            command.Parameters.Add("@card", MySqlDbType.String).Value = tb_Card.Text;
            command.Parameters.Add("@i_owe", MySqlDbType.String).Value = tb_IOwe.Text;
            command.Parameters.Add("@owe_me", MySqlDbType.String).Value = tb_OweMe.Text;
            command.Parameters.Add("@saved", MySqlDbType.String).Value = tb_Saved.Text;
            command.Parameters.Add("@wasted", MySqlDbType.String).Value = tb_Wasted.Text;
            command.Parameters.Add("@str_wasted", MySqlDbType.String).Value = tb_Wasted_Str.Text;
            command.Parameters.Add("@in_come", MySqlDbType.String).Value = tb_Income.Text;
            command.Parameters.Add("@str_in_come", MySqlDbType.String).Value = tb_In_Come_Str.Text;

            command.Parameters.Add("@date", MySqlDbType.DateTime).Value = days[Id_selected_day].date;
            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;

            db.openConnection();

            try
            {
                command.ExecuteNonQuery();
                db.closeConnection();
            }
            catch (MySqlException ex)
            {

                MessageBox.Show($"Неизвесная Ошибка \n {ex.Message}", $"Ошибка {ex.Number}!",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Clear_list_days();
            Fill_Top_Bar();
            Fill_Array_Days();
            Show_list_days(30);
            Cancel_Edit();
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            command = new MySqlCommand(SqlCommand.Save_NewDay, db.getConnection());

            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
            command.Parameters.Add("@cash", MySqlDbType.String).Value = tb_Cash.Text;
            command.Parameters.Add("@card", MySqlDbType.String).Value = tb_Card.Text;
            command.Parameters.Add("@i_owe", MySqlDbType.String).Value = tb_IOwe.Text;
            command.Parameters.Add("@owe_me", MySqlDbType.String).Value = tb_OweMe.Text;
            command.Parameters.Add("@saved", MySqlDbType.String).Value = tb_Saved.Text;
            command.Parameters.Add("@wasted", MySqlDbType.String).Value = tb_Wasted.Text;
            command.Parameters.Add("@str_wasted", MySqlDbType.String).Value = tb_Wasted_Str.Text;
            command.Parameters.Add("@in_come", MySqlDbType.String).Value = tb_Income.Text;
            command.Parameters.Add("@str_in_come", MySqlDbType.String).Value = tb_In_Come_Str.Text;
            command.Parameters.Add("@date", MySqlDbType.DateTime).Value = _dateTime.Value;

            db.openConnection();

            try
            {
                command.ExecuteNonQuery();
                db.closeConnection();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // 1062 ошибка повторяющегося уникального ключа (в нашем случае ето дата)
                {
                    MessageBox.Show($"День с такой датой уже существует!\nВы можете отредактировать его!", "Ошибка 1062!",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Неизвесная Ошибка \n {ex.Message}", $"Ошибка {ex.Number}!",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            
            Clear_list_days();
            Fill_Top_Bar();
            Fill_ComboBox_Month();
            Fill_Array_Days();
            Show_list_days(30);
            Cancel_Edit();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Cancel_Edit();
        }

        private void create_button_Chanel_Save(bool is_edit)
        {
            btEdit.Visible = false;
            cancel = new Button
            {
                Text = "CANCEL",
                Width = btEdit.Width / 2 - 2,
                Height = btEdit.Height,
                Location = btEdit.Location,
                BackColor = btEdit.BackColor,
                FlatStyle = btEdit.FlatStyle,
                Font = btEdit.Font,
                ForeColor = btEdit.ForeColor,
                TextAlign = btEdit.TextAlign,
            };
            cancel.FlatAppearance.BorderSize = btEdit.FlatAppearance.BorderSize;
            mainContainer.Panel2.Controls.Add(cancel);
            cancel.Click += btCancel_Click;

            save_create = new Button
            {
                Width = btEdit.Width / 2 - 2,
                Height = btEdit.Height,
                Location = new Point(cancel.Location.X + btEdit.Width / 2, cancel.Location.Y),
                BackColor = btEdit.BackColor,
                FlatStyle = btEdit.FlatStyle,
                Font = btEdit.Font,
                ForeColor = btEdit.ForeColor,
                TextAlign = btEdit.TextAlign,
            };
            save_create.FlatAppearance.BorderSize = btEdit.FlatAppearance.BorderSize;
            mainContainer.Panel2.Controls.Add(save_create);

            if (is_edit)
            {
                save_create.Text = "SAVE";
                save_create.Click += btSave_Click;
            }
            else
            {
                save_create.Text = "CREATE";
                save_create.Click += btCreate_Click;
            }
        }

        private void Cancel_Edit()
        {
            state_edit = false;
            if (_dateTime != null)
                mainContainer.Panel2.Controls.Remove(_dateTime);
            mainContainer.Panel2.Controls.Remove(tb_Cash);
            mainContainer.Panel2.Controls.Remove(tb_Card);
            mainContainer.Panel2.Controls.Remove(tb_OweMe);
            mainContainer.Panel2.Controls.Remove(tb_IOwe);
            mainContainer.Panel2.Controls.Remove(tb_Saved);
            mainContainer.Panel2.Controls.Remove(tb_Wasted);
            mainContainer.Panel2.Controls.Remove(tb_Income);
            mainContainer.Panel2.Controls.Remove(tb_Wasted_Str);
            mainContainer.Panel2.Controls.Remove(tb_In_Come_Str);
            mainContainer.Panel2.Controls.Remove(cancel);
            mainContainer.Panel2.Controls.Remove(save_create);

            Show_Selected_Day(0);

            btEdit.Visible = true;
            lbDate.Visible = true;
            lbCash.Visible = true;
            lbCard.Visible = true;
            lbOweMe.Visible = true;
            lbIOwe.Visible = true;
            lbSaved.Visible = true;
            lbWasted.Visible = true;
            lbIncome.Visible = true;
            lb_Wasted_Str.Visible = true;
            lb_In_Come_Str.Visible = true;
        }

        private void _dateTime_ValueChanged(object sender, EventArgs e)
        {
            lbNameDay.Text = _dateTime.Value.DayOfWeek.ToString();
        }

        private bool Check_state_edit()
        {
            if (state_edit)
            {
                MessageBox.Show("Завершите редактировани", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }

        private void Initialize_Text_Box()
        {
            Color bColor = mainContainer.BackColor;
            Color fColor = Color.Gray;

            //
            //lbCash
            //
            tb_Cash = new TextBox
            {
                Size = lbCash.Size,
                Location = lbCash.Location
            };
            tb_Cash.BackColor = bColor;
            tb_Cash.ForeColor = fColor;
            tb_Cash.TextAlign = HorizontalAlignment.Center;
            tb_Cash.BorderStyle = BorderStyle.None;
            tb_Cash.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbCash.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_Cash);
            tb_Cash.BringToFront();
            //
            //lbCard
            //
            tb_Card = new TextBox
            {
                Size = lbCard.Size,
                Location = lbCard.Location
            };
            tb_Card.BackColor = bColor;
            tb_Card.ForeColor = fColor;
            tb_Card.TextAlign = HorizontalAlignment.Center;
            tb_Card.BorderStyle = BorderStyle.None;
            tb_Card.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbCard.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_Card);
            tb_Card.BringToFront();
            //
            //lbOweMe
            //
            tb_OweMe = new TextBox
            {
                Size = lbOweMe.Size,
                Location = lbOweMe.Location
            };
            tb_OweMe.BackColor = bColor;
            tb_OweMe.ForeColor = fColor;
            tb_OweMe.TextAlign = HorizontalAlignment.Center;
            tb_OweMe.BorderStyle = BorderStyle.None;
            tb_OweMe.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbOweMe.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_OweMe);
            tb_OweMe.BringToFront();
            //
            //lbIOwe
            //
            tb_IOwe = new TextBox
            {
                Size = lbIOwe.Size,
                Location = lbIOwe.Location
            };
            tb_IOwe.BackColor = bColor;
            tb_IOwe.ForeColor = fColor;
            tb_IOwe.TextAlign = HorizontalAlignment.Center;
            tb_IOwe.BorderStyle = BorderStyle.None;
            tb_IOwe.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbIOwe.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_IOwe);
            tb_IOwe.BringToFront();
            //
            //lbSaved
            //
            tb_Saved = new TextBox
            {
                Size = lbSaved.Size,
                Location = lbSaved.Location
            };
            tb_Saved.BackColor = bColor;
            tb_Saved.ForeColor = fColor;
            tb_Saved.TextAlign = HorizontalAlignment.Center;
            tb_Saved.BorderStyle = BorderStyle.None;
            tb_Saved.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbSaved.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_Saved);
            //
            //lbWasted
            //
            tb_Wasted = new TextBox
            {
                Size = lbWasted.Size,
                Location = lbWasted.Location
            };
            tb_Wasted.BackColor = bColor;
            tb_Wasted.ForeColor = fColor;
            tb_Wasted.TextAlign = HorizontalAlignment.Center;
            tb_Wasted.BorderStyle = BorderStyle.None;
            tb_Wasted.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbWasted.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_Wasted);
            //
            //lbIncome
            //
            tb_Income = new TextBox
            {
                Size = lbIncome.Size,
                Location = lbIncome.Location
            };
            tb_Income.BackColor = bColor;
            tb_Income.ForeColor = fColor;
            tb_Income.TextAlign = HorizontalAlignment.Center;
            tb_Income.BorderStyle = BorderStyle.None;
            tb_Income.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbIncome.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_Income);
            //
            //lb_Wasted_Str
            //
            tb_Wasted_Str = new TextBox
            {
                Size = lb_Wasted_Str.Size,
                Location = lb_Wasted_Str.Location
            };
            tb_Wasted_Str.BackColor = Color.FromArgb(35, 35, 35);
            tb_Wasted_Str.ForeColor = fColor;
            tb_Wasted_Str.TextAlign = HorizontalAlignment.Left;
            tb_Wasted_Str.BorderStyle = BorderStyle.None;
            tb_Wasted_Str.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            tb_Wasted_Str.Multiline = true;
            tb_Wasted_Str.ScrollBars = ScrollBars.Vertical;
            lb_Wasted_Str.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_Wasted_Str);
            //
            //lb_In_Come_Str
            //
            tb_In_Come_Str = new TextBox
            {
                Size = lb_In_Come_Str.Size,
                Location = lb_In_Come_Str.Location
            };
            tb_In_Come_Str.BackColor = Color.FromArgb(35, 35, 35);
            tb_In_Come_Str.ForeColor = fColor;
            tb_In_Come_Str.TextAlign = HorizontalAlignment.Left;
            tb_In_Come_Str.BorderStyle = BorderStyle.None;
            tb_In_Come_Str.Font = new Font("a_LatinoNr", 10f, FontStyle.Regular, GraphicsUnit.Point, 204);
            tb_In_Come_Str.Multiline = true;
            tb_In_Come_Str.ScrollBars = ScrollBars.Vertical;

            lb_In_Come_Str.Visible = false;
            mainContainer.Panel2.Controls.Add(tb_In_Come_Str);
            //////////////////////////////////////////////////////////
        }
        #endregion

        

        

        Button cancel;
        Button save_create;
        DateTimePicker _dateTime;
        TextBox tb_Cash;
        TextBox tb_Card;
        TextBox tb_OweMe;
        TextBox tb_IOwe;
        TextBox tb_Saved;
        TextBox tb_Wasted;
        TextBox tb_Income;
        TextBox tb_Wasted_Str;
        TextBox tb_In_Come_Str;
    }
}
