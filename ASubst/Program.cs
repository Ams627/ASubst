
using System;
using System.IO;
using System.Linq;

namespace ASubst
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0 && args[0] == "--help")
                {
                    PrintUsageAndExit();
                }
                var optionsArgs = args.Where(x => x[0] == '-').SelectMany(x => x.Skip(1)).ToHashSet();
                var normalArgs = args.Where(x => x[0] != '-').ToList();

                var haveDrive = normalArgs.Any() && normalArgs.First().Length == 1 && char.IsLetter(normalArgs[0][0]);
                var drive = haveDrive ? normalArgs[0][0] : '-';

                if (normalArgs.Count == 2 && haveDrive)
                {
                    // unmap existing drive - don't care if we fail:
                    DriveOperations.UnmapDrive(drive);

                    // map new drive:
                    DriveOperations.MapDrive(drive, normalArgs[1]);
                }
                else if (optionsArgs.Contains('d') && haveDrive && normalArgs.Count == 1)
                {
                    if (!DriveOperations.UnmapDrive(drive))
                    {
                        Console.Error.WriteLine($"drive {drive} is not mapped.");
                    }
                }
                else if (optionsArgs.Contains('t') && haveDrive && normalArgs.Count == 1)
                {
                    var dir = DriveOperations.GetDriveMapping(drive);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        Console.WriteLine(dir);
                    }
                }
                else if (normalArgs.Count == 1 && haveDrive)
                {
                    DriveOperations.UnmapDrive(drive);

                    // map new drive to current directory:
                    DriveOperations.MapDrive(drive, Directory.GetCurrentDirectory());
                }
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }
        }

        private static void PrintUsageAndExit()
        {
            Console.WriteLine("asubst - reimplementation of the subst command for a modern bash/zsh environment.");
            Console.WriteLine("Usage:");
            Console.WriteLine("    asubst [options] [drive]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -d delete drive");
            Console.WriteLine("   -t print the drive for the current directory if there is one");
            Console.WriteLine();
            Console.WriteLine("Without any options, asubst takes a single parameter (a drive letter) and maps the");
            Console.WriteLine("drive to the current directory.");
            Environment.Exit(-1);
        }
    }
}
