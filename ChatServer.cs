using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class ChatServer
    {
        private TcpListener _listener;
        private List<Client> _clients;
        private bool _isRunning;

        public ChatServer()
        {
            _clients = new List<Client>();
            _isRunning = false;
        }

        // Метод для запуска сервера
        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 8888);
            _listener.Start();
            _isRunning = true;

            // Запускаем прослушивание подключений в отдельном потоке
            Task.Run(() => AcceptClientsAsync());

            Console.WriteLine("Сервер запущен и прослушивает подключения...");
        }

        // Метод для остановки сервера
        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();

            // Отключаем всех клиентов
            foreach (var client in _clients)
            {
                client.Disconnect();
            }

            Console.WriteLine("Сервер остановлен.");
        }

        // Метод для принятия подключений
        private async Task AcceptClientsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    var chatClient = new Client(client, this);
                    _clients.Add(chatClient);
                    Console.WriteLine($"Новый клиент подключен. {chatClient.Socket}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при принятии подключения: {ex.Message}");
                }
            }
        }

        // Метод для рассылки сообщения всем клиентам
        public async Task BroadcastMessageAsync(string message, Client sender)
        {
            foreach (var client in _clients)
            {
                if (client != sender)
                {
                    await client.SendMessageAsync(message);
                }
            }
        }

        // Метод для удаления клиента из списка
        public void RemoveClient(Client client)
        {
            _clients.Remove(client);
            Console.WriteLine($"Клиент отключен.  {client.Socket}");
        }
    }
}
