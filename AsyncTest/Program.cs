using System;
using System.Threading.Tasks;

namespace AsyncTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double sync = 0f;
            double async = 0f;

            int cursorX, cursorY;

            Console.Write("Synchronous: ");
            (cursorX, cursorY) = Console.GetCursorPosition();
            for (int i = 0; i < 1000; i++)
            {
                Console.SetCursorPosition(cursorX, cursorY);
                Console.Write(i + 1);
                sync += SyncMain();
            }

            Console.WriteLine();

            Console.Write("Asynchronous: ");
            (cursorX, cursorY) = Console.GetCursorPosition();
            for (int i = 0; i < 1000; i++)
            {
                Console.SetCursorPosition(cursorX, cursorY);
                Console.Write(i + 1);
                Task<double> asyncMain = AsyncMain();
                async += await asyncMain;
                asyncMain.Wait();
            }

            Console.WriteLine();

            Console.WriteLine("Sync Mean Time: {0:F2} ms", sync / 1000f);
            Console.WriteLine("Async Mean Time: {0:F2} ms", async / 1000f);
        }

        static double SyncMain()
        {
            DateTime t = DateTime.Now;
            int a = Fetch();
            int b = Decode();
            int c = Execute();

            return DateTime.Now.Subtract(t).TotalMilliseconds;
        }

        static async Task<Double> AsyncMain()
        {
            DateTime t = DateTime.Now;
            Task<int> fetch = FetchAsync();
            Task<int> decode = DecodeAsync();
            Task<int> execute = ExecuteAsync();

            int a = await fetch;
            int b = await decode;
            int c = await execute;

            return DateTime.Now.Subtract(t).TotalMilliseconds;
        }

        static async Task<int> FetchAsync()
        {
            int i = 0;
            for (i = 0; i < int.MaxValue / 1024; i++) { }
            return i;
        }

        static async Task<int> DecodeAsync()
        {
            int i = 0;
            for (i = 0; i < 1024; i++) { }
            return i;
        }

        static async Task<int> ExecuteAsync()
        {
            int i = 0;
            for (i = 0; i < int.MaxValue / 8; i++) { }
            return i;
        }

        static int Fetch()
        {
            int i = 0;
            for (i = 0; i < int.MaxValue / 1024; i++) { }
            return i;
        }

        static int Decode()
        {
            int i = 0;
            for (i = 0; i < 1024; i++) { }
            return i;
        }

        static int Execute()
        {
            int i = 0;
            for (i = 0; i < int.MaxValue / 8; i++) { }
            return i;
        }
    }
}
