using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components;
using GSVM.Components.Clocks;
using GSVM.Components.Controllers;
using GSVM.Devices;

namespace GSVM
{
    public class VirtualMachine
    {
        internal CPU cpu;
        internal Northbridge northbridge;
        internal Southbridge southbridge;

        public bool Debug
        {
            get
            {
                return cpu.Debug;
            }
            set
            {
                cpu.Debug = value;
            }
        }

        public event EventHandler UpdateDebugger;

        public VirtualMachine(CPU cpu, ClockGenerator clock, Northbridge northbridge, Southbridge southbridge)
        {
            this.cpu = cpu;
            this.cpu.Parent = this;

            this.northbridge = northbridge;
            this.southbridge = southbridge;
            this.northbridge.Clock = clock;
        }

        public void Start()
        {
            // Get length

            int cmos = 1;

            DiskDriveRequest request = new DiskDriveRequest(DiskDrive.LEN, 0, 0);

            southbridge.WriteToPort(cmos, request);
            southbridge.ClockTick();
            DiskDriveRequest result = southbridge.ReadFromPort<DiskDriveRequest>(cmos);
            int length = (int)result.data;

            int sectorSize = 512;
            int chunks = length / sectorSize;
            int rem = length % sectorSize;
            List<byte> bios = new List<byte>();

            // Get BIOS

            for (int i = 0; i < chunks; i++)
            {
                request = new DiskDriveRequest(DiskDrive.READ, (uint)i * 512, 512);
                southbridge.WriteToPort(cmos, request);
                southbridge.ClockTick();
                result = southbridge.ReadFromPort<DiskDriveRequest>(cmos);
                bios.AddRange(result.diskData);
            }

            if (rem != 0)
            {
                request = new DiskDriveRequest(DiskDrive.READ, (uint)chunks * 512, (uint)rem);
                southbridge.WriteToPort(cmos, request);
                southbridge.ClockTick();
                result = southbridge.ReadFromPort<DiskDriveRequest>(cmos);
                bios.AddRange(result.diskData);
            }

            northbridge.WriteMemory(0, bios.ToArray(), (uint)bios.Count);

            cpu.Start();
        }

        public void Stop()
        {
            cpu.Stop();
        }

        public byte[] GetRegisters()
        {
            return cpu.GetRegisters();
        }

        public void LoadMemory(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            if (data.Length == 0)
                throw new ArgumentException();

            northbridge.WriteMemory(0, data, (uint)data.Length);
            //memory.Write(0, data);
        }

        public byte[] CoreDump()
        {
            return northbridge.ReadMemory(0, northbridge.Memory.Length);
            //return memory.Read(0, memory.Length);
        }

        public void DebugStep()
        {
            northbridge.DebugTick();
        }

        public void RaiseUpdateDebugger()
        {
            UpdateDebugger?.Invoke(this, new EventArgs());
        }
    }
}
