using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPerLines
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            if (args.Count() < 2)
            {
                Console.WriteLine("SplitPerLines filename numberOfLines [/dest=destPath] [/skip=linesToSkip]");
                return;
            }
            if (args.Count() > 1)
            {
                arguments = args.Skip(1)
                    .Select(m => m.Split('='))
                    .ToDictionary(m => m[0].ToLower(), v => v.Length == 1 ? "" : v[1]);

            }
            string path = args[0];
            int partSize = Convert.ToInt32(args[1]);
            string argValue = null;

            var fi = new System.IO.FileInfo(path);
            long totalSize = fi.Length;
            var fn = fi.Name;


            string destPath = fi.Directory.FullName;
            if (arguments.TryGetValue("/dest", out argValue))
                destPath = argValue;
            if (!destPath.EndsWith("\\"))
                destPath = destPath + "\\";

            int skip = 0;
            if (arguments.TryGetValue("/skip", out argValue))
                skip = Convert.ToInt32(argValue); ;





            int lines = 0;
            int part = 1;
            string sPercent = "";

            using (FileStream fsr = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bsr = new BufferedStream(fsr))
            using (StreamReader sr = new StreamReader(bsr))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines++;
                    if (lines > skip)
                    {
                        System.IO.File.AppendAllText(destPath + fn + $".{part.ToString("000")}", line + "\n");
                        if (lines % partSize == 0)
                        {
                            part++;
                        }
                    }
                    string aktPerc = ((double)bsr.Position / (double)totalSize).ToString("P2");
                    if (aktPerc != sPercent)
                    {
                        Console.WriteLine($"{sPercent} ({lines})");
                        sPercent = aktPerc;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Lines: {lines} , parts:{part}");
            Console.WriteLine("Lines: " + lines);
        }
    }
}
