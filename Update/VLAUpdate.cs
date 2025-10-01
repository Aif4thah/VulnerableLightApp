using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace VulnerableWebApplication.Update
{
    public static class VLAUpdate
    {
        // Windows
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

        // libc (Linux)
        [DllImport("libc", SetLastError = true)]
        static extern int mprotect(IntPtr addr, UIntPtr len, int prot);

        [DllImport("libc", SetLastError = true)]
        static extern int posix_memalign(out IntPtr memptr, UIntPtr alignment, UIntPtr size);

        [DllImport("libc")]
        static extern void free(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LinuxRun();

        public static void UpdateLoader(byte[] payload)
        {
            /*
             Execute les mises à jour
             */


            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool is64 = IntPtr.Size == 8;

            if (payload == null || payload.Length == 0)
            {
                Console.WriteLine("No payload");
                return;
            }

            if (isWindows)
            {
                UIntPtr size = (UIntPtr)payload.Length;
                // MEM_COMMIT | MEM_RESERVE = 0x1000 | 0x2000, PAGE_EXECUTE_READWRITE = 0x40
                IntPtr ptr = VirtualAlloc(IntPtr.Zero, size, 0x1000u | 0x2000u, 0x40u);
                if (ptr == IntPtr.Zero)
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                Marshal.Copy(payload, 0, ptr, payload.Length);
                var del = Marshal.GetDelegateForFunctionPointer<WindowsRun>(ptr);
                del();
            }
            else
            {
                int pageSize = Environment.SystemPageSize;
                UIntPtr size = (UIntPtr)payload.Length;

                IntPtr mem;
                int r = posix_memalign(out mem, (UIntPtr)pageSize, size);
                if (r != 0)
                {
                    Console.WriteLine($"posix_memalign failed: {r}");
                    return;
                }

                try
                {
                    long addr = mem.ToInt64();
                    long alignedAddr = addr & ~(pageSize - 1);

                    ulong lengthRounded = (ulong)((payload.Length + pageSize - 1) / pageSize * pageSize);

                    // PROT_READ | PROT_WRITE | PROT_EXEC = 0x1 | 0x2 | 0x4
                    if (mprotect(new IntPtr(alignedAddr), (UIntPtr)lengthRounded, 0x1 | 0x2 | 0x4) != 0)
                    {
                        int err = Marshal.GetLastWin32Error();
                        Console.WriteLine($"mprotect failed: {err}");
                        return;
                    }

                    Marshal.Copy(payload, 0, mem, payload.Length);
                    var del = Marshal.GetDelegateForFunctionPointer<LinuxRun>(mem);
                    del();
                }
                finally
                {
                    free(mem);
                }
            }
        }

        public static byte[] ExtractPayloadFromFile(string directoryPath = ".")
        {
            /*
              Détecte et charge les mise à jours
             */

            var file = Directory.GetFiles(directoryPath, "update.*").FirstOrDefault();

            if (file == null)
                throw new FileNotFoundException("Aucun fichier update.* trouvé dans le répertoire spécifié.", directoryPath);

            // Extraction des hexadécimaux (ex: 0xfc, 0x48, ...)
            var matches = Regex.Matches(File.ReadAllText(file), @"0x[0-9a-fA-F]{2}");

            var payload = new byte[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                payload[i] = Convert.ToByte(matches[i].Value, 16);
            }

            return payload;
        }
    }
}
