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
        /*Команды для работы с БД*/
        DataBase db = new DataBase();
        MySqlCommand command;
        MySqlDataReader reader;
        /*-----------------------*/

        /*ID текущего пользователя*/
        private int currentUserID;

        /*(основная)Скрытая логин форма, чтобы потом корректно завершить работу програмы */
        private LoginForm LF;

        /*Масив с таблице дней (которая слева)*/
        public Day[] days;

        public MainForm(LoginForm lf, int id)
        {
            currentUserID = id;
            LF = lf;
            startForm();
        }

        private void startForm()
        {
            
            InitializeComponent();
            fill_Top_Bar();
            fill_Array_Week_Days();
            //show_Group_List_Day(); to delet
            show_Selected_Day();

            /*
            mainContainer.Panel2.Controls.Remove(lbCash);

            tb_cash.BackColor = Color.Gray;
            tb_cash.Cursor = Cursors.PanNW;
            tb_cash.BorderStyle = BorderStyle.FixedSingle;
            tb_cash.Location = new Point(lbCash.Location.X, lbCash.Location.Y);
            tb_cash.Multiline = false;
            tb_cash.Name = "tb_cash";
            tb_cash.Size = tb_cash.Size;*/
        
        }

        private void show_Selected_Day(int i = 0)
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

        private void fill_Array_Week_Days()
        {
            db.openConnection();
            int count;

            /* Берем из БД количество заполненых дней у етого пользоватля */
            {
                command = new MySqlCommand("SELECT COUNT(*) FROM days WHERE days.id_user = @currentUserID", db.getConnection());
                command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;
                reader = command.ExecuteReader();
                reader.Read();
                count = reader.GetInt32(0);
                reader.Close();
            }

            command = new MySqlCommand("SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
                "FROM days WHERE days.id_user = @currentUserID ORDER BY date DESC", db.getConnection());

            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;

            reader = command.ExecuteReader();

            days = new Day[count];

            for (int i = 0; i < days.Length; i++)
            {
                reader.Read();
                get_day_from_base(i);

            }
            reader.Close();
            db.closeConnection();
        }

     /*   private void show_Group_List_Day()
        {
            Color Color_For_Empty = Color.Gray;
            //Day1
            {
                gbDay0.Text = days[0].date.DayOfWeek.ToString();
                _lb0_mini_Date.Text = days[0].date.ToShortDateString();
                _lb0_mini_Cash.Text = days[0].cash;
                _lb0_mini_income.Text = days[0].in_come;
                _lb0_mini_wasted.Text = days[0].wasted;
                if (days[0].is_empty)
                    gbDay0.ForeColor = Color_For_Empty;
            }
            //Day2
            {
                gbDay1.Text = days[1].date.DayOfWeek.ToString();
                _lb1_mini_Date.Text = days[1].date.ToShortDateString();
                _lb1_mini_Cash.Text = days[1].cash;
                _lb1_mini_income.Text = days[1].in_come;
                _lb1_mini_wasted.Text = days[1].wasted;
                if (days[1].is_empty)
                    gbDay1.ForeColor = Color_For_Empty;
            }
            //Day3
            {
                gbDay2.Text = days[2].date.DayOfWeek.ToString();
                _lb2_mini_Date.Text = days[2].date.ToShortDateString();
                _lb2_mini_Cash.Text = days[2].cash;
                _lb2_mini_income.Text = days[2].in_come;
                _lb2_mini_wasted.Text = days[2].wasted;
                if (days[2].is_empty)
                    gbDay2.ForeColor = Color_For_Empty;
            }
            //Day4
            {
                gbDay3.Text = days[3].date.DayOfWeek.ToString();
                _lb3_mini_Date.Text = days[3].date.ToShortDateString();
                _lb3_mini_Cash.Text = days[3].cash;
                _lb3_mini_income.Text = days[3].in_come;
                _lb3_mini_wasted.Text = days[3].wasted;
                if (days[3].is_empty)
                    gbDay3.ForeColor = Color_For_Empty;
            }
            //Day5
            {
                gbDay4.Text = days[4].date.DayOfWeek.ToString();
                _lb4_mini_Date.Text = days[4].date.ToShortDateString();
                _lb4_mini_Cash.Text = days[4].cash;
                _lb4_mini_income.Text = days[4].in_come;
                _lb4_mini_wasted.Text = days[4].wasted;
                if (days[4].is_empty)
                    gbDay4.ForeColor = Color_For_Empty;
            }
            //Day6
            {
                gbDay5.Text = days[5].date.DayOfWeek.ToString();
                _lb5_mini_Date.Text = days[5].date.ToShortDateString();
                _lb5_mini_Cash.Text = days[5].cash;
                _lb5_mini_income.Text = days[5].in_come;
                _lb5_mini_wasted.Text = days[5].wasted;
                if (days[5].is_empty)
                    gbDay5.ForeColor = Color_For_Empty;
            }
            //Day7
            {
                gbDay6.Text = days[6].date.DayOfWeek.ToString();
                _lb6_mini_Date.Text = days[6].date.ToShortDateString();
                _lb6_mini_Cash.Text = days[6].cash;
                _lb6_mini_income.Text = days[6].in_come;
                _lb6_mini_wasted.Text = days[6].wasted;
                if (days[6].is_empty)
                    gbDay6.ForeColor = Color_For_Empty;
            }

        }*/ // to delet

        private void get_day_from_base(int i)
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

            mainContainer.Panel1.Controls.Add(days[i].Create_and_get_Group());

        }

        /*private void get_empty_day(int i)
        {
            days[i] = new System.Windows.Forms.Day();
            days[i].cash = "--.--";
            days[i].card = "--.--";
            days[i].i_owe = "--.--";
            days[i].owe_me = "--.--";
            days[i].saved = "--.--";
            days[i].wasted = "--.--";
            days[i].str_wasted = "";
            days[i].in_come = "--.--";
            days[i].str_income = "";
            days[i].date = DateTime.Today.AddDays(-i);
            days[i].is_empty = true;
        }*/ // to-delet


        private void fill_Top_Bar()
        {
            string userlogin;
            int cash;
            int i_owe;
            int saved;
            db.openConnection();
            {
                command = new MySqlCommand(
                    "SELECT users.login, days.cash, days.i_owe, days.saved " +
                    "FROM users " +
                    "INNER JOIN days ON days.id_user = users.id " +
                    "WHERE users.id = @currentUserID"
                    , db.getConnection());

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

    }
}
