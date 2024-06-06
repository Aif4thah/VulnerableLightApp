using GraphQL;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace VulnerableWebApplication.TestCpu
{
    public class TestCpu
    {
        public static void TestAffinity(string Str)
        {
            string BinStr =  ConvertToBinary(Str);

            Console.WriteLine("Total proc: {0}", Environment.ProcessorCount);
            foreach (char bit in BinStr)
            {
                Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)(bit - '0' +1);
                CalculateSHA512(Str);
            }
            Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)5;
        }

        public static string ConvertToBinary(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            StringBuilder binary = new StringBuilder();

            foreach (byte b in bytes) binary.Append(Convert.ToString(b,2).PadLeft(8,'0'));

            return binary.ToString();
        }

        public static void CalculateSHA512(string input)
        {
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                while (stopWatch.Elapsed.TotalSeconds < 5)
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var bytes = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                        var hash = sha256.ComputeHash(bytes);
                    }
                }
                stopWatch.Stop();
            }
        }

    }
}
