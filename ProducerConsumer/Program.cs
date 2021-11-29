using System;
using System.Threading;
using System.Collections.Generic;

namespace ProducerConsumer
{
    class Program
    {

        public delegate void someDel();
        private static List<Thread> consumers = new List<Thread>();
        private static Queue<Action> tasks = new Queue<Action>();
        private static EventWaitHandle newTaskAvailable = new AutoResetEvent(false);
        private static object queueLock = new object();

        private static void EnqueueTask(Action task)
        {
            lock (queueLock)
            {
                tasks.Enqueue(task);
            }
            newTaskAvailable.Set();
            
        }

        private static void DoWork()
        {
            while (true)
            {
                Action task = null;

                lock (queueLock)
                {
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                    }
                }
                
                if (task != null)
                {
                    task();
                }
                else
                {
                    newTaskAvailable.WaitOne(); // c3 - c2
                }
                
            }
        }
        static void Main(string[] args)
        {
            consumers.Add(new Thread(() => DoWork())); // c1
            consumers.Add(new Thread(() => DoWork())); // c2
            consumers.Add(new Thread(() => DoWork())); // c3

            foreach(Thread t in consumers)
            {
                t.Start();
            }

            while (true)
            {
                Random r = new Random();
                
                EnqueueTask(() =>
                {
                    int number = r.Next(10);
                    Console.WriteLine(number);
                });

                Thread.Sleep(r.Next(1000));
            }

        }
    }
}
