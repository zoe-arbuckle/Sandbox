namespace AsyncAwait
{
    public sealed class Lesson1_AsyncAwaitBasics
    {
        public async Task<string> SimulateNetworkRequestAsync()
        {
            await Task.Delay(1500);
            return "User data retrieved at " + DateTime.Now.ToString("HH:mm:ss.fff");
        }

        public async Task FetchMultipleUsersSequentialAsync(int numberOfUsers = 3)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < numberOfUsers; i++)
            {
                Console.WriteLine(await SimulateNetworkRequestAsync());
            }

            Console.WriteLine($"Total time: {watch.ElapsedMilliseconds}ms");
        }
    }
}
