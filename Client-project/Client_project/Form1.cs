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
using System.Security.Cryptography;
using System.IO;


namespace Client_project
{
    public partial class Form1 : Form
    {

        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        string RSA3072PublicEncryptionKey;
        string RSA3072PublicVerificationKey;
        static string generateHexStringFromByteArray(byte[] input)
        {
            string hexString = BitConverter.ToString(input);
            return hexString.Replace("-", "");
        }
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            //Reading the public RSA key which will be used for encryption purposes
            
            using (System.IO.StreamReader fileReader =
            new System.IO.StreamReader("server_enc_dec_pub.txt"))
            {
                RSA3072PublicEncryptionKey = fileReader.ReadLine();
            }

            //Reading the public RSA key which will be used for signature verification purposes
            
            using (System.IO.StreamReader fileReader =
            new System.IO.StreamReader("server_sign_verify_pub.txt"))
            {
                RSA3072PublicVerificationKey = fileReader.ReadLine();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Getting the input values from the input prompts on the GUI
            string ipVar = ip_field.Text;
            string portNumVar = port_field.Text;

            string usernameVar = username_field.Text;
            string passwordVar = password_field.Text;

            string channelVar = channel_combobox.SelectedItem.ToString();
            logs.AppendText("Channel: " + channelVar + "\n");

            int portNumInt;
            if ((Int32.TryParse(portNumVar, out portNumInt)) && (ipVar != ""))
            {
                try
                {
                    clientSocket.Connect(ipVar, portNumInt);
                    connect_button.Enabled = false;
                    disconnect_button.Enabled = true;
                    connected = true;
                    logs.AppendText("Connected to the server. \n");

                    //The part below takes the SHA-512 hash of the password
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordVar);
                    //byte[] passwordBytes = Convert.FromBase64String(passwordVar);
                    string hashedString;
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        byte[] hashedBytes = sha512.ComputeHash(passwordBytes);
                        //hashedString = generateHexStringFromByteArray(hashedBytes);
                        //hashedString = Encoding.UTF8.GetString(hashedBytes);
                        hashedString = Convert.ToBase64String(passwordBytes);
                    }


                    //Concatenating the hashed password with the username and channel input
                    string concatanatedHashString = hashedString + "|ar|" + usernameVar + "|ar|" + channelVar;
                    //string concatanatedHashString = "denemeString";

                    //Encrypting the data using RSA public key
                    byte[] concatanatedHashBytes = Encoding.UTF8.GetBytes(concatanatedHashString);
                    //byte[] concatanatedHashBytes = Convert.FromBase64String(concatanatedHashString);

                    byte[] encryptedBytes = new byte[384];
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                    {
                        rsa.FromXmlString(RSA3072PublicEncryptionKey);
                        try
                        {
                            encryptedBytes = rsa.Encrypt(concatanatedHashBytes, true);
                        }
                        catch(Exception ex)
                        {
                            logs.AppendText(ex.ToString());
                        }
                    }
                    clientSocket.Send(encryptedBytes);

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

        private void Receive() {
            while (connected) {
                try {
                    // Receive the combined length first
                    byte[] combinedLengthBytes = new byte[4];
                    clientSocket.Receive(combinedLengthBytes);
                    int combinedLength = BitConverter.ToInt32(combinedLengthBytes, 0);

                    // Create the buffer according to the combined length
                    Byte[] buffer = new Byte[combinedLength];
                    clientSocket.Receive(buffer);

                    // Separate the received message and signature
                    int messageLength = buffer.Length - 384;
                    byte[] messageBytes = new byte[messageLength];
                    byte[] signature = new byte[384];

                    Buffer.BlockCopy(buffer, 0, messageBytes, 0, messageLength);
                    Buffer.BlockCopy(buffer, messageLength, signature, 0, 384);

                    // Convert the message bytes to a string
                    string message = Encoding.UTF8.GetString(messageBytes);
                    int nullCharIndex = message.IndexOf("\0");
                    if (nullCharIndex >= 0) {
                        message = message.Substring(0, nullCharIndex);
                    }

                    // Verify the message using the server's RSA public key for signature verification
                    bool isVerified = verifyWithRSA(message, 3072, RSA3072PublicVerificationKey, signature);
                    //logs.AppendText("Received signature: " + BitConverter.ToString(signature).Replace("-", ""));

                    if (isVerified) {
                        logs.AppendText("Signature is verified: " + message + "\n");
                        if(message == "error") {
                            connect_button.Enabled = true;
                            logs.AppendText(username_field.Text + " is already exist.");
						}
						else if(message == "success") {
                            logs.AppendText(username_field.Text + " successfully enroled.");
                        }
                    }
                    else {
                        logs.AppendText("Failed to verify: " + message + "\n");
                    }
                }
                catch (Exception ex) {
                    if (!terminating) {
                        logs.AppendText("Connection has been lost with the server. \n");
                    }

                    // Print the exception details to the logs
                    logs.AppendText("Error: " + ex.Message + "\n");

                    clientSocket.Close();
                    connected = false;
                }
            }
        }






        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void disconnect_button_Click(object sender, EventArgs e)
        {
            terminating = true;
            clientSocket.Close();
            connected = false;
            logs.AppendText("Successfully disconnected.\n");

            disconnect_button.Enabled = false;
            connect_button.Enabled = true;

            connect_button.BackColor = SystemColors.Control;
            disconnect_button.BackColor = SystemColors.Control;
        }


        // signing with RSA
        static bool verifyWithRSA(string input, int algoLength, string xmlString, byte[] signature)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try
            {
                result = rsaObject.VerifyData(byteInput, "SHA256", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }
	}
}
