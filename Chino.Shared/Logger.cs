using System.Diagnostics;

namespace Chino
{
    public class Logger
    {
        private Logger() { }

        public static void D(string message)
        {
#if DEBUG
            Debug.Print(message);
#endif
        }

        public static void I(string message)
        {
            Debug.Print(message);
        }

        public static void E(string message)
        {
            Debug.Print(message);
        }
    }
}
