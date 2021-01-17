using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Constructs
{
    /// <summary>
    /// Represents a 32-bit pointer with an address and length.
    /// </summary>
    public struct SmartPointer
    {
        /// <summary>
        /// Gets or sets the memory address being pointed to.
        /// </summary>
        public uint Address { get; set; }
        /// <summary>
        /// Gets or sets the length of the value at that address in bytes.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// Instantiates a new SmartPointer
        /// </summary>
        /// <param name="address">Memory address being pointed to.</param>
        /// <param name="length">Length of the value in bytes.</param>
        public SmartPointer(uint address, uint length)
        {
            Address = address;
            Length = length;
        }

        /// <summary>
        /// Determines if the provided pointer collides with this pointer.
        /// </summary>
        /// <param name="ptr">Another pointer</param>
        /// <returns>Returns true if there is overlap. Otherwise, returns false.</returns>
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
