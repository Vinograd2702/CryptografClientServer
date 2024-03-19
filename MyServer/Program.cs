using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using MyClient;
using MyServer;
namespace SocketTcpServer
{
    class Program
    {
        static int port = 8005; // порт для приема входящих запросов
        static string address = "127.0.0.1"; // адрес сервера
        static void Main(string[] args)
        {
            CryptoServer server = new CryptoServer(address, port);

            server.StartServer();
        }
    }
}