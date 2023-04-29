using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Client_project
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool connected = false;
        Socket clientSocket;

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ip = IP.Text;
            string portNum = port.Text;
            int port_num;

            string RSAxmlKey2048;
            using (System.IO.StreamReader fileReader =
            new System.IO.StreamReader("RSA2048key.txt"))
            {
                RSAxmlKey2048 = fileReader.ReadLine();
            }
            if (Int32.TryParse(portNum, out port_num))
            {
                try
                {
                    clientSocket.Connect(ip, port_num);
                    connect_button.Enabled = false;
                    connected = true;
                    logs.AppendText("Connected to the server. \n");

                    Thread receiveThread = new Thread(new ThreadStart(Receive));
                    receiveThread.Start();

                }
                catch {
                    logs.AppendText("Could not connect to the server. \n");
                }

            }
            else {
                logs.AppendText("Check the port number. \n");
            }

        }

        private void Receive()
        {
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);

                    string message = Encoding.Default.GetString(buffer);
                    message = message.Substring(0, message.IndexOf("\0"));
                    logs.AppendText(message + "\n");
                }
                catch {
                    if (!terminating)
                    {
                        logs.AppendText("Connection has lost with the server. \n");
                    }

                    clientSocket.Close();
                    connected = false;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void send_button_Click(object sender, EventArgs e)
        {
            String message = message_textBox.Text;

            if (message != "" && message.Length < 63)
            {
                Byte[] buffer = new Byte[64];
                buffer = Encoding.Default.GetBytes(message);
                clientSocket.Send(buffer);
            }
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }
    }
}
