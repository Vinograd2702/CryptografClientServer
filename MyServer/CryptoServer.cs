using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Domain;
using MyClient;

namespace MyServer
{
    public class CryptoServer
    {
        RSACryptoServiceProvider ServerRSA; // провайдер сервера
        RSACryptoServiceProvider ClientRSA; // провайдер клиента
        RSAParameters ServerPrivateKey;
        RSAParameters ServerPublicKey;
        RSAParameters ClientPublicKey;
        IPEndPoint IpPoint;
        Socket Socket;

        public CryptoServer(string address, int port)
        {
            ServerRSA = new RSACryptoServiceProvider();
            ClientRSA = new RSACryptoServiceProvider();
            ServerPrivateKey = ServerRSA.ExportParameters(true);

            ServerPublicKey = ServerRSA.ExportParameters(false);

            ClientPublicKey = new RSAParameters();

            IpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            Socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartServer()
        {
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                Socket.Bind(IpPoint);
                // начинаем прослушивание
                Socket.Listen(10);
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    Socket handler = Socket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[149]; // буфер для получаемых данных 148 + 1 сервисный
                    do
                    {
                        bytes = handler.Receive(data);

                        var messageStruct = ConverterMessage.ByteToStruct(data);
                        var bilder = new StringBuilder();
                        var response = new MyMessage();


                        if (messageStruct.ServiseInfo == 7)
                        {

                            // отрезаю лишние биты из струтуры сообщения

                            var cutterMessageData = new byte[128];
                            
                            Array.Copy(messageStruct.Message, cutterMessageData, 128);


                            // декодирую полученное сообщение закрытым ключем сервера

                            var decryptArray = ServerRSA.Decrypt(cutterMessageData, true);



                            builder.Clear();
                            builder.Append(Encoding.UTF8.GetString(decryptArray,
                                0, decryptArray.Length));
                            Console.WriteLine("запрос клиента: " + builder.ToString());


                            // передаю сообщение обратно

                            var encryptArrayToSend = ClientRSA.Encrypt(decryptArray, true);

                            response.ServiseInfo = 7;
                            Array.Copy(encryptArrayToSend, response.Message, encryptArrayToSend.Length);


                            

                            data = ConverterMessage.StructToByte(response);

                            handler.Send(data);
                        }
                        else if (messageStruct.ServiseInfo == 4)
                        {
                            // получить строку с сообщением с клюючем клиента
                            
                            var bytePublicClientKey = messageStruct.Message;

                            //импортировать ключ в провайдер клиента
                            
                            ClientRSA.ImportCspBlob(bytePublicClientKey);
                            ClientPublicKey = ClientRSA.ExportParameters(false);

                            Console.WriteLine("получен публичный ключ от клиента");
                            // создать свою структуру со своим жкспротиремым ключем
                            //
                            var bytePublicServerKey = ServerRSA.ExportCspBlob(false);

                            response.ServiseInfo = 4;
                            response.Message = bytePublicServerKey;
                            // отправить его клиенту
                            //
                            data = ConverterMessage.StructToByte(response);

                            try
                            {
                                handler.Send(data);
                                Console.WriteLine("отправлен публичный ключ клиенту");
                            }
                            catch (Exception ex)
                            {
                                CloseConnection();
                                Console.WriteLine(ex.ToString());
                            }
                        }


                        var strMesssage = builder.ToString();

                        //MyMessage message = strMesssage.

                        builder.Append(Encoding.Unicode.GetString(data,
                        0, bytes));

                    }
                    while (handler.Connected == true);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
            }
        }

        public void CloseConnection()
        {
            Socket.Close();
        }
    }
}
