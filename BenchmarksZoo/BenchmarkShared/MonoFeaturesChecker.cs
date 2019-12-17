using System;
using System.IO;

namespace BenchmarksShared
{
    public class MonoFeaturesChecker
    {
        private const string CheckMonoFeaturesShell = @"
dir=""$(mktemp -d)"";
cd ""$dir"" >/dev/null;
echo 'class Z { public static void Main() { System.Console.WriteLine(""SUCCESSFUL COMPILATION""); }}' > class1.cs;
{csc} class1.cs > csc.log 2>&1
{mono} {mono-options} class1.exe;
code=$?;
# echo ""exit code: $code"";
cd ""$HOME"" >/dev/null
rm -rf ""$dir"" 2>/dev/null
exit $code

";

        // private string tmp = @"";

        public static bool IsSupported(string monoBinPath, string arguments)
        {
            var csc = string.IsNullOrEmpty(monoBinPath) ? "csc" : Path.Combine(monoBinPath, "csc");
            var mono = string.IsNullOrEmpty(monoBinPath) ? "mono" : Path.Combine(monoBinPath, "mono");
            var shellCommand = CheckMonoFeaturesShell
                .Replace("{csc}", csc)
                .Replace("{mono}", mono)
                .Replace("{mono-options}", arguments);

            var shellCommandEscaped = shellCommand
                /* .Replace(Environment.NewLine, " ") */
                .Replace("\"", "\\\"");
            
            var result = ProcessRunner.HiddenExec("sh", $"-c \"{shellCommandEscaped}\"");
            return result.ExitCode == 0;
        }

        public static bool IsMonoSupported(string monoBinPath = null)
        {
            return IsSupported(monoBinPath, "");
        }

        public static bool IsLlvmForMonoSupported(string monoBinPath = null)
        {
            return IsSupported(monoBinPath, "--llvm");
        }
        
    }
}