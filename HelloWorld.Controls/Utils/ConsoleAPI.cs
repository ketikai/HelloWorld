using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HelloWorld.Controls.Utils
{
    public static class ConsoleAPI
    {
        private static readonly object Lock = new();
        public static event Action<bool>? StateChangedEvent;

        public static bool IsOpened => GetConsoleWindow() != IntPtr.Zero;

        public static void OpenConsole()
        {
            IntPtr hwnd = GetConsoleWindow();
            if (hwnd == IntPtr.Zero)
            {
                lock (Lock)
                {
                    hwnd = GetConsoleWindow();
                    if (hwnd == IntPtr.Zero)
                    {
                        AllocConsole();
                        hwnd = GetConsoleWindow();
                        if (hwnd != IntPtr.Zero)
                        {
                            IntPtr hMenu = GetSystemMenu(hwnd, false);
                            if (hMenu != IntPtr.Zero) DeleteMenu(hMenu, MenuPosition.SC_CLOSE, MenuFlags.MF_BYCOMMAND);
                        }
                        SetConsoleCtrlHandler(null, true);
                        SetConsoleOutputCP(CP_UTF8);
                        AttachConsole(ATTACH_PARENT_PROCESS);
                        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
                        Console.SetIn(new StreamReader(Console.OpenStandardInput()));
                        StateChangedEvent?.Invoke(true);
                    }
                    else
                    {
                        SetForegroundWindow(hwnd);
                    }
                }
            }
            else
            {
                SetForegroundWindow(hwnd);
            }
        }

        public static void CloseConsole()
        {
            if (IsOpened)
            {
                lock (Lock)
                {
                    if (IsOpened)
                    {
                        FreeConsole();
                        StateChangedEvent?.Invoke(false);
                    }
                }
            }
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int processId);

        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleOutputCP(uint codePageId);

        private const uint CP_UTF8 = 65001;

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine? handlerRoutine, bool add);

        private delegate bool HandlerRoutine(uint dwControlType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hwnd, bool revert);

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr menu, uint position, uint flags);

        private static class MenuPosition
        {
             public static readonly uint SC_CLOSE = 0xF060;
        }

        private static class MenuFlags
        {
            public static readonly uint MF_BYCOMMAND = 0x00000000;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
    }
}
