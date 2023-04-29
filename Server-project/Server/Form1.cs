﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Server
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool listening = false;
        bool remoteConnected = false;

        string RSA3072PrivateEncryptionKey;
        string RSA3072PrivateVerificationKey;

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket remoteSocket;
        List<Socket> socketList = new List<Socket>();

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
            //Reading the private RSA key which will be used for encryption purposes
            
            using (System.IO.StreamReader fileReader =
            new System.IO.StreamReader("server_enc_dec_pub_prv.txt"))
            {
                RSA3072PrivateEncryptionKey = fileReader.ReadLine();
            }

            //Reading the private RSA key which will be used for signature verification purposes
            
            using (System.IO.StreamReader fileReader =
            new System.IO.StreamReader("server_signing_verification_pub_prv.txt"))
            {
                RSA3072PrivateVerificationKey = fileReader.ReadLine();
            }
            logs.Enabled = false;
        }

        private void listenButton_Click(object sender, EventArgs e)
        {
            int serverPort;
            Thread acceptThread;

            if (Int32.TryParse(clientPort.Text, out serverPort))
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                serverSocket.Listen(3);

                listening = true;
                listenButton.Enabled = false;
                acceptThread = new Thread(new ThreadStart(Accept));
                acceptThread.Start();

                logs.AppendText("Started listening on port: " + serverPort + "\n");
            }
            else
            {
                logs.AppendText("Please check port number \n");
            }
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {
                    //socketList.Add(serverSocket.Accept());
                    Socket newClient = serverSocket.Accept();
                    //before accepting the connection, server needs to get the username-password info, decrypt them and do the necessary comparisons
                    Byte[] firstBuffer = new Byte[10000000];
                    newClient.Receive(firstBuffer);
                    //string acceptingInfoString = Encoding.UTF8.GetString(firstBuffer);
                    //acceptingInfoString = acceptingInfoString.Substring(0, acceptingInfoString.IndexOf("\0"));

                    //Decrypting the received RSA encrypted data
                    byte[] decryptedBytes;
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                    {
                       
                        rsa.FromXmlString(RSA3072PrivateEncryptionKey);

                        decryptedBytes = rsa.Decrypt(firstBuffer, true);
                    }
                    string decryptedString = Encoding.UTF8.GetString(decryptedBytes);
                    string[] tokens = decryptedString.Split(new[] { "|aralik|" }, StringSplitOptions.None);

                    string hashedPassword = tokens[0];
                    string usernameVar = tokens[1];
                    string channelVar = tokens[2];



                    //Thread receiveThread;
                    //receiveThread = new Thread(new ThreadStart(Receive));
                    //receiveThread.Start();
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        logs.AppendText("The socket stopped working \n");
                    }
                }
            }
        }

        private void Receive()
        {
            Socket s = socketList[socketList.Count - 1];
            bool connected = true;

            while (connected && !terminating)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    s.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    logs.AppendText(incomingMessage + "\n");

                    if (remoteConnected)
                    {
                        try
                        {
                            remoteSocket.Send(buffer);

                            buffer = new Byte[64];
                            remoteSocket.Receive(buffer);

                            s.Send(buffer);
                        }
                        catch
                        {
                            remoteConnected = false;
                            remoteSocket = null;
                            connectButton.Enabled = true;
                        }
                    }
                    else
                    {
                        logs.AppendText("Not connected to remote server");
                    }

                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("A client has disconnected\n");
                    }

                    s.Close();
                    socketList.Remove(s);
                    connected = false;
                }
            }
        }



        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }
    }
}
