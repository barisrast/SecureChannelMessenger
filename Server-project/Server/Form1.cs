using System;
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
using System.IO;

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
            new System.IO.StreamReader("server_sign_verify_pub_prv.txt"))
            {
                RSA3072PrivateVerificationKey = fileReader.ReadLine();
            }
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

                IF100_Master_TextBox.Enabled = true;
                IF100_GenerateKey_Button.Enabled = true;
                MATH101_Master_TextBox.Enabled = true;
                MATH101_GenerateKey_Button.Enabled = true;
                SPS101_Master_TextBox.Enabled = true;
                SPS101_GenerateKey_Button.Enabled = true;

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
                    Byte[] receivedData2 = new Byte[384];
                    newClient.Receive(receivedData2);
    
                    //Decrypting the received RSA encrypted data
                    string initialMessage = Encoding.UTF8.GetString(receivedData2);

                    if (initialMessage.StartsWith("|enroll|"))
                    {
                        logs.AppendText("User sent enrollment request. \n");
                        byte[] decryptedBytes = null;
                        Byte[] receivedData = new byte[384];
                        newClient.Receive(receivedData);

                        string receivedString = generateHexStringFromByteArray(receivedData);
                        logs.AppendText("This is the encrypted message that contains username/password/course:" + receivedString+"\n");
                        RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider();
                        rsaObject.FromXmlString(RSA3072PrivateEncryptionKey);
                        try
                        {
                            decryptedBytes = rsaObject.Decrypt(receivedData, true);
                        }
                        catch (Exception e)
                        {
                            logs.AppendText(e.Message);

                        }
                        

                        string decryptedString = Encoding.UTF8.GetString(decryptedBytes);
                        logs.AppendText("This is the decrypted version of the previous message: "+decryptedString+"\n");
                        string[] tokens = decryptedString.Split(new[] { "|ar|" }, StringSplitOptions.None);

                        string hashedPassword = tokens[0];
                        string usernameVar = tokens[1];
                        string channelVar = tokens[2];
                        //logs.AppendText(hashedPassword + usernameVar + channelVar);
                        logs.AppendText(usernameVar + " wants to enroll to " + channelVar + ", hash of his password is " + hashedPassword + "\n");

                        bool usernameExists = false;
                        foreach (string line in File.ReadLines(@"../../username-db.txt", Encoding.UTF8))
                        {
                            string[] databaseToken = line.Split(new[] { "||" }, StringSplitOptions.None);
                            string usernameDatabase = databaseToken[0];

                            if (usernameDatabase == usernameVar)
                            {
                                usernameExists = true;
                                break;
                            }
                        }

                        string usernameResponseString = "";
                        if (usernameExists)
                        {
                            logs.AppendText(usernameVar + " is already registered to this course!\n");
                            usernameResponseString = "error";
 

                        }
                        else
                        {
                            usernameResponseString = "success";
                            logs.AppendText(usernameVar + " has successfuly registered to "+channelVar+"!\n");

                            DateTime now = DateTime.Now;
                            now.ToString("F");

                            string finalLine = usernameVar + "||" + hashedPassword + "||" + channelVar + "||" + now;

                            using (StreamWriter file = new StreamWriter("../../username-db.txt", append: true))
                            {
                                file.WriteLine(finalLine);
                            }

                            Thread receiveThread = new Thread(() => Receive(newClient));
                            receiveThread.Start();
                        }

                        // Sign the success/error message
                        byte[] signature = signWithRSA(usernameResponseString, 3072, RSA3072PrivateVerificationKey);
                        string signatureStringTemp = generateHexStringFromByteArray(signature);
                        logs.AppendText("Signed the "+ usernameResponseString+" message with this signature: "+ signatureStringTemp+"\n");

                        byte[] messageBytes = Encoding.UTF8.GetBytes(usernameResponseString);
                        byte[] combinedMessage = new byte[messageBytes.Length + signature.Length];

                        // Combine the message and signature
                        Buffer.BlockCopy(messageBytes, 0, combinedMessage, 0, messageBytes.Length);
                        Buffer.BlockCopy(signature, 0, combinedMessage, messageBytes.Length, signature.Length);

                        // Send the combined length first
                        byte[] combinedLengthBytes = BitConverter.GetBytes(combinedMessage.Length);
                        newClient.Send(combinedLengthBytes);

                        // Send the combined message and signature
                        newClient.Send(combinedMessage);
                        if(usernameResponseString == "error")
                        {
                            logs.AppendText("Closing the connection...");
                            newClient.Close();
                            socketList.Remove(newClient);
                        }
                    }
                    else if (initialMessage.StartsWith("|login|"))
                    {
                        logs.AppendText("User sent log-in request\n");
                        //getting the username information from the client
                        Byte[] usernameData = new byte[128];
                        newClient.Receive(usernameData);
                        string usernameString = Encoding.UTF8.GetString(usernameData).Trim('\0');


                        //checking if the user has an account in the database
                        bool usernameExists = false;
                        foreach (string line in File.ReadLines(@"../../username-db.txt", Encoding.UTF8))
                        {
                            string[] databaseToken = line.Split(new[] { "||" }, StringSplitOptions.None);
                            string usernameDatabase = databaseToken[0];
                            if (usernameDatabase == usernameString)
                            {
                                usernameExists = true;
                                break;
                            }
                        }

                        if (usernameExists) {
                            // Generate a 128-bit random number and send it to the client
                            byte[] randomNumber = new byte[16];
                            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                                rng.GetBytes(randomNumber);
                            }
                            string tempRandomVar = generateHexStringFromByteArray(randomNumber);
                            logs.AppendText("Sending this 128-bit random number to the client for challenge: " + tempRandomVar + "\n");
                            newClient.Send(randomNumber);

                            //receiving the hmac from the client
                            byte[] hmacByteBuffer = new byte[384];
                            newClient.Receive(hmacByteBuffer);
                            string hmacByteString2 = Encoding.UTF8.GetString(hmacByteBuffer).Trim('\0');
                            byte[] kedi = Encoding.UTF8.GetBytes(hmacByteString2);
                            string hmacHexVar = Encoding.UTF8.GetString(kedi);
                            logs.AppendText("The HMAC user sent is this: " + hmacHexVar + "\n");
                            hmacByteBuffer = Encoding.UTF8.GetBytes(hmacByteString2);

                            //getting the hash of the password of the user
                            string currentUserPasswordHash = null;
                            string currentUserChannel = "";

                            foreach (string line in File.ReadLines(@"../../username-db.txt", Encoding.UTF8)) {
                                string[] databaseToken = line.Split(new[] { "||" }, StringSplitOptions.None);
                                string usernameDatabase = databaseToken[0];

                                if (usernameDatabase == usernameString) {
                                    currentUserPasswordHash = databaseToken[1];
                                    currentUserChannel = databaseToken[2];
                                    break;
                                }
                            }
                            // Extract AES key and IV from the hashed password bytes
                            byte[] hashedPasswordBytes = hexStringToByteArray(currentUserPasswordHash);
                            byte[] aesKey = new byte[16];
                            byte[] aesIV = new byte[16];
                            Buffer.BlockCopy(hashedPasswordBytes, 0, aesKey, 0, 16);
                            Buffer.BlockCopy(hashedPasswordBytes, 16, aesIV, 0, 16);

                            //applying hmac to see if it is equal to the hmac the client sent
                            string lowerQuarterPassword = currentUserPasswordHash.Substring(0, currentUserPasswordHash.Length / 4);
                            byte[] lowerQuarterBytes = hexStringToByteArray(lowerQuarterPassword);

                            string randomString = Encoding.UTF8.GetString(randomNumber);
                            byte[] hmacPasswordResult = applyHMACwithSHA512(randomString, lowerQuarterBytes);

                            string varVar = generateHexStringFromByteArray(hmacPasswordResult);
                            logs.AppendText("Server's result of the HMAC of random number: " + varVar + "\n");
                            string hmac1 = Encoding.UTF8.GetString(hmacPasswordResult);
                            string hmac2 = Encoding.UTF8.GetString(hmacByteBuffer);


                            //If the HMACs are equal, we understand that the client sent the correct password
                            if (hmac1 == hmac2) {
                                if (hashedMasterKeys.ContainsKey(currentUserChannel)) {
                                    logs.AppendText("Retrieved Master Key: " + BitConverter.ToString(hashedMasterKeys[currentUserChannel]).Replace("-", ""));
                                    logs.AppendText(usernameString + " authenticated.\n");

                                    // Get the hashed master key bytes for the user's channel
                                    byte[] hashedMasterKeyBytes = hashedMasterKeys[currentUserChannel];

                                    // Split the byte array to generate the AES key, IV and HMAC key for the channel
                                    byte[] aesKeyForChannel = new byte[16];
                                    byte[] ivForChannel = new byte[16];
                                    byte[] hmacKeyForChannel = new byte[16];

                                    Buffer.BlockCopy(hashedMasterKeyBytes, 0, aesKeyForChannel, 0, 16);
                                    Buffer.BlockCopy(hashedMasterKeyBytes, 16, ivForChannel, 0, 16);
                                    Buffer.BlockCopy(hashedMasterKeyBytes, 32, hmacKeyForChannel, 0, 16);


                                    // Encrypt the success message, delimiter, keys and IV using AES-128 encryption with the key and IV derived from client's password
                                    string toBeEncrypted = "Authentication successful||" + generateHexStringFromByteArray(aesKeyForChannel) + generateHexStringFromByteArray(ivForChannel) + generateHexStringFromByteArray(hmacKeyForChannel);
                                    byte[] encryptedMessageWithKeys = encryptWithAES128(toBeEncrypted, aesKey, aesIV);

                                    // Convert the encrypted message to base64 string
                                    string encryptedMessageWithKeysBase64 = Convert.ToBase64String(encryptedMessageWithKeys);

                                    // Sign the base64 string with the server's RSA key
                                    byte[] signature = signWithRSA(encryptedMessageWithKeysBase64, 3072, RSA3072PrivateVerificationKey);

                                    // Send the encrypted message and signature to the client
                                    newClient.Send(encryptedMessageWithKeys);
                                    logs.AppendText("Sending encrypted message with keys: " + encryptedMessageWithKeysBase64 + "\n\n");
                                    newClient.Send(signature);
                                    logs.AppendText("Sending RSA signature: " + Convert.ToBase64String(signature) + "\n\n");


                                    // Log the keys and IV for the channel
                                    logs.AppendText("AES Key for Channel: " + generateHexStringFromByteArray(aesKeyForChannel) + "\n");
                                    logs.AppendText("IV for Channel: " + generateHexStringFromByteArray(ivForChannel) + "\n");
                                    logs.AppendText("HMAC Key for Channel: " + generateHexStringFromByteArray(hmacKeyForChannel) + "\n");
                                }
                                else {
                                    // Channel keys and IV are not available
                                    string channelUnavailableString = "Channel Unavailable";
                                    byte[] encryptedChannelUnavailableString = encryptWithAES128(channelUnavailableString, aesKey, aesIV);
                                    string base64EncryptedChannelUnavailableString = Convert.ToBase64String(encryptedChannelUnavailableString);
                                    byte[] signature = signWithRSA(base64EncryptedChannelUnavailableString, 3072, RSA3072PrivateVerificationKey);

                                    // Send encrypted message and signature to the client
                                    newClient.Send(encryptedChannelUnavailableString);
                                    //logs.AppendText("Sending encrypted message: " + Convert.ToBase64String(encryptedChannelUnavailableString) + "\n\n");
                                    newClient.Send(signature);
                                    //logs.AppendText("Sending RSA signature: " + Convert.ToBase64String(signature) + "\n\n");

                                    logs.AppendText("Channel keys and IV are not available for " + currentUserChannel + ". Sent 'Channel Unavailable' message to the client.\n");
                                }

                            }
                            else {
                                string successString = "Authentication unsuccessful";
                                byte[] encryptedSuccessString = encryptWithAES128(successString, aesKey, aesIV);
                                string base64EncryptedSuccessString = Convert.ToBase64String(encryptedSuccessString);
                                byte[] signature = signWithRSA(base64EncryptedSuccessString, 3072, RSA3072PrivateVerificationKey);

                                // Send encrypted message and signature to the client
                                newClient.Send(encryptedSuccessString);
                                logs.AppendText("Sending encrypted message: " + Convert.ToBase64String(encryptedSuccessString) + "\n\n");
                                newClient.Send(signature);
                                logs.AppendText("Sending RSA signature: " + Convert.ToBase64String(signature) + "\n\n");
                            }


                        }

                        else {
                            string usernameProblem = "You do not have an acccount with this username!";
                            byte[] problemBytes = Encoding.UTF8.GetBytes(usernameProblem);
                            newClient.Send(problemBytes);
                            newClient.Close();
                            socketList.Remove(newClient);

                        }

                    }

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

        private void Receive(Socket s)
        {
            //Socket s = socketList[socketList.Count - 1];
            bool connected = true;

            while (connected && !terminating)
            {
                try
                {
                    Byte[] buffer = new Byte[384];
                    s.Receive(buffer);

                    string incomingMessage = Encoding.UTF8.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    logs.AppendText(incomingMessage + "\n");
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

        // verifying with RSA
        static byte[] signWithRSA(string input, int algoLength, string xmlString) {
            // convert input string to byte array
            byte[] byteInput = Encoding.UTF8.GetBytes(input); // use UTF8 instead of Default
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            byte[] result = null;

            try {
                result = rsaObject.SignData(byteInput, CryptoConfig.MapNameToOID("SHA256"));
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return result;
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
        static byte[] encryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.UTF8.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CBC;
            aesObject.Padding = PaddingMode.PKCS7;
            // feedback size should be equal to block size
            //aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            
            // create an encryptor with the settings provided
            ICryptoTransform encryptor = aesObject.CreateEncryptor();
            byte[] result = null;

            try
            {
                result = encryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        // hash function: SHA-512
        static byte[] hashWithSHA512(string input) {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA512CryptoServiceProvider sha512Hasher = new SHA512CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha512Hasher.ComputeHash(byteInput);

            return result;
        }


        /* MASTER KEY GENERATOR FUNCTIONS FOR THE SPECIFIED CHANNELS */
        Dictionary<string, byte[]> hashedMasterKeys = new Dictionary<string, byte[]>();

        private void GenerateKey_IF100_Click(object sender, EventArgs e) {
            if(IF100_Master_TextBox.Text != "") {
                hashedMasterKeys["IF100"] = hashWithSHA512(IF100_Master_TextBox.Text);
                logs.AppendText("Master Key for IF100 successfully generated.\n");
                IF100_Channel_Logs.Enabled = true;
                IF100_GenerateKey_Button.Enabled = false;
                IF100_Master_TextBox.Enabled = false;
			}
			else {
                logs.AppendText("Please enter a key for IF100 channel!\n");
            }
		}

		private void MATH101_GenerateKey_Button_Click(object sender, EventArgs e) {
            if (MATH101_Master_TextBox.Text != "") {
                hashedMasterKeys["MATH101"] = hashWithSHA512(MATH101_Master_TextBox.Text);
                logs.AppendText("Master Key for MATH101 successfully generated.\n");
                MATH101_Channel_Logs.Enabled = true;
                MATH101_GenerateKey_Button.Enabled = false;
                MATH101_Master_TextBox.Enabled = false;
            }
            else {
                logs.AppendText("Please enter a key for MATH101 channel!\n");
            }
        }

		private void GenerateKey_SPS101_Click(object sender, EventArgs e) {
            if (SPS101_Master_TextBox.Text != "") {
                hashedMasterKeys["SPS101"] = hashWithSHA512(SPS101_Master_TextBox.Text);
                logs.AppendText("Master Key for SPS101 successfully generated.\n");
                SPS101_Channel_Logs.Enabled = true;
                SPS101_GenerateKey_Button.Enabled = false;
                SPS101_Master_TextBox.Enabled = false;
            }
            else {
                logs.AppendText("Please enter a key for SPS101 channel!\n");
            }
        }
	}
}
