using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PySubStruct : PyRep
    {
        public PyRep Definition { get; set; }

        public PySubStruct()
            : base(PyObjectType.SubStruct)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            Definition = context.ReadObject();
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.SubStruct);
            Definition.Encode(output);
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PySubStruct]");
            printer.addItem(Definition);
        }

    }

}