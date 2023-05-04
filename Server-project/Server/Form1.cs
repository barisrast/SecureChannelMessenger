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

                        if (usernameExists)
                        {
                            // Generate a 128-bit random number and send it to the client
                            byte[] randomNumber = new byte[16];
                            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                            {
                                rng.GetBytes(randomNumber);
                            }
                            string tempRandomVar = generateHexStringFromByteArray(randomNumber);
                            logs.AppendText("Sending this 128-bit random number to the client for challenge: " + tempRandomVar + "\n");
                            newClient.Send(randomNumber);

                            //receiving the hmac from the client
                            byte[] hmacByteBuffer = new byte[384];
                            newClient.Receive(hmacByteBuffer);
                            string hmacByteString2= Encoding.UTF8.GetString(hmacByteBuffer).Trim('\0');
                            byte[] kedi = Encoding.UTF8.GetBytes(hmacByteString2);
                            string hmacHexVar = generateHexStringFromByteArray(kedi);
                            logs.AppendText("The HMAC user sent is this: "+ kedi+ "\n");
                            hmacByteBuffer = Encoding.UTF8.GetBytes(hmacByteString2);

                            //getting the hash of the password of the user
                            string currentUserPasswordHash = null;
                            foreach (string line in File.ReadLines(@"../../username-db.txt", Encoding.UTF8))
                            {
                                string[] databaseToken = line.Split(new[] { "||" }, StringSplitOptions.None);
                                string usernameDatabase = databaseToken[0];

                                if (usernameDatabase == usernameString)
                                {
                                    currentUserPasswordHash = databaseToken[1];
                                    break;
                                }
                            }
                            //applying hmac to see if it is equal to the hmac the client sent
                            string lowerQuarterPassword = currentUserPasswordHash.Substring(0, currentUserPasswordHash.Length / 4);
                            byte[] lowerQuarterBytes = hexStringToByteArray(lowerQuarterPassword);
                           
                            
                            string randomString = Encoding.UTF8.GetString(randomNumber);
                            byte[] hmacPasswordResult = applyHMACwithSHA512(randomString, lowerQuarterBytes);

                            string varVar = generateHexStringFromByteArray(hmacPasswordResult);
                            logs.AppendText("Server's result of the HMAC of random number: " + varVar + "\n");
                            string hmac1 = Encoding.UTF8.GetString(hmacPasswordResult);
                            string hmac2 = Encoding.UTF8.GetString(hmacByteBuffer);
                            //logs.AppendText("client hmaci===" + hmac2 + "\n");
                            //logs.AppendText("server hmaci===" + hmac1 + "\n");


                            //If the HMACs are equal, we understand that the client sent the correct password
                            if (hmac1 == hmac2)
                            {
                                string successString = "Authentication successful";
                                
                                byte[] hashPasswordBytes = hexStringToByteArray(currentUserPasswordHash);

                                byte[] secondHalfBytes = new byte[32];
                                Buffer.BlockCopy(hashPasswordBytes, 32, secondHalfBytes, 0, 32);

                                byte[] aesKeyBytes = new byte[16];
                                Buffer.BlockCopy(secondHalfBytes, 0, aesKeyBytes, 0, 16);

                                byte[] aesIVBytes = new byte[16];
                                Buffer.BlockCopy(secondHalfBytes, 16, aesIVBytes, 0, 16);
                                byte[] aesBuffer = new byte[256];
                                logs.AppendText("size of th key ----" + aesKeyBytes.Length.ToString() + "\n");
                                logs.AppendText("size of th IV ----" + aesIVBytes.Length.ToString() + "\n");
                                aesBuffer = encryptWithAES128(successString, aesKeyBytes, aesIVBytes);
                                string aesIVString2 = Encoding.UTF8.GetString(aesBuffer);
                                //logs.AppendText("encrysdgfgfdsgfddfgdfgfpted::" + aesIVString2 + "\n");
                                logs.AppendText("\n \n");
                                logs.AppendText("serverdan gelen data: " + aesIVString2 + "\n");
                                string keys = generateHexStringFromByteArray(aesKeyBytes);
                                string ivs = generateHexStringFromByteArray(aesIVBytes);

                                logs.AppendText("\n\n" + "this is the key::: " + keys + "\n");
                                logs.AppendText("\n\n" + "this is the iv::: " + ivs + "\n");
                                newClient.Send(aesBuffer);
                                logs.AppendText("Authentication succesful");
                            }
                            else
                            {
                                string successString = "Authentication unsuccessful";
                                byte[] hashPasswordBytes = hexStringToByteArray(currentUserPasswordHash);

                                byte[] secondHalfBytes = new byte[32];
                                Buffer.BlockCopy(hashPasswordBytes, 32, secondHalfBytes, 0, 32);

                                byte[] aesKeyBytes = new byte[16];
                                Buffer.BlockCopy(secondHalfBytes, 0, aesKeyBytes, 0, 16);

                                byte[] aesIVBytes = new byte[16];
                                Buffer.BlockCopy(secondHalfBytes, 16, aesIVBytes, 0, 16);
                                byte[] aesBuffer = new byte[256];
                                logs.AppendText("size of th key ----" + aesKeyBytes.Length.ToString() + "\n");
                                logs.AppendText("size of th IV ----" + aesIVBytes.Length.ToString() + "\n");
                                aesBuffer = encryptWithAES128(successString, aesKeyBytes, aesIVBytes);
                                string aesIVString2 = Encoding.UTF8.GetString(aesBuffer);
                                //logs.AppendText("encrysdgfgfdsgfddfgdfgfpted::" + aesIVString2 + "\n");
                                logs.AppendText("\n \n");
                                logs.AppendText("serverdan gelen data: " + aesIVString2 + "\n");
                                string keys = generateHexStringFromByteArray(aesKeyBytes);
                                string ivs = generateHexStringFromByteArray(aesIVBytes);

                                logs.AppendText("\n\n" + "this is the key::: " + keys + "\n");
                                logs.AppendText("\n\n" + "this is the iv::: " + ivs + "\n");
                                newClient.Send(aesBuffer);
                                logs.AppendText("Authentication unsuccessful");
                            }


                        }
                        else
                        {
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
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            byte[] result = null;

            try {
                result = rsaObject.SignData(byteInput, "SHA256");
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
    }
}
