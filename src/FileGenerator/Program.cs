using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;

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
                uint hundredThousand = 100000;
                uint numOfFiles = Convert.ToUInt32(args[1]);
                List<Thread> threads= new List<Thread>();
                if(numOfFiles < hundredThousand)
                {
                    threads.Add(CreateFiles("0", basePath, numOfFiles));
                }
                else
                {
                    int i = 0;
                    while(numOfFiles > hundredThousand)
                    {
                        threads.Add(CreateFiles(i++.ToString(), basePath, hundredThousand));
                        numOfFiles -= hundredThousand;
                    }
                    threads.Add(CreateFiles(i++.ToString(), basePath, numOfFiles));
                }

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
