using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSVM.Components.Controllers;

namespace GSVM.Components.Clocks
{
    public abstract class ClockGenerator
    {
        protected Northbridge northbridge;

        public static event EventHandler CPUTicked;

        public static bool ClockEnabled { get; protected set; }
        public bool Enabled { get; protected set; }

        public ClockGenerator(Northbridge northbridge)
        {
            this.northbridge = northbridge;
        }

        public virtual void Start()
        {
            ClockEnabled = true;
            Enabled = true;
        }

        public virtual void Stop()
        {
            ClockEnabled = false;
            Enabled = false;
        }

        public static void RaiseTick()
        {
            CPUTicked?.Invoke(null, new EventArgs());
        }
    }
}
