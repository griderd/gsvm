using System.IO;
using GSVM;
using GSVM.Peripherals.Monitors;
using GSVM.Peripherals.IODevices;
using GSVM.Devices.DisplayAdapters;
using GSVM.Components;
using GSVM.Components.Processors;
using GSVM.Components.Clocks;
using GSVM.Components.Controllers;
using GSVM.Devices;

MonochromeDisplayAdapter displayAdapter = new();
Monitor monitor = new(1024, 768, "GSVM", displayAdapter);

Memory2 cmosMem = new(File.ReadAllBytes("cmos.bin"));
DiskDrive cmos = new(cmosMem);
Memory2 disk1Mem = new(File.ReadAllBytes("disk1.bin"));
DiskDrive disk1 = new(disk1Mem);

Memory2 memory = new(64 * 1024);

CPU2 cpu = new();
Southbridge southbridge = new(cmos);
Northbridge northbridge = new(cpu, southbridge, memory, displayAdapter);
ThreadedClock clock = new(northbridge);
VirtualMachine vm = new(cpu, clock, northbridge, southbridge);
southbridge.AddDevice(disk1);

Keyboard keyboard = new();
southbridge.AddDevice(keyboard);

northbridge.WriteDisplay(0, System.Text.Encoding.ASCII.GetBytes("Hello World!"));

vm.Start();
monitor.Show();

while (!monitor.IsExiting) { }

vm.Stop();