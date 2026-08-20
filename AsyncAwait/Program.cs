using AsyncAwait;

/*
 * =====================================
 * Basics of Async/Await
 * =====================================
 */
//Common.CustomConsoleLogs.Section("Basics of Async/Await");
//Lesson1_AsyncAwaitBasics basics = new Lesson1_AsyncAwaitBasics();
//await basics.FetchMultipleUsersSequentialAsync();

/*
 * =====================================
 * Parallel Async Operations with Task.WhenAll() and Task.WhenAny()
 * =====================================
 */
Common.CustomConsoleLogs.Section("Parallel Async Operations with Task.WhenAll() and Task.WhenAny()");
Lesson2_ParallelAsyncOperations parallel = new();
await parallel.Main();