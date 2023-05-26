namespace Server
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
			this.label3 = new System.Windows.Forms.Label();
			this.clientPort = new System.Windows.Forms.TextBox();
			this.listenButton = new System.Windows.Forms.Button();
			this.logs = new System.Windows.Forms.RichTextBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.colorDialog2 = new System.Windows.Forms.ColorDialog();
			this.IF100_Channel_Logs = new System.Windows.Forms.RichTextBox();
			this.SPS101_Channel_Logs = new System.Windows.Forms.RichTextBox();
			this.MATH101_Channel_Logs = new System.Windows.Forms.RichTextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.IF100_Master_TextBox = new System.Windows.Forms.TextBox();
			this.MATH101_Master_TextBox = new System.Windows.Forms.TextBox();
			this.SPS101_Master_TextBox = new System.Windows.Forms.TextBox();
			this.IF100_GenerateKey_Button = new System.Windows.Forms.Button();
			this.MATH101_GenerateKey_Button = new System.Windows.Forms.Button();
			this.SPS101_GenerateKey_Button = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 30);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Port:";
			// 
			// clientPort
			// 
			this.clientPort.Location = new System.Drawing.Point(50, 27);
			this.clientPort.Margin = new System.Windows.Forms.Padding(2);
			this.clientPort.Name = "clientPort";
			this.clientPort.Size = new System.Drawing.Size(76, 20);
			this.clientPort.TabIndex = 6;
			// 
			// listenButton
			// 
			this.listenButton.Location = new System.Drawing.Point(154, 27);
			this.listenButton.Margin = new System.Windows.Forms.Padding(2);
			this.listenButton.Name = "listenButton";
			this.listenButton.Size = new System.Drawing.Size(92, 24);
			this.listenButton.TabIndex = 7;
			this.listenButton.Text = "Listen";
			this.listenButton.UseVisualStyleBackColor = true;
			this.listenButton.Click += new System.EventHandler(this.listenButton_Click);
			// 
			// logs
			// 
			this.logs.Location = new System.Drawing.Point(420, 27);
			this.logs.Margin = new System.Windows.Forms.Padding(2);
			this.logs.Name = "logs";
			this.logs.ReadOnly = true;
			this.logs.Size = new System.Drawing.Size(250, 456);
			this.logs.TabIndex = 9;
			this.logs.Text = "";
			// 
			// IF100_Channel_Logs
			// 
			this.IF100_Channel_Logs.Enabled = false;
			this.IF100_Channel_Logs.Location = new System.Drawing.Point(171, 92);
			this.IF100_Channel_Logs.Name = "IF100_Channel_Logs";
			this.IF100_Channel_Logs.Size = new System.Drawing.Size(206, 106);
			this.IF100_Channel_Logs.TabIndex = 11;
			this.IF100_Channel_Logs.Text = "";
			// 
			// SPS101_Channel_Logs
			// 
			this.SPS101_Channel_Logs.Enabled = false;
			this.SPS101_Channel_Logs.Location = new System.Drawing.Point(171, 377);
			this.SPS101_Channel_Logs.Name = "SPS101_Channel_Logs";
			this.SPS101_Channel_Logs.Size = new System.Drawing.Size(206, 106);
			this.SPS101_Channel_Logs.TabIndex = 12;
			this.SPS101_Channel_Logs.Text = "";
			// 
			// MATH101_Channel_Logs
			// 
			this.MATH101_Channel_Logs.Enabled = false;
			this.MATH101_Channel_Logs.Location = new System.Drawing.Point(171, 233);
			this.MATH101_Channel_Logs.Name = "MATH101_Channel_Logs";
			this.MATH101_Channel_Logs.Size = new System.Drawing.Size(206, 106);
			this.MATH101_Channel_Logs.TabIndex = 13;
			this.MATH101_Channel_Logs.Text = "";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(226, 217);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(98, 13);
			this.label4.TabIndex = 16;
			this.label4.Text = "MATH101 Channel";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(226, 361);
			this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 13);
			this.label5.TabIndex = 17;
			this.label5.Text = "SPS101 Channel";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(238, 76);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 18;
			this.label1.Text = "IF100 Channel";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.label2.Location = new System.Drawing.Point(9, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(135, 13);
			this.label2.TabIndex = 19;
			this.label2.Text = "Generate a master key";
			// 
			// IF100_Master_TextBox
			// 
			this.IF100_Master_TextBox.Enabled = false;
			this.IF100_Master_TextBox.Location = new System.Drawing.Point(35, 146);
			this.IF100_Master_TextBox.Name = "IF100_Master_TextBox";
			this.IF100_Master_TextBox.Size = new System.Drawing.Size(100, 20);
			this.IF100_Master_TextBox.TabIndex = 20;
			// 
			// MATH101_Master_TextBox
			// 
			this.MATH101_Master_TextBox.Enabled = false;
			this.MATH101_Master_TextBox.Location = new System.Drawing.Point(35, 264);
			this.MATH101_Master_TextBox.Name = "MATH101_Master_TextBox";
			this.MATH101_Master_TextBox.Size = new System.Drawing.Size(100, 20);
			this.MATH101_Master_TextBox.TabIndex = 21;
			// 
			// SPS101_Master_TextBox
			// 
			this.SPS101_Master_TextBox.Enabled = false;
			this.SPS101_Master_TextBox.Location = new System.Drawing.Point(35, 388);
			this.SPS101_Master_TextBox.Name = "SPS101_Master_TextBox";
			this.SPS101_Master_TextBox.Size = new System.Drawing.Size(100, 20);
			this.SPS101_Master_TextBox.TabIndex = 22;
			// 
			// IF100_GenerateKey_Button
			// 
			this.IF100_GenerateKey_Button.Enabled = false;
			this.IF100_GenerateKey_Button.Location = new System.Drawing.Point(69, 172);
			this.IF100_GenerateKey_Button.Name = "IF100_GenerateKey_Button";
			this.IF100_GenerateKey_Button.Size = new System.Drawing.Size(75, 23);
			this.IF100_GenerateKey_Button.TabIndex = 23;
			this.IF100_GenerateKey_Button.Text = "Generate";
			this.IF100_GenerateKey_Button.UseVisualStyleBackColor = true;
			this.IF100_GenerateKey_Button.Click += new System.EventHandler(this.GenerateKey_IF100_Click);
			// 
			// MATH101_GenerateKey_Button
			// 
			this.MATH101_GenerateKey_Button.Enabled = false;
			this.MATH101_GenerateKey_Button.Location = new System.Drawing.Point(69, 290);
			this.MATH101_GenerateKey_Button.Name = "MATH101_GenerateKey_Button";
			this.MATH101_GenerateKey_Button.Size = new System.Drawing.Size(75, 23);
			this.MATH101_GenerateKey_Button.TabIndex = 24;
			this.MATH101_GenerateKey_Button.Text = "Generate";
			this.MATH101_GenerateKey_Button.UseVisualStyleBackColor = true;
			this.MATH101_GenerateKey_Button.Click += new System.EventHandler(this.MATH101_GenerateKey_Button_Click);
			// 
			// SPS101_GenerateKey_Button
			// 
			this.SPS101_GenerateKey_Button.Enabled = false;
			this.SPS101_GenerateKey_Button.Location = new System.Drawing.Point(69, 414);
			this.SPS101_GenerateKey_Button.Name = "SPS101_GenerateKey_Button";
			this.SPS101_GenerateKey_Button.Size = new System.Drawing.Size(75, 23);
			this.SPS101_GenerateKey_Button.TabIndex = 25;
			this.SPS101_GenerateKey_Button.Text = "Generate";
			this.SPS101_GenerateKey_Button.UseVisualStyleBackColor = true;
			this.SPS101_GenerateKey_Button.Click += new System.EventHandler(this.GenerateKey_SPS101_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(10, 130);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(34, 13);
			this.label6.TabIndex = 26;
			this.label6.Text = "IF100";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(10, 248);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(56, 13);
			this.label7.TabIndex = 27;
			this.label7.Text = "MATH101";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(10, 372);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(46, 13);
			this.label8.TabIndex = 28;
			this.label8.Text = "SPS101";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 506);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.SPS101_GenerateKey_Button);
			this.Controls.Add(this.MATH101_GenerateKey_Button);
			this.Controls.Add(this.IF100_GenerateKey_Button);
			this.Controls.Add(this.SPS101_Master_TextBox);
			this.Controls.Add(this.MATH101_Master_TextBox);
			this.Controls.Add(this.IF100_Master_TextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.MATH101_Channel_Logs);
			this.Controls.Add(this.SPS101_Channel_Logs);
			this.Controls.Add(this.IF100_Channel_Logs);
			this.Controls.Add(this.logs);
			this.Controls.Add(this.listenButton);
			this.Controls.Add(this.clientPort);
			this.Controls.Add(this.label3);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox clientPort;
        private System.Windows.Forms.Button listenButton;
        private System.Windows.Forms.RichTextBox logs;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.ColorDialog colorDialog2;
		private System.Windows.Forms.RichTextBox IF100_Channel_Logs;
		private System.Windows.Forms.RichTextBox SPS101_Channel_Logs;
		private System.Windows.Forms.RichTextBox MATH101_Channel_Logs;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox IF100_Master_TextBox;
		private System.Windows.Forms.TextBox MATH101_Master_TextBox;
		private System.Windows.Forms.TextBox SPS101_Master_TextBox;
		private System.Windows.Forms.Button IF100_GenerateKey_Button;
		private System.Windows.Forms.Button MATH101_GenerateKey_Button;
		private System.Windows.Forms.Button SPS101_GenerateKey_Button;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
	}
}

