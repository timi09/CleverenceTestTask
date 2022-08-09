namespace СleverenceTestTask
{
    internal class Program
    {
        private static void myEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Enter handler");
            Thread.Sleep(4000);
            Console.WriteLine("Exit handler");
        }

        static void Main(string[] args)
        {
            EventHandler h = new EventHandler(myEventHandler);

            AsyncCaller ac = new AsyncCaller(h);

            bool completedOK = ac.Invoke(5000, null, EventArgs.Empty);
            Console.WriteLine(completedOK);

            completedOK = ac.Invoke(3000, null, EventArgs.Empty);
            Console.WriteLine(completedOK);
        }
    }

    // #1
    internal static class Server
    {
        private static int count;

        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public static int GetCount()
        {
            locker.EnterReadLock();
            try
            {
                return count;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            locker.EnterWriteLock();
            try
            {
                checked
                {
                    count += value;
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }

    // #2
    /// <summary>
    /// Класс AsyncCaller дает возможность полусинхронного вызова делегата.
    /// </summary
    internal class AsyncCaller
    {
        public readonly EventHandler Handler;

        public AsyncCaller(EventHandler handler)
        {
            Handler = handler;
        }

        /// <summary>
        /// Метод Invoke полусинхронно вызывает делегат.
        /// Это означает, что делегат будет вызван, и вызывающий поток будет ждать,
        /// пока вызов не выполнится, но если выполнение делегата займет больше чем заданное число миллисекунд,
        /// то Invoke выйдет и вернет значение false.
        /// </summary>
        /// <returns>true если делегат выполнен успешно за заданное число миллисекунд, иначе false</returns>
        public bool Invoke(int millisecondsTimeout, object sender, EventArgs e)
        {
            Task task = Task.Run(() => Handler.Invoke(sender, e));
            return task.Wait(millisecondsTimeout);
        }
    }
}