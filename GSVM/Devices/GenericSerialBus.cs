using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Devices
{
    public class GenericSerialBus : GenericDeviceBus<GenericSerialRequest>
    {
        public GenericSerialBus()
        {
            GenerateID();
        }

        public override void ClockTick()
        {
            base.ClockTick();

            ReadyToRead = true;
            ReadyToWrite = false;
        }

        protected override bool InterruptChannelOk(uint channel)
        {
            return true;
        }
    }
}
