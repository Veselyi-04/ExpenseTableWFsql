using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginWFsql
{
    public class Day
    {
        private int index;
        public float cash;
        public float card;
        public float i_owe;
        public float owe_me;
        public float saved;
        public float wasted;
        public string str_wasted;
        public float in_come;
        public string str_income;
        public DateTime date;

        public bool is_show = false;

        public Day(int index)
        {
            this.index = index + 1;
        }

        public GroupBox Create_and_get_Group()
        {
            //
            // InitializeComponent
            //
            groupBox = new GroupBox();
            lb_in_come = new Label();
            lb_wasted = new Label();
            lb_cash = new Label();
            lb_date = new Label();
            bt_show = new Button();
            pb_wasted = new PictureBox();
            pb_cash = new PictureBox();
            pb_income = new PictureBox();
            line_income = new Panel();
            line_wasted = new Panel();
            line_cash = new Panel();
            // 
            // groupBox
            //
            groupBox.BackColor = Color.FromArgb(45, 45, 45);
            groupBox.Controls.Add(lb_in_come);
            groupBox.Controls.Add(lb_wasted);
            groupBox.Controls.Add(lb_cash);
            groupBox.Controls.Add(lb_date);
            groupBox.Controls.Add(bt_show);
            groupBox.Controls.Add(pb_wasted);
            groupBox.Controls.Add(pb_cash);
            groupBox.Controls.Add(pb_income);
            groupBox.Controls.Add(line_income);
            groupBox.Controls.Add(line_wasted);
            groupBox.Controls.Add(line_cash);
            groupBox.Cursor = Cursors.Hand;
            groupBox.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox.ForeColor = Color.White;
            groupBox.Location = new Point(2, (65 * index + 25)); // 20 ето пложение начального захардкоженого дня
            groupBox.Size = new Size(205, 60);
            groupBox.Text = date.DayOfWeek.ToString();
            //
            // lb_in_come
            //
            lb_in_come.AutoEllipsis = true;
            lb_in_come.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_in_come.Location = new Point(96, 15);
            lb_in_come.Size = new Size(40, 15);
            lb_in_come.Text = in_come.ToString();
            //
            // lb_wasted
            //
            lb_wasted.AutoEllipsis = true;
            lb_wasted.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_wasted.Location = new Point(96, 36);
            lb_wasted.Size = new Size(40, 15);
            lb_wasted.Text = wasted.ToString();
            //
            // lb_cash
            //
            lb_cash.AutoEllipsis = true;
            lb_cash.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_cash.Location = new Point(25, 36);
            lb_cash.Size = new Size(40, 15);
            lb_cash.Text = cash.ToString();
            //
            // lb_date
            //
            lb_date.AutoEllipsis = true;
            lb_date.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_date.BorderStyle = BorderStyle.FixedSingle;
            lb_date.Location = new Point(146, 8);
            lb_date.Size = new Size(58, 15);
            lb_date.Text = date.ToShortDateString();
            //
            // bt_show
            //
            bt_show.FlatAppearance.BorderSize = 0;
            bt_show.FlatStyle = FlatStyle.Flat;
            bt_show.Font = new Font("a_LatinoNr", 8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            bt_show.Location = new Point(145, 25);
            bt_show.Size = new Size(58, 30);
            bt_show.Text = "SHOW";
            bt_show.UseVisualStyleBackColor = true;
            //
            // pb_wasted
            //
            pb_wasted.Image = Properties.Resources.money_wasted3;
            pb_wasted.Location = new Point(76, 29);
            pb_wasted.Name = "pictureBox20";
            pb_wasted.Size = new Size(20, 20);
            pb_wasted.SizeMode = PictureBoxSizeMode.StretchImage;
            //
            // pb_cash
            //
            pb_cash.Image = Properties.Resources.purse3;
            pb_cash.Location = new Point(3, 24);
            pb_cash.Name = "pictureBox13";
            pb_cash.Size = new Size(25, 25);
            pb_cash.SizeMode = PictureBoxSizeMode.StretchImage;
            //
            // pb_income
            //
            pb_income.Image = Properties.Resources.money_Income;
            pb_income.Location = new Point(76, 8);
            pb_income.Name = "pictureBox11";
            pb_income.Size = new Size(20, 20);
            pb_income.SizeMode = PictureBoxSizeMode.StretchImage;
            //
            // line_income
            //
            line_income.BackColor = Color.White;
            line_income.Location = new Point(99, 28);
            line_income.Size = new Size(35, 1);
            line_income.BringToFront();
            //
            // line_wasted
            //
            line_wasted.BackColor = Color.White;
            line_wasted.Location = new Point(99, 49);
            line_wasted.Size = new Size(35, 1);
            line_wasted.BringToFront();
            //
            // line_cash
            //
            line_cash.BackColor = Color.White;
            line_cash.Location = new Point(28, 49);
            line_cash.Size = new Size(35, 1);
            line_cash.BringToFront();

            return groupBox;
        }

        public GroupBox Get_GroupBox()
        {
            return groupBox;
        }

        public bool Check_show()
        {
            return is_show;
        }

        private GroupBox groupBox;
        private Label lb_in_come;
        private Label lb_wasted;
        private Label lb_cash;
        private Label lb_date;
        private Button bt_show;
        private PictureBox pb_wasted;
        private PictureBox pb_cash;
        private PictureBox pb_income;
        private Panel line_income;
        private Panel line_wasted;
        private Panel line_cash;

    }

}
