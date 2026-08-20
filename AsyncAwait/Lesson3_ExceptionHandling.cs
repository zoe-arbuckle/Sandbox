namespace AsyncAwait
{
    public sealed class Lesson3_ExceptionHandling
    {
        // Simulates an async operation that might fail
        public async Task<string> FetchUserDataAsync(int userId, bool shouldFail = false)
        {
            await Task.Delay(1000);

            if (shouldFail)
            {
                throw new HttpRequestException($"Failed to fetch user {userId}");
            }

            return $"User {userId} data retrieved successfully";
        }

        // Example 1: Single exception handling
        public async Task HandleSingleExceptionAsync()
        {
            Console.WriteLine("=== Single Exception Handling ===");

            try
            {
                string result = await FetchUserDataAsync(1, shouldFail: true);
                Console.WriteLine(result);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error caught: {ex.Message}");
            }

            Console.WriteLine();
        }

        // Example 2: Multiple async operations with WhenAll - AggregateException
        public async Task HandleMultipleExceptionsAsync()
        {
            Console.WriteLine("=== Multiple Exceptions (Task.WhenAll) ===");

            try
            {
                Task<string>[] tasks = new Task<string>[]
                {
                    FetchUserDataAsync(1, shouldFail: false),
                    FetchUserDataAsync(2, shouldFail: true),  // This will fail
                    FetchUserDataAsync(3, shouldFail: true)   // This will fail
                };

                string[] results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            catch (AggregateException aggEx)
            {
                Console.WriteLine($"Aggregate Exception caught with {aggEx.InnerExceptions.Count} errors:");
                foreach (var ex in aggEx.InnerExceptions)
                {
                    Console.WriteLine($"  - {ex.Message}");
                }
            }
            catch (HttpRequestException ex)
            {
                // This catches individual exceptions, not aggregate
                Console.WriteLine($"Single exception: {ex.Message}");
            }

            Console.WriteLine();
        }

        // Example 3: Partial failure handling with WhenAll
        public async Task HandlePartialFailureAsync()
        {
            Console.WriteLine("=== Partial Failure with Continuation ===");

            Task<string>[] tasks = new Task<string>[]
            {
                FetchUserDataAsync(1, shouldFail: false),
                FetchUserDataAsync(2, shouldFail: true),
                FetchUserDataAsync(3, shouldFail: false)
            };

            // Use ContinueWith to handle failures gracefully
            Task<string[]> allTasksWithFallback = Task.WhenAll(tasks)
                .ContinueWith(async t =>
                {
                    if (t.IsFaulted)
                    {
                        Console.WriteLine("Some tasks failed, attempting recovery...");
                        // Return partial results or default values
                        return new[] { "Default 1", "Default 2", "Default 3" };
                    }
                    return await t;
                }).Unwrap();

            try
            {
                string[] results = await allTasksWithFallback;
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        }

        // Example 4: Finally with async
        public async Task FinallyWithAsyncAsync()
        {
            Console.WriteLine("=== Finally Block with Async ===");

            try
            {
                await FetchUserDataAsync(1, shouldFail: true);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Caught: {ex.Message}");
            }
            finally
            {
                // Cleanup code always runs
                Console.WriteLine("Cleanup: Logging operation result");
                await Task.Delay(500); // Can have async operations in finally
                Console.WriteLine("Cleanup complete");
            }

            Console.WriteLine();
        }

        public async Task<bool> DemonstrateExceptionPropagationAsync()
        {
            bool success = true;
            try
            {
                Task<string>[] tasks = new Task<string>[]
                {
                    FetchUserDataAsync(1, shouldFail: false),
                    FetchUserDataAsync(2, shouldFail: true),  // This will fail
                    FetchUserDataAsync(3, shouldFail: false)
                };

                string[] results = await Task.WhenAll(tasks);
            }
            catch (AggregateException aggEx)
            {
                Console.WriteLine($"Operation failed with {aggEx.InnerExceptions.Count} error(s): {string.Join(", ", aggEx.InnerExceptions.Select(e => e.Message))}");
                success = false;
            }
            catch (HttpRequestException ex)
            {
                // This catches individual exceptions, not aggregate
                Console.WriteLine($"Operation failed with 1 error(s): {ex.Message}");
                success = false;
            }
            return success;
        }

        public async Task Main()
        {
            await HandleSingleExceptionAsync();
            await HandleMultipleExceptionsAsync();
            await HandlePartialFailureAsync();
            await FinallyWithAsyncAsync();

            var success = await DemonstrateExceptionPropagationAsync();
            Console.WriteLine($"Method success: {success}");
        }
    }
}
