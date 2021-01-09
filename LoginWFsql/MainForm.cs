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
        public Day[] week = new Day[7];

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
            show_Group_List_Day();
            show_Selected_Day();

            /*TextBox tb_cash = new TextBox();
            mainContainer.Panel2.Controls.Add(tb_cash);
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
            lbNameDay.Text = week[i].date.DayOfWeek.ToString();
            lbDate.Text = week[i].date.ToShortDateString();
            lbCash.Text = week[i].cash;
            lbCard.Text = week[i].card;
            lbIOwe.Text = week[i].i_owe;
            lbOweMe.Text = week[i].owe_me;
            lbSaved.Text = week[i].saved;
            lbWasted.Text = week[i].wasted;
            tbWasted.Text = week[i].str_wasted;
            lbIncome.Text = week[i].in_come;
            tbIncome.Text = week[i].str_income;
        }

        private void fill_Array_Week_Days()
        {
            db.openConnection();

            MySqlCommand command = new MySqlCommand("SELECT `cash`, `card`, `i_owe`, `owe_me`, `saved`, `wasted`, `str_wasted`, `in_come`, `str_in_come`, `date` " +
                "FROM days WHERE days.id_user = @currentUserID ORDER BY date DESC", db.getConnection());

            command.Parameters.Add("@currentUserID", MySqlDbType.Int32).Value = currentUserID;

            reader = command.ExecuteReader();

            for (int i = 0; i < week.Length; i++)
            {
                if (reader.Read())
                {
                    while(reader.GetDateTime(9) < DateTime.Today.AddDays(-i))
                    {
                        get_empty_day(i);
                        i++;
                    }

                    get_day_from_base(i);
                }
                else
                    get_empty_day(i);
            }
            reader.Close();
            db.closeConnection();
        }

        private void show_Group_List_Day()
        {
            Color Color_For_Empty = Color.Gray;
            /*Day1*/
            {
                gbDay0.Text = week[0].date.DayOfWeek.ToString();
                _lb0_mini_Date.Text = week[0].date.ToShortDateString();
                _lb0_mini_Cash.Text = week[0].cash;
                _lb0_mini_income.Text = week[0].in_come;
                _lb0_mini_wasted.Text = week[0].wasted;
                if (week[0].is_empty)
                    gbDay0.ForeColor = Color_For_Empty;
            }
            /*Day2*/
            {
                gbDay1.Text = week[1].date.DayOfWeek.ToString();
                _lb1_mini_Date.Text = week[1].date.ToShortDateString();
                _lb1_mini_Cash.Text = week[1].cash;
                _lb1_mini_income.Text = week[1].in_come;
                _lb1_mini_wasted.Text = week[1].wasted;
                if (week[1].is_empty)
                    gbDay1.ForeColor = Color_For_Empty;
            }
            /*Day3*/
            {
                gbDay2.Text = week[2].date.DayOfWeek.ToString();
                _lb2_mini_Date.Text = week[2].date.ToShortDateString();
                _lb2_mini_Cash.Text = week[2].cash;
                _lb2_mini_income.Text = week[2].in_come;
                _lb2_mini_wasted.Text = week[2].wasted;
                if (week[2].is_empty)
                    gbDay2.ForeColor = Color_For_Empty;
            }
            /*Day4*/
            {
                gbDay3.Text = week[3].date.DayOfWeek.ToString();
                _lb3_mini_Date.Text = week[3].date.ToShortDateString();
                _lb3_mini_Cash.Text = week[3].cash;
                _lb3_mini_income.Text = week[3].in_come;
                _lb3_mini_wasted.Text = week[3].wasted;
                if (week[3].is_empty)
                    gbDay3.ForeColor = Color_For_Empty;
            }
            /*Day5*/
            {
                gbDay4.Text = week[4].date.DayOfWeek.ToString();
                _lb4_mini_Date.Text = week[4].date.ToShortDateString();
                _lb4_mini_Cash.Text = week[4].cash;
                _lb4_mini_income.Text = week[4].in_come;
                _lb4_mini_wasted.Text = week[4].wasted;
                if (week[4].is_empty)
                    gbDay4.ForeColor = Color_For_Empty;
            }
            /*Day6*/
            {
                gbDay5.Text = week[5].date.DayOfWeek.ToString();
                _lb5_mini_Date.Text = week[5].date.ToShortDateString();
                _lb5_mini_Cash.Text = week[5].cash;
                _lb5_mini_income.Text = week[5].in_come;
                _lb5_mini_wasted.Text = week[5].wasted;
                if (week[5].is_empty)
                    gbDay5.ForeColor = Color_For_Empty;
            }
            /*Day7*/
            {
                gbDay6.Text = week[6].date.DayOfWeek.ToString();
                _lb6_mini_Date.Text = week[6].date.ToShortDateString();
                _lb6_mini_Cash.Text = week[6].cash;
                _lb6_mini_income.Text = week[6].in_come;
                _lb6_mini_wasted.Text = week[6].wasted;
                if (week[6].is_empty)
                    gbDay6.ForeColor = Color_For_Empty;
            }

        }

        private void get_day_from_base(int i)
        {
            week[i] = new Day();
            week[i].cash = reader.GetString(0);
            week[i].card = reader.GetString(1);
            week[i].i_owe = reader.GetString(2);
            week[i].owe_me = reader.GetString(3);
            week[i].saved = reader.GetString(4);
            week[i].wasted = reader.GetString(5);
            week[i].str_wasted = reader.GetString(6);
            week[i].in_come = reader.GetString(7);
            week[i].str_income = reader.GetString(8);
            week[i].date = reader.GetDateTime(9);
            week[i].is_empty = false;
        }

        private void get_empty_day(int i)
        {
            week[i] = new Day();
            week[i].cash = "--.--";
            week[i].card = "--.--";
            week[i].i_owe = "--.--";
            week[i].owe_me = "--.--";
            week[i].saved = "--.--";
            week[i].wasted = "--.--";
            week[i].str_wasted = "";
            week[i].in_come = "--.--";
            week[i].str_income = "";
            week[i].date = DateTime.Today.AddDays(-i);
            week[i].is_empty = true;
        }


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
