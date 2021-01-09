using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginWFsql
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            startform();
        }

        private void startform()
        {
            InitializeComponent();
            /* placehold (подсказка в текстовых полях) */
            LoginField.Text = "Enter login...";
            LoginField.ForeColor = Color.Gray;

            PasswordField.Text = "Enter password...";
            PasswordField.ForeColor = Color.Gray;
            PasswordField.UseSystemPasswordChar = false;
            
        }

        private void btRegistration_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm(this);
            this.Hide();
            regForm.Show();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string loginUser = LoginField.Text;
            string passUser = PasswordField.Text;

            DataBase db = new DataBase();
            db.openConnection();
            {

                // Для команд MySql
                MySqlCommand command = new MySqlCommand("SELECT `id` FROM `users` " +
                    "WHERE `login`= @uL " +
                    "AND `pass` = @uP",
                    db.getConnection()); // @uL - @uP заглушка чтобы не передавать туда реальные даные чтоб их было сложнее взломать

                //Здесь в заглушки помещаем реальные даные
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;
                command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = passUser;

                //Здесь считываем даные которые запросили командой
                MySqlDataReader reader = command.ExecuteReader(); 

                //Провераем ситались ли какието даные
                if (reader.HasRows)
                {
                    int id;
                    reader.Read();
                    {
                        id = reader.GetInt32(0); // ето будет ид
                    }reader.Close();//считываем Ид пользователя в ->id
                    

                    /*Создаем основную форму, передаем туда ИД и ету форму
                    чтобы потом корректно закрыть програму*/
                    MainForm mf = new MainForm(this, id);
                    mf.Show();//показываем основное окно
                    this.Hide();// текущее (логин) скрываем от пользователя
                }
                else // если нет знаит такого акк не существует
                    MessageBox.Show("Такого пользователя не существует");
            }db.closeConnection();
            
        }


        /*--------------------placehold-----------------------------*/
        private void LoginField_Enter(object sender, EventArgs e)
        {
            if (LoginField.Text == "Enter login...")
            {
                LoginField.Text = "";
                LoginField.ForeColor = Color.White;
            }
        }
        private void LoginField_Leave(object sender, EventArgs e)
        {
            if (LoginField.Text == "")
            {
                LoginField.Text = "Enter login...";
                LoginField.ForeColor = Color.Gray;
            }
        }
        private void PasswordField_Enter(object sender, EventArgs e)
        {
            if (PasswordField.Text == "Enter password...")
            {
                PasswordField.Text = "";
                PasswordField.ForeColor = Color.White;
                PasswordField.UseSystemPasswordChar = true;
            }
        }
        private void PasswordField_Leave(object sender, EventArgs e)
        {
            if (PasswordField.Text == "")
            {
                PasswordField.Text = "Enter password...";
                PasswordField.ForeColor = Color.Gray;
                PasswordField.UseSystemPasswordChar = false;
            }
        }
        /*--------------------placehold-----------------------------*/
    }
}
