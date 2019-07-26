using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Constructs
{
    public struct SmartPointer
    {
        public uint Address { get; set; }
        public uint Length { get; set; }

        public SmartPointer(uint address, uint length)
        {
            Address = address;
            Length = length;
        }
    }
}
