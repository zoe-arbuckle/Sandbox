namespace AsyncAwait
{
    public sealed class Lesson4_ConfigureAwaitAndContext
    {
        private static int _syncContextCallCount = 0;

        // Simulates a server API call
        public async Task<string> FetchUserAsync(int userId)
        {
            await Task.Delay(500);
            return $"User {userId}";
        }

        // Example 1: Without ConfigureAwait - captures synchronization context
        public async Task<string> FetchWithContextAsync(int userId)
        {
            Console.WriteLine($"[FetchWithContext] Thread: {Thread.CurrentThread.ManagedThreadId}, Context: {SynchronizationContext.Current?.GetType().Name ?? "None"}");
            
            string result = await FetchUserAsync(userId);
            
            // After await, execution returns to the original context
            Console.WriteLine($"[FetchWithContext] After await - Thread: {Thread.CurrentThread.ManagedThreadId}");
            
            return result;
        }

        // Example 2: With ConfigureAwait(false) - doesn't capture context
        public async Task<string> FetchWithoutContextAsync(int userId)
        {
            Console.WriteLine($"[FetchWithoutContext] Thread: {Thread.CurrentThread.ManagedThreadId}, Context: {SynchronizationContext.Current?.GetType().Name ?? "None"}");
            
            // ConfigureAwait(false) tells the runtime: "I don't need to return to the original context"
            string result = await FetchUserAsync(userId).ConfigureAwait(false);
            
            // After await, execution might be on a different thread
            Console.WriteLine($"[FetchWithoutContext] After await - Thread: {Thread.CurrentThread.ManagedThreadId}");
            
            return result;
        }

        // Example 3: Demonstrating potential deadlock without ConfigureAwait
        public string FetchUserSynchronously_BadPractice(int userId)
        {
            // WARNING: This demonstrates a DEADLOCK scenario - DO NOT use in production!
            // Blocking the UI thread on an async operation that needs the UI thread = deadlock
            
            Console.WriteLine("=== Deadlock Demo (Bad Practice) ===");
            Console.WriteLine("Calling async method from sync context and blocking with .Result...\n");
            
            try
            {
                // This can deadlock if FetchUserAsync tries to marshal back to the UI thread
                // The UI thread is blocked waiting for the result,
                // but the async operation needs the UI thread to complete
                string result = FetchWithContextAsync(1).Result;
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // Example 4: ConfigureAwait for library code (best practice)
        public async Task<string> LibraryMethodAsync(int userId)
        {
            Console.WriteLine("=== Library Method (Best Practice) ===");
            
            // Library code should use ConfigureAwait(false)
            // because it doesn't know the caller's context
            string user = await FetchUserAsync(userId).ConfigureAwait(false);
            string details = await FetchUserDetailsAsync(userId).ConfigureAwait(false);
            
            return $"{user}: {details}";
        }

        public async Task<string> FetchUserDetailsAsync(int userId)
        {
            await Task.Delay(300);
            return $"Details for user {userId}";
        }

        // Example 5: UI Context Scenario (conceptual - UI framework specific)
        public async Task UiContextExampleAsync()
        {
            Console.WriteLine("=== UI Context Example (Conceptual) ===");
            Console.WriteLine("Current thread: " + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Current context: " + (SynchronizationContext.Current?.GetType().Name ?? "None"));
            
            // Simulating UI framework context
            await Task.Delay(200).ConfigureAwait(true); // true = return to context (default behavior)
            Console.WriteLine("After ConfigureAwait(true) - Back on original thread: " + Thread.CurrentThread.ManagedThreadId);
            
            await Task.Delay(200).ConfigureAwait(false); // false = don't return to context
            Console.WriteLine("After ConfigureAwait(false) - Might be different thread: " + Thread.CurrentThread.ManagedThreadId);
        }

        // Example 6: ConfigureAwait in a chain
        public async Task<string> ChainedAsyncWithConfigureAwaitAsync(int userId)
        {
            Console.WriteLine("=== Chained Async Calls with ConfigureAwait ===");
            
            // Each await should have ConfigureAwait if you don't need context
            var user = await FetchUserAsync(userId).ConfigureAwait(false);
            var details = await FetchUserDetailsAsync(userId).ConfigureAwait(false);
            
            // Return values - no context needed
            return $"{user} - {details}";
        }

        // Example 7: When to use ConfigureAwait(true) - UI updates
        public async Task UpdateUiAsync(int userId)
        {
            Console.WriteLine("=== When ConfigureAwait(true) is needed ===");
            Console.WriteLine("Need to return to UI context to update UI controls");
            
            // If you NEED to return to the UI context (to update UI controls)
            string userData = await FetchUserAsync(userId).ConfigureAwait(true);
            
            // Now we're back on the UI thread, safe to update UI controls
            // (In real code: textBox.Text = userData;)
            Console.WriteLine($"Updating UI with: {userData}");
        }
    }
}
