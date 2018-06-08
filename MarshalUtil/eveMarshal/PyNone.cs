using System.IO;

namespace eveMarshal
{

    public class PyNone : PyRep
    {
        
        public PyNone()
            : base(PyObjectType.None)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.None);
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyNone]");
        }
    }

}