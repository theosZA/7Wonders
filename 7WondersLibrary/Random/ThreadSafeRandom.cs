using System;
using System.Threading;

namespace _7Wonders
{
    internal static class ThreadSafeRandom
    {
        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }

        [ThreadStatic] private static Random Local;
    }
}
