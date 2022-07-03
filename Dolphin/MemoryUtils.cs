using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DragoonToolkit.Dolphin
{
    static internal class MemoryUtils
    {
        const int PROCESS_WM_READ = 0x0010;

        public enum AllocationProtectEnum : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        public enum StateEnum : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }

        public enum TypeEnum : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public AllocationProtectEnum AllocationProtect;
            public IntPtr RegionSize;
            public StateEnum State;
            public AllocationProtectEnum Protect;
            public TypeEnum Type;
        }

        // Not the best impl
        public struct PSAPI_WORKING_SET_EX_BLOCK
        {
            public IntPtr Valid;
        }
        public struct PSAPI_WORKING_SET_EX_INFORMATION
        {
            public IntPtr VirtualAddress;
            public PSAPI_WORKING_SET_EX_BLOCK VirtualAttributes;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern uint VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("Psapi.dll")]
        public static extern uint QueryWorkingSetEx(IntPtr hProcess, out PSAPI_WORKING_SET_EX_INFORMATION ws, long cb);
        public static string CaptureDolphinRAMLocation(Process process)
        {
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            MEMORY_BASIC_INFORMATION MEM_INFO;

            long CurrentAddress = 0x00000000;
            long MEM1StartAddress = 0x00000000;
            long ARAMStartAddress = 0x00000000;
            bool FoundMEM1 = false;
            uint RetVal = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
            while(RetVal == (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)))
            {
                RetVal = VirtualQueryEx(process.Handle, (IntPtr)CurrentAddress, out MEM_INFO, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if((long)MEM_INFO.RegionSize == 0x2000000 && MEM_INFO.Type == TypeEnum.MEM_MAPPED)
                {
                    if(!FoundMEM1)
                    {
                        MEM1StartAddress = (long)MEM_INFO.BaseAddress;
                        string AddressInHex = MEM1StartAddress.ToString("x");
                        Trace.WriteLine($"MEM1 Starting Address: {AddressInHex}");
                        FoundMEM1 = true;
                    } else
                    {
                        if (MEM1StartAddress + 0x2000000 == CurrentAddress)
                        {
                            ARAMStartAddress = CurrentAddress;
                            break;
                        }
                    }
                    // TODO: Need to check page has valid working set (BROKEN)
                    // PSAPI_WORKING_SET_EX_INFORMATION ws;
                    // worked = QueryWorkingSetEx(process.Handle, out ws, (uint)Marshal.SizeOf(typeof(PSAPI_WORKING_SET_EX_INFORMATION)));
                    // Trace.WriteLine(worked);
                    // Trace.WriteLine((uint)Marshal.SizeOf(typeof(PSAPI_WORKING_SET_EX_BLOCK)));
                    // Trace.WriteLine((uint)Marshal.SizeOf(typeof(PSAPI_WORKING_SET_EX_INFORMATION)));
                    // Trace.WriteLine((long)ws.VirtualAttributes.Valid);
                }
                CurrentAddress += (long)MEM_INFO.RegionSize;
            }

            if(MEM1StartAddress == 0x00000000)
            {
                Trace.WriteLine("Dolphin running but not game");
                return "bad";
            }
            
            return "good";
        }
    }
}
