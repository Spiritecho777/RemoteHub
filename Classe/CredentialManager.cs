using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RemoteHub.Classe
{
    public static class CredentialManager
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CREDENTIAL
        {
            public uint Flags;
            public uint Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        private static class NativeMethods
        {
            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)] 
            public static extern bool CredDelete(string target, uint type, uint flags);
        }

        public static void SaveCredential(string target, string username, string password)
        {
            IntPtr targetPtr = Marshal.StringToCoTaskMemUni(target);
            IntPtr usernamePtr = Marshal.StringToCoTaskMemUni(username);

            // Convertir le password en bytes Unicode (sans null terminator)
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            IntPtr passwordPtr = Marshal.AllocCoTaskMem(passwordBytes.Length);
            Marshal.Copy(passwordBytes, 0, passwordPtr, passwordBytes.Length);

            var cred = new CREDENTIAL
            {
                Flags = 0,
                Type = 1,
                TargetName = targetPtr,
                Comment = IntPtr.Zero,
                LastWritten = new FILETIME(),
                CredentialBlobSize = (uint)passwordBytes.Length,
                CredentialBlob = passwordPtr,
                Persist = 2,
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                TargetAlias = IntPtr.Zero,
                UserName = usernamePtr
            };

                bool written = NativeMethods.CredWrite(ref cred, 0);

                if (!written)
                {
                    int error = Marshal.GetLastWin32Error();
                    Debug.WriteLine($"CredWrite failed with error {error}");
                    throw new System.ComponentModel.Win32Exception(error);
                }
        }

        public static void DeleteCredential(string target)
        {
            if (!NativeMethods.CredDelete(target, 2, 0))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}
