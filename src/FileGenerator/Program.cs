using System;
using System.Threading;
using System.IO;

namespace FileGenerator
{
    public class Program
    {
        private static string CreateFile(string basePath)
        {
            var uniqueFileId = Guid.NewGuid();
            var uniqueFilePath = uniqueFileId.ToString().Replace('-', Path.DirectorySeparatorChar);
            var uniqueFilePathWithBasePath = Path.Combine(basePath, uniqueFilePath);
            var parentPath = Path.GetDirectoryName(uniqueFilePathWithBasePath);

            Directory.CreateDirectory(parentPath);
            File.WriteAllText(uniqueFilePathWithBasePath, uniqueFilePathWithBasePath);

            return uniqueFilePathWithBasePath;
        }

        private static Thread CreateFiles(string jobName, string basePath, uint fileCount)
        {
            var thread = new Thread(() => {
                for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
                {
                    var uniqueFilePath = CreateFile(basePath);

                    if (fileIndex % 1000 == 0)
                    {
                        Console.WriteLine($"{jobName}\t{fileIndex}: {uniqueFilePath}");
                    }
                }
            }); 

            thread.IsBackground = true;
            thread.Start();
            return thread;
        }

        public static int Main(string[] args)
        {
            if (args.Length != 1 || Directory.Exists(args[0]))
            {
                Console.WriteLine("ERROR: Must specify path.");
                Console.WriteLine();
                Console.WriteLine("\tUsage: FileGenerator.exe [path]");
                return 1;
            }

            try
            {
                var basePath = args[0];

                var threads = new Thread[] {
                    CreateFiles("0", basePath, 100000),
                    CreateFiles("1", basePath, 100000),
                    CreateFiles("2", basePath, 100000),
                    CreateFiles("3", basePath, 100000),
                    CreateFiles("4", basePath, 100000),
                    CreateFiles("5", basePath, 100000),
                    CreateFiles("6", basePath, 100000),
                    CreateFiles("7", basePath, 100000),
                    CreateFiles("8", basePath, 100000),
                    CreateFiles("9", basePath, 100000)
                };

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 2;
            }
        }
    }
}
