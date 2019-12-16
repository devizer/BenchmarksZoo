using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            if (IsWindows) return ret;

            Console.WriteLine($"NATIVE LIBRARIES:");
            ProcessStartInfo si = new ProcessStartInfo("sudo", $"lsof -p {processesList}");
            var home = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
            {
                si.WorkingDirectory = home;
            }
            si.RedirectStandardOutput = true;
            
            Process p = Process.Start(si);
            using (p)
            {
                p.WaitForExit();
                var output = p.StandardOutput.ReadToEnd();
                var arr = output.Split(new[] {'\r', '\n'});
                foreach (var line in arr)
                {
                    if (line.Length == 0) continue;
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