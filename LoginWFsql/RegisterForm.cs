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
    public partial class RegisterForm : Form
    {
        private LoginForm LG;
        public RegisterForm() /* конструтор 1 */
        {
            startform();
        }
        public RegisterForm(LoginForm lg) /* конструтор 2 */
        {
            LG = lg;
            startform();
        }

        /*--------------------placehold-----------------------------*/
        /* Поле ввода  имени*/
        private void NameField_Enter(object sender, EventArgs e)
        {
            namePlaceHold(state: true);
        }
        private void NameField_Leave(object sender, EventArgs e)
        {
            namePlaceHold(state: false);
        }
        /* Поле ввода логина*/
        private void LoginField_Enter(object sender, EventArgs e)
        {
            loginPlaceHold(state: true);
        }
        private void LoginField_Leave(object sender, EventArgs e)
        {
            loginPlaceHold(state: false);
        }
        /* Поле ввода пароля */
        private void PasswordField_Enter(object sender, EventArgs e)
        {
            passwordPlaceHold(state: true);
        }
        private void PasswordField_Leave(object sender, EventArgs e)
        {
            passwordPlaceHold(state: false);
        }
        /* Поле проверки пароля*/
        private void PassCheckField_Enter(object sender, EventArgs e)
        {
            passCheckPlaceHold(state: true);
        }
        private void PassCheckField_Leave(object sender, EventArgs e)
        {
            passCheckPlaceHold(state: false);
        }

        /*--------------------placehold--functions----------------------*/
        private void namePlaceHold(bool state)
        {
            string text_placehold = "введите имя...";
            if (!state && NameField.Text == "") // если выходим из поля
            {
                NameField.Text = text_placehold;
                NameField.ForeColor = Color.Gray;
            }
            else if (state && NameField.Text == text_placehold) // если заходим в поле
            {
                NameField.Text = "";
                NameField.ForeColor = Color.White;
            }
        }
        private void loginPlaceHold(bool state)
        {
            string text_placehold = "*придумайте логин...";
            if (!state && LoginField.Text == "") // если выходим из поля
            {
                LoginField.Text = text_placehold;
                LoginField.ForeColor = Color.Gray;
            }
            else if (state && LoginField.Text == text_placehold) // если заходим в поле
            {
                LoginField.Text = "";
                LoginField.ForeColor = Color.White;
            }
        }
        private void passwordPlaceHold(bool state)
        {
            string text_placehold = "*придумайте пароль...";
            if (!state && PasswordField.Text == "")// если выходим из поля
            {
                PasswordField.Text = text_placehold;
                PasswordField.ForeColor = Color.Gray;
                PasswordField.UseSystemPasswordChar = false;
            }
            else if (state && PasswordField.Text == text_placehold)// если заходим в поле
            {
                PasswordField.Text = "";
                PasswordField.ForeColor = Color.White;
                PasswordField.UseSystemPasswordChar = true;
            }
        }
        private void passCheckPlaceHold(bool state)
        {
            string text_placehold = "*повторите пароль...";
            if (!state && PassCheckField.Text == "")// если выходим из поля
            {
                PassCheckField.Text = text_placehold;
                PassCheckField.ForeColor = Color.Gray;
                PassCheckField.UseSystemPasswordChar = false;
            }
            else if (state && PassCheckField.Text == text_placehold)// если заходим в поле
            {
                PassCheckField.Text = "";
                PassCheckField.ForeColor = Color.White;
                PassCheckField.UseSystemPasswordChar = true;
            }
        }


        private void btOnOfShowPass_Click(object sender, EventArgs e)
        {
            // временный костыль
            if (PasswordField.Text == "*придумайте пароль..." || PasswordField.Text == "")
                return;

            PasswordField.UseSystemPasswordChar = !PasswordField.UseSystemPasswordChar;
            btOnOfShowPass.Visible = false;
            btOnOfShowPass2.Visible = true;
        }

        private void btOnOfShowPass2_Click(object sender, EventArgs e)
        {
            PasswordField.UseSystemPasswordChar = !PasswordField.UseSystemPasswordChar;
            btOnOfShowPass.Visible = true;
            btOnOfShowPass2.Visible = false;
        }

        private void btRegistration_Click(object sender, EventArgs e)
        {
            if (!checkValidData())
                return;

            DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand("INSERT INTO `users` (`login`, `pass`)" +
                "VALUES(@login, @pass)", db.getConnection());

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = LoginField.Text;
            command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = PasswordField.Text;

            db.openConnection();
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Вы успешно зарегистрировались!\n Авторизуйтесь!");
                    LG.Show();
                    this.Close();
                }
                else
                    MessageBox.Show("Ошибка регистрации!");
            }
            db.closeConnection();
        }

        private bool checkValidData()
        {
            if (!validLogin())
                return false;
            if (!validPassword())
                return false;
            return true;
        }

        private bool validPassword()
        {
            /* Проверка на пустоту и пробелы */
            if (PasswordField.Text == "" || PasswordField.Text.Contains(' '))
            {
                MessageBox.Show("Поле: Пароль - должно быть заполнено!\n" +
                    "И не должно содержать пробелов!");
                PasswordField.Text = "";
                return false;
            }
            /* Проверка на длинну */
            if (PasswordField.Text.Length < 4)
            {
                MessageBox.Show("Пароль должен быть длиннее 4-рех символов!");
                PasswordField.Text = "";
                return false;
            }
            /* Проверка паролей на совпадение */
            if (PasswordField.Text != PassCheckField.Text)
            {
                MessageBox.Show("Пароли не совпадают!");
                PassCheckField.Text = "";
                return false;
            }
            return true;
        }

        private bool validLogin()
        {
            // проверка на то заполнено ли оно
            if (LoginField.Text == "")
            {
                MessageBox.Show("Поле: Логин - должно быть заполнено!");
                return false;
            }
            // проверка на пробел
            if (LoginField.Text.Contains(' '))
            {
                MessageBox.Show("Поле: Логин - не должно содержать Пробелов");
                return false;
            }

            // проверка на уникальность
            if (!checkUniqueLogin())
            {
                MessageBox.Show("Етот логин уже занят");
                return false;
            }

            return true;
        }

        private bool checkUniqueLogin()
        {
            DataBase db = new DataBase();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` " +
                "WHERE `login`= @login",
                db.getConnection());

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = LoginField.Text;
            adapter.SelectCommand = command;
            adapter.Fill(table);

            return !(table.Rows.Count > 0);
        }

        private void startform()
        {
            InitializeComponent();
            /* placehold (подсказки в текстовых полях) */
            namePlaceHold(false); /* Поле ввода  имени*/
            loginPlaceHold(false); /* Поле ввода логина*/
            passwordPlaceHold(false); /* Поле ввода пароля */
            passCheckPlaceHold(false); /* Поле проверки пароля*/
        }
    }
}
