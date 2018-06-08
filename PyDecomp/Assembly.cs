using System;
using System.Collections.Generic;
using System.IO;

namespace Python
{
    public class Assembly
    {
        public int length;
        public OpArgs items;

        public Assembly() { }

        public bool load(BinaryReader reader)
        {
            byte magic = reader.ReadByte();
            if (magic != 0x73 && magic != 0x74)//115 (116 = non standard)
            {
                return false;
            }
            length = reader.ReadInt32();
            byte[] raw = reader.ReadBytes(length);
            BinaryReader rawReader = new BinaryReader(new MemoryStream(raw));
            items = new OpArgs();
            if (!items.load(rawReader))
            {
                return false;
            }
            return true;
        }
        public void dump(PrettyPrinter printer)
        {
            printer.addLine("Length: " + length.ToString() + " bytes.");
            items.dump(printer);
        }
    }

    public class OpArgs
    {
        List<Arg> args = new List<Arg>(3);

        public bool load(BinaryReader reader)
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Arg arg = new Arg();
                args.Add(arg);
                if (!arg.load(reader))
                {
                    return false;
                }
            }
            return true;
        }

        public void dump(PrettyPrinter printer)
        {
            int lineWidth = args.Count.ToString().Length;
            // TO-DO: find the actual width required?
            int commandWidth = 20;
            foreach (Arg arg in args)
            {
                string line = arg.byteNumber.ToString().PadLeft(lineWidth, ' ');
                if (arg.haveArg())
                {
                    printer.addLine(line + "  " + arg.opCode.ToString().PadRight(commandWidth, ' ') + ":" + arg.arg.ToString());
                }
                else
                {
                    printer.addLine(line + "  " + arg.opCode.ToString().PadRight(commandWidth, ' '));
                }
            }
        }

        public Arg this[int index]
        {
            get
            {
                return args[index];
            }
            set
            {
                args[index] = value;
            }
        }


    }

    public class Arg
    {
        public OpCode opCode;
        public UInt16 arg;
        public UInt32 byteNumber;

        public bool load(BinaryReader reader)
        {
            byteNumber = (UInt32)reader.BaseStream.Position;
            opCode = (OpCode)reader.ReadByte();
            if (haveArg())
            {
                arg = reader.ReadUInt16();
            }
            return true;
        }
        public bool haveArg()
        {
            if (opCode >= OpCode.Store_Name)
            {
                return true;
            }
            return false;
        }
    }

}
