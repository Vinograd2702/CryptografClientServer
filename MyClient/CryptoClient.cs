using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace MyClient
{
    public class CryptoClient
    {
        RSACryptoServiceProvider ClientRSA; // провайдер клиента
        RSACryptoServiceProvider ServerRSA; // провайдер сервера
        RSAParameters ClientPrivateKey;
        RSAParameters ClientPublicKey;
        RSAParameters ServerPublicKey;
        IPEndPoint IpPoint;
        Socket Socket;
        public CryptoClient(string address, int port)
        {
            
            ClientRSA = new RSACryptoServiceProvider();
            ServerRSA = new RSACryptoServiceProvider();
            ClientPrivateKey = ClientRSA.ExportParameters(true);
            ClientPublicKey = ClientRSA.ExportParameters(false);
            ServerPublicKey = new RSAParameters();

            IpPoint = new IPEndPoint(IPAddress.Parse(address), port);
            Socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void ConnectToServer()
        {
            //  подключаюсь к серверу
            
            try
            {
                Socket.Connect(IpPoint);
                SendPublicKeyToServer();
            }
            catch (Exception ex)
            {
                CloseConnection();
                Console.WriteLine(ex.ToString());
            }

            
        }

        public void CloseConnection()
        {
            Socket.Close();
        }

        public void SendMessageToServer(string message)
        {
            StringBuilder builder = new StringBuilder();

            if (Socket.Connected)
            {
                var messageToSend = new MyMessage
                {
                    ServiseInfo = 7
                };

                var noEncryptArrayToSend = Encoding.UTF8.GetBytes(message);

                // кодирую сообщение публичным ключем сервера

                var encryptArrayToSend = ServerRSA.Encrypt(noEncryptArrayToSend, true);

                // копирую массив с ключем в массив сообщения, чтобы не уменьшить длинну константного сообщения
                Array.Copy(encryptArrayToSend, messageToSend.Message, encryptArrayToSend.Length);
                // преобразую структуру в байт массив

                byte[] data = ConverterMessage.StructToByte(messageToSend);
                // Отправил сообщение на сервер
                Socket.Send(data);

                // Получил сообщение от сервера
                var bytes = Socket.Receive(data);
                // Расшифрую приватным ключем сервера

                var responseMessage = ConverterMessage.ByteToStruct(data);

                if (responseMessage.ServiseInfo == 7)
                {
                    var cutterMessageData = new byte[128];

                    Array.Copy(responseMessage.Message, cutterMessageData, 128);

                    var decryptArrayFromServer = ClientRSA.Decrypt(cutterMessageData, true);



                    builder.Clear();
                    builder.Append(Encoding.UTF8.GetString(decryptArrayFromServer,
                                    0, decryptArrayFromServer.Length));
                    Console.WriteLine("ответ сервера: " + builder.ToString());
                }


            }
        }


        // отправить структуру баййтами публичного ключа
        // получить публичный ключ
        // импортировать ключ в соответствующий провайдер

        public void SendPublicKeyToServer()
        {
            if (Socket.Connected)
            {
                var builder = new StringBuilder();
                // экспорт публичного ключа в масси байт (size = 148)
                var bytePublicKey = ClientRSA.ExportCspBlob(false);
                // создал сообщение 
                var messageToSend = new MyMessage
                {
                    ServiseInfo = 4,
                    Message = bytePublicKey
                };

                byte[] data = ConverterMessage.StructToByte(messageToSend);
                Socket.Send(data);
                Console.WriteLine("серверу отправлен публичный ключ клиента");
                // Получил сообщение от сервера
                var bytes = Socket.Receive(data);

                // декодировать в струтуру сообщение
                //
                var response = ConverterMessage.ByteToStruct(data);

                // проверить что сервис инфо равно 4
                //
                if (response.ServiseInfo == 4)
                {
                    // если да - получить ключ сервера и имортировать в провайдер сервера
                    //

                    ServerRSA.ImportCspBlob(response.Message);
                    Console.WriteLine("сервер отправил публичный ключ клиенту");
                }

                ServerPublicKey = ServerRSA.ExportParameters(false);

                builder.Clear();

            }
        }
        

    }
}

