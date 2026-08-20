using AsyncAwait;

/*
 * =====================================
 * Basics of Async/Await
 * =====================================
 */
Common.CustomConsoleLogs.Section("Basics of Async/Await");
Lesson1_AsyncAwaitBasics basics = new Lesson1_AsyncAwaitBasics();
await basics.FetchMultipleUsersSequentialAsync();

/*
 * =====================================
 * Parallel Async Operations with Task.WhenAll() and Task.WhenAny()
 * =====================================
 */
Common.CustomConsoleLogs.Section("Parallel Async Operations with Task.WhenAll() and Task.WhenAny()");
Lesson2_ParallelAsyncOperations parallel = new();
await parallel.Main();

/*
 * =====================================
 * Exception Handling in Async Code
 * =====================================
 */
Common.CustomConsoleLogs.Section("Exception Handling in Async Code");
Lesson3_ExceptionHandling exceptionHandling = new();
await exceptionHandling.Main();

/*
 * =====================================
 * ConfigureAwait() and Synchronization Context
 * =====================================
 */
Common.CustomConsoleLogs.Section("ConfigureAwait() and Synchronization Context");
Lesson4_ConfigureAwaitAndContext context = new();
await context.Main();

/*
 * =====================================
 * Advanced Async Patterns
 * =====================================
 */
Common.CustomConsoleLogs.Section("Advanced Async Patterns");
Lesson5_AdvancedPatterns advancedPatterns = new();
await advancedPatterns.Main();