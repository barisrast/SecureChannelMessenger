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

        byte[] g_aesKeyForChannelEnrolled;
        byte[] g_ivForChannelEnrolled;
        byte[] g_hmacKeyForChannelEnrolled;

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

            int portNumInt;
            if ((Int32.TryParse(portNumVar, out portNumInt)) && (ipVar != ""))
            {
                try
                {
                    clientSocket.Connect(ipVar, portNumInt);
                    connect_button.Enabled = false;
                    disconnect_button.Enabled = true;
                    connected = true;
                    disconnect_button.BackColor = System.Drawing.Color.IndianRed;

                    string enrollMessage = "|enroll|";
                    byte[] enrollBytes = Encoding.UTF8.GetBytes(enrollMessage);
                    clientSocket.Send(enrollBytes);
                    logs.AppendText("Connected to the server. \n");

                    //The part below takes the SHA-512 hash of the password
                     byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordVar);
                    string hashedString;
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        byte[] hashedBytes = sha512.ComputeHash(passwordBytes);
                        hashedString = generateHexStringFromByteArray(hashedBytes);
                    }
                    logs.AppendText("SHA-512 hash of the input password: " +  hashedString+"\n");


                    //Concatenating the hashed password with the username and channel input
                    string concatanatedHashString = hashedString + "|ar|" + usernameVar + "|ar|" + channelVar;

                    //Encrypting the data using RSA public key
                    byte[] concatanatedHashBytes = Encoding.UTF8.GetBytes(concatanatedHashString);

                    byte[] encryptedBytes = new byte[384];
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                    {
                        rsa.FromXmlString(RSA3072PublicEncryptionKey);
                        try
                        {
                            encryptedBytes = rsa.Encrypt(concatanatedHashBytes, true);
                        }
                        catch (Exception ex)
                        {
                            logs.AppendText(ex.ToString());
                        }
                    }
                    clientSocket.Send(encryptedBytes);

                    Thread receiveThread = new Thread(new ThreadStart(Receive));
                    receiveThread.Start();

                }
                catch
                {
                    logs.AppendText("Could not connect to the server. \n");
                }

            }
            else
            {
                logs.AppendText("Check the port number. \n");
            }

        }

  private void Receive()
        {
            while (connected)
            {   
                try
                {


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
                    if (nullCharIndex >= 0)
                    {
                        message = message.Substring(0, nullCharIndex);
                    }

                    // Verify the message using the server's RSA public key for signature verification
                    bool isVerified = verifyWithRSA(message, 3072, RSA3072PublicVerificationKey, signature);
                    //logs.AppendText("Received signature: " + BitConverter.ToString(signature).Replace("-", ""));

                    if (isVerified)
                    {
                        logs.AppendText("Signature is verified: " + message + "\n");
                        if (message == "error")
                        {
                            connect_button.Enabled = true;
                            logs.AppendText(username_field.Text + " is already exist.");
                        }
                        else if (message == "success")
                        {
                            logs.AppendText(username_field.Text + " successfully enrolled.");
                        }
                    }
                    else
                    {
                        logs.AppendText("Failed to verify: " + message + "\n");
                    }
                }
                catch (Exception ex)
                {
                    if (!terminating)
                    {
                        logs.AppendText("Connection has been lost with the server. \n");
                    }

                    // Print the exception details to the logs
                    // logs.AppendText("Error: " + ex.Message + "\n");

                    clientSocket.Close();
                    connected = false;
                    disconnect_button.BackColor = SystemColors.Control;
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

            disconnect_button.BackColor = SystemColors.Control;
        }


        private void login_button_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ipVar = login_ip_field.Text;
            string portNumVar = login_port_field.Text;

            string usernameVar = username_login_field.Text;
            string passwordVar = password_login_field.Text;

            int portNumInt;
            if ((Int32.TryParse(portNumVar, out portNumInt)) && (ipVar != ""))
            {
                try 
                {
                    clientSocket.Connect(ipVar, portNumInt);
                    string loginMessage = "|login|";
                    byte[] loginBytes = Encoding.UTF8.GetBytes(loginMessage);
                    clientSocket.Send(loginBytes);
                    logs.AppendText("Connected to the server. \n");

                    //Sending the username to the server.
                    byte[] usernameByteA = new byte[128];
                    usernameByteA = Encoding.UTF8.GetBytes(usernameVar);
                    clientSocket.Send(usernameByteA);

                    //Receiving the random number from the server
                    Byte[] randomNumberBytes = new Byte[16];
                    clientSocket.Receive(randomNumberBytes);
                    //string randomString = BitConverter.ToString(randomNumberBytes).Replace("-", "");
                    string randomString = Encoding.UTF8.GetString(randomNumberBytes);
                    logs.AppendText("random:"+randomString+"\n");
                    //logs.AppendText("====" + randomString + "====");
                    //The part below takes the SHA-512 hash of the password
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordVar);
                    //byte[] passwordBytes = Convert.FromBase64String(passwordVar);
                    string hashedString;
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        byte[] hashedBytes = sha512.ComputeHash(passwordBytes);
                        hashedString = generateHexStringFromByteArray(hashedBytes);
                        //hashedString = Encoding.UTF8.GetString(hashedBytes);
                        //hashedString = Convert.ToBase64String(passwordBytes);
                    }

                    //getting the lower quarter of the password hash
                    string lowerQuarterPassword = hashedString.Substring(0, hashedString.Length / 4);
                    byte[] lowerQuarterBytes = hexStringToByteArray(lowerQuarterPassword);
                    //byte[] lowerQuarterBytes = Encoding.UTF8.GetBytes(lowerQuarterPassword);

                    //string randomNumberString = BitConverter.ToString(randomNumber).Replace("-", "");
                    byte[] hmacPaswordResult = new byte[384];
                    byte[] hmacPasswordResult = applyHMACwithSHA512(randomString , lowerQuarterBytes);
                    //sending this HMAC to the server:
                    clientSocket.Send(hmacPasswordResult);

                    byte[] authenticationResponseBuffer = new byte[256];
                    clientSocket.Receive(authenticationResponseBuffer);
                    string encryptedString = Convert.ToBase64String(authenticationResponseBuffer).TrimEnd('=').TrimEnd('A') + "=";
                    logs.AppendText("Encrypted message: " + encryptedString + "\n");

                    // Receive the RSA signature from the server
                    byte[] signatureBuffer = new byte[384];
                    clientSocket.Receive(signatureBuffer);
                    logs.AppendText("Recieved RSA signature: " + Convert.ToBase64String(signatureBuffer) + "\n\n");

                    // Extract AES key and IV from the hashed password bytes
                    byte[] hashedPasswordBytes = hexStringToByteArray(hashedString);
                    byte[] aesKey = new byte[16];
                    byte[] aesIV = new byte[16];
                    Buffer.BlockCopy(hashedPasswordBytes, 0, aesKey, 0, 16);
                    Buffer.BlockCopy(hashedPasswordBytes, 16, aesIV, 0, 16);

                    // Verify the RSA signature
                    bool isSignatureValid = verifyWithRSA(encryptedString, 3072, RSA3072PublicVerificationKey, signatureBuffer);
                    if (isSignatureValid) {
                        logs.AppendText("RSA Signature is valid.\n");

                        try {
                            // Decrypt the authentication message and encrypted keys and IV
                            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
                            byte[] decryptedBytes = decryptWithAES128(encryptedBytes, aesKey, aesIV);

                            // Separate the success message from the encrypted keys and IV
                            string decryptedString = Encoding.UTF8.GetString(decryptedBytes);
                            string[] parts = decryptedString.Split(new string[] { "||" }, StringSplitOptions.None);
                            string successMessage = parts[0];
                            string encryptedKeysAndIV = parts.Length > 1 ? parts[1] : null;

                            // If there are keys and IV, convert them from hex string to byte array
                            if (encryptedKeysAndIV != null) {
                                byte[] decryptedKeysAndIVBytes = hexStringToByteArray(encryptedKeysAndIV);

                                // Load the keys and IV for secure channel communication...
                                // Assume the first 16 bytes are the AES key, the next 16 bytes are the IV, and the remaining bytes are the HMAC key
                                byte[] channelAESKey = new byte[16];
                                byte[] channelIV = new byte[16];
                                byte[] channelHMACKey = new byte[decryptedKeysAndIVBytes.Length - 32];
                                Buffer.BlockCopy(decryptedKeysAndIVBytes, 0, channelAESKey, 0, 16);
                                Buffer.BlockCopy(decryptedKeysAndIVBytes, 16, channelIV, 0, 16);
                                Buffer.BlockCopy(decryptedKeysAndIVBytes, 32, channelHMACKey, 0, channelHMACKey.Length);

                                g_aesKeyForChannelEnrolled = channelAESKey;
                                g_ivForChannelEnrolled = channelIV;
                                g_hmacKeyForChannelEnrolled = channelHMACKey;

                                // Log the keys and IV for the channel
                                logs.AppendText("Channel AES Key: " + BitConverter.ToString(channelAESKey).Replace("-", "") + "\n");
                                logs.AppendText("Channel IV: " + BitConverter.ToString(channelIV).Replace("-", "") + "\n");
                                logs.AppendText("Channel HMAC Key: " + BitConverter.ToString(channelHMACKey).Replace("-", "") + "\n");

                                logs.AppendText("Loaded keys and IV for secure channel communication!\n");
                                sendMessage_TextBox.Enabled = true;
                                sendMessage_button.Enabled = true;

                            }

                            logs.AppendText("Decrypted message: " + successMessage + "\n");
                        }
                        catch (Exception ex) {
                            logs.AppendText("Password is incorrect, try again.\n");
                        }
                    }
                    else {
                        logs.AppendText("RSA Signature is NOT valid.\n");
                    }

                }
                catch
                {
                    logs.AppendText("Could not connect to the server. \n");
                }

            }
            else
            {
                logs.AppendText("Check the port number. \n");
            }
        }

        // helper functions
        static string generateHexStringFromByteArray(byte[] input)
        {
            string hexString = BitConverter.ToString(input);
            return hexString.Replace("-", "");
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        // HMAC with SHA-512
        static byte[] applyHMACwithSHA512(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA512 hmacSHA512 = new HMACSHA512(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA512.ComputeHash(byteInput);

            return result;
        }
        // encryption with AES-128
        static byte[] encryptWithAES128(string input, byte[] key, byte[] IV) {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform encryptor = aesObject.CreateEncryptor();
            byte[] result = null;

            try {
                result = encryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }
        static byte[] decryptWithAES128(byte[] byteInput, byte[] key, byte[] IV) {
            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CBC;
            aesObject.Padding = PaddingMode.PKCS7;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;

            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }


        // signing with RSA
        static bool verifyWithRSA(string input, int algoLength, string xmlString, byte[] signature) {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try {
                result = rsaObject.VerifyData(byteInput, "SHA256", signature);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return result;
        }

		private void sendMessage_button_Click(object sender, EventArgs e) {
            if (sendMessage_TextBox.Text != "") {
                string message_to_channel = sendMessage_TextBox.Text;
                byte[] encrypted_sended_message = encryptWithAES128(message_to_channel, g_aesKeyForChannelEnrolled, g_ivForChannelEnrolled);
                byte[] hmaced_message = applyHMACwithSHA512(message_to_channel, g_hmacKeyForChannelEnrolled);

                string hex_hmac = generateHexStringFromByteArray(hmaced_message);
                string hex_encryp = generateHexStringFromByteArray(encrypted_sended_message);

                Byte[] buffer = new Byte[4096];
                buffer = Encoding.Default.GetBytes(hex_encryp + ":" + hex_hmac);
                EnrolledChannel_logs.AppendText("Message to distrubuted sended to the server: " + hex_encryp + ":" + hex_hmac + "\n");
                clientSocket.Send(buffer);
                sendMessage_TextBox.Clear();
            }
            else {
                EnrolledChannel_logs.AppendText("Message cannot be empty \n");
            }
        }
	}


}
