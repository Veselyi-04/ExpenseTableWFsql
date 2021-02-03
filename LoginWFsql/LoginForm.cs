using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoginWFsql
{
    public partial class LoginForm : Form
    {
        DataBase db = new DataBase();
        MySqlCommand command;
        MySqlDataReader reader;

        public LoginForm()
        {
            StartForm();
        }

        private void StartForm()
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

            db.OpenConnection();
            {

                command = SqlCommand.SearchUser(loginUser, db.GetConnection());


                //Здесь считываем даные которые запросили командой
                reader = command.ExecuteReader(); 
                
                if(!reader.HasRows)
                {
                    MessageBox.Show("Пользователя с таким именем не существует", "Неверно указаное имя!");
                    reader.Close();
                    return;
                }
                reader.Close();

                command = SqlCommand.LoginUserGetID(loginUser, passUser, db.GetConnection());
                reader = command.ExecuteReader();
                
                //Провераем считались ли какието даные
                if (reader.HasRows)
                {
                    reader.Read();

                    int id = reader.GetInt32(0); // ето будет ид
                    reader.Close();
                    /*Создаем основную форму, передаем туда ИД и ету форму
                    чтобы потом корректно закрыть програму*/
                    MainForm mf = new MainForm(this, id);
                    mf.Show();//показываем основное окно
                    this.Hide();// текущее (логин) скрываем от пользователя
                }
                else // если нет знаит такого акк не существует
                {
                    MessageBox.Show("Неверный пароль!");
                    reader.Close();
                }
                    
            }db.CloseConnection();
            
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

        private void LoginField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 001 && e.KeyChar != 003 && e.KeyChar != 022)
            {
                e.Handled = true;
            }
        }
        /*--------------------placehold-----------------------------*/
    }
}
