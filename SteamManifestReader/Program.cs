using SteamKit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SteamManifestReader
{
    class Program
    {
        static string inputFile = string.Empty;
        static string outputFile = string.Empty;

        static void Main(string[] args)
        {
            if (args.Length > 0 && args.Length < 3)
            {
                // Arg 1
                inputFile = args[0];

                // Arg 2
                if (args.Length > 1)
                    outputFile = args[1];

                // Manifest Process
                HandleManifest();
            }
            else
            {
                Console.WriteLine("Usage: SteamManifestReader.exe <input.manifest> [fciv_out.xml]");
                return;
            }
#if DEBUG
            Console.ReadLine();
#endif
        }

        private static void HandleManifest()
        {
            if (!string.IsNullOrEmpty(inputFile) && File.Exists(inputFile))
            {
                byte[] fileBytes = File.ReadAllBytes(inputFile);

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                DepotManifest depotManifest = null;
                try
                {
                    // DepotManifest constructor is internal, so use Activator to call it
                    depotManifest = (DepotManifest)Activator.CreateInstance(typeof(DepotManifest), flags, null, new object[] { fileBytes }, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something bad happened! Exception message:\r\n{0}", ex.Message);
                }

                if (depotManifest != null)
                {
                    FCIVExport exporter = new FCIVExport(outputFile);
                    foreach (DepotManifest.FileData filedata in depotManifest.Files)
                    {
                        bool isDir = filedata.Flags == EDepotFileFlag.Directory;
                        string strHash = filedata.FileHash.Aggregate(new StringBuilder(), (sb, v) => sb.Append(v.ToString("x2"))).ToString().ToUpper();
                        Console.WriteLine("{0} {1}", filedata.FileName, isDir ? "" : strHash);

                        if (!isDir)
                        {
                            exporter.AddEntry(filedata.FileName, strHash);
                        }
                    }
                    exporter.Finalize();
                }
            }
            else
            {
                Console.WriteLine("File {0} doesn't exists!", inputFile);
            }

        }
    }
}
