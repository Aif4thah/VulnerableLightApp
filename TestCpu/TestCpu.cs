using GraphQL;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace VulnerableWebApplication.TestCpu
{
    /*
     Test de perfs sur un seul proc (compatible Windows et Linux uniquement) 
     */
    public class TestCpu
    {
        public static void TestAffinity()
        {
            byte[] bytes = File.ReadAllBytes("appsettings.json");
            StringBuilder binary = new StringBuilder();
            var sha256 = SHA256.Create();
            foreach (byte b in bytes) binary.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            string BinStr = binary.ToString();

            Console.WriteLine("Total proc: {0}", Environment.ProcessorCount);
            foreach (char bit in BinStr)
            {
                Thread.Sleep(1000);
                if (bit == '0') Thread.Sleep(4000);
                else
                {
                    Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)(bit - '0' + 1);
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    while (stopWatch.Elapsed.TotalSeconds < 4) sha256.ComputeHash(bytes);
                    stopWatch.Stop();
                }
            }
            Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)5;
        }

    }
}
