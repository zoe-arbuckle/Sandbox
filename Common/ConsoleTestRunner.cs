namespace Common
{
    public static class ConsoleTestRunner
    {
        public static void Section(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"-- {title} ----------------");
            Console.ResetColor();
        }

        public static void Pass(string label)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {label}");
            Console.ResetColor();
        }

        public static void Fail(string label, string reason)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {label}: {reason}");
            Console.ResetColor();
        }

        public static void Run(string label, Action test)
        {
            try { test(); }
            catch (Exception ex) { Fail(label, ex.Message); }
        }
    }
}
