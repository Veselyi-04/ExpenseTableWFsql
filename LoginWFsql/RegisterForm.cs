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
        Validations loginValidState;

        // Отвечает за состояние пароля(Подходит\Не подходит)
        private bool validStatePassword;

        // Отвечает за состояние кнопки Скрыть\Показать Пароль
        private bool showHidePassword;

        // Отвечает за состояние подсказок в полях пароля Скрыты или Показаны
        private Password_Placehold statePasswordPlacehold;
        private Password_Placehold statePassCheckPlacehold;

        public RegisterForm(LoginForm lg)
        {
            LG = lg;
            StartForm();
        }

        private void StartForm()
        {
            InitializeComponent();
            /* placehold (подсказки в текстовых полях) */
            LoginPlaceHold(false); /* Поле ввода логина*/
            PasswordPlaceHold(false); /* Поле ввода пароля */
            PassCheckPlaceHold(false); /* Поле проверки пароля*/
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
            CheckValidLogin();
        }

        private void CheckValidLogin()
        {
            if (loginValidState == Validations.PLACE_HOLD)
            {
                pbLogin_Valid_unValid.Image = null;
                loginValidState = Validations.unVALID;
                return;
            }
            else if (tbLoginField.Text == "")
            {
                pbLogin_Valid_unValid.Image = null;
                loginValidState = Validations.unVALID;
                return;
            }
            else if (tbLoginField.Text.Length < 4)
            {
                pbLogin_Valid_unValid.Image = Properties.Resources.unValid1;
                loginValidState = Validations.unVALID;
                return;
            }




            db.OpenConnection();
            {
                command = SqlCommand.SearchUser(tbLoginField.Text, db.GetConnection());
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    pbLogin_Valid_unValid.Image = Properties.Resources.unValid1;
                    loginValidState = Validations.unVALID;
                }
                else
                {
                    pbLogin_Valid_unValid.Image = Properties.Resources.Valid1;
                    loginValidState = Validations.VALID;
                }

                reader.Close();
            }
            db.CloseConnection();
        }

        #region placehold
        /* Поле ввода логина*/
        private void LoginField_Enter(object sender, EventArgs e)
        {
            LoginPlaceHold(state: true);
        }
        private void LoginField_Leave(object sender, EventArgs e)
        {
            LoginPlaceHold(state: false);
        }
        /* Поле ввода пароля */
        private void PasswordField_Enter(object sender, EventArgs e)
        {
            PasswordPlaceHold(state: true);
        }
        private void PasswordField_Leave(object sender, EventArgs e)
        {
            PasswordPlaceHold(state: false);
        }
        /* Поле проверки пароля*/
        private void PassCheckField_Enter(object sender, EventArgs e)
        {
            PassCheckPlaceHold(state: true);
        }
        private void PassCheckField_Leave(object sender, EventArgs e)
        {
            PassCheckPlaceHold(state: false);
        }

        private void LoginPlaceHold(bool state)
        {
            string text_placehold = "*придумайте логин...";
            if (!state && tbLoginField.Text == "") // если выходим из поля
            {
                loginValidState = Validations.PLACE_HOLD;
                tbLoginField.Text = text_placehold;
                tbLoginField.ForeColor = Color.Gray;
            }
            else if (state && tbLoginField.Text == text_placehold) // если заходим в поле
            {
                tbLoginField.Text = "";
                tbLoginField.ForeColor = Color.White;
            }
        }
        private void PasswordPlaceHold(bool state)
        {
            string text_placehold = "*придумайте пароль...";
            if (state && statePasswordPlacehold == Password_Placehold.on)// если заходим в поле
            {
                tbPasswordField.Text = "";
                tbPasswordField.ForeColor = Color.White;
                tbPasswordField.UseSystemPasswordChar = !showHidePassword;
                statePasswordPlacehold = Password_Placehold.off;

            }
            else if (!state && tbPasswordField.Text == "")// если выходим из поля
            {
                tbPasswordField.Text = text_placehold;
                tbPasswordField.ForeColor = Color.Gray;
                tbPasswordField.UseSystemPasswordChar = false;
                statePasswordPlacehold = Password_Placehold.on;
            }
        }
        private void PassCheckPlaceHold(bool state)
        {
            string text_placehold = "*повторите пароль...";
            if (state && statePassCheckPlacehold == Password_Placehold.on)// если заходим в поле
            {
                tbPassCheckField.Text = "";
                tbPassCheckField.ForeColor = Color.White;
                tbPassCheckField.UseSystemPasswordChar = !showHidePassword;
                statePassCheckPlacehold = Password_Placehold.off;
            }
            else if (!state && tbPassCheckField.Text == "")// если выходим из поля
            {
                tbPassCheckField.Text = text_placehold;
                tbPassCheckField.ForeColor = Color.Gray;
                tbPassCheckField.UseSystemPasswordChar = false;
                statePassCheckPlacehold = Password_Placehold.on;
            }
        }

        #endregion

        private void pbShow_Hide_Click(object sender, EventArgs e)
        {
            if (showHidePassword)
            {
                ShowHidePassword(false);
                showHidePassword = false;
            }
            else
            {
                ShowHidePassword(true);
                showHidePassword = true;
            }
        }

        private void ShowHidePassword(bool Show_Hide)
        {
            if (Show_Hide)
            {
                tbPasswordField.UseSystemPasswordChar = false;
                tbPassCheckField.UseSystemPasswordChar = false;
                pbShow_Hide.Image = Properties.Resources.Show;
            }
            else
            {
                if (statePasswordPlacehold == Password_Placehold.off)
                {
                    tbPasswordField.UseSystemPasswordChar = true;
                }
                if (statePassCheckPlacehold == Password_Placehold.off)
                {
                    tbPassCheckField.UseSystemPasswordChar = true;
                }
                pbShow_Hide.Image = Properties.Resources.Hide;
            }
        }

        private void tbPasswordField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 001 && e.KeyChar != 003 && e.KeyChar != 022)
            {
                e.Handled = true;
            }
        }

        private void tbPasswordField_KeyUp(object sender, KeyEventArgs e)
        {
            CheckValidPassword();
        }

        private void CheckValidPassword()
        {
            if (tbPasswordField.Text.Length < 4)
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.unValid1;
                validStatePassword = false;
            }
            else if (tbPasswordField.Text.Length >= 4 && statePassCheckPlacehold == Password_Placehold.on)
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.Valid1;
                validStatePassword = false;
            }
            else if (tbPasswordField.Text == tbPassCheckField.Text && statePasswordPlacehold != Password_Placehold.on)
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.Valid1;
                validStatePassword = true;
            }
            else
            {
                pbPassword_Valid_unValid.Image = Properties.Resources.unValid1;
                validStatePassword = false;
            }
        }


        private void btRegistration_Click(object sender, EventArgs e)
        {
            if (loginValidState != Validations.VALID)
            {
                MessageBox.Show($"Неподходящий логин!", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (validStatePassword != true)
            {
                MessageBox.Show($"Неподходящий пароль!", "Eror!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            command = SqlCommand.CreateNewUser(tbLoginField.Text, tbPasswordField.Text, db.GetConnection());

            db.OpenConnection();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвесная ошибка регистрации!\n{ex.Message}");
                return;
            }
            db.CloseConnection();

            CreateFirsDay();

            MessageBox.Show($"Вы зарегистрировались!\nПройдите авторизацию!", "Info");
            LG.Show();
            this.Close();
        }
        private void CreateFirsDay()
        {
            command = SqlCommand.LoginUserGetID(tbLoginField.Text, tbPasswordField.Text, db.GetConnection());

            db.OpenConnection();
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

            command = SqlCommand.CreateNewDay(id, DateTime.Today,
                cash_uah, card_uah, 0f, 0f, saved_uah,
                cash_eur, card_eur, 0f, 0f, saved_eur, db.GetConnection());

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвесная ошибка создания первого дня!\n{ex.Message}");
            }
            db.CloseConnection();
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

        private Point lastpoint;

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
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 001 && e.KeyChar != 003 && e.KeyChar != 022)
            {
                e.Handled = true;
            }
        }
        private void tbCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 44 && e.KeyChar != 45 && e.KeyChar != 001 && e.KeyChar != 003 && e.KeyChar != 022)
            {
                e.Handled = true;
            }
        }
    }
}
