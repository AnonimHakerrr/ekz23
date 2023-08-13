    using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace ekz2301
{

    class ServerProgram
    {
        static TcpListener listener;
        static List<TcpClient> clients = new List<TcpClient>();
        static void Main()
        {
            listener = new TcpListener(IPAddress.Any, 12345);
            listener.Start();

            Console.WriteLine("Server started, waiting for clients...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        static void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + message);

                BroadcastMessage(message, stream);
            }

            clients.Remove(client);
            stream.Close();
            client.Close();
        }

        static void BroadcastMessage(string message, NetworkStream excludeStream = null)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                if (stream != excludeStream)
                {
                    stream.Write(messageBytes, 0, messageBytes.Length);
                    stream.Flush();
                }
            }
        }
    }

}
