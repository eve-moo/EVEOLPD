using System;
using System.IO;
using System.Text;
using Python;

namespace eveMarshal
{

    public class PyBuffer : PyRep
    {
        public byte[] Data { get; private set; }

        public PyBuffer()
            : base(PyObjectType.Buffer)
        {
            Data = null;
        }

        public PyBuffer(byte[] data)
            : base(PyObjectType.Buffer)
        {
            Data = data;
        }

        public PyBuffer(string data)
            : base(PyObjectType.Buffer)
        {
            Data = Encoding.ASCII.GetBytes(data);
        }

        public override void Decode(Unmarshal context, MarshalOpcode op)
        {
            var size = context.reader.ReadSizeEx();
            Data = context.reader.ReadBytes((int)size);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.Buffer);
            output.WriteSizeEx(Data.Length);
            output.Write(Data);
        }

        public override string ToString()
        {
            return "<" + BitConverter.ToString(Data) + ">";
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[PyBuffer " + Data.Length + " bytes]" + PrettyPrinter.PrintRawData(this));
            if(Data[0] == Unmarshal.HeaderByte || Data[0] == Unmarshal.ZlibMarker)
            {
                byte[] d = Data;
                if (d[0] == Unmarshal.ZlibMarker)
                {
                    d = Zlib.Decompress(d);
                }
                if (d!= null && d[0] == Unmarshal.PythonMarker && printer.decompilePython)
                {
                    // We have a python file.
                    Bytecode code = new Bytecode();
                    if (code.load(d, true))
                    {
                        Python.PrettyPrinter pp = new Python.PrettyPrinter();
                        pp.indentLevel = printer.indentLevel + 1;
                        pp.indent = printer.indent;
                        code.dump(pp);
                        printer.addLine(pp.dump);
                    }
                }
                else
                {
                    Unmarshal un = new Unmarshal();
                    PyRep rep = un.Process(d);
                    if (rep != null)
                    {
                        if (Data[0] == Unmarshal.ZlibMarker)
                        {
                            printer.addLine("<compressed-data>");
                        }
                        printer.addItem(rep);
                    }
                }
            }
        }

    }

}