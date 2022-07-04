using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static DragoonToolkit.Dolphin.MemoryUtils;

namespace DragoonToolkit.Dolphin
{
    public class DolphinProcess
    {
        const int PROCESS_WM_READ = 0x0010;
        const uint ARAM_FAKESIZE = 0x2000000;
        const string DOLPHIN_PROCESS_NAME = "Dolphin";

        long MEM1StartAddress = 0x00000000;
        long ARAMStartAddress = 0x00000000;

        private Process DProcess = null;

        public DolphinProcess()
        {
            // Find process id of Dolphin
            Process[] processCollection = Process.GetProcesses();
            foreach (Process p in processCollection)
            {
                if (p.ProcessName.Equals(DOLPHIN_PROCESS_NAME))
                {
                    DProcess = p;
                    Trace.WriteLine($"Dolphin process located on {p.Id}");
                    break;
                }
            }

            if (DProcess == null)
            {
                Trace.WriteLine("Dolphin process not found...");
                return;
            }

            CaptureDolphinRAMLocation();

            byte[] buffer = new byte[4];
            ReadFromMemory(0x00000000, buffer, 4);
            WriteToMemory(0x00000000, ConvertHexStringToByteArray("DDDDDDDD"), 4);
            ReadFromMemory(0x00000000, buffer, 4);
        }

        public long[] CaptureDolphinRAMLocation()
        {
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, DProcess.Id);
            MEMORY_BASIC_INFORMATION MEM_INFO;

            long CurrentAddress = 0x00000000;
            MEM1StartAddress = 0x00000000;
            ARAMStartAddress = 0x00000000;
            bool FoundMEM1 = false;
            uint RetVal = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
            while (RetVal == (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)))
            {
                RetVal = VirtualQueryEx(DProcess.Handle, (IntPtr)CurrentAddress, out MEM_INFO, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if ((long)MEM_INFO.RegionSize == 0x2000000 && MEM_INFO.Type == TypeEnum.MEM_MAPPED)
                {
                    if (!FoundMEM1)
                    {
                        MEM1StartAddress = (long)MEM_INFO.BaseAddress;
                        string AddressInHex = MEM1StartAddress.ToString("x");
                        Trace.WriteLine($"MEM1 Starting Address: {AddressInHex}");
                        FoundMEM1 = true;
                    }
                    else
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

            if (MEM1StartAddress == 0x00000000)
            {
                Trace.WriteLine("Dolphin running but not game");
                return null;
            }

            return new long[] { MEM1StartAddress, ARAMStartAddress };
        }

        // TODO: Implement BSwap
        public string ReadFromMemory(uint offset, byte[] buffer, int size, bool withBSwap=false)
        {
            long address = MEM1StartAddress + offset;
            
            IntPtr amountRead;
            bool success = ReadProcessMemory(DProcess.Handle, (IntPtr)address, buffer, size, out amountRead);
            Trace.WriteLine(Convert.ToHexString(buffer));

            return Convert.ToHexString(buffer);
        }

        public string WriteToMemory(uint offset, byte[] buffer, uint size, bool withBSwap = false)
        {
            long address = MEM1StartAddress + offset;

            IntPtr amountRead;
            bool success = WriteProcessMemory(DProcess.Handle, (IntPtr)address, buffer, size, out amountRead);
            Trace.WriteLine(Convert.ToHexString(buffer));

            return Convert.ToHexString(buffer);
        }
    }
}