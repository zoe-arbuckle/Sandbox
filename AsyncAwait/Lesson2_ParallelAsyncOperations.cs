namespace AsyncAwait
{
    public sealed class Lesson2_ParallelAsyncOperations
    {
        public async Task<string> SimulateNetworkRequestAsync(int userId)
        {
            await Task.Delay(1500);
            return $"User {userId} data retrieved at {DateTime.Now:HH:mm:ss.fff}";
        }

        // Sequential approach (from Lesson 1)
        public async Task FetchUsersSequentialAsync(int numberOfUsers = 3)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("=== Sequential Approach ===");

            for (int i = 1; i <= numberOfUsers; i++)
            {
                Console.WriteLine(await SimulateNetworkRequestAsync(i));
            }

            watch.Stop();
            Console.WriteLine($"Total time: {watch.ElapsedMilliseconds}ms\n");
        }

        // Parallel approach using Task.WhenAll()
        public async Task FetchUsersParallelAsync(int numberOfUsers = 3)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("=== Parallel Approach (Task.WhenAll) ===");

            // Create all tasks without awaiting
            Task<string>[] tasks = new Task<string>[numberOfUsers];
            for (int i = 1; i <= numberOfUsers; i++)
            {
                tasks[i - 1] = SimulateNetworkRequestAsync(i);
            }

            // Wait for all tasks to complete
            string[] results = await Task.WhenAll(tasks);

            // Print results
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            watch.Stop();
            Console.WriteLine($"Total time: {watch.ElapsedMilliseconds}ms\n");
        }

        // WhenAny approach - returns as soon as first task completes
        public async Task FetchFirstUserAvailableAsync(int numberOfUsers = 3)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("=== Race Approach (Task.WhenAny) ===");

            Task<string>[] tasks = new Task<string>[numberOfUsers];
            for (int i = 1; i <= numberOfUsers; i++)
            {
                tasks[i - 1] = SimulateNetworkRequestAsync(i);
            }

            // Returns immediately when first task completes
            Task<string> firstCompleted = await Task.WhenAny(tasks);
            Console.WriteLine($"First result: {await firstCompleted}");

            watch.Stop();
            Console.WriteLine($"Total time: {watch.ElapsedMilliseconds}ms\n");
        }

        public async Task Main()
        {
            await FetchUsersSequentialAsync();
            await FetchUsersParallelAsync();
            // Task.WhenAny() is fast but only returns one result because Task.WhenAny() returns as soon as the first task completes. 
            await FetchFirstUserAvailableAsync();
        }
    }
}
