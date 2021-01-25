using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginWFsql
{
    public delegate void DayEventHandler(int INDEX);

    public class Purse
    {
        public float cash;
        public float card;
        public float i_owe;
        public float owe_me;
        public float saved;
        public float wasted;
        public float in_come;
    }


    public class Day
    {
        
        private readonly int index;

        public Purse purse_uah;
        public Purse purse_eur;

        public string str_wasted;
        public string str_income;
        public DateTime date;

        public Color emptyColor = Color.Gray;
        public bool is_show = false;
        public bool is_empty = false;

        public event DayEventHandler Click_Button = null;

        public Day(int index)
        {
            this.index = index;
        }
        
        /// <summary>
        /// Создает новый GroupBox(), для дальнейшего его вывода в Лист с днями.
        /// Сам определяет пустой ето день или заполненый.
        /// </summary>
        public void Create_New_Group_Box(Currency currency = Currency.NULL)
        {
            if (is_empty)
                Create_Empty_Group_Box();
            else
                Create_Filled_Group_Box(currency);
        }

        /// <summary>
        /// Создает груп бокс существующего дня.
        /// </summary>
        private void Create_Filled_Group_Box(Currency currency)
        {
            //
            // InitializeComponent
            //
            GroupBox = new GroupBox();
            lb_in_come = new Label();
            lb_wasted = new Label();
            lb_cash = new Label();
            lb_date = new Label();
            lb_currency = new Label();
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
            GroupBox.BackColor = Color.FromArgb(45, 45, 45);
            GroupBox.Controls.Add(lb_in_come);
            GroupBox.Controls.Add(lb_wasted);
            GroupBox.Controls.Add(lb_cash);
            GroupBox.Controls.Add(lb_date);
            GroupBox.Controls.Add(lb_currency);
            GroupBox.Controls.Add(bt_show);
            GroupBox.Controls.Add(pb_wasted);
            GroupBox.Controls.Add(pb_cash);
            GroupBox.Controls.Add(pb_income);
            GroupBox.Controls.Add(line_income);
            GroupBox.Controls.Add(line_wasted);
            GroupBox.Controls.Add(line_cash);
            GroupBox.Cursor = Cursors.Hand;
            GroupBox.Font = new Font("Arial Rounded MT", 8.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GroupBox.ForeColor = Color.White; // 65 ето ширина груп бокса
            GroupBox.Location = new Point(4, (65 * index + 67)); // 68 ето начальное минимальное положение так как выше еще есть кнопки
            GroupBox.Size = new Size(204, 60);
            GroupBox.Text = date.DayOfWeek.ToString();
            //
            // lb_in_come
            //
            lb_in_come.AutoEllipsis = true;
            lb_in_come.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_in_come.Location = new Point(96, 15);
            lb_in_come.Size = new Size(42, 15);
            //
            // lb_wasted
            //
            lb_wasted.AutoEllipsis = true;
            lb_wasted.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_wasted.Location = new Point(96, 36);
            lb_wasted.Size = new Size(42, 15);
            //
            // lb_cash
            //
            lb_cash.AutoEllipsis = true;
            lb_cash.Font = new Font("a_LatinoNr", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_cash.Location = new Point(25, 36);
            lb_cash.Size = new Size(40, 15);
            //
            // lb_currency
            //
            lb_currency.Font = new Font("Consolas", 15F);
            lb_currency.Location = new Point(50, 14);
            lb_currency.AutoSize = true;
            switch (currency)
            {
                case Currency.UAH:
                    {
                        lb_in_come.Text = purse_uah.in_come.ToString();
                        lb_wasted.Text = purse_uah.wasted.ToString();
                        lb_cash.Text = purse_uah.cash.ToString();
                        lb_currency.Text = "₴";
                    }
                    break;
                case Currency.EUR:
                    {
                        lb_in_come.Text = purse_eur.in_come.ToString();
                        lb_wasted.Text = purse_eur.wasted.ToString();
                        lb_cash.Text = purse_eur.cash.ToString();
                        lb_currency.Text = "€";
                    }
                    break;
            }
            //
            // lb_date
            //
            lb_date.AutoEllipsis = true;
            lb_date.Font = new Font("Arial Rounded MT Bold", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lb_date.BorderStyle = BorderStyle.FixedSingle;
            lb_date.Location = new Point(145, 7);
            lb_date.Size = new Size(58, 15); 
            lb_date.Text = date.ToShortDateString();
            //
            // bt_show
            //
            bt_show.FlatAppearance.BorderSize = 0;
            bt_show.FlatStyle = FlatStyle.Flat;
            bt_show.Font = new Font("Arial Rounded MT Bold", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bt_show.Location = new Point(145, 25);
            bt_show.Size = new Size(54, 30);//58
            bt_show.Text = "SHOW";
            bt_show.UseVisualStyleBackColor = true;
            bt_show.Click += new EventHandler(Invoke_Event);

            //
            // pb_wasted
            //
            pb_wasted.Image = Properties.Resources.wasted5;
            pb_wasted.Location = new Point(73, 32); // 76 29
            pb_wasted.Name = "pictureBox20";
            pb_wasted.Size = new Size(25, 25);
            pb_wasted.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_wasted.BringToFront();
            //
            // pb_cash
            //
            pb_cash.Image = Properties.Resources.purse3;
            pb_cash.Location = new Point(3, 27);
            pb_cash.Name = "pictureBox13";
            pb_cash.Size = new Size(28, 28);
            pb_cash.SizeMode = PictureBoxSizeMode.StretchImage;
            //
            // pb_income
            //
            pb_income.Image = Properties.Resources.income4;
            pb_income.Location = new Point(73, 11); // 76 8
            pb_income.Name = "pictureBox11";
            pb_income.Size = new Size(25, 25);
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
        }

        /// <summary>
        /// Создает пустой груп бокс, (чтото вроде заглушки с кнопкой для создания дня на том месте).
        /// </summary>
        private void Create_Empty_Group_Box()
        {
            //
            // InitializeComponent
            //
            GroupBox = new GroupBox();
            lb_date = new Label();
            bt_show = new Button();
            // 
            // groupBox
            //
            GroupBox.Controls.Add(lb_date);
            GroupBox.Controls.Add(bt_show);
            GroupBox.Cursor = Cursors.Hand;
            GroupBox.BackColor = Color.FromArgb(45, 45, 45);
            GroupBox.Font = new Font("Arial Rounded MT", 8.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GroupBox.ForeColor = emptyColor;
            GroupBox.Size = new Size(204, 60);
            //GroupBox.Scale(new SizeF(0.95f, 0.95f));
            GroupBox.Location = new Point(4, (65 * index + 25)); // 25 ето начальное минимальное положение так как выше еще есть 3 кнопки (3 потомучто 4 мы скрываем)
            GroupBox.Text = date.DayOfWeek.ToString();
            //
            // lb_date
            //
            lb_date.AutoEllipsis = true;
            lb_date.AutoSize = true;
            lb_date.Font = new Font("Arial Rounded MT Bold", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lb_date.Location = new Point(60, 15);
            lb_date.Text = date.ToShortDateString();
            //
            // bt_show
            //
            bt_show.FlatAppearance.BorderSize = 0;
            bt_show.FlatStyle = FlatStyle.Flat;
            bt_show.Font = new Font("Arial Rounded MT Bold", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bt_show.Text = "CREATE NEW DAY";
            bt_show.TextAlign = ContentAlignment.BottomCenter;
            bt_show.Dock = DockStyle.Bottom;
            bt_show.Location = new Point(3, 14);
            bt_show.Size = new Size(194, 39);
            bt_show.UseVisualStyleBackColor = true;
            bt_show.Click += new EventHandler(Invoke_Event);
            //bt_show.BackColor = Color.FromArgb(55, 55, 55);
        }
        private void Invoke_Event(object sender, EventArgs e)
        {
            Click_Button.Invoke(index);
        }

        /// <summary>
        /// Возвращает булевое значение для проверки, выведен ли етот день в список
        /// </summary>
        /// <returns> bool: is_show </returns>
        public bool Check_show()
        {
            return is_show;
        }

        public GroupBox GroupBox { get; set; }
        private Label lb_in_come;
        private Label lb_wasted;
        private Label lb_cash;
        private Label lb_date;
        private Label lb_currency;
        public Button bt_show { get; set; }
        private PictureBox pb_wasted;
        private PictureBox pb_cash;
        private PictureBox pb_income;
        private Panel line_income;
        private Panel line_wasted;
        private Panel line_cash;
    }

}
