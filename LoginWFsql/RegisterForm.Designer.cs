
namespace LoginWFsql
{
    partial class RegisterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterForm));
            this.btRegistration = new System.Windows.Forms.Button();
            this.NameField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LoginField = new System.Windows.Forms.TextBox();
            this.PasswordField = new System.Windows.Forms.TextBox();
            this.PassCheckField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btOnOfShowPass2 = new System.Windows.Forms.PictureBox();
            this.btOnOfShowPass = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btOnOfShowPass2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btOnOfShowPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btRegistration
            // 
            this.btRegistration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.btRegistration.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.btRegistration.FlatAppearance.BorderSize = 0;
            this.btRegistration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRegistration.Font = new System.Drawing.Font("a_LatinoNr", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btRegistration.ForeColor = System.Drawing.Color.White;
            this.btRegistration.Location = new System.Drawing.Point(80, 338);
            this.btRegistration.Name = "btRegistration";
            this.btRegistration.Size = new System.Drawing.Size(155, 37);
            this.btRegistration.TabIndex = 12;
            this.btRegistration.Text = "Регистрация";
            this.btRegistration.UseVisualStyleBackColor = false;
            this.btRegistration.Click += new System.EventHandler(this.btRegistration_Click);
            // 
            // NameField
            // 
            this.NameField.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.NameField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameField.Font = new System.Drawing.Font("Constantia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NameField.ForeColor = System.Drawing.Color.White;
            this.NameField.Location = new System.Drawing.Point(-1, 106);
            this.NameField.Name = "NameField";
            this.NameField.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NameField.Size = new System.Drawing.Size(314, 27);
            this.NameField.TabIndex = 8;
            this.NameField.Tag = "";
            this.NameField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NameField.Enter += new System.EventHandler(this.NameField_Enter);
            this.NameField.Leave += new System.EventHandler(this.NameField_Leave);
            // 
            // label1
            // 
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("a_LatinoNr", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(289, 48);
            this.label1.TabIndex = 7;
            this.label1.Text = "РЕГИСТРАЦИЯ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // LoginField
            // 
            this.LoginField.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.LoginField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LoginField.Font = new System.Drawing.Font("Constantia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LoginField.ForeColor = System.Drawing.Color.White;
            this.LoginField.Location = new System.Drawing.Point(-1, 182);
            this.LoginField.Name = "LoginField";
            this.LoginField.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LoginField.Size = new System.Drawing.Size(314, 27);
            this.LoginField.TabIndex = 13;
            this.LoginField.Tag = "";
            this.LoginField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.LoginField.Enter += new System.EventHandler(this.LoginField_Enter);
            this.LoginField.Leave += new System.EventHandler(this.LoginField_Leave);
            // 
            // PasswordField
            // 
            this.PasswordField.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.PasswordField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PasswordField.Font = new System.Drawing.Font("Constantia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PasswordField.ForeColor = System.Drawing.Color.White;
            this.PasswordField.Location = new System.Drawing.Point(-1, 261);
            this.PasswordField.Name = "PasswordField";
            this.PasswordField.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PasswordField.Size = new System.Drawing.Size(314, 27);
            this.PasswordField.TabIndex = 15;
            this.PasswordField.Tag = "";
            this.PasswordField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PasswordField.UseSystemPasswordChar = true;
            this.PasswordField.Enter += new System.EventHandler(this.PasswordField_Enter);
            this.PasswordField.Leave += new System.EventHandler(this.PasswordField_Leave);
            // 
            // PassCheckField
            // 
            this.PassCheckField.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.PassCheckField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PassCheckField.Font = new System.Drawing.Font("Constantia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PassCheckField.ForeColor = System.Drawing.Color.White;
            this.PassCheckField.Location = new System.Drawing.Point(-1, 299);
            this.PassCheckField.Name = "PassCheckField";
            this.PassCheckField.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PassCheckField.ShortcutsEnabled = false;
            this.PassCheckField.Size = new System.Drawing.Size(314, 27);
            this.PassCheckField.TabIndex = 17;
            this.PassCheckField.Tag = "";
            this.PassCheckField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PassCheckField.UseSystemPasswordChar = true;
            this.PassCheckField.Enter += new System.EventHandler(this.PassCheckField_Enter);
            this.PassCheckField.Leave += new System.EventHandler(this.PassCheckField_Leave);
            // 
            // label2
            // 
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Font = new System.Drawing.Font("a_LatinoNr", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(132)))), ((int)(((byte)(32)))));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label2.Location = new System.Drawing.Point(117, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 40);
            this.label2.TabIndex = 18;
            this.label2.Text = "Имя";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label3
            // 
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.Font = new System.Drawing.Font("a_LatinoNr", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(132)))), ((int)(((byte)(32)))));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label3.Location = new System.Drawing.Point(117, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 40);
            this.label3.TabIndex = 19;
            this.label3.Text = "Логин*";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label4
            // 
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Font = new System.Drawing.Font("a_LatinoNr", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(132)))), ((int)(((byte)(32)))));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label4.Location = new System.Drawing.Point(117, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(171, 40);
            this.label4.TabIndex = 20;
            this.label4.Text = "Пароль*";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btOnOfShowPass2
            // 
            this.btOnOfShowPass2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.btOnOfShowPass2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btOnOfShowPass2.Image = global::LoginWFsql.Properties.Resources.eyeClose3;
            this.btOnOfShowPass2.Location = new System.Drawing.Point(275, 261);
            this.btOnOfShowPass2.Name = "btOnOfShowPass2";
            this.btOnOfShowPass2.Size = new System.Drawing.Size(27, 27);
            this.btOnOfShowPass2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btOnOfShowPass2.TabIndex = 22;
            this.btOnOfShowPass2.TabStop = false;
            this.btOnOfShowPass2.Visible = false;
            this.btOnOfShowPass2.Click += new System.EventHandler(this.btOnOfShowPass2_Click);
            // 
            // btOnOfShowPass
            // 
            this.btOnOfShowPass.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.btOnOfShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btOnOfShowPass.Image = global::LoginWFsql.Properties.Resources.eyeOpen3;
            this.btOnOfShowPass.Location = new System.Drawing.Point(275, 261);
            this.btOnOfShowPass.Name = "btOnOfShowPass";
            this.btOnOfShowPass.Size = new System.Drawing.Size(27, 27);
            this.btOnOfShowPass.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btOnOfShowPass.TabIndex = 21;
            this.btOnOfShowPass.TabStop = false;
            this.btOnOfShowPass.Click += new System.EventHandler(this.btOnOfShowPass_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::LoginWFsql.Properties.Resources.password;
            this.pictureBox3.Location = new System.Drawing.Point(71, 215);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(40, 40);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 16;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::LoginWFsql.Properties.Resources.user5;
            this.pictureBox2.Location = new System.Drawing.Point(71, 136);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 40);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 14;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LoginWFsql.Properties.Resources.idCardName;
            this.pictureBox1.Location = new System.Drawing.Point(71, 60);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // RegisterForm
            // 
            this.AcceptButton = this.btRegistration;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(314, 386);
            this.Controls.Add(this.btOnOfShowPass2);
            this.Controls.Add(this.btOnOfShowPass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PassCheckField);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.PasswordField);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.LoginField);
            this.Controls.Add(this.btRegistration);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.NameField);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(330, 425);
            this.MinimumSize = new System.Drawing.Size(330, 425);
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegisterForm";
            ((System.ComponentModel.ISupportInitialize)(this.btOnOfShowPass2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btOnOfShowPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btRegistration;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox NameField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox LoginField;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.TextBox PasswordField;
        private System.Windows.Forms.TextBox PassCheckField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox btOnOfShowPass;
        private System.Windows.Forms.PictureBox btOnOfShowPass2;
    }
}