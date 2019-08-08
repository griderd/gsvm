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
            DiskDriveRequest request = new DiskDriveRequest(true, 100, 0);
            request.error = 0xff;

            southbridge.WriteToPort(0, request);
            southbridge.ClockTick();
            DiskDriveRequest result = southbridge.ReadFromPort<DiskDriveRequest>(0);
            int length = BitConverter.ToInt32(result.data, 0);

            int chunks = length / 32;
            int rem = length % 32;
            List<byte> bios = new List<byte>();

            // Get BIOS

            for (int i = 0; i < chunks; i++)
            {
                request = new DiskDriveRequest(true, (uint)i * 32, 32);
                southbridge.WriteToPort(0, request);
                southbridge.ClockTick();
                result = southbridge.ReadFromPort<DiskDriveRequest>(0);
                bios.AddRange(result.data);
            }

            request = new DiskDriveRequest(true, (uint)chunks * 32, (uint)rem);
            southbridge.WriteToPort(0, request);
            southbridge.ClockTick();
            result = southbridge.ReadFromPort<DiskDriveRequest>(0);
            bios.AddRange(result.data);

            northbridge.WriteMemory(0, bios.ToArray());

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
            northbridge.WriteMemory(0, data);
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
