using System.Net.Sockets;

namespace Listener
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите адрес сервера: ");
            string addres = Console.ReadLine();
            Console.Write("Введите порт сервера: ");
            string port = Console.ReadLine();
            ServListener sListener = new ServListener(addres,port);
            if (!sListener.Init())
            {
                Console.WriteLine($"Ошибка инициализации сервера.\r\n" +
                    $"Ошибка: {sListener.m_LastException.Message}\r\n" +
                    $"Путь: {sListener.m_LastException.StackTrace}\r\n" +
                    $"Доп.информация: {sListener.m_LastException.InnerException?.Message}");
                Console.Write("Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine($"Инициализация сервера успешна");
            }

            Console.Write($"Введите команду из списка\r\n" +
                    $"start\r\n" +
                    $"stop\r\n" +
                    $"info\r\n" +
                    $"statistics\r\n" +
                    $"exit\r\n");
            while (true)
            {
                
                string command = Console.ReadLine();
                switch (command)
                {
                    case "start":
                        {
                            Console.Clear();
                            sListener.Start();
                            break;
                        }
                    case "stop":
                        {
                            //Console.Clear();
                            sListener.Stop();
                            break;
                        }
                    case "info":
                        {
                            //Console.Clear();
                            sListener.Info();
                            break;
                        }
                    case "statistics":
                        {
                            //Console.Clear();
                            sListener.Statistics();
                            break;
                        }
                    case "exit":
                        {
                            return;
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            break;
                        }
                }
                

            }
            // какой протокол? Куда сервер посылает информацию? есть ли признак окончания передачи? Как идет вещаение, если принимающего нет, отправляет в пустоту? сообщения идут непрерывным потоком, или каждое сообщение отпалвяется ?

        }

    }
}