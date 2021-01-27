using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LoginWFsql
{
    public enum Currency
    {
        NULL,
        UAH,
        EUR
    }

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

        // Для того чтобы выводить ранее выбраный день полсе отмены редактирования.
        /// <summary>
        /// Запоминает последний день который мы выводили в MainPanel
        /// </summary>
        private int Id_selected_day;

        // Переменные для ля работы с даными из интерфейса.
        private Buttons_Push buttons_Push = Buttons_Push.NULL;
        private CellState cellState1 = CellState.NULL;
        private CellState cellState2 = CellState.NULL;
        private CellState cellState3 = CellState.NULL;
        /// <summary>
        /// true = puctire1 | false = picture3
        /// </summary>
        private bool picture_select;

        // Валюта.
        Currency currency = Currency.UAH;
        string str_currency = "₴"; // €  ₴

        Label[] labels_currency = new Label[11];

        // Кнопки для создания нового дня.
        private DateTimePicker _dateTime = null;
        private Button bt_Create = null;
        private Button bt_Cancel = null;

        public MainForm(LoginForm lf, int id)
        {
            currentUserID = id;
            LF = lf;
            StartForm();
        }

        private void StartForm()
        {
            InitializeComponent();
            Select_Login();
            Fill_Top_Bar();
            Fill_ComboBox_Month();
            Fill_Array_Days();
            Show_list_days(30);
            Show_Selected_Day(0);
            Create_Currency_Labels();
        }

        private void Create_Currency_Labels()
        {
            labels_currency[0] = new Label
            {
                Text = str_currency,
                Location = new Point(_lb_state_cash.Location.X + _lb_state_cash.Width, _lb_state_cash.Location.Y),
                Font = new Font("Consolas;", 12f),
                AutoSize = true,
                ForeColor = _lb_state_cash.ForeColor
            };
            panelTopStats.Controls.Add(labels_currency[0]);

            labels_currency[1] = new Label
            {
                Text = str_currency,
                Location = new Point(_lb_state_i_owe.Location.X + _lb_state_i_owe.Width, _lb_state_i_owe.Location.Y),
                Font = new Font("Consolas;", 12f),
                AutoSize = true,
                ForeColor = _lb_state_i_owe.ForeColor
            };
            panelTopStats.Controls.Add(labels_currency[1]);

            labels_currency[2] = new Label
            {
                Text = str_currency,
                Location = new Point(_lb_state_saved.Location.X + _lb_state_saved.Width, _lb_state_saved.Location.Y),
                Font = new Font("Consolas;", 12f),
                AutoSize = true,
                ForeColor = _lb_state_saved.ForeColor
            };
            panelTopStats.Controls.Add(labels_currency[2]);

            labels_currency[3] = new Label
            {
                Text = str_currency,
                Location = new Point(lbWasted.Location.X + lbWasted.Width, lbWasted.Location.Y),
                Font = lbWasted.Font,
                AutoSize = true,
                ForeColor = lbWasted.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[3]);

            labels_currency[4] = new Label
            {
                Text = str_currency,
                Location = new Point(lbIncome.Location.X + lbIncome.Width, lbIncome.Location.Y),
                Font = lbIncome.Font,
                AutoSize = true,
                ForeColor = lbIncome.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[4]);

            labels_currency[5] = new Label
            {
                Text = str_currency,
                Location = new Point(lbCash.Location.X + lbCash.Width, lbCash.Location.Y),
                Font = lbCash.Font,
                AutoSize = true,
                ForeColor = lbCash.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[5]);

            labels_currency[6] = new Label
            {
                Text = str_currency,
                Location = new Point(lbCard.Location.X + lbCard.Width, lbCard.Location.Y),
                Font = lbCard.Font,
                AutoSize = true,
                ForeColor = lbCard.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[6]);

            labels_currency[7] = new Label
            {
                Text = str_currency,
                Location = new Point(lbIOwe.Location.X + lbIOwe.Width, lbIOwe.Location.Y),
                Font = lbIOwe.Font,
                AutoSize = true,
                ForeColor = lbIOwe.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[7]);

            labels_currency[8] = new Label
            {
                Text = str_currency,
                Location = new Point(lbOweMe.Location.X + lbOweMe.Width, lbOweMe.Location.Y),
                Font = lbOweMe.Font,
                AutoSize = true,
                ForeColor = lbOweMe.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[8]);

            labels_currency[9] = new Label
            {
                Text = str_currency,
                Location = new Point(lbSaved.Location.X + lbSaved.Width, lbSaved.Location.Y),
                Font = lbSaved.Font,
                AutoSize = true,
                ForeColor = lbSaved.ForeColor
            };
            mainContainer.Panel2.Controls.Add(labels_currency[9]);

        }

        private void Clear_Currency_Labels()
        {
            for (int i = 0; i < labels_currency.Length; i++)
            {
                if (i <= 2)
                {
                    panelTopStats.Controls.Remove(labels_currency[i]);
                }
                else
                {
                    mainContainer.Panel2.Controls.Remove(labels_currency[i]);
                }
                labels_currency[i] = null;
            }
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

                days[i].Create_New_Group_Box(currency);
                days[i].Click_Button += new DayEventHandler(btDay_click_Handler);
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
                days[i].Click_Button += new DayEventHandler(btDay_click_Handler);
                days[i].GroupBox.Location = new Point(2, (65 * i + 25));
                mainContainer.Panel1.Controls.Add(days[i].GroupBox);
                days[i].is_show = true;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки SHOW\/Принимает индекс дня
        /// </summary>
        /// <param name="INDEX">Индекс дня, на котором мы нажали кнопку</param>
        private void btDay_click_Handler(int INDEX)
        {
            /*if (Check_state_edit())
                return;*/
            // Тут определяем етот день пустой или нет
            if (days[INDEX].is_empty)// Да: ТОгда ето кнопка создания дня.
            {
                Show_Selected_Day(INDEX);
                End_Create();
                Create_New_Day();
            }
            else// Нет: тогда ето кнопка SHOW и мы показываем етот день
            {
                End_Create();
                Show_Selected_Day(INDEX);
            }
        }

        /// <summary>
        /// Включает или отключаест видимость, кнопки CreateNewDay над списком дней
        /// </summary>
        /// <param name="On_Off"> true = on, false = off.</param>
        private void On_Of_btCrateNewDay(bool On_Off)
        {
            if (On_Off)
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
            tb_Wasted_Str.Text = days[i].str_wasted;
            tb_In_Come_Str.Text = days[i].str_income;

            switch (currency)
            {
                case Currency.UAH:
                    {
                        lbCash.Text = days[i].purse_uah.cash.ToString();
                        lbCard.Text = days[i].purse_uah.card.ToString();
                        lbIOwe.Text = days[i].purse_uah.i_owe.ToString();
                        lbOweMe.Text = days[i].purse_uah.owe_me.ToString();
                        lbSaved.Text = days[i].purse_uah.saved.ToString();
                        lbWasted.Text = days[i].purse_uah.wasted.ToString();
                        lbIncome.Text = days[i].purse_uah.in_come.ToString();
                    }
                    break;
                case Currency.EUR:
                    {
                        lbCash.Text = days[i].purse_eur.cash.ToString();
                        lbCard.Text = days[i].purse_eur.card.ToString();
                        lbIOwe.Text = days[i].purse_eur.i_owe.ToString();
                        lbOweMe.Text = days[i].purse_eur.owe_me.ToString();
                        lbSaved.Text = days[i].purse_eur.saved.ToString();
                        lbWasted.Text = days[i].purse_eur.wasted.ToString();
                        lbIncome.Text = days[i].purse_eur.in_come.ToString();
                    }
                    break;
            }

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
            // Можно будет сделать в запросе sql
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

                while (currentDate != lastDay)
                {
                    Get_empty_day(i, lastDay);

                    lastDay = lastDay.AddDays(-1);
                    i++;
                }

                Get_day_from_base(i);
                lastDay = lastDay.AddDays(-1);
                i++;
            }

            while (i < count_days)
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
            days[i].str_wasted = reader.GetString(0);
            days[i].str_income = reader.GetString(1);
            days[i].date = reader.GetDateTime(2);

            days[i].purse_uah = new Purse();
            days[i].purse_uah.cash = reader.GetFloat(3);
            days[i].purse_uah.card = reader.GetFloat(4);
            days[i].purse_uah.i_owe = reader.GetFloat(5);
            days[i].purse_uah.owe_me = reader.GetFloat(6);
            days[i].purse_uah.saved = reader.GetFloat(7);
            days[i].purse_uah.wasted = reader.GetFloat(8);
            days[i].purse_uah.in_come = reader.GetFloat(9);

            days[i].purse_eur = new Purse();
            days[i].purse_eur.cash = reader.GetFloat(10);
            days[i].purse_eur.card = reader.GetFloat(11);
            days[i].purse_eur.i_owe = reader.GetFloat(12);
            days[i].purse_eur.owe_me = reader.GetFloat(13);
            days[i].purse_eur.saved = reader.GetFloat(14);
            days[i].purse_eur.wasted = reader.GetFloat(15);
            days[i].purse_eur.in_come = reader.GetFloat(16);
        }// purse_uah

        /// <summary>
        /// Заполняет конкретный день масива пустым днем
        /// </summary>
        private void Get_empty_day(int i, DateTime date)
        {
            days[i] = new Day(i);
            days[i].str_wasted = "";
            days[i].str_income = "";
            days[i].date = date;
            days[i].purse_uah = new Purse();
            days[i].purse_uah.cash = 0.0f;
            days[i].purse_uah.card = 0.0f;
            days[i].purse_uah.i_owe = 0.0f;
            days[i].purse_uah.owe_me = 0.0f;
            days[i].purse_uah.saved = 0.0f;
            days[i].purse_uah.wasted = 0.0f;
            days[i].purse_uah.in_come = 0.0f;

            days[i].purse_eur = new Purse();
            days[i].purse_eur.cash = 0.0f;
            days[i].purse_eur.card = 0.0f;
            days[i].purse_eur.i_owe = 0.0f;
            days[i].purse_eur.owe_me = 0.0f;
            days[i].purse_eur.saved = 0.0f;
            days[i].purse_eur.wasted = 0.0f;
            days[i].purse_eur.in_come = 0.0f;

            days[i].is_empty = true;
        }// purse_uah

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
            int cash;
            int i_owe;
            int saved;
            db.openConnection();
            {
                command = SqlCommand.Fill_TopBar(currency, currentUserID, db.getConnection());

                reader = command.ExecuteReader();

                reader.Read();
                {
                    cash = reader.GetInt32(0);
                    i_owe = reader.GetInt32(1);
                    saved = reader.GetInt32(2);
                }
                reader.Close();

            }
            db.closeConnection();

            _lb_state_cash.Text = cash.ToString();
            _lb_state_i_owe.Text = i_owe.ToString();
            _lb_state_saved.Text = saved.ToString();

        }
        private void Select_Login()
        {
            command = SqlCommand.Select_Login(currentUserID, db.getConnection());

            db.openConnection();
            {
                reader = command.ExecuteReader();
                reader.Read();
                _lb_state_userlogin.Text = reader.GetString(0);
                reader.Close();
            }
            db.closeConnection();
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

        /// <summary>
        /// Используется для завершения или отмены создания нового дня.
        /// Удаляет временные кнопки и включает все ранее выключеные кнопки.
        /// </summary>
        private void End_Create()
        {
            this.Controls.Remove(_dateTime);
            this.Controls.Remove(bt_Create);
            this.Controls.Remove(bt_Cancel);
            _dateTime = null;
            bt_Create = null;
            bt_Cancel = null;

            btSave.Visible = true;
            bt_Delete.Visible = true;

            mainContainer.Panel2.Enabled = true;
            tb_quantity.Enabled = true;
            tb_comment.Enabled = true;
            bt_Wasted.Enabled = true;
            bt_Owe_Me.Enabled = true;
            bt_I_Owe.Enabled = true;
            bt_In_Come.Enabled = true;
            bt_Transfer.Enabled = true;
        }

        /// <summary>
        /// Обновляет даные
        /// </summary>
        private void Refresh_Data()
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

        private void Create_New_Day()
        {
            bt_Create = new Button
            {
                Location = new Point(btSave.Location.X + 232, btSave.Location.Y + 40),
                Size = btSave.Size,
                Text = "Create",
                Font = btSave.Font,
                FlatStyle = FlatStyle.Flat,
                ForeColor = btSave.ForeColor,
                BackColor = Color.DarkGreen
            };
            btSave.Visible = false;
            bt_Create.FlatAppearance.BorderSize = 0;
            bt_Create.Click += Bt_Create_Click;
            this.Controls.Add(bt_Create);
            bt_Create.BringToFront();

            bt_Cancel = new Button
            {
                Location = new Point(bt_Delete.Location.X + 232, bt_Delete.Location.Y + 40),
                Size = bt_Delete.Size,
                Text = "Cancel",
                Font = bt_Delete.Font,
                FlatStyle = FlatStyle.Flat,
                ForeColor = bt_Delete.ForeColor,
                BackColor = Color.DarkRed
            };
            bt_Delete.Visible = false;
            bt_Cancel.FlatAppearance.BorderSize = 0;
            bt_Cancel.Click += Bt_Cancel_Click;
            this.Controls.Add(bt_Cancel);
            bt_Cancel.BringToFront();

            mainContainer.Panel2.Enabled = false;
            tb_quantity.Enabled = false;
            tb_comment.Enabled = false;
            bt_Wasted.Enabled = false;
            bt_Owe_Me.Enabled = false;
            bt_I_Owe.Enabled = false;
            bt_In_Come.Enabled = false;
            bt_Transfer.Enabled = false;

            DateTime currentDate;
            currentDate = DateTime.Parse(lbDate.Text);

            command = SqlCommand.Select_PrevDay(currency, currentUserID, currentDate, db.getConnection());
            db.openConnection();
            {
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lbCash.Text = reader.GetString(0);
                    lbCard.Text = reader.GetString(1);
                    lbIOwe.Text = reader.GetString(2);
                    lbOweMe.Text = reader.GetString(3);
                    lbSaved.Text = reader.GetString(4);
                }
                else
                {
                    lbCash.Text = "0";
                    lbCard.Text = "0";
                    lbIOwe.Text = "0";
                    lbOweMe.Text = "0";
                    lbSaved.Text = "0";
                }

                lbIncome.Text = "0";
                tb_In_Come_Str.Text = "";
                lbWasted.Text = "0";
                tb_Wasted_Str.Text = "";
                reader.Close();
            }
            db.closeConnection();
        }

        /// <summary>
        /// При создании дня - lbNameDay будет менятся в зависиости от выбраного дня в DateTimePicker
        /// </summary>
        private void _dateTime_ValueChanged(object sender, EventArgs e)
        {
            lbNameDay.Text = _dateTime.Value.DayOfWeek.ToString();
            lbDate.Text = _dateTime.Value.ToShortDateString();

            DateTime currentDate = DateTime.Parse(lbDate.Text);

            command = SqlCommand.Select_PrevDay(currency, currentUserID, currentDate, db.getConnection());
            db.openConnection();
            {
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lbCash.Text = reader.GetString(0);
                    lbCard.Text = reader.GetString(1);
                    lbIOwe.Text = reader.GetString(2);
                    lbOweMe.Text = reader.GetString(3);
                    lbSaved.Text = reader.GetString(4);
                }
                else
                {
                    lbCash.Text = "0";
                    lbCard.Text = "0";
                    lbIOwe.Text = "0";
                    lbOweMe.Text = "0";
                    lbSaved.Text = "0";
                }

                lbIncome.Text = "0";
                tb_In_Come_Str.Text = "";
                lbWasted.Text = "0";
                tb_Wasted_Str.Text = "";
                reader.Close();
            }
            db.closeConnection();
        }


        #region Кнопки [закрыть]/[свернуть] и движение окна [TopPanel]
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
            btClose.Font = new Font("Crosterian", 17F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
            btClose.Location = new Point(btClose.Location.X - 3, btClose.Location.Y - 3);
            btClose.BackColor = Lighting_Color(btClose.BackColor);
        }

        private void btClose_MouseLeave(object sender, EventArgs e)
        {
            btClose.Font = new Font("Crosterian", 14.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
            btClose.Location = new Point(btClose.Location.X + 3, btClose.Location.Y + 3);
            btClose.BackColor = Blackout_Color(btClose.BackColor);
        }

        private void btMinimize_MouseEnter(object sender, EventArgs e)
        {
            btMinimize.Font = new Font("Crosterian", 22F, FontStyle.Bold);
            btMinimize.Location = new Point(btMinimize.Location.X - 4, btMinimize.Location.Y - 5); //619, -8  -- //615, -13
            btMinimize.BackColor = Lighting_Color(btMinimize.BackColor);
        }

        private void btMinimize_MouseLeave(object sender, EventArgs e)
        {
            this.btMinimize.Font = new Font("Crosterian", 18F, FontStyle.Bold);
            this.btMinimize.Location = new Point(btMinimize.Location.X + 4, btMinimize.Location.Y + 5);
            this.btMinimize.BackColor = Blackout_Color(btMinimize.BackColor);
        }
        /*________________________________________________________________*/
        #endregion

        #region Кнопки [<7][<30], [ComboBoxMonth], [CrateNewDay] -[LeftPanel]
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
            End_Create();
            _dateTime = new DateTimePicker
            {
                Location = new Point(lbDate.Location.X - 4 + 232, lbDate.Location.Y - 6 + 40), // х232, у40 - позиция панель2 в глобальном пространстве
                Size = new Size(150, 25),
                Font = lbDate.Font,
                Value = DateTime.Now,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Now
            };
            lbDate.Text = DateTime.Now.ToShortDateString();
            this.Controls.Add(_dateTime);

            _dateTime.ValueChanged += _dateTime_ValueChanged;
            _dateTime.BringToFront();

            Create_New_Day();
        }

        private void Bt_Cancel_Click(object sender, EventArgs e)
        {
            End_Create();
            Show_Selected_Day(Id_selected_day);
        }

        private void Bt_Create_Click(object sender, EventArgs e)
        {
            command = SqlCommand.Create_NewDay(currentUserID, float.Parse(lbCash.Text), float.Parse(lbCard.Text), float.Parse(lbIOwe.Text), float.Parse(lbOweMe.Text),
           float.Parse(lbSaved.Text), float.Parse(lbWasted.Text), tb_Wasted_Str.Text, float.Parse(lbIncome.Text), tb_In_Come_Str.Text, DateTime.Parse(lbDate.Text), db.getConnection());

            db.openConnection();

            try
            {
                command.ExecuteNonQuery();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвесная ошибка при сохранение даных!\n{ex.Message}", "Eror!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            End_Create();
            Refresh_Data();
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
            lb_To_30.BackColor = Lighting_Color(lb_To_30.BackColor);
        }

        private void lb_To_30_MouseLeave(object sender, EventArgs e)
        {
            lb_To_30.BackColor = Blackout_Color(lb_To_30.BackColor);
        }

        private void lb_To_30_MouseDown(object sender, MouseEventArgs e)
        {
            lb_To_30.BackColor = Blackout_Color(lb_To_30.BackColor);
        }

        private void lb_To_30_MouseUp(object sender, MouseEventArgs e)
        {
            lb_To_30.BackColor = Lighting_Color(lb_To_30.BackColor);
        }

        private void lb_To_7_MouseEnter(object sender, EventArgs e)
        {
            lb_To_7.BackColor = Lighting_Color(lb_To_7.BackColor);
        }

        private void lb_To_7_MouseLeave(object sender, EventArgs e)
        {
            lb_To_7.BackColor = Blackout_Color(lb_To_7.BackColor);
        }

        private void lb_To_7_MouseDown(object sender, MouseEventArgs e)
        {
            lb_To_7.BackColor = Blackout_Color(lb_To_7.BackColor);
        }

        private void lb_To_7_MouseUp(object sender, MouseEventArgs e)
        {
            lb_To_7.BackColor = Lighting_Color(lb_To_7.BackColor);
        }

        /*________________________________________________________________*/
        #endregion

        #region Кнопки [Save][Delete] -[MainPanel]
        private void btSave_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(lbDate.Text);

            if (!check_valid_data())
                return;

            realization();

            command = SqlCommand.Update_Day(currency, currentUserID, float.Parse(lbCash.Text), float.Parse(lbCard.Text), float.Parse(lbIOwe.Text), float.Parse(lbOweMe.Text),
           float.Parse(lbSaved.Text), float.Parse(lbWasted.Text), tb_Wasted_Str.Text, float.Parse(lbIncome.Text), tb_In_Come_Str.Text, date, db.getConnection());


            db.openConnection();
            command.ExecuteNonQuery();
            db.closeConnection();

            cancel();
            Refresh_Data();
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
                        tb_Wasted_Str.Text += $"[-{tb_quantity.Text + str_currency}] {tb_comment.Text}" + Environment.NewLine;
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
                        tb_Wasted_Str.Text += $"[-{tb_quantity.Text + str_currency}] {tb_comment.Text}" + Environment.NewLine;
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
                        tb_Wasted_Str.Text += $"[~{tb_quantity.Text + str_currency}] {tb_comment.Text}" + Environment.NewLine;
                        if (cellState3 != CellState.I_OWE)
                            tb_In_Come_Str.Text += $"[~{tb_quantity.Text + str_currency}] {tb_comment.Text}" + Environment.NewLine;
                    }
                    break;
                case Buttons_Push.I_OWE:// Беру в долг
                    {
                        if (cellState2 == CellState.Cash)
                        {
                            lbCash.Text = (cash + quantity).ToString();
                        }
                        else if (cellState2 == CellState.Card)
                        {
                            lbCard.Text = (card + quantity).ToString();
                        }
                        lbIOwe.Text = (i_owe + quantity).ToString();
                        tb_In_Come_Str.Text += $"[+{tb_quantity.Text + str_currency}] {tb_comment.Text}" + Environment.NewLine;
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
                        tb_In_Come_Str.Text += $"[-{tb_quantity.Text + str_currency}] {tb_comment.Text}" + Environment.NewLine;
                    }
                    break;
            }
        }

        private bool check_valid_data()
        {
            if (tb_quantity.Text == "")
            {
                MessageBox.Show("В поле сума нужно записать знаение");
                return false;
            }

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
        /// Проверяет общее колво дней в БД и включает или отключает кнопку. Чтобы мы не могли удалить последний день.
        /// </summary>
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

        #endregion

        #region Кнопки [Кнопки Манипуляции Даными] [CURRENCY] [MainPanel]
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

            pb_currency.Enabled = false;
            switch (currency)
            {
                case Currency.UAH:
                    pb_currency.Image = Properties.Resources.uah0;
                    break;
                case Currency.EUR:
                    pb_currency.Image = Properties.Resources.eur0;
                    break;
            }

            lbSavedText.Enabled = false;
            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            picture2.Image = Properties.Resources.selected_wait1;
            lb_select_cell.Visible = true;

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

            pb_currency.Enabled = false;
            switch (currency)
            {
                case Currency.UAH:
                    pb_currency.Image = Properties.Resources.uah0;
                    break;
                case Currency.EUR:
                    pb_currency.Image = Properties.Resources.eur0;
                    break;
            }

            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            picture2.Image = Properties.Resources.selected_wait1;
            lb_select_cell.Visible = true;

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
            if (buttons_Push == Buttons_Push.TRANSFER || buttons_Push == Buttons_Push.TRANSFER_CURRENCY)
            {
                cancel();
                return;
            }

            buttons_Push = Buttons_Push.TRANSFER;
            cellState2 = CellState.TRANSFER;
            bt_Wasted.Enabled = false;
            bt_Owe_Me.Enabled = false;
            bt_I_Owe.Enabled = false;
            bt_In_Come.Enabled = false;
            bt_Transfer_Currency.Visible = true;

            pb_currency.Enabled = false;
            switch (currency)
            {
                case Currency.UAH:
                    pb_currency.Image = Properties.Resources.uah0;
                    break;
                case Currency.EUR:
                    pb_currency.Image = Properties.Resources.eur0;
                    break;
            }

            lbIOweText.Enabled = false;
            lbCashText.Enabled = true;
            lbCardText.Enabled = true;
            lbSavedText.Enabled = true;
            lbOweMeText.Enabled = true;

            picture_select = true;
            picture1.Image = Properties.Resources.selected_wait1;
            picture1.Cursor = Cursors.Hand;
            picture1.Click += Picture1_Click;

            picture2.Image = Properties.Resources.transfer;
            lb_select_cell.Visible = true;


            picture3.Image = Properties.Resources.selected_wait0;
            picture3.Cursor = Cursors.Hand;
            picture3.Click += Picture3_Click;

            lbCashText.Cursor = Cursors.Hand;
            lbCashText.Click += LbCashText_Click;
            lbCardText.Cursor = Cursors.Hand;
            lbCardText.Click += LbCardText_Click;
            lbSavedText.Cursor = Cursors.Hand;
            lbSavedText.Click += lbSavedText_Click;
            lbOweMeText.Cursor = Cursors.Hand;
            lbOweMeText.Click += LbOweMeText_Click;
            lbIOweText.Cursor = Cursors.Hand;
            lbIOweText.Click += LbIOweText_Click;

            lbSymbol.Text = "~";
            bt_Transfer.Text = "Отмена";
            btSave.Enabled = true;

            lb_select_cell.ForeColor = Color.IndianRed;
        }

        private void bt_I_Owe_Click(object sender, EventArgs e)
        {
            if (buttons_Push == Buttons_Push.I_OWE)
            {
                cancel();
                return;
            }

            buttons_Push = Buttons_Push.I_OWE;
            bt_Wasted.Enabled = false;
            bt_Owe_Me.Enabled = false;
            bt_I_Owe.Enabled = true;
            bt_In_Come.Enabled = false;
            bt_Transfer.Enabled = false;

            pb_currency.Enabled = false;
            switch (currency)
            {
                case Currency.UAH:
                    pb_currency.Image = Properties.Resources.uah0;
                    break;
                case Currency.EUR:
                    pb_currency.Image = Properties.Resources.eur0;
                    break;
            }

            lbCashText.Enabled = true;
            lbCardText.Enabled = true;
            lbSavedText.Enabled = false;
            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            picture2.Image = Properties.Resources.selected_wait1;
            lb_select_cell.Visible = true;

            lbCashText.Cursor = Cursors.Hand;
            lbCashText.Click += LbCashText_Click;
            lbCardText.Cursor = Cursors.Hand;
            lbCardText.Click += LbCardText_Click;

            lbSymbol.Text = "+";
            bt_I_Owe.Text = "Отмена";
            btSave.Enabled = true;

            lb_select_cell.ForeColor = Color.IndianRed;
        }

        private void bt_In_Come_Click(object sender, EventArgs e)
        {
            if (buttons_Push == Buttons_Push.IN_COME)
            {
                cancel();
                return;
            }

            buttons_Push = Buttons_Push.IN_COME;
            bt_Wasted.Enabled = false;
            bt_Owe_Me.Enabled = false;
            bt_I_Owe.Enabled = false;
            bt_In_Come.Enabled = true;
            bt_Transfer.Enabled = false;

            pb_currency.Enabled = false;
            switch (currency)
            {
                case Currency.UAH:
                    pb_currency.Image = Properties.Resources.uah0;
                    break;
                case Currency.EUR:
                    pb_currency.Image = Properties.Resources.eur0;
                    break;
            }

            lbCashText.Enabled = true;
            lbCardText.Enabled = true;
            lbSavedText.Enabled = true;
            lbIOweText.Enabled = false;
            lbOweMeText.Enabled = false;

            picture2.Image = Properties.Resources.selected_wait1;
            lb_select_cell.Visible = true;

            lbCashText.Cursor = Cursors.Hand;
            lbCashText.Click += LbCashText_Click;
            lbCardText.Cursor = Cursors.Hand;
            lbCardText.Click += LbCardText_Click;
            lbSavedText.Cursor = Cursors.Hand;
            lbSavedText.Click += lbSavedText_Click;

            lbSymbol.Text = "+";
            bt_In_Come.Text = "Отмена";
            btSave.Enabled = true;

            lb_select_cell.ForeColor = Color.IndianRed;
        }

        private void Picture1_Click(object sender, EventArgs e)
        {
            _picture_select_halder(true);

            if (buttons_Push != Buttons_Push.TRANSFER_CURRENCY)
            {
                lbIOweText.Enabled = false;
                lbCashText.Enabled = true;
                lbCardText.Enabled = true;
                lbSavedText.Enabled = true;
                lbOweMeText.Enabled = true;
            }
        }
        private void Picture3_Click(object sender, EventArgs e)
        {
            _picture_select_halder(false);

            if (buttons_Push != Buttons_Push.TRANSFER_CURRENCY)
            {
                lbIOweText.Enabled = true;
                lbCashText.Enabled = true;
                lbCardText.Enabled = true;
                lbSavedText.Enabled = true;
                lbOweMeText.Enabled = false;
            }

        }
        private void _picture_select_halder(bool true_false)
        {
            if (true_false)
            {
                picture1.Image = Properties.Resources.selected_wait1;
                cellState1 = CellState.NULL;
                if (cellState3 == CellState.NULL)
                    picture3.Image = Properties.Resources.selected_wait0;

                picture_select = true;
            }
            else
            {
                picture3.Image = Properties.Resources.selected_wait1;
                cellState3 = CellState.NULL;
                if (cellState1 == CellState.NULL)
                    picture1.Image = Properties.Resources.selected_wait0;

                picture_select = false;
            }

        }

        private void LbIOweText_Click(object sender, EventArgs e)
        {
            Set_Cell_Halder(CellState.I_OWE);
        }

        private void LbOweMeText_Click(object sender, EventArgs e)
        {
            Set_Cell_Halder(CellState.OWE_ME);
        }

        private void lbSavedText_Click(object sender, EventArgs e)
        {
            Set_Cell_Halder(CellState.Saved);
        }

        private void LbCardText_Click(object sender, EventArgs e)
        {
            Set_Cell_Halder(CellState.Card);
        }

        private void LbCashText_Click(object sender, EventArgs e)
        {
            Set_Cell_Halder(CellState.Cash);
        }

        private void Set_Cell_Halder(CellState pressed_lb)
        {
            if (buttons_Push == Buttons_Push.TRANSFER || buttons_Push == Buttons_Push.TRANSFER_CURRENCY)
            {
                switch (pressed_lb)
                {
                    case CellState.Cash:
                        {
                            if (picture_select)
                            {
                                cellState1 = CellState.Cash;
                                picture1.Image = pbCash.Image;
                            }
                            else
                            {
                                cellState3 = CellState.Cash;
                                picture3.Image = pbCash.Image;
                            }
                        }
                        break;
                    case CellState.Card:
                        {
                            if (picture_select)
                            {
                                cellState1 = CellState.Card;
                                picture1.Image = pbCard.Image;
                            }
                            else
                            {
                                cellState3 = CellState.Card;
                                picture3.Image = pbCard.Image;
                            }
                        }
                        break;
                    case CellState.Saved:
                        {
                            if (picture_select)
                            {
                                cellState1 = CellState.Saved;
                                picture1.Image = pbSaved.Image;
                            }
                            else
                            {
                                cellState3 = CellState.Saved;
                                picture3.Image = pbSaved.Image;
                            }
                        }
                        break;
                    case CellState.I_OWE:
                        {
                            if (picture_select)
                            {
                                cellState1 = CellState.I_OWE;
                                picture1.Image = pbIOwe.Image;
                            }
                            else
                            {
                                cellState3 = CellState.I_OWE;
                                picture3.Image = pbIOwe.Image;
                            }
                        }
                        break;
                    case CellState.OWE_ME:
                        {
                            if (picture_select)
                            {
                                cellState1 = CellState.OWE_ME;
                                picture1.Image = pbOweMe.Image;
                            }
                            else
                            {
                                cellState3 = CellState.OWE_ME;
                                picture3.Image = pbOweMe.Image;
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (pressed_lb)
                {
                    case CellState.Cash:
                        {
                            cellState2 = CellState.Cash;
                            picture2.Image = pbCash.Image;
                        }
                        break;
                    case CellState.Card:
                        {
                            cellState2 = CellState.Card;
                            picture2.Image = pbCard.Image;
                        }
                        break;
                    case CellState.Saved:
                        {
                            cellState2 = CellState.Saved;
                            picture2.Image = pbSaved.Image;
                        }
                        break;
                    case CellState.I_OWE:
                        {
                            cellState2 = CellState.I_OWE;
                            picture2.Image = pbIOwe.Image;
                        }
                        break;
                    case CellState.OWE_ME:
                        {
                            cellState2 = CellState.OWE_ME;
                            picture2.Image = pbOweMe.Image;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Не разрешает в поле для сумы записать лишних символов
        /// </summary>
        private void tb_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != 44)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Прекращает редагирование
        /// </summary>
        private void cancel()
        {
            if (buttons_Push == Buttons_Push.TRANSFER_CURRENCY)
                cancel_transfer_currency();

            // Clear. End EDIT cancel
            buttons_Push = Buttons_Push.NULL;
            // Включаю все кнопки
            bt_Wasted.Enabled = true;
            bt_Owe_Me.Enabled = true;
            bt_Transfer.Enabled = true;
            bt_I_Owe.Enabled = true;
            bt_In_Come.Enabled = true;
            bt_Transfer_Currency.Visible = false;

            pb_currency.Enabled = true;
            switch (currency)
            {
                case Currency.UAH:
                    pb_currency.Image = Properties.Resources.UAH1;
                    break;
                case Currency.EUR:
                    pb_currency.Image = Properties.Resources.EUR1;
                    break;
            }

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
            lbIOweText.Click -= LbIOweText_Click;
            lbOweMeText.Cursor = Cursors.Default;
            lbOweMeText.Click -= LbOweMeText_Click;

            // Чищу выбраные ячейки
            picture1.Image = null;
            picture1.Cursor = Cursors.Default;
            picture1.Click -= Picture1_Click;

            lb_select_cell.Visible = false;

            picture2.Image = null;

            picture3.Image = null;
            picture3.Cursor = Cursors.Default;
            picture3.Click -= Picture3_Click;
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
            bt_Transfer.Text = "Перевод";
            bt_I_Owe.Text = "Взял в долг";
            bt_In_Come.Text = "Доход";

            btSave.Enabled = false;
            //Делит должен ставиться в тру если чек делит позволяет
            lb_select_cell.ForeColor = Color.Gainsboro;
        }

        private void pb_currency_Click(object sender, EventArgs e)
        {
            switch (currency)
            {
                case Currency.UAH:
                    currency = Currency.EUR;
                    pb_currency.Image = Properties.Resources.EUR1;
                    str_currency = "€";
                    break;
                case Currency.EUR:
                    currency = Currency.UAH;
                    pb_currency.Image = Properties.Resources.UAH1;
                    str_currency = "₴";
                    break;
            }

            Show_Selected_Day(Id_selected_day);
            Clear_list_days();
            Show_list_days(6);
            Fill_Top_Bar();

            Clear_Currency_Labels();
            Create_Currency_Labels();
        }

        private void pb_currency_MouseEnter(object sender, EventArgs e)
        {
            pb_currency.Size = new Size(pb_currency.Width + 5, pb_currency.Height + 5);
            pb_currency.Location = new Point(pb_currency.Location.X - 2, pb_currency.Location.Y - 2);
        }

        private void pb_currency_MouseLeave(object sender, EventArgs e)
        {
            pb_currency.Size = new Size(pb_currency.Width - 5, pb_currency.Height - 5);
            pb_currency.Location = new Point(pb_currency.Location.X + 2, pb_currency.Location.Y + 2);
        }

        private enum Buttons_Push
        {
            NULL,
            WASTED,
            I_OWE,// Беру в долг (Я должен)
            TRANSFER,
            TRANSFER_CURRENCY,
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

        #endregion

        #region Перевод между Валютами!

        private void bt_Transfer_Currency_Click(object sender, EventArgs e)
        {
            buttons_Push = Buttons_Push.TRANSFER_CURRENCY;

            // Отключение всего лишнего.
            {
                lbCashText.Visible = false;
                lbCash.Visible = false;
                lbCardText.Visible = false;
                lbCard.Visible = false;
                lbSavedText.Visible = false;
                lbSaved.Visible = false;
                lbOweMeText.Visible = false;
                lbOweMe.Visible = false;
                lbIOweText.Visible = false;
                lbIOwe.Visible = false;

                lbSymbol.Visible = false;
                lb_select_cell.Visible = false;
                bt_Transfer_Currency.Visible = false;

                pbOweMe.Visible = false;
                pbIOwe.Visible = false;
            }

            // Подготовление верхней части (Purse).
            {
                // Картинки
                pbCash.Size = new Size(40, 40);
                pbCash.Location = new Point(38, 49);
                pbSaved.Size = new Size(40, 40);
                pbSaved.Location = new Point(195, 49);
                pbCard.Size = new Size(40, 40);
                pbCard.Location = new Point(343, 49);

                // Создание новых Label для значений из Purse-UAH/EUR.
                lb_Cash_UAH = new Label
                {
                    Location = new Point(81, 49),
                    Font = new Font("Consolas", 12f),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Text = days[Id_selected_day].purse_uah.cash.ToString(),
                    Cursor = Cursors.Hand,
                };
                lb_Cash_EUR = new Label
                {
                    Location = new Point(81, 70),
                    Font = new Font("Consolas", 12f),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Text = days[Id_selected_day].purse_eur.cash.ToString(),
                    Cursor = Cursors.Hand,
                };
                lb_Saved_UAH = new Label
                {
                    Location = new Point(241, 49),
                    Font = new Font("Consolas", 12f),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Text = days[Id_selected_day].purse_uah.saved.ToString(),
                    Cursor = Cursors.Hand,
                };
                lb_Saved_EUR = new Label
                {
                    Location = new Point(241, 70),
                    Font = new Font("Consolas", 12f),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Text = days[Id_selected_day].purse_eur.saved.ToString(),
                    Cursor = Cursors.Hand,
                };
                lb_Card_UAH = new Label
                {
                    Location = new Point(389, 49),
                    Font = new Font("Consolas", 12f),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Text = days[Id_selected_day].purse_uah.card.ToString(),
                    Cursor = Cursors.Hand,
                };
                lb_Card_EUR = new Label
                {
                    Location = new Point(389, 70),
                    Font = new Font("Consolas", 12f),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Text = days[Id_selected_day].purse_eur.card.ToString(),
                    Cursor = Cursors.Hand,
                };

                // Добавление обработчиков нажатия
                lb_Cash_UAH.Click += Lb_Cash_UAH_Click;
                labels_currency[5].Click += Lb_Cash_UAH_Click;
                lb_Cash_EUR.Click += Lb_Cash_EUR_Click;
                labels_currency[6].Click += Lb_Cash_EUR_Click;
                lb_Saved_UAH.Click += Lb_Saved_UAH_Click;
                labels_currency[7].Click += Lb_Saved_UAH_Click;
                lb_Saved_EUR.Click += Lb_Saved_EUR_Click;
                labels_currency[8].Click += Lb_Saved_EUR_Click;
                lb_Card_UAH.Click += Lb_Card_UAH_Click;
                labels_currency[9].Click += Lb_Card_UAH_Click;
                lb_Card_EUR.Click += Lb_Card_EUR_Click;

                // Добавление обработчиков наводки
                lb_Cash_UAH.MouseEnter += Lb_Cash_UAH_MouseEnter;
                lb_Cash_UAH.MouseLeave += Lb_Cash_UAH_MouseLeave;
                lb_Cash_EUR.MouseEnter += Lb_Cash_EUR_MouseEnter; ;
                lb_Cash_EUR.MouseLeave += Lb_Cash_EUR_MouseLeave; ;
                lb_Saved_UAH.MouseEnter += Lb_Saved_UAH_MouseEnter; ;
                lb_Saved_UAH.MouseLeave += Lb_Saved_UAH_MouseLeave; ;
                lb_Saved_EUR.MouseEnter += Lb_Saved_EUR_MouseEnter; ;
                lb_Saved_EUR.MouseLeave += Lb_Saved_EUR_MouseLeave; ;
                lb_Card_UAH.MouseEnter += Lb_Card_UAH_MouseEnter; ;
                lb_Card_UAH.MouseLeave += Lb_Card_UAH_MouseLeave; ;
                lb_Card_EUR.MouseEnter += Lb_Card_EUR_MouseEnter; ;
                lb_Card_EUR.MouseLeave += Lb_Card_EUR_MouseLeave; ;

                // Добавление их на панель.
                mainContainer.Panel2.Controls.Add(lb_Cash_UAH);
                mainContainer.Panel2.Controls.Add(lb_Cash_EUR);
                mainContainer.Panel2.Controls.Add(lb_Saved_UAH);
                mainContainer.Panel2.Controls.Add(lb_Saved_EUR);
                mainContainer.Panel2.Controls.Add(lb_Card_UAH);
                mainContainer.Panel2.Controls.Add(lb_Card_EUR);

                // Значки валют для новых Label
                // lb_Cash_UAH
                labels_currency[5].Location = new Point(lb_Cash_UAH.Location.X + lb_Cash_UAH.Width, lb_Cash_UAH.Location.Y);
                labels_currency[5].Text = "₴";
                labels_currency[5].Cursor = Cursors.Hand;
                labels_currency[6].Location = new Point(lb_Cash_EUR.Location.X + lb_Cash_EUR.Width, lb_Cash_EUR.Location.Y);
                labels_currency[6].Text = "€";
                labels_currency[6].Cursor = Cursors.Hand;
                // lb_Saved_UAH
                labels_currency[7].Location = new Point(lb_Saved_UAH.Location.X + lb_Saved_UAH.Width, lb_Saved_UAH.Location.Y);
                labels_currency[7].Text = "₴";
                labels_currency[7].Font = new Font("Consolas", 12f);
                labels_currency[7].Cursor = Cursors.Hand;
                labels_currency[8].Location = new Point(lb_Saved_EUR.Location.X + lb_Saved_EUR.Width, lb_Saved_EUR.Location.Y);
                labels_currency[8].Text = "€";
                labels_currency[8].Font = new Font("Consolas", 12f);
                labels_currency[8].Cursor = Cursors.Hand;
                // lb_Card_UAH
                labels_currency[9].Location = new Point(lb_Card_UAH.Location.X + lb_Card_UAH.Width, lb_Card_UAH.Location.Y);
                labels_currency[9].Text = "₴";
                labels_currency[9].Font = new Font("Consolas", 12f);
                labels_currency[9].Cursor = Cursors.Hand;
                labels_currency[10] = new Label
                {
                    Text = "€",
                    Font = new Font("Consolas", 12f),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Location = new Point(lb_Card_EUR.Location.X + lb_Card_EUR.Width, lb_Card_EUR.Location.Y)
                };
                labels_currency[10].Cursor = Cursors.Hand;
                labels_currency[10].Click += Lb_Card_EUR_Click;
                mainContainer.Panel2.Controls.Add(labels_currency[10]);
            }

            //Подготовка нижней части
            {
                // Левый текст бокс
                // tb_quantity
                // -
                tb_quantity.Location = new Point(55, 114);
                tb_quantity.Enter += Tb_quantity_Enter;

                lb_sum_1.Location = new Point(tb_quantity.Location.X, tb_quantity.Location.Y - 16);
                lb_sum_1.Text = "сума: -";

                lb_currency1 = new Label
                {
                    Text = "€/₴",
                    Location = new Point(tb_quantity.Location.X + tb_quantity.Width, tb_quantity.Location.Y + 8),
                    Font = new Font("Consolas", 12f),
                    AutoSize = true,
                    ForeColor = Color.White,
                };
                mainContainer.Panel2.Controls.Add(lb_currency1);


                // Правый текст бокс
                // tb_quantity2
                // +
                tb_quantity2 = new TextBox
                {
                    Location = new Point(307, tb_quantity.Location.Y),
                    Font = new Font("Consolas", 12f),
                    AutoSize = true,
                    TextAlign = HorizontalAlignment.Center,
                    ForeColor = tb_quantity.ForeColor,
                    BorderStyle = tb_quantity.BorderStyle,
                    BackColor = tb_quantity.BackColor,
                };
                tb_quantity2.Enter += Tb_quantity2_Enter;
                mainContainer.Panel2.Controls.Add(tb_quantity2);
                tb_quantity2.BringToFront();

                lb_sum_2 = new Label
                {
                    Text = "сумa: +",
                    Location = new Point(tb_quantity2.Location.X, tb_quantity2.Location.Y - 16),
                    Font = lb_sum_1.Font,
                    AutoSize = true,
                    ForeColor = lb_sum_1.ForeColor,
                };
                mainContainer.Panel2.Controls.Add(lb_sum_2);


                lb_currency2 = new Label
                {
                    Text = "€/₴",
                    Location = new Point(tb_quantity2.Location.X - 35, tb_quantity2.Location.Y + 8),
                    Font = new Font("Consolas", 12f),
                    AutoSize = true,
                    ForeColor = Color.White,
                };
                mainContainer.Panel2.Controls.Add(lb_currency2);

                // Картинки
                picture1.Size = new Size(35, 35);
                picture1.Location = new Point(tb_quantity.Location.X - picture1.Width - 5, tb_quantity.Location.Y - 5);
                picture2.Size = new Size(35, 35);

                // Ето для того чтобы средняя картинка была всегда по середине между двумя ТекстБоксами
                int X = (tb_quantity.Location.X + tb_quantity2.Location.X + tb_quantity2.Width) / 2 - (picture2.Width / 2);
                picture2.Location = new Point(X, tb_quantity.Location.Y - 5);

                picture3.Size = new Size(35, 35);
                picture3.Location = new Point(tb_quantity2.Location.X + tb_quantity2.Width + 5, tb_quantity2.Location.Y - 5);

                // Подсказка
                lb_prompt = new Label
                {
                    Text = "Введите суму которую нужно отнять и суму которую нужно прибавить!",
                    Location = new Point(picture1.Location.X + 10, picture1.Location.Y + picture1.Height),
                    Font = new Font("Constantia", 8F, FontStyle.Bold, GraphicsUnit.Point, 204),
                    AutoSize = true,
                    ForeColor = Color.IndianRed,
                };
                mainContainer.Panel2.Controls.Add(lb_prompt);
                lb_prompt.BringToFront();
            }
        }

        private void cancel_transfer_currency()
        {
            // Включение всего нужного.
            {
                lbCashText.Visible = true;
                lbCash.Visible = true;
                lbCardText.Visible = true;
                lbCard.Visible = true;
                lbSavedText.Visible = true;
                lbSaved.Visible = true;
                lbOweMeText.Visible = true;
                lbOweMe.Visible = true;
                lbIOweText.Visible = true;
                lbIOwe.Visible = true;

                lbSymbol.Visible = true;
                lb_select_cell.Visible = true;
                bt_Transfer_Currency.Visible = true;

                pbOweMe.Visible = true;
                pbIOwe.Visible = true;
            }

            // Откат верхней части (Purse).
            {
                // Картинки Откат
                pbCash.Size = new Size(30, 30);
                pbCash.Location = new Point(13, 44);
                pbSaved.Size = new Size(25, 25);
                pbSaved.Location = new Point(20, 75);
                pbCard.Size = new Size(30, 30);
                pbCard.Location = new Point(263, 45);

                // Удаление новых Label для значений из Purse-UAH/EUR.
                mainContainer.Panel2.Controls.Remove(lb_Cash_UAH);
                mainContainer.Panel2.Controls.Remove(lb_Cash_EUR);
                mainContainer.Panel2.Controls.Remove(lb_Saved_UAH);
                mainContainer.Panel2.Controls.Remove(lb_Saved_EUR);
                mainContainer.Panel2.Controls.Remove(lb_Card_UAH);
                mainContainer.Panel2.Controls.Remove(lb_Card_EUR);
                lb_Cash_UAH = null;
                lb_Cash_EUR = null;
                lb_Saved_UAH = null;
                lb_Saved_EUR = null;
                lb_Card_UAH = null;
                lb_Card_EUR = null;

                // Удаление обработчиков нажатия
                labels_currency[5].Click -= Lb_Cash_UAH_Click;
                labels_currency[6].Click -= Lb_Cash_EUR_Click;
                labels_currency[7].Click -= Lb_Saved_UAH_Click;
                labels_currency[8].Click -= Lb_Saved_EUR_Click;
                labels_currency[9].Click -= Lb_Card_UAH_Click;

                // Значки валют для новых Label
                Clear_Currency_Labels();
                Create_Currency_Labels();

                mainContainer.Panel2.Controls.Remove(labels_currency[10]);
                labels_currency[10] = null;
            }

            //Откат нижней части
            {
                // Левый текст бокс
                // tb_quantity
                tb_quantity.Location = new Point(181, 139);
                tb_quantity.Enter -= Tb_quantity_Enter;

                lb_sum_1.Location = new Point(tb_quantity.Location.X, tb_quantity.Location.Y - 16);
                lb_sum_1.Text = "сума:";

                mainContainer.Panel2.Controls.Remove(lb_currency1);
                lb_currency1 = null;

                // Правый текст бокс
                // tb_quantity2
                mainContainer.Panel2.Controls.Remove(tb_quantity2);
                tb_quantity2 = null;

                mainContainer.Panel2.Controls.Remove(lb_sum_2);
                lb_sum_2 = null;

                mainContainer.Panel2.Controls.Remove(lb_currency2);
                lb_currency2 = null;
                

                // Картинки
                picture1.Size = new Size(30, 30);
                picture1.Location = new Point(45, 117);

                picture2.Size = new Size(30, 30);
                picture2.Location = new Point(81, 117);

                picture3.Size = new Size(30, 30);
                picture3.Location = new Point(117, 117);

                // Подсказка
                mainContainer.Panel2.Controls.Remove(lb_prompt);
                lb_prompt = null;
            }
        }

        // Обработчики нажатия
        private void Tb_quantity_Enter(object sender, EventArgs e)
        {
            if (cellState1 == CellState.NULL)
                _picture_select_halder(true);// Выбрана левая ячейка
        }

        private void Tb_quantity2_Enter(object sender, EventArgs e)
        {
            if (cellState3 == CellState.NULL)
                _picture_select_halder(false); // Выбрана правая ячейка
        }

        private void Lb_Card_EUR_Click(object sender, EventArgs e)
        {
            Lb_EUR_UAH_Click_HALDER(CellState.Card, Currency.EUR);
        }

        private void Lb_Card_UAH_Click(object sender, EventArgs e)
        {
            Lb_EUR_UAH_Click_HALDER(CellState.Card, Currency.UAH);
        }

        private void Lb_Saved_EUR_Click(object sender, EventArgs e)
        {
            Lb_EUR_UAH_Click_HALDER(CellState.Saved, Currency.EUR);
        }

        private void Lb_Saved_UAH_Click(object sender, EventArgs e)
        {
            Lb_EUR_UAH_Click_HALDER(CellState.Saved, Currency.UAH);
        }

        private void Lb_Cash_EUR_Click(object sender, EventArgs e)
        {
            Lb_EUR_UAH_Click_HALDER(CellState.Cash, Currency.EUR);
        }

        private void Lb_Cash_UAH_Click(object sender, EventArgs e)
        {
            Lb_EUR_UAH_Click_HALDER(CellState.Cash, Currency.UAH);
        }

        private void Lb_EUR_UAH_Click_HALDER(CellState cell_select, Currency currency)
        {
            switch (cell_select)
            {
                case CellState.Cash:
                    {
                        Set_Cell_Halder(CellState.Cash);
                    }
                    break;
                case CellState.Card:
                    {
                        Set_Cell_Halder(CellState.Card);
                    }
                    break;
                case CellState.Saved:
                    {
                        Set_Cell_Halder(CellState.Saved);
                    }
                    break;
            }

            switch (currency)
            {
                case Currency.UAH:
                    {
                        if (picture_select)
                            lb_currency1.Text = "UAH";
                        else
                            lb_currency2.Text = "UAH";
                    }
                    break;
                case Currency.EUR:
                    {
                        if (picture_select)
                            lb_currency1.Text = "EUR";
                        else
                            lb_currency2.Text = "EUR";
                    }
                    break;
            }
        }
        /*--------------------------------------------------------------------------------------------------*/


        // Обработка наведенея 
        private void Lb_Card_EUR_MouseEnter(object sender, EventArgs e)
        {
            lb_Card_EUR.BackColor = Blackout_Color(lb_Card_EUR.BackColor);
        }
        private void Lb_Card_UAH_MouseEnter(object sender, EventArgs e)
        {
            lb_Card_UAH.BackColor = Blackout_Color(lb_Card_UAH.BackColor);
        }
        private void Lb_Saved_EUR_MouseEnter(object sender, EventArgs e)
        {
            lb_Saved_EUR.BackColor = Blackout_Color(lb_Saved_EUR.BackColor);
        }
        private void Lb_Saved_UAH_MouseEnter(object sender, EventArgs e)
        {
            lb_Saved_UAH.BackColor = Blackout_Color(lb_Saved_UAH.BackColor);
        }
        private void Lb_Cash_EUR_MouseEnter(object sender, EventArgs e)
        {
            lb_Cash_EUR.BackColor = Blackout_Color(lb_Cash_EUR.BackColor);
        }
        private void Lb_Cash_UAH_MouseEnter(object sender, EventArgs e)
        {
            lb_Cash_UAH.BackColor = Blackout_Color(lb_Cash_UAH.BackColor);
        }

        private void Lb_Card_UAH_MouseLeave(object sender, EventArgs e)
        {
            lb_Card_UAH.BackColor = Lighting_Color(lb_Card_UAH.BackColor);
        }
        private void Lb_Saved_EUR_MouseLeave(object sender, EventArgs e)
        {
            lb_Saved_EUR.BackColor = Lighting_Color(lb_Saved_EUR.BackColor);
        }
        private void Lb_Saved_UAH_MouseLeave(object sender, EventArgs e)
        {
            lb_Saved_UAH.BackColor = Lighting_Color(lb_Saved_UAH.BackColor);
        }
        private void Lb_Cash_EUR_MouseLeave(object sender, EventArgs e)
        {
            lb_Cash_EUR.BackColor = Lighting_Color(lb_Cash_EUR.BackColor);
        }
        private void Lb_Cash_UAH_MouseLeave(object sender, EventArgs e)
        {
            lb_Cash_UAH.BackColor = Lighting_Color(lb_Cash_UAH.BackColor);
        }
        private void Lb_Card_EUR_MouseLeave(object sender, EventArgs e)
        {
            lb_Card_EUR.BackColor = Lighting_Color(lb_Card_EUR.BackColor);
        }
        /*--------------------------------------------------------------------------------------------------*/

        TextBox tb_quantity2;
        Label lb_sum_2;
        Label lb_currency1;
        Label lb_currency2;
        Label lb_prompt;

        Label lb_Cash_UAH;
        Label lb_Cash_EUR;
        Label lb_Saved_UAH;
        Label lb_Saved_EUR;
        Label lb_Card_UAH;
        Label lb_Card_EUR;

        #endregion


        /// <summary>
        /// Возвращает цвет немного темнее чем тот что сюда передали
        /// </summary>
        private Color Blackout_Color(Color current)
        {
            int r = current.R - 10;
            int g = current.G - 10;
            int b = current.B - 10;
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Возвращает цвет немного светлее чем тот что сюда передали
        /// </summary>
        /// <param name="button"></param>
        private Color Lighting_Color(Color current)
        {
            int r = current.R + 10;
            int g = current.G + 10;
            int b = current.B + 10;
            return Color.FromArgb(r, g, b);
        }

        
    }
}
