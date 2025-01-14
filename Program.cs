namespace ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создаем экземпляр сервера и запускаем его
            ChatServer server = new ChatServer();
            server.Start();

            Console.WriteLine("Сервер запущен. Нажмите Enter для завершения...");
            Console.ReadLine();

            // Останавливаем сервер при завершении работы
            server.Stop();
        }
    }
}
