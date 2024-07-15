namespace Migration.Toolkit.Common.Helpers;

using System.Runtime.InteropServices;

public static class ConsoleHelper
{
    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    public static void EnableVirtualTerminalProcessing()
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        uint mode;
        GetConsoleMode(handle, out mode);
        mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        SetConsoleMode(handle, mode);
    }

    public const string RED = "\x1b[31m";
    public const string YELLOW = "\x1b[33m";
    public const string GREEN = "\x1b[32m";
    public const string RESET = "\x1b[0m";

    public static string Yellow(string ctext) => $"{YELLOW}{ctext}{RESET}";
    public static string Green(string ctext) => $"{GREEN}{ctext}{RESET}";
    public static string Red(string ctext) => $"{RED}{ctext}{RESET}";

}