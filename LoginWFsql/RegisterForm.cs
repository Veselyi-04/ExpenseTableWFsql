using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoginWFsql
{
    public partial class RegisterForm : Form
    {
        private LoginForm LG;
        DataBase db = new DataBase();
        MySqlCommand command;
        MySqlDataReader reader;

        // Отвечает за состояние логина(Подходит\Не подходит)
        Validations login_valid_state;

        // Отвечает за состояние пароля(Подходит\Не подходит)
        private bool valid_state_password;

        // Отвечает за состояние кнопки Скрыть\Показать Пароль
        private bool show_hide_password;

        // Отвечает за состояние подсказок в полях пароля Скрыты или Показаны
        private Password_Placehold state_password_placehold;
        private Password_Placehold state_pass_check_placehold;

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

        private enum Validations
        {
            VALID,
            unVALID,
            PLACE_HOLD,
        }
        private enum Password_Placehold
        {
            on,
            off
        }

        private void LoginField_TextChanged(object sender, EventArgs e)
        {
            Check_Valid_Login();
        }

        private void Check_Valid_Login()
        {
            if (login_valid_state == Validations.PLACE_HOLD)
            {
                pbLogin_Valid_unValid.Image = null;
                login_valid_state = Validations.unVALID;
                return;
            }
            else if (tbLoginField.Text == "")
            {
                pbLogin_Valid_unValid.Image = null;
                login_valid_state = Validations.unVALID;
                return;
            }
            else if (tbLoginField.Text.Length < 4)
            {
                pbLogin_Valid_unValid.Image = Properties.Resources.unValid1;
                login_valid_state = Validations.unVALID;
                return;
            }




            db.openConnection();
            {
                command = SqlCommand.Search_User(tbLoginField.Text, db.getConnection());
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    pbLogin_Valid_unValid.Image = Properties.Resources.unValid1;
                    login_valid_state = Validations.unVALID;
                }
                else
                {
                    pbLogin_Valid_unValid.Image = Properties.Resources.Valid1;
                    login_valid_state = Validations.VALID;
                }

                reader.Close();
            }
            db.closeConnection();
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
            if (state && state_password_placehold == Password_Placehold.on)// если заходим в поле
            {
                tbPasswordField.Text = "";
                tbPasswordField.ForeColor = Color.White;
                tbPasswordField.UseSystemPasswordChar = !show_hide_password;
                state_password_placehold = Password_Placehold.off;

            }
            else if (!state && tbPasswordField.Text == "")// если выходим из поля
            {
                tbPasswordField.Text = text_placehold;
                tbPasswordField.ForeColor = Color.Gray;
                tbPasswordField.UseSystemPasswordChar = false;
                state_password_placehold = Password_Placehold.on;
            }
        }
        private void passCheckPlaceHold(bool state)
        {
            string text_placehold = "*повторите пароль...";
            if (state && state_pass_check_placehold == Password_Placehold.on)// если заходим в поле
            {
                tbPassCheckField.Text = "";
                tbPassCheckField.ForeColor = Color.White;
                tbPassCheckField.UseSystemPasswordChar = !show_hide_password;
                state_pass_check_placehold = Password_Placehold.off;
            }
            else if (!state && tbPassCheckField.Text == "")// если выходим из поля
            {
                tbPassCheckField.Text = text_placehold;
                tbPassCheckField.ForeColor = Color.Gray;
                tbPassCheckField.UseSystemPasswordChar = false;
                state_pass_check_placehold = Password_Placehold.on;
            }
        }

        #endregion

        private void pbShow_Hide_Click(object sender, EventArgs e)
        {
            if (show_hide_password)
            {
                Show_Hide_Password(false);
                show_hide_password = false;
            }
            else
            {
                Show_Hide_Password(true);
                show_hide_password = true;
            }
        }

        private void Show_Hide_Password(bool Show_Hide)
        {
            if (Show_Hide)
            {
                tbPasswordField.UseSystemPasswordChar = false;
                tbPassCheckField.UseSystemPasswordChar = false;
                pbShow_Hide.Image = Properties.Resources.Show;
            }
            else
            {
                if (state_password_placehold == Password_Placehold.off)
                {
                    tbPasswordField.UseSystemPasswordChar = true;
                }
                if (state_pass_check_placehold == Password_Placehold.off)
                {
                    tbPassCheckField.UseSystemPasswordChar = true;
                }
                pbShow_Hide.Image = Properties.Resources.Hide;
            }
        }

        private void tbPasswordField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void tbPasswordField_KeyUp(object sender, KeyEventArgs e)
        {
            Check_Valid_Password();
        }

        private void Check_Valid_Password()
        {
            if (tbPasswordField.Text.Length < 4)
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.unValid1;
                valid_state_password = false;
            }
            else if (tbPasswordField.Text.Length >= 4 && state_pass_check_placehold == Password_Placehold.on)
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.Valid1;
                valid_state_password = false;
            }
            else if (tbPasswordField.Text == tbPassCheckField.Text && state_password_placehold != Password_Placehold.on)
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.Valid1;
                valid_state_password = true;
            }
            else
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.unValid1;
                valid_state_password = false;
            }
        }


        private void btRegistration_Click(object sender, EventArgs e)
        {
            if (login_valid_state != Validations.VALID)
            {
                MessageBox.Show($"Неподходящий логин!", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (valid_state_password != true)
            {
                MessageBox.Show($"Неподходящий пароль!", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            command = SqlCommand.Create_New_User(tbLoginField.Text, tbPasswordField.Text, db.getConnection());

            db.openConnection();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвесная ошибка регистрации!\n{ex.Message}");
                return;
            }
            db.closeConnection();

            Create_First_Day();

            MessageBox.Show($"Вы зарегистрировались!\nПройдите авторизацию!", "Info");
            LG.Show();
            this.Close();
        }
        private void Create_First_Day()
        {
            command = SqlCommand.Login_User_getID(tbLoginField.Text, tbPasswordField.Text, db.getConnection());

            db.openConnection();
            reader = command.ExecuteReader();
            reader.Read();
            int id = reader.GetInt32(0);
            reader.Close();

            float cash_uah = tbCash_UAH.Text == "" ? 0 : float.Parse(tbCash_UAH.Text);
            float card_uah = tbCard_UAH.Text == "" ? 0 : float.Parse(tbCard_UAH.Text);
            float saved_uah = tbSaved_UAH.Text == "" ? 0 : float.Parse(tbSaved_UAH.Text);
            float cash_eur = tbCash_EUR.Text == "" ? 0 : float.Parse(tbCash_EUR.Text);
            float card_eur = tbCard_EUR.Text == "" ? 0 : float.Parse(tbCard_EUR.Text);
            float saved_eur = tbSaved_EUR.Text == "" ? 0 : float.Parse(tbSaved_EUR.Text);

            command = SqlCommand.Create_NewDay(id, DateTime.Today,
                cash_uah, card_uah, 0f, 0f, saved_uah,
                cash_eur, card_eur, 0f, 0f, saved_eur, db.getConnection());

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвесная ошибка создания первого дня!\n{ex.Message}");
            }
            db.closeConnection();
        }


        private void bt_Close_Click(object sender, EventArgs e)
        {
            LG.Close();
        }
        private void bt_Cancel_Click(object sender, EventArgs e)
        {
            LG.Show();
            this.Close();
        }


        private void RegisterForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastpoint.X;
                this.Top += e.Y - lastpoint.Y;
            }
        }
        private void RegisterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastpoint.X = e.X;
                lastpoint.Y = e.Y;
            }
        }

        Point lastpoint;

        private void topPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastpoint.X;
                this.Top += e.Y - lastpoint.Y;
            }
        }
        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastpoint.X = e.X;
                lastpoint.Y = e.Y;
            }
        }

        private void tbQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 44)
            {
                e.Handled = true;
            }
        }
        private void tbCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 45)
            {
                e.Handled = true;
            }
        }
    }
}
