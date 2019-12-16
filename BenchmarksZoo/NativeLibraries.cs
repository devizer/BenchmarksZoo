using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters;

namespace BenchmarksZoo
{
    public class NativeLibraries
    {
        private static bool IsWindows => System.Environment.OSVersion.Platform == PlatformID.Win32NT;

        public static List<string> Get()
        {
            return Get(Process.GetCurrentProcess().Id.ToString());
        }
        
        public static List<string> Get(int processId)
        {
            return Get(processId.ToString());
        }

        public static List<string> Get(string processesList)
        {
            List<string> ret = new List<string>();
            var filesAndFolders = GetOpenedFilesAndFolders(processesList);
            foreach (var item in filesAndFolders)
            {
                // TODO: is Symlink?
                if (File.Exists(item))
                    ret.Add(item);
            }

            return ret;
        }


        public static List<string> GetOpenedFilesAndFolders(string processesList)
        {
            List<string> ret = new List<string>();
            if (IsWindows) return ret;

            ProcessStartInfo si = new ProcessStartInfo("sudo", $"lsof -p {processesList}");
            var home = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
            {
                si.WorkingDirectory = home;
            }
            si.RedirectStandardOutput = true;
            si.UseShellExecute = false;
            
            Process p = Process.Start(si);
            using (p)
            {
                p.WaitForExit();
                var output = p.StandardOutput.ReadToEnd();
                var arr = output.Split(new[] {'\r', '\n'});
                bool isFirst = true;
                foreach (var line in arr)
                {
                    if (line.Length == 0) continue;
                    if (isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    int pos = line.IndexOf("/");
                    if (pos < 0) continue;
                    string file = line.Substring(pos);
                    ret.Add(file);
                }
            }

            return ret;
        }



    }
}