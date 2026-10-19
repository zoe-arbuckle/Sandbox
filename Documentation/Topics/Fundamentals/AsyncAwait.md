# Async/Await
## References
- Github Copilot

## Notes
### Basics of Async/Await
**Async/await** is used to write C# code that looks and reads like synchronous code. 

- Synchronous code: Executes line-by-line.
- Asynchronous code: Can start a long-running operation and move on to other work while waiting. Execution resumes when the operation completes.

`async` keyword marks a method that can perform asynchronous operations

`await` keyword pauses execution at that point until an asynchronous operation completes

`Task<T>` and `Task` represent future results of async work

```csharp
// Synchronous version (BLOCKING)
public string FetchDataSync()
{
    System.Threading.Thread.Sleep(2000); // Simulates network delay
    return "Data fetched!";
}

public void SyncApproach()
{
    Console.WriteLine("Starting fetch...");
    string result = FetchDataSync(); // Program waits here for 2 seconds
    Console.WriteLine(result);
    Console.WriteLine("Done!");
}

// Asynchronous version (NON-BLOCKING)
public async Task<string> FetchDataAsync()
{
    await Task.Delay(2000); // Simulates network delay
    return "Data fetched!";
}

public async Task AsyncApproach()
{
    Console.WriteLine("Starting fetch...");
    string result = await FetchDataAsync(); // Waits, but thread is free for other work
    Console.WriteLine(result);
    Console.WriteLine("Done!");
}

// Usage
public static async Task Main(string[] args)
{
    await new Program().AsyncApproach();
}
```
#### Common Mistakes
- Forgetting `await` - if task is created but not awaited, the method will return immediately without waiting
- Blocking async code with `.Result()` or `.Wait()` - this defeats the purpose of asynchronous code and blocks the thread
- `async void` (except for event handlers) - hard to track when `async void` methods complete, and exceptions are hard to catch. It is 
 best to use `async Task` or `async Task<T>`

### Parallel Async Operations with `Task.WhenAll()` and `Task.WhenAny()`

When you have multiple async operations, they can be run either sequentially or parallelly. 

Sequential: one after another. Simple, but slower. Total time is Task 1 + Task 2 + Task 3.

Parallel: all at once. Much faster for I/O bound opterations. Tasks start simultaneously, so total time is longest task duration.

`Task.WhenAll()` - wait for all tasks to complete
`Task.WhenAny()` - wait for the first task to complete

```csharp
// Sequential: Takes ~4500ms (1500 + 1500 + 1500)
public async Task SequentialFetch()
{
    await FetchUserAsync(1);  // 1500ms
    await FetchUserAsync(2);  // 1500ms
    await FetchUserAsync(3);  // 1500ms
}

// Parallel with WhenAll: Takes ~1500ms (all run simultaneously)
public async Task ParallelFetch()
{
    Task task1 = FetchUserAsync(1);
    Task task2 = FetchUserAsync(2);
    Task task3 = FetchUserAsync(3);

    await Task.WhenAll(task1, task2, task3);
}

// Parallel with WhenAny: Takes ~1500ms (stops at first completion)
public async Task FirstAvailableFetch()
{
    Task task1 = FetchUserAsync(1);
    Task task2 = FetchUserAsync(2);
    Task task3 = FetchUserAsync(3);

    Task firstCompleted = await Task.WhenAny(task1, task2, task3);
    // Continue with just the first result
}
```

#### Common Mistakes
- awaiting in the loop - this kills parallelism
- not handling exceptions in `Task.WhenAll()`
- forgetting to await the result from `Task.WhenAny()`

### Exception Handling in Async Code
Exception handling in asynchronous code looks similar to synchronous code, but with some critical differences.

- Single exception: thrown normally from an async method, caught in `catch`
```csharp
try
{
    await FetchUserAsync(1);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```
