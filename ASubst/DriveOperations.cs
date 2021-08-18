using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ASubst
{
    static class DriveOperations
    {
        [DllImport("kernel32.dll")]
        static extern bool DefineDosDevice(uint dwFlags, string lpDeviceName, string lpTargetPath);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

        public static bool UnmapDrive(char driveLetter)
        {
            return DefineDosDevice(2, $"{driveLetter}:", null);
        }

        public static string GetDriveMapping(char driveLetter)
        {
            var sb = new StringBuilder();
            QueryDosDevice($"{driveLetter}:", sb, 65536);
            var str = sb.ToString();
            if (str.StartsWith("\\??\\"))
            {
                return str.Substring(4).Trim();
            }
            return str.Trim();
        }

        public static void MapDrive(char driveLetter, string dir)
        {
            DefineDosDevice(0, $"{driveLetter}:", dir);
        }
    }
}
