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
                    //Byte[] firstBuffer = new Byte[256];
                    //int receivedDataLength = newClient.Receive(firstBuffer);
                    //byte[] receivedData = new byte[receivedDataLength];
                    //Array.Copy(firstBuffer, receivedData, receivedDataLength);

                    //string acceptingInfoString = Encoding.UTF8.GetString(firstBuffer);
                    //acceptingInfoString = acceptingInfoString.Substring(0, acceptingInfoString.IndexOf("\0"));
                    //string araba = Encoding.UTF8.GetString(receivedData);
                    //logs.AppendText(araba);
                    //Decrypting the received RSA encrypted data
                    string initialMessage = Encoding.UTF8.GetString(receivedData2);

                    if (initialMessage.StartsWith("|enroll|"))
                    {

                        byte[] decryptedBytes = null;
                        Byte[] receivedData = new byte[384];
                        newClient.Receive(receivedData);


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
                        string[] tokens = decryptedString.Split(new[] { "|ar|" }, StringSplitOptions.None);

                        string hashedPassword = tokens[0];
                        string usernameVar = tokens[1];
                        string channelVar = tokens[2];
                        //logs.AppendText(hashedPassword + usernameVar + channelVar);

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
                            logs.AppendText(usernameVar + " is already registered!\n");
                            usernameResponseString = "error";
                        }
                        else
                        {
                            usernameResponseString = "success";
                            logs.AppendText(usernameVar + " has registered!\n");

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
                        // logs.AppendText(BitConverter.ToString(signature).Replace("-", ""));

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
                    }
                    else if (initialMessage.StartsWith("|login|"))
                    {
                        //getting the username information from the client
                        Byte[] usernameData = new byte[128];
                        newClient.Receive(usernameData);
                        string usernameString = Encoding.UTF8.GetString(usernameData).Trim('\0');
                        //logs.AppendText("4444444444"+usernameString + "4444444444444");

                        //check if the user has an account in the database
                        bool usernameExists = false;
                        foreach (string line in File.ReadLines(@"../../username-db.txt", Encoding.UTF8))
                        {
                            string[] databaseToken = line.Split(new[] { "||" }, StringSplitOptions.None);
                            string usernameDatabase = databaseToken[0];
                            //logs.AppendText(usernameDatabase + "==" + usernameString+"\n");
                            if (usernameDatabase == usernameString)
                            {
                                usernameExists = true;
                                break;
                            }
                        }
                        //usernameExists = true;
                        //usernameString = "try";

                        if (usernameExists)
                        {
                            // Generate a 128-bit random number and send it to the client
                            byte[] randomNumber = new byte[16];
                            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                            {
                                rng.GetBytes(randomNumber);
                            }
                            newClient.Send(randomNumber);

                            //receiving the hmac from the client
                            byte[] hmacByteBuffer = new byte[384];
                            newClient.Receive(hmacByteBuffer);
                            string hmacByteString2= Encoding.UTF8.GetString(hmacByteBuffer).Trim('\0');
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
                            //currentUserPasswordHash = "A44D9C56248A2B9BC170E6D57FAA415FCC1BD688A7AFD0A773717225BBEC8CBC29C2951CB003D64948244795ED779BB7FA3BB8765D6E65B707DB58CF3C88193B";
                            //logs.AppendText("size of the password ")
                            //applying hmac to see if it is equal to the hmac the client sent
                            //byte[] xb = Encoding.UTF8.GetBytes(currentUserPasswordHash);
                            byte[] xb = hexStringToByteArray(currentUserPasswordHash);
                            logs.AppendText("hash in boyutu: "+xb.Length);
                            string lowerQuarterPassword = currentUserPasswordHash.Substring(0, currentUserPasswordHash.Length / 4);
                            byte[] lowerQuarterBytes = hexStringToByteArray(lowerQuarterPassword);
                            //byte[] lowerQuarterBytes = Encoding.UTF8.GetBytes(lowerQuarterPassword);
                           
                            
                            string randomString = Encoding.UTF8.GetString(randomNumber);
                            logs.AppendText(randomString);
                            byte[] hmacPasswordResult = applyHMACwithSHA512(randomString, lowerQuarterBytes);

                            string hmac1 = Encoding.UTF8.GetString(hmacPasswordResult);
                            string hmac2 = Encoding.UTF8.GetString(hmacByteBuffer);
                            logs.AppendText("client hmaci===" + hmac2 + "\n");
                            logs.AppendText("server hmaci===" + hmac1 + "\n");

                            //bool isEqualhmac = hmacPasswordResult.SequenceEqual(hmacByteBuffer);

                            if (hmac1 == hmac2)
                            {
                                string successString = "Authentication successful";
                                //currentUserPasswordHash = "A44D9C56248A2B9BC170E6D57FAA415FCC1BD688A7AFD0A773717225BBEC8CBC29C2951CB003D64948244795ED779BB7FA3BB8765D6E65B707DB58CF3C88193B";
                                byte[] hashPasswordBytes = hexStringToByteArray(currentUserPasswordHash);
                                //logs.AppendText("hash in botutu=====" +hg.Length +"\n");

                                byte[] secondHalfBytes = new byte[32];
                                Buffer.BlockCopy(hashPasswordBytes, 32, secondHalfBytes, 0, 32);

                                byte[] aesKeyBytes = new byte[16];
                                Buffer.BlockCopy(secondHalfBytes, 0, aesKeyBytes, 0, 16);

                                byte[] aesIVBytes = new byte[16];
                                Buffer.BlockCopy(secondHalfBytes, 16, aesIVBytes, 0, 16);
                                byte[] aesBuffer = new byte[384];
                                logs.AppendText("size of th key ----" + aesKeyBytes.Length.ToString() + "\n");
                                logs.AppendText("size of th IV ----" + aesIVBytes.Length.ToString() + "\n");
                                aesBuffer = encryptWithAES128(successString, aesKeyBytes, aesIVBytes);
                                //string aesIVString2 = Encoding.UTF8.GetString(aesBuffer);
                                //logs.AppendText("encrysdgfgfdsgfddfgdfgfpted::" + aesIVString2 + "\n");
                                newClient.Send(aesBuffer);
                                logs.AppendText("Authentication succesful");
                            }
                            else
                            {
                                string successString = "Authentication unsuccessful";
                                int midpoint = successString.Length / 2;


                                string upperHalf = currentUserPasswordHash.Substring(midpoint);
                                int secondMid = upperHalf.Length / 2;

                                string aesKeyString = upperHalf.Substring(0, secondMid);
                                string aesIVString = upperHalf.Substring(midpoint);
                                byte[] aesKeyBytes = Encoding.UTF8.GetBytes(aesKeyString);
                                byte[] aesIVBytes = Encoding.UTF8.GetBytes(aesIVString);
                                byte[] aesBuffer = new byte[384];
                                logs.AppendText("size of th key ----" + aesKeyBytes.Length.ToString() + "\n");
                                logs.AppendText("size of th IV ----" + aesIVBytes.Length.ToString() + "\n");
                                aesBuffer = encryptWithAES128(successString, aesKeyBytes, aesIVBytes);
                                
                                newClient.Send(aesBuffer);

                                logs.AppendText("Authentication unsuccessful");
                            }

                            //getting the lower quarter of the password hash
                            //string lowerQuarterPassword = currentUserPasswordHash.Substring(0, currentUserPasswordHash.Length / 4);
                            //byte[] lowerQuarterBytes = hexStringToByteArray(lowerQuarterPassword);
                            //string randomNumberString = BitConverter.ToString(randomNumber).Replace("-", "");

                            //byte[] hmacPasswordResult = applyHMACwithSHA512(randomNumberString , lowerQuarterBytes);

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
            //aesObject.Padding = PaddingMode.PKCS7;
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
