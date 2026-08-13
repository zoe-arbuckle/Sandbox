namespace Common
{
    public static class CustomConsoleLogs
    {
        public static void Section(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"-- {title} ----------------");
            Console.ResetColor();
        }
    }
}
