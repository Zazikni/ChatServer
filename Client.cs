using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Client
    {
        public string Socket
        {
            get
            {
                return _tcpClient.Client.RemoteEndPoint.ToString();
            }
        }


        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private ChatServer _server;

        public Client(TcpClient tcpClient, ChatServer server)
        {
            _tcpClient = tcpClient;
            _server = server;
            _stream = _tcpClient.GetStream();

            // Запускаем чтение сообщений в отдельном потоке
            Task.Run(() => ReceiveMessagesAsync());
        }

        // Метод для отключения клиента
        public void Disconnect()
        {
            _stream.Close();
            _tcpClient.Close();
            _server.RemoveClient(this);
        }

        // Метод для отправки сообщения клиенту асинхронно
        public async Task SendMessageAsync(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        // Метод для получения сообщений от клиента
        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    await _server.BroadcastMessageAsync(message, this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении сообщения: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
