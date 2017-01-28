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
        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            string filepath = args[0];

            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                byte[] fileBytes = File.ReadAllBytes(filepath);

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                DepotManifest depotManifest = null;
                try
                {
                    // DepotManifest constructor is internal, so use Activator to call it
                    depotManifest = (DepotManifest)Activator.CreateInstance(typeof(DepotManifest), flags, null, new object[] { fileBytes }, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something bad happened! Exception message:\r\n{0}",ex.Message);
                }
                
                if(depotManifest != null)
                {
                    foreach (DepotManifest.FileData filedata in depotManifest.Files)
                    {
                        bool isDir = filedata.Flags == EDepotFileFlag.Directory;
                        string strHash = filedata.FileHash.Aggregate(new StringBuilder(), (sb, v) => sb.Append(v.ToString("x2"))).ToString().ToUpper();
                        Console.WriteLine("{0} {1}", filedata.FileName, isDir ? "" : strHash);
                    }
                }

                Console.Read();
            }
        }
    }
}
