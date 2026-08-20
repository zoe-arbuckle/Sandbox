namespace AsyncAwait
{
    public sealed class Lesson5_AdvancedPatterns
    {
        // ===== ValueTask vs Task =====
        // ValueTask: Struct-based, stack-allocated, zero-heap allocation if completed synchronously
        // Task: Class-based, heap-allocated, always allocates memory

        // Example 1: Using Task (heap allocation)
        public async Task<string> FetchUserWithTaskAsync(int userId)
        {
            await Task.Delay(100);
            return $"User {userId}";
        }

        // Example 2: Using ValueTask (better if often completes synchronously)
        public async ValueTask<string> FetchUserWithValueTaskAsync(int userId)
        {
            await Task.Delay(100);
            return $"User {userId}";
        }

        // Example 3: Synchronous completion with ValueTask (no allocation!)
        private static readonly Dictionary<int, string> _userCache = new()
        {
            { 1, "User 1" },
            { 2, "User 2" },
            { 3, "User 3" }
        };

        public ValueTask<string> GetCachedUserAsync(int userId)
        {
            // If found in cache, returns immediately WITHOUT allocating a Task
            if (_userCache.TryGetValue(userId, out var user))
            {
                return new ValueTask<string>(user); // No async, no allocation
            }

            // If not cached, fall back to async operation
            return new ValueTask<string>(FetchFromDatabaseAsync(userId));
        }

        private async Task<string> FetchFromDatabaseAsync(int userId)
        {
            await Task.Delay(200);
            return $"User {userId} from DB";
        }

        // ===== Async Streams (IAsyncEnumerable) =====
        // Like IEnumerable but async - stream data as it becomes available

        // Example 4: Old way - collect all data then return
        public async Task<List<string>> FetchAllUsersOldWayAsync(int count)
        {
            var users = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                await Task.Delay(200); // Simulating network delay
                users.Add($"User {i}");
            }
            return users; // Return all at once after waiting for all
        }

        // Example 5: New way - stream data as it arrives
        public async IAsyncEnumerable<string> FetchAllUsersAsyncStreamAsync(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                await Task.Delay(200); // Simulating network delay
                yield return $"User {i}"; // Return one at a time as it's ready
            }
        }

        // ===== CancellationToken - Stop async operations gracefully =====

        // Example 6: Async method with cancellation support
        public async Task<string> FetchUserWithCancellationAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                // Pass cancellation token to Task.Delay
                await Task.Delay(2000, cancellationToken);
                return $"User {userId}";
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Fetch for user {userId} was cancelled");
                throw;
            }
        }

        // Example 7: Multiple operations with shared cancellation token
        public async Task FetchMultipleUsersWithCancellationAsync(int userCount, CancellationToken cancellationToken)
        {
            try
            {
                Task[] tasks = new Task[userCount];
                for (int i = 1; i <= userCount; i++)
                {
                    tasks[i - 1] = FetchUserWithCancellationAsync(i, cancellationToken);
                }

                await Task.WhenAll(tasks);
                Console.WriteLine("All users fetched successfully");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled");
            }
        }

        // Example 8: Timeout using CancellationToken
        public async Task<string> FetchUserWithTimeoutAsync(int userId, int timeoutMs = 1000)
        {
            using (var cts = new CancellationTokenSource(timeoutMs))
            {
                try
                {
                    return await FetchUserWithCancellationAsync(userId, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Operation timed out after {timeoutMs}ms");
                    throw;
                }
            }
        }

        // ===== Async patterns and best practices =====

        // Example 9: Properly disposing async resources
        public async Task DemoAsyncResourceDisposalAsync()
        {
            // Simulating an async resource that needs cleanup
            await using (var resource = new AsyncResource())
            {
                await resource.InitializeAsync();
                string data = await resource.FetchDataAsync();
                Console.WriteLine($"Data: {data}");
            } // Cleanup happens automatically via IAsyncDisposable
        }

        // Mock async disposable resource
        private class AsyncResource : IAsyncDisposable
        {
            public async Task InitializeAsync()
            {
                await Task.Delay(100);
                Console.WriteLine("Resource initialized");
            }

            public async Task<string> FetchDataAsync()
            {
                await Task.Delay(100);
                return "Sample data";
            }

            public async ValueTask DisposeAsync()
            {
                await Task.Delay(50);
                Console.WriteLine("Resource cleaned up");
            }
        }

        // Example 10: Comparison of patterns
        public async Task DemonstratePatternsAsync()
        {
            Console.WriteLine("=== ValueTask: Cached lookup ===");
            var user1 = await GetCachedUserAsync(1); // No allocation, synchronous
            Console.WriteLine(user1);

            Console.WriteLine("\n=== ValueTask: Non-cached lookup ===");
            var user99 = await GetCachedUserAsync(99); // Allocation, async
            Console.WriteLine(user99);

            Console.WriteLine("\n=== Async Streams ===");
            await foreach (var user in FetchAllUsersAsyncStreamAsync(3))
            {
                Console.WriteLine($"Received: {user}");
            }

            Console.WriteLine("\n=== Timeout Demo ===");
            try
            {
                await FetchUserWithTimeoutAsync(1, timeoutMs: 500);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Timeout occurred as expected");
            }

            Console.WriteLine("\n=== Async Resource Disposal ===");
            await DemoAsyncResourceDisposalAsync();
        }

        public async Task Main()
        {
            await GetCachedUserAsync(1);
            await GetCachedUserAsync(99);
            Console.WriteLine("GetCachedUserAsync(99) allocated memory because the value was not cached and could not complete synchronously.");

            await foreach (var user in FetchAllUsersAsyncStreamAsync(3))
                Console.WriteLine($"Fetched user: {user}");
            Console.WriteLine("Async streams return information as it comes through, rather than waiting for all data to be available.");

            try
            {
                await FetchUserWithTimeoutAsync(1, timeoutMs: 500);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Timeouts are important to prevent long-running tasks from never stopping, or exceeding time limits that could affect user experience for example.");

            await DemonstratePatternsAsync();
        }
    }
}
