using IO;

namespace ToKorean
{
    internal static class Program
    {

        private static ILogManager Log { get { return LogManager.Instance; } }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Log.Logging("Program", "Main", "sta >");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}