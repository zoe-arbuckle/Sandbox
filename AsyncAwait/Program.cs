using AsyncAwait;

/*
 * =====================================
 * Basics of Async/Await
 * =====================================
 */
Common.CustomConsoleLogs.Section("Basics of Async/Await");
Lesson1_AsyncAwaitBasics basics = new Lesson1_AsyncAwaitBasics();
await basics.FetchMultipleUsersSequentialAsync();