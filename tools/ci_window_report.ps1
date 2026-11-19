# Used by CI when a self-test of the built app does not finish in time: prints the windows the
# process has open, with the text inside them, so a waiting message box or error dialog is named
# in the log instead of a bare "timed out". Reads window text only; changes nothing.
param([Parameter(Mandatory = $true)][int]$ProcessId)

Add-Type @"
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public static class CiWindowReport
{
    private delegate bool EnumProc(IntPtr window, IntPtr data);
    [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc callback, IntPtr data);
    [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr parent, EnumProc callback, IntPtr data);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr window, StringBuilder text, int max);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr window, StringBuilder text, int max);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);

    private static string Text(IntPtr window)
    {
        var text = new StringBuilder(2048);
        GetWindowText(window, text, text.Capacity);
        return text.ToString().Replace("\r", " ").Replace("\n", " ");
    }

    private static string Class(IntPtr window)
    {
        var text = new StringBuilder(256);
        GetClassName(window, text, text.Capacity);
        return text.ToString();
    }

    public static List<string> Of(int processId)
    {
        var lines = new List<string>();
        EnumWindows((window, data) =>
        {
            uint owner;
            GetWindowThreadProcessId(window, out owner);
            if (owner != (uint)processId || !IsWindowVisible(window)) return true;
            lines.Add("window [" + Class(window) + "] \"" + Text(window) + "\"");
            EnumChildWindows(window, (child, more) =>
            {
                string text = Text(child);
                if (text.Length > 0 && lines.Count < 120) lines.Add("    [" + Class(child) + "] " + text);
                return true;
            }, IntPtr.Zero);
            return true;
        }, IntPtr.Zero);
        return lines;
    }
}
"@

$lines = [CiWindowReport]::Of($ProcessId)
if (-not $lines -or $lines.Count -eq 0) { Write-Host "The process shows no window (it is busy or waiting without a dialog)." }
else { Write-Host "Windows the process has open:"; $lines | ForEach-Object { Write-Host "  $_" } }
