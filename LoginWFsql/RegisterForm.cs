using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LoginWFsql
{
    public partial class RegisterForm : Form
    {
        private LoginForm LG;
        DataBase db = new DataBase();
        MySqlCommand command;
        MySqlDataReader reader;

        Validations login_valid_state;
        public RegisterForm(LoginForm lg)
        {
            LG = lg;
            startform();
        }

        private void startform()
        {
            InitializeComponent();
            /* placehold (подсказки в текстовых полях) */
            loginPlaceHold(false); /* Поле ввода логина*/
            passwordPlaceHold(false); /* Поле ввода пароля */
            passCheckPlaceHold(false); /* Поле проверки пароля*/
        }

        #region placehold
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

        private void loginPlaceHold(bool state)
        {
            string text_placehold = "*придумайте логин...";
            if (!state && tbLoginField.Text == "") // если выходим из поля
            {
                login_valid_state = Validations.PLACE_HOLD;
                tbLoginField.Text = text_placehold;
                tbLoginField.ForeColor = Color.Gray;
            }
            else if (state && tbLoginField.Text == text_placehold) // если заходим в поле
            {
                tbLoginField.Text = "";
                tbLoginField.ForeColor = Color.White;
            }
        }
        private void passwordPlaceHold(bool state)
        {
            string text_placehold = "*придумайте пароль...";
            if (!state && tbPasswordField.Text == "")// если выходим из поля
            {
                tbPasswordField.Text = text_placehold;
                tbPasswordField.ForeColor = Color.Gray;
                tbPasswordField.UseSystemPasswordChar = false;
            }
            else if (state && tbPasswordField.Text == text_placehold)// если заходим в поле
            {
                tbPasswordField.Text = "";
                tbPasswordField.ForeColor = Color.White;
                tbPasswordField.UseSystemPasswordChar = true;
            }
        }
        private void passCheckPlaceHold(bool state)
        {
            string text_placehold = "*повторите пароль...";
            if (!state && tbPassCheckField.Text == "")// если выходим из поля
            {
                tbPassCheckField.Text = text_placehold;
                tbPassCheckField.ForeColor = Color.Gray;
                tbPassCheckField.UseSystemPasswordChar = false;
            }
            else if (state && tbPassCheckField.Text == text_placehold)// если заходим в поле
            {
                tbPassCheckField.Text = "";
                tbPassCheckField.ForeColor = Color.White;
                tbPassCheckField.UseSystemPasswordChar = true;
            }
        }

        #endregion

        private void btRegistration_Click(object sender, EventArgs e)
        {
            //if (!checkValidData())
                return;

            MySqlCommand command = new MySqlCommand("INSERT INTO `users` (`login`, `pass`)" +
                "VALUES(@login, @pass)", db.getConnection());

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = tbLoginField.Text;
            command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = tbPasswordField.Text;

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

        private bool validPassword()
        {
            /* Проверка на пустоту и пробелы */
            if (tbPasswordField.Text == "" || tbPasswordField.Text.Contains(' '))
            {
                MessageBox.Show("Поле: Пароль - должно быть заполнено!\n" +
                    "И не должно содержать пробелов!");
                tbPasswordField.Text = "";
                return false;
            }
            /* Проверка на длинну */
            if (tbPasswordField.Text.Length < 4)
            {
                MessageBox.Show("Пароль должен быть длиннее 4-рех символов!");
                tbPasswordField.Text = "";
                return false;
            }
            /* Проверка паролей на совпадение */
            if (tbPasswordField.Text != tbPassCheckField.Text)
            {
                MessageBox.Show("Пароли не совпадают!");
                tbPassCheckField.Text = "";
                return false;
            }
            return true;
        }


        private void RegisterForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastpoint.X;
                this.Top += e.Y - lastpoint.Y;
            }
        }
        Point lastpoint;
        private void RegisterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastpoint.X = e.X;
                lastpoint.Y = e.Y;
            }
        }

        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            LG.Show();
            this.Close();
        }

        private void LoginField_TextChanged(object sender, EventArgs e)
        {
            Check_Valid_Login();
        }

        private void Check_Valid_Login()
        {
            if (login_valid_state == Validations.PLACE_HOLD)
            {
                pbValid_unValid.Image = null;
                login_valid_state = Validations.unVALID;
                return;
            }
            else if (tbLoginField.Text == "")
            {
                pbValid_unValid.Image = null;
                login_valid_state = Validations.unVALID;
                return;
            }
            else if (tbLoginField.Text.Length < 4)
            {
                pbValid_unValid.Image = Properties.Resources.unValid1;
                login_valid_state = Validations.unVALID;
                return;
            }




            db.openConnection();
            {
                command = SqlCommand.Search_User(tbLoginField.Text, db.getConnection());
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    pbValid_unValid.Image = Properties.Resources.unValid1;
                    login_valid_state = Validations.unVALID;
                }
                else
                {
                    pbValid_unValid.Image = Properties.Resources.Valid1;
                    login_valid_state = Validations.VALID;
                }

                reader.Close();
            }
            db.closeConnection();
        }

        private enum Validations
        {
            VALID,
            unVALID,
            PLACE_HOLD,
        }

    }
}
