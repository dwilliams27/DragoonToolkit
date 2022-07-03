using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragoonToolkit.Dolphin
{
    internal class DolphinProcess
    {
        readonly string DOLPHIN_PROCESS_NAME = "Dolphin";
        private Process DProcess = null;
        
        public DolphinProcess()
        {
            // Find process id of Dolphin
            Process[] processCollection = Process.GetProcesses();
            foreach (Process p in processCollection)
            {
                if(p.ProcessName.Equals(DOLPHIN_PROCESS_NAME))
                {
                    DProcess = p;
                    Trace.WriteLine($"Dolphin process located on {p.Id}");
                    break;
                }
            }

            if(DProcess == null)
            {
                Trace.WriteLine("Dolphin process not found...");
                return;
            }

            GrabDolphinRAM();
        }

        private void GrabDolphinRAM()
        {
            MemoryUtils.CaptureDolphinRAMLocation(DProcess);
        }
    }
}
