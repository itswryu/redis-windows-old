//author: https://www.zhihu.com/question/424272611/answer/2611312760

using System.Diagnostics;
using System.ServiceProcess;

namespace RedisService
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416: Verify Platform Compatibility", Justification = "<Suspend>")]
        static void Main()
        {
            ServiceBase.Run(new RedisService());
        }
    }

    partial class RedisService : ServiceBase
    {

        private Process? process = new();

        protected override void OnStart(string[] args)
        {
            var basePath = Path.Combine(AppContext.BaseDirectory);
            var diskSymbol = basePath[..basePath.IndexOf(":")];
            var confPath = basePath.Replace(diskSymbol + ":", "/cygdrive/" + diskSymbol);
            var exeName = "redis-server.exe";
            var confName = "redis.conf";
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.Trim().Equals("SentinelService", StringComparison.OrdinalIgnoreCase))
            {
                exeName = "redis-sentinel.exe";
                confName = "sentinel.conf";
            }

            ProcessStartInfo processStartInfo = new(Path.Combine(basePath, exeName).Replace("\\", "/"), String.Format("\"{0}\"", Path.Combine(confPath, confName).Replace("\\", "/")));
            processStartInfo.WorkingDirectory = basePath;
            process = Process.Start(processStartInfo);
        }

        protected override void OnStop()
        {
            if (process != null)
            {
                process.Kill();
                process.Dispose();
            }
        }
    }

}