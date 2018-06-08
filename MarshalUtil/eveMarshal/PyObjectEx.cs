using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PyObjectEx : PyRep
    {
        private const byte PackedTerminator = 0x2D;

        public bool IsType2 { get; private set; }
        public PyRep Header { get; private set; }
        public Dictionary<PyRep, PyRep> Dictionary { get; private set; }
        public List<PyRep> List { get; private set; }

        public PyObjectEx(bool isType2, PyRep header)
            : base(PyObjectType.ObjectEx)
        {
            IsType2 = isType2;
            Header = header;
            Dictionary = new Dictionary<PyRep, PyRep>();
        }

        public PyObjectEx()
            : base(PyObjectType.ObjectEx)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            if (op == MarshalOpcode.ObjectEx2)
            {
                IsType2 = true;
            }

            Dictionary = new Dictionary<PyRep, PyRep>();
            List = new List<PyRep>();
            Header = context.ReadObject();

            while (context.reader.BaseStream.Position < context.reader.BaseStream.Length)
            {
                var b = context.reader.ReadByte();
                if (b == PackedTerminator)
                    break;
                context.reader.BaseStream.Seek(-1, SeekOrigin.Current);
                List.Add(context.ReadObject());
            }

            while (context.reader.BaseStream.Position < context.reader.BaseStream.Length)
            {
                var b = context.reader.ReadByte();
                if (b == PackedTerminator)
                    break;
                context.reader.BaseStream.Seek(-1, SeekOrigin.Current);
                var key = context.ReadObject();
                var value = context.ReadObject();
                Dictionary.Add(key, value);
            }
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(IsType2 ? MarshalOpcode.ObjectEx2 : MarshalOpcode.ObjectEx1);
            Header.Encode(output);
            // list
            output.Write(PackedTerminator);
            // dict
            output.Write(PackedTerminator);
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyObjectEx " + (IsType2 ? "Type2" : "Normal") + "]" + PrettyPrinter.PrintRawData(this));
            printer.indentLevel++;
            printer.addLine("Header:");
            printer.addItem(Header);
            printer.addLine("List:");
            foreach (var item in List)
            {
                printer.addItem(item);
            }
            printer.addLine("Dictionary:");
            foreach (var kvp in Dictionary)
            {
                printer.indentLevel++;
                printer.addLine("Key:");
                printer.addItem(kvp.Key);
                printer.addLine("Value:");
                printer.addItem(kvp.Value);
                printer.indentLevel--;
            }
            printer.indentLevel--;
        }

    }

}