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

        public bool Overlaps(SmartPointer ptr)
        {
            if ((ptr.Address >= Address) & (ptr.Address < Address + Length))
                return true;
            if ((ptr.Address + ptr.Length >= Address) & (ptr.Address + ptr.Length <= Address + Length))
                return true;
            return false;
        }
    }
}
