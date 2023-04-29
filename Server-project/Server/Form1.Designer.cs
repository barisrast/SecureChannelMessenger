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
            this.label1 = new System.Windows.Forms.Label();
            this.ipAdress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.portNum = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.clientPort = new System.Windows.Forms.TextBox();
            this.listenButton = new System.Windows.Forms.Button();
            this.logs = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // ipAdress
            // 
            this.ipAdress.Location = new System.Drawing.Point(61, 196);
            this.ipAdress.Name = "ipAdress";
            this.ipAdress.Size = new System.Drawing.Size(100, 22);
            this.ipAdress.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port:";
            // 
            // portNum
            // 
            this.portNum.Location = new System.Drawing.Point(61, 236);
            this.portNum.Name = "portNum";
            this.portNum.Size = new System.Drawing.Size(100, 22);
            this.portNum.TabIndex = 3;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(61, 277);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port:";
            // 
            // clientPort
            // 
            this.clientPort.Location = new System.Drawing.Point(61, 33);
            this.clientPort.Name = "clientPort";
            this.clientPort.Size = new System.Drawing.Size(100, 22);
            this.clientPort.TabIndex = 6;
            // 
            // listenButton
            // 
            this.listenButton.Location = new System.Drawing.Point(61, 71);
            this.listenButton.Name = "listenButton";
            this.listenButton.Size = new System.Drawing.Size(75, 23);
            this.listenButton.TabIndex = 7;
            this.listenButton.Text = "Listen";
            this.listenButton.UseVisualStyleBackColor = true;
            this.listenButton.Click += new System.EventHandler(this.listenButton_Click);
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(191, 30);
            this.logs.Name = "logs";
            this.logs.Size = new System.Drawing.Size(194, 267);
            this.logs.TabIndex = 9;
            this.logs.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 341);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.listenButton);
            this.Controls.Add(this.clientPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.portNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ipAdress);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ipAdress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portNum;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox clientPort;
        private System.Windows.Forms.Button listenButton;
        private System.Windows.Forms.RichTextBox logs;
    }
}

