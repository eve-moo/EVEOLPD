using System.IO;

namespace eveMarshal
{

    public class PyFloat : PyRep
    {
        public double Value { get; private set; }

        public PyFloat()
            : base(PyObjectType.Float)
        {
            
        }

        public PyFloat(double value)
            : base(PyObjectType.Float)
        {
            Value = value;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            if (op == MarshalOpcode.RealZero)
                Value = 0.0d;
            else
                Value = context.reader.ReadDouble();
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            if (Value == 0.0d)
                output.WriteOpcode(MarshalOpcode.RealZero);
            else
            {
                output.WriteOpcode(MarshalOpcode.Real);
                output.Write(Value);
            }
        }

        public override string ToString()
        {
            return "<" + Value + ">";
        }
        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyFloat " + Value + "]");
        }
    }

}