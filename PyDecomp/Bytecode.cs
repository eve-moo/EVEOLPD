using System;
using System.IO;

namespace Python
{
    public class Bytecode
    {
        public UInt16 magic;
        public UInt16 crlf;
        public UInt32 modification_timestamp;
        public CodeBlock body;

        public Bytecode() { }

        public bool load(byte[] data, bool headerless = false)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            if (!headerless)
            {
                magic = reader.ReadUInt16();
                crlf = reader.ReadUInt16();
                modification_timestamp = reader.ReadUInt32();
                if (crlf != 0x0A0D)
                {
                    return false;
                }
            }
            body = loadObject(reader) as CodeBlock;
            return body != null;
        }
        public void dump(PrettyPrinter printer)
        {
            string indent = printer.getIndent();
            printer.addLine("magic: " + magic.ToString("X"));
            printer.addLine("crlf: " + crlf.ToString("X"));
            printer.addLine("modification_timestamp: " + modification_timestamp.ToString("X"));
            printer.addLine("body:");
            printer.addItem(body);
        }

        public static PyObject loadObject(BinaryReader reader)
        {
            int pos = (int)reader.BaseStream.Position;
            PyObject ret;
            int type = reader.ReadByte();
            switch (type)
            {
                case (int)ObjectType.False:
                case (int)ObjectType.None:
                case (int)ObjectType.True:
                    ret = new PyObject();
                    break;
                case (int)ObjectType.Code:
                    ret = new CodeBlock();
                    break;
                case (int)ObjectType.StringRef:
                    ret = new PyStringRef();
                    break;
                case (int)ObjectType.String:
                case (int)ObjectType.UnicodeString:
                case (int)ObjectType.Interned:
                    ret = new PyString();
                    break;
                case (int)ObjectType.Tuple:
                    ret = new PyTuple();
                    break;
                case (int)ObjectType.Int:
                    ret = new PyInt();
                    break;
                case 103:
                    // Not 100% sure this is correct but it seams to work.
                    ret = new PyLong();
                    break;
                default:
                    return null;
            }
            ret.type = (ObjectType)type;
            if (!ret.load(reader))
            {
                return null;
            }
            return ret;
        }

    }

}
