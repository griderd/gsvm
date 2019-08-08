using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Components.Controllers
{
    public abstract class GraphicsCard
    {
        Northbridge northbridge;
        protected Memory memory;

        public GraphicsCard(Northbridge northbridge, Memory memory)
        {
            this.northbridge = northbridge;
            this.memory = memory;
        }

        public abstract void ClockTick();
    }
}