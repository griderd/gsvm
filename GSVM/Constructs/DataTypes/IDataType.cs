using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSVM.Constructs.DataTypes
{
    public interface IDataType
    {
        SmartPointer Pointer { get; }
        uint Address { get; set; }
        uint Length { get; }
        byte[] ToBinary();
        void FromBinary(byte[] value);
    }
}
