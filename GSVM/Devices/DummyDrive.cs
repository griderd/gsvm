using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components;

namespace GSVM.Devices
{
    public class DummyDrive : DiskDrive
    {
        public DummyDrive()
            : base(0, true)
        {

        }
    }
}
