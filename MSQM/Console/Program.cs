using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using MSQMConsole.Common;
using MSQMConsole.Helpers;

namespace MSQMConsole
{
    class Program
    {
        public enum OperationType
        {
            CreateQueue = 1,
            DeleteQueue,
            PeekAll,
            GetAll,
            GetCount
        }

        static void Main(string[] args)
        {
            while (true)
            {
                if (!Menu())
                    break;
            }
        }

        private static bool Menu()
        {
            Console.WriteLine();
            Console.WriteLine("1. Create queue");
            Console.WriteLine("2. Delete queue");
            Console.WriteLine("3. Peek all messages");
            Console.WriteLine("4. Get all queues");
            Console.WriteLine("5. Get count");
            Console.WriteLine("Press any other key to exit");

            char symbol = Console.ReadKey(false).KeyChar;
            OperationType type = (OperationType)Char.GetNumericValue(symbol);
            Console.Clear();

            switch (type)
            {
                case OperationType.CreateQueue:
                    CreateQueue();
                    break;
                case OperationType.DeleteQueue:
                    DeleteQueue();
                    break;
                case OperationType.PeekAll:
                    PeekAll();
                    break;
                case OperationType.GetAll:
                    GetAllQueues();
                    break;
                case OperationType.GetCount:
                    GetMessagesCount();
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static void CreateQueue()
        {
            try
            {
                Console.WriteLine("\n\tEnter name of queue:");

                string name = Console.ReadLine();
                MhQueue.Create(name);

                Console.WriteLine($"The {name} was created succesfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteQueue()
        {
            try
            {
                Console.WriteLine("\n\tEnter name of queue:");

                string name = Console.ReadLine();
                MhQueue.Delete(name);

                Console.WriteLine($"The {name} was deleted succesfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void PeekAll()
        {
            try
            {
                Console.WriteLine("\n\tEnter name of queue:");
                string name = Console.ReadLine();

                MhQueue queue = new MhQueue(name);
                IEnumerable<IMhMessage> messages = queue.GetMessages();

                foreach (var message in messages)
                {
                    Console.WriteLine($"Message: {message.Body.ToString()}    SentTime: {message.SentTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void GetAllQueues()
        {
            foreach(MessageQueue q in MhQueue.GetAllQueues())
                Console.WriteLine("Queue: {0}\n", q.FormatName);
        }

        private static void GetMessagesCount()
        {
            Console.WriteLine("\n\tEnter name of queue:");
            string name = Console.ReadLine();
            MhQueue queue = new MhQueue(name);
            Console.WriteLine("Queue name: " + name + "; Msg's count" + queue.Count);
        }
    }
}
