using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using MyClient;
using Domain;
namespace SocketTcpClient
{
    class Program
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера


        static void Main(string[] args)
        {
            CryptoClient client = new CryptoClient(address, port);

            client.ConnectToServer();

            bool DoProgram = true;

            while (DoProgram)
            {
                Console.WriteLine("1) Send");
                Console.WriteLine("2) Close connection");


                var choise = Convert.ToInt32(Console.ReadLine());

                if ( choise == 2 )
                {
                    DoProgram = false;
                }
                else
                {
                    Console.Write("Enter the message -> ");
                    client.SendMessageToServer(Console.ReadLine());
                }
            };
        }
    }
}
