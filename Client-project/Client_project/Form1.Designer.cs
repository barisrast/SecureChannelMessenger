using System.Windows.Forms;

namespace Client_project
{
    partial class Form1
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
			this.connect_button = new System.Windows.Forms.Button();
			this.ip_field = new System.Windows.Forms.TextBox();
			this.port_field = new System.Windows.Forms.TextBox();
			this.logs = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.channel_combobox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.username_field = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.password_field = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.disconnect_button = new System.Windows.Forms.Button();
			this.username_login_field = new System.Windows.Forms.TextBox();
			this.password_login_field = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.login_button = new System.Windows.Forms.Button();
			this.login_port_field = new System.Windows.Forms.TextBox();
			this.login_ip_field = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.sendMessage_TextBox = new System.Windows.Forms.TextBox();
			this.sendMessage_button = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.EnrolledChannel_logs = new System.Windows.Forms.RichTextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// connect_button
			// 
			this.connect_button.Location = new System.Drawing.Point(201, 72);
			this.connect_button.Name = "connect_button";
			this.connect_button.Size = new System.Drawing.Size(100, 35);
			this.connect_button.TabIndex = 0;
			this.connect_button.Text = "Register";
			this.connect_button.UseVisualStyleBackColor = true;
			this.connect_button.Click += new System.EventHandler(this.button1_Click);
			// 
			// ip_field
			// 
			this.ip_field.Location = new System.Drawing.Point(73, 21);
			this.ip_field.Name = "ip_field";
			this.ip_field.Size = new System.Drawing.Size(100, 20);
			this.ip_field.TabIndex = 2;
			// 
			// port_field
			// 
			this.port_field.Location = new System.Drawing.Point(73, 50);
			this.port_field.Name = "port_field";
			this.port_field.Size = new System.Drawing.Size(100, 20);
			this.port_field.TabIndex = 3;
			// 
			// logs
			// 
			this.logs.Location = new System.Drawing.Point(366, 12);
			this.logs.Name = "logs";
			this.logs.Size = new System.Drawing.Size(203, 313);
			this.logs.TabIndex = 4;
			this.logs.Text = "";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(39, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(20, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "IP:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Port:";
			// 
			// channel_combobox
			// 
			this.channel_combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.channel_combobox.FormattingEnabled = true;
			this.channel_combobox.Items.AddRange(new object[] {
            "IF100",
            "MATH101",
            "SPS101"});
			this.channel_combobox.Location = new System.Drawing.Point(73, 142);
			this.channel_combobox.Name = "channel_combobox";
			this.channel_combobox.Size = new System.Drawing.Size(121, 21);
			this.channel_combobox.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(-1, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "Username:";
			// 
			// username_field
			// 
			this.username_field.Location = new System.Drawing.Point(73, 80);
			this.username_field.Name = "username_field";
			this.username_field.Size = new System.Drawing.Size(100, 20);
			this.username_field.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Password:";
			// 
			// password_field
			// 
			this.password_field.Location = new System.Drawing.Point(73, 112);
			this.password_field.Name = "password_field";
			this.password_field.PasswordChar = '*';
			this.password_field.Size = new System.Drawing.Size(100, 20);
			this.password_field.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 145);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(49, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "Channel:";
			// 
			// disconnect_button
			// 
			this.disconnect_button.Location = new System.Drawing.Point(260, 309);
			this.disconnect_button.Name = "disconnect_button";
			this.disconnect_button.Size = new System.Drawing.Size(100, 35);
			this.disconnect_button.TabIndex = 15;
			this.disconnect_button.Text = "Disconnect";
			this.disconnect_button.UseVisualStyleBackColor = false;
			this.disconnect_button.Click += new System.EventHandler(this.disconnect_button_Click);
			// 
			// username_login_field
			// 
			this.username_login_field.Location = new System.Drawing.Point(82, 279);
			this.username_login_field.Name = "username_login_field";
			this.username_login_field.Size = new System.Drawing.Size(100, 20);
			this.username_login_field.TabIndex = 16;
			// 
			// password_login_field
			// 
			this.password_login_field.Location = new System.Drawing.Point(82, 305);
			this.password_login_field.Name = "password_login_field";
			this.password_login_field.Size = new System.Drawing.Size(100, 20);
			this.password_login_field.TabIndex = 17;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 282);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 13);
			this.label6.TabIndex = 18;
			this.label6.Text = "Username:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(14, 309);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(56, 13);
			this.label7.TabIndex = 19;
			this.label7.Text = "Password:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.6F);
			this.label8.Location = new System.Drawing.Point(35, 187);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(95, 32);
			this.label8.TabIndex = 20;
			this.label8.Text = "Log-in";
			// 
			// login_button
			// 
			this.login_button.Location = new System.Drawing.Point(201, 251);
			this.login_button.Name = "login_button";
			this.login_button.Size = new System.Drawing.Size(100, 35);
			this.login_button.TabIndex = 21;
			this.login_button.Text = "Log-in";
			this.login_button.UseVisualStyleBackColor = true;
			this.login_button.Click += new System.EventHandler(this.login_button_Click);
			// 
			// login_port_field
			// 
			this.login_port_field.Location = new System.Drawing.Point(82, 253);
			this.login_port_field.Name = "login_port_field";
			this.login_port_field.Size = new System.Drawing.Size(100, 20);
			this.login_port_field.TabIndex = 22;
			// 
			// login_ip_field
			// 
			this.login_ip_field.Location = new System.Drawing.Point(82, 227);
			this.login_ip_field.Name = "login_ip_field";
			this.login_ip_field.Size = new System.Drawing.Size(100, 20);
			this.login_ip_field.TabIndex = 23;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(37, 230);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(20, 13);
			this.label9.TabIndex = 24;
			this.label9.Text = "IP:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(30, 256);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(29, 13);
			this.label10.TabIndex = 25;
			this.label10.Text = "Port:";
			// 
			// sendMessage_TextBox
			// 
			this.sendMessage_TextBox.Enabled = false;
			this.sendMessage_TextBox.Location = new System.Drawing.Point(60, 381);
			this.sendMessage_TextBox.Name = "sendMessage_TextBox";
			this.sendMessage_TextBox.Size = new System.Drawing.Size(151, 20);
			this.sendMessage_TextBox.TabIndex = 26;
			// 
			// sendMessage_button
			// 
			this.sendMessage_button.Enabled = false;
			this.sendMessage_button.Location = new System.Drawing.Point(217, 379);
			this.sendMessage_button.Name = "sendMessage_button";
			this.sendMessage_button.Size = new System.Drawing.Size(75, 23);
			this.sendMessage_button.TabIndex = 27;
			this.sendMessage_button.Text = "Send";
			this.sendMessage_button.UseVisualStyleBackColor = true;
			this.sendMessage_button.Click += new System.EventHandler(this.sendMessage_button_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(57, 365);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(148, 13);
			this.label11.TabIndex = 28;
			this.label11.Text = "Send a message to a channel";
			// 
			// EnrolledChannel_logs
			// 
			this.EnrolledChannel_logs.Location = new System.Drawing.Point(320, 381);
			this.EnrolledChannel_logs.Name = "EnrolledChannel_logs";
			this.EnrolledChannel_logs.ReadOnly = true;
			this.EnrolledChannel_logs.Size = new System.Drawing.Size(229, 96);
			this.EnrolledChannel_logs.TabIndex = 29;
			this.EnrolledChannel_logs.Text = "";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(363, 365);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(136, 13);
			this.label12.TabIndex = 30;
			this.label12.Text = "Enrolled channel messages";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(588, 505);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.EnrolledChannel_logs);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.sendMessage_button);
			this.Controls.Add(this.sendMessage_TextBox);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.login_ip_field);
			this.Controls.Add(this.login_port_field);
			this.Controls.Add(this.login_button);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.password_login_field);
			this.Controls.Add(this.username_login_field);
			this.Controls.Add(this.disconnect_button);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.password_field);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.username_field);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.channel_combobox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.logs);
			this.Controls.Add(this.port_field);
			this.Controls.Add(this.ip_field);
			this.Controls.Add(this.connect_button);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.TextBox ip_field;
        private System.Windows.Forms.TextBox port_field;
        private System.Windows.Forms.RichTextBox logs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox channel_combobox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox username_field;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox password_field;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button disconnect_button;
        private System.Windows.Forms.TextBox username_login_field;
        private System.Windows.Forms.TextBox password_login_field;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button login_button;
        private TextBox login_port_field;
        private TextBox login_ip_field;
        private Label label9;
        private Label label10;
		private TextBox sendMessage_TextBox;
		private Button sendMessage_button;
		private Label label11;
		private RichTextBox EnrolledChannel_logs;
		private Label label12;
	}
}

