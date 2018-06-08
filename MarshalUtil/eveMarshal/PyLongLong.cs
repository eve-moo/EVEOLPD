using System.IO;

namespace eveMarshal
{

    public class PyLongLong : PyRep
    {
        public long Value { get; private set; }

        public PyLongLong()
            : base(PyObjectType.Long)
        {
            
        }

        public PyLongLong(long val)
            : base(PyObjectType.Long)
        {
            Value = val;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            Value = context.reader.ReadInt64();
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.IntegerLongLong);
            output.Write(Value);
        }

        public override string ToString()
        {
            return "<" + Value + ">";
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyLongLong " + Value + "]");
        }
    }

}