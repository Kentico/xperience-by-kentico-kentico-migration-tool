namespace Migration.Toolkit.TestConsole;

using System.Runtime.InteropServices;

public class ConsoleHelper
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
}