- Multiple exceptions: from `Task.WhenAll()`, multiple failures are wrapped in `AggregateException`. When there is only one failure, 
the original exception type is rethrown.
```csharp
try
{
    Task[] tasks = new[] {
        FetchUserAsync(1),
        FetchUserAsync(2), // Fails
        FetchUserAsync(3)  // Fails
    };
    await Task.WhenAll(tasks);
}
catch (AggregateException ex)
{
    foreach (var innerEx in ex.InnerExceptions)
    {
        Console.WriteLine($"Error: {innerEx.Message}");
    }
}
```
- `Task.WhenAny()`: Only throws if the winning task fails
- `finally` blocks: always execute, even in async code
- Hidden exceptions: if a task is not awaited, exceptions won't be caught

#### Common Mistakes
- Catching `HttpRequestException` instead of `AggregateException`
- Not awaiting, so exceptions are lost
- Forgetting async operations can throw in `finally` - async code in `finally` should be wrapped as well

### `ConfigureAwait()` and Synchronization Context
Synchronization context is C#'s way of managing which thread executes code after an `await`. This is critical for performance
and avoiding deadlocks.

After `await`, where does execution resume?

By default, async methods try to return to the original context, eg. If called from the UI thread, it returns to the UI thread after await. This is useful for UI updates but adds overhead.

`ConfigureAwait()` lets you control this behavior: 
- `ConfigureAwait(true)` is the default - tries to return the original context
```csharp
public async Task<string> FetchDataAsync()
{
    // After this await, execution returns to the calling context
    string data = await httpClient.GetStringAsync("...");
    return data;
}
```
- `ConfigureAwait(false)` ignores the context, and uses any thread from the thread pool
```csharp
public async Task<string> FetchDataAsync()
{
    // After this await, don't bother returning to original context
    string data = await httpClient.GetStringAsync("...").ConfigureAwait(false);
    return data;
}
```

This matters because the UI thread is precious and limited, and should be released for other work if not needed. 
This improves performance in library code and prevents deadlocks

Rule of thumb:
- *Library code (methods others will call)*: Use `ConfigureAwait(false)`
- *UI Code (updating UI controls)*: Use `ConfigureAwait(true)` or omit
- *Server/console code*: Usually use `ConfigureAwait(false)`

#### Common Mistakes
- Blocking on async code, causing a deadlock
- Using `ConfigureAwait(true)` in library code - this is wrong because you don't control the caller
- Mixing contexts in a chain - pick one approach

### Advanced Async Patterns
#### `ValueTask` vs `Task`
`Task` is a class, and therefore always allocates memory on every call. It is better for long-running operations.

`ValueTask` is a struct, and therefore has zero allocation if the operation completes synchronously. This is great for 
cache lookups and validation checks, and should be used when most calls complete immediately.

```csharp
public ValueTask<User> GetUserAsync(int id)
{
    if (_cache.TryGetValue(id, out var user))
    {
        return new ValueTask<User>(user); // Sync, no allocation
    }
    return new ValueTask<User>(FetchFromDatabaseAsync(id)); // Async
}
```

#### Async Streams (`IAsyncEnumerable<T>`)
Instead of waiting for all data then returning a list, return data as it becomes available.
```csharp
// Old: Wait for all, then return
async Task<List<User>> GetAllUsers() 
{
    // Must wait for all 1000 users... slow!
    return allUsers;
}

// New: Stream one at a time
async IAsyncEnumerable<User> GetAllUsers()
{
    foreach (var user in users)
        yield return user; // Return immediately as available
}

// Usage
await foreach (var user in GetAllUsers()) // Process as they arrive
{
    Console.WriteLine(user);
}
```

#### `CancellationToken`
`CancellationToken` allows you to gracefully stop aysnc operations. This is critical for timeouts, user-initiated cancellation,
and cleanup before shutdown.
```csharp
public async Task FetchDataAsync(CancellationToken ct)
{
    await Task.Delay(2000, ct); // Can be cancelled
}

// Usage - timeout after 1 second
using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
{
    await FetchDataAsync(cts.Token);
}
```

#### Common Mistakes
- Using `Task` for cached lookups
- Blocking on async streams
- Ignoring `CancellationToken`
- Not disposing `CancellationTokenSource`
