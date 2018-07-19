using System;
using System.IO;
using System.Threading.Tasks;

namespace PooledValueTaskSource
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Engine eng = new Engine();

            string content = await eng.ReadFileAsync3(@"C:\Notes.md");
            Console.WriteLine("Result: " + content);

            try
            {
                var task = eng.ReadFileAsync3(@"C:\Notes.md");
                string content2 = await task;
                string content3 = await task;
                Console.WriteLine("Result: " + content2 + content3);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
            }

            string content4 = await eng.ReadFileAsync3(@"c:\dummy.txt");
            Console.WriteLine("Result: " + content4);

            string content5 = await eng.ReadFileAsync3(@"C:\Notes.md");
            Console.WriteLine("Result: " + content5);

            var task2 = eng.ReadFileAsync3(@"C:\Notes.md");
            await Task.Delay(2000);
            string content6 = await task2;
            Console.WriteLine("Result: " + content6);

            Console.ReadLine();
        }
    }

    public class Engine
    {
        public Task<string> ReadFileAsync(string filename)
        {
            if (!File.Exists(filename))
                return Task.FromResult(string.Empty);
            return File.ReadAllTextAsync(filename);
        }

        public async ValueTask<string> ReadFileAsync1(string filename)
        {
            if (!File.Exists(filename))
                return string.Empty;
            return await File.ReadAllTextAsync(filename);
        }

        public ValueTask<string> ReadFileAsync2(string filename)
        {
            if (!File.Exists(filename))
                return new ValueTask<string>(string.Empty);
            return new ValueTask<string>(File.ReadAllTextAsync(filename));
        }

        public ValueTask<string> ReadFileAsync3(string filename)
        {
            if (!File.Exists(filename))
                return new ValueTask<string>("!");
            var cachedOp = pool.Rent();
            return cachedOp.RunAsync(filename, pool);
        }

        private ObjectPool<FileReadingPooledValueTaskSource> pool = 
            new ObjectPool<FileReadingPooledValueTaskSource>(() => new FileReadingPooledValueTaskSource(), 10);
    }
}
