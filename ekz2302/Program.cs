
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ekz2302
{

    class ClientProgram
    {
        static TcpClient client;
        static NetworkStream stream;

        static void Main()
        {
            client = new TcpClient();
            client.Connect("127.0.0.1", 12345);
            stream = client.GetStream();

            Console.WriteLine("Connected to server. Type 'exit' to quit.");

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "exit")
                    break;

                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }

            stream.Close();
            client.Close();
        }

        static void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;

                string serverMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Server: " + serverMessage);
            }
        }
    }


}
