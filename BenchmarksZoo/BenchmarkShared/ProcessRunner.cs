using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BenchmarksShared
{
    public class ProcessRunner
    {
        public class ProcessResult
        {
            public Exception Exception { get; internal set; }
            public string Output { get; internal set; }
            public Exception OutputException { get; internal set; }
            public string Error { get; internal set; }
            public Exception ErrorException { get; internal set; }
            public int ExitCode { get; internal set; }
        }

        public static ProcessResult HiddenExec(string command, string args)
        {
            ProcessStartInfo si = new ProcessStartInfo(command, args)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                // WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
            };

            Process p = new Process()
            {
                StartInfo = si,
            };

            // error is written to output xml
            ManualResetEvent outputDone = new ManualResetEvent(false);
            ManualResetEvent errorDone = new ManualResetEvent(false);

            string my_output = null;
            string my_error = null;
            Exception my_outputException = null;
            Exception my_errorException = null;

            Thread t1 = new Thread(() =>
                {
                    try
                    {
                        my_error = p.StandardError.ReadToEnd();
                        // my_error = DumpToEnd(p.StandardError).ToString();
                    }
                    catch (Exception ex)
                    {
                        my_errorException = ex;
                    }
                    finally
                    {
                        errorDone.Set();
                    }
                }
#if !NETCOREAPP && !NETSTANDARD
                , 64 * 1024
#endif
            ) {IsBackground = true};

            Thread t2 = new Thread(() =>
                    {
                        try
                        {
                            my_output = p.StandardOutput.ReadToEnd();
                            // my_output = DumpToEnd(p.StandardOutput).ToString();  
                        }
                        catch (Exception ex)
                        {
                            my_outputException = ex;
                        }
                        finally
                        {
                            outputDone.Set();
                        }
                    }
#if !NETCOREAPP && !NETSTANDARD
                    , 64 * 1024
#endif
                )
                {IsBackground = true};

            using (p)
            {
                try
                {
                    p.Start();
                }
                catch (Exception ex)
                {
                    return new ProcessResult() {Exception = ex, ExitCode = -1};
                }

                t2.Start();
                t1.Start();
                errorDone.WaitOne();
                outputDone.WaitOne();
                p.WaitForExit();

                return new ProcessResult()
                {
                    ExitCode = p.ExitCode,
                    Error = my_error,
                    Output = my_output,
                    Exception = null,
                    ErrorException = my_errorException,
                    OutputException = my_outputException,
                };
            }
        }
    }
}