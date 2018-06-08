using System;
using System.IO;

namespace eveMarshal
{
    
    public class PyIntegerVar : PyRep
    {
        public byte[] Raw { get; private set; }

        public PyIntegerVar()
            : base (PyObjectType.IntegerVar)
        {

        }

        public PyIntegerVar(byte[] data)
            : base(PyObjectType.IntegerVar)
        {
            Raw = data;
        }

        public PyIntegerVar(int data)
            : base(PyObjectType.IntegerVar)
        {
            Raw = GetData(data);
        }

        public PyIntegerVar(long data)
            : base(PyObjectType.IntegerVar)
        {
            Raw = GetData(data);
        }

        public PyIntegerVar(short data)
            : base(PyObjectType.IntegerVar)
        {
            Raw = GetData(data);
        }

        public PyIntegerVar(byte data)
            : base(PyObjectType.IntegerVar)
        {
            Raw = new []{data};
        }
        
        private static byte[] GetData(long value)
        {
            if (value < 128)
                return new[]{(byte)value};
            if (value < Math.Pow(2, 15))
                return BitConverter.GetBytes((short)value);
            if (value < Math.Pow(2, 31))
                return BitConverter.GetBytes((int)value);
            return BitConverter.GetBytes(value);
        }

        public Int64 Value
        {
            get
            {
                if (Raw.Length == 1)
                    return Raw[0];
                if (Raw.Length == 2)
                    return BitConverter.ToInt16(Raw, 0);
                if (Raw.Length == 4)
                    return BitConverter.ToInt32(Raw, 0);
                if (Raw.Length == 8)
                    return BitConverter.ToInt64(Raw, 0);
                return -1;
            }
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            var len = context.reader.ReadSizeEx();
            Raw = context.reader.ReadBytes((int) len);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.IntegerVar);
            output.WriteSizeEx(Raw.Length);
            output.Write(Raw);
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyIntegerVar " + IntValue + "]");
        }
    }

}