using System;
using System.IO;
using static Python.Bytecode;

namespace Python
{
    public class CodeBlock : PyObject
    {
        public UInt32 arg_count;
        public UInt32 local_count;
        public UInt32 stack_size;
        public UInt32 flags;
        public Assembly assembly;
        public PyObject consts;
        public PyObject names;
        public PyObject varNames;
        public PyObject freeVars;
        public PyObject cellVars;
        public PyObject filename;
        public PyObject name;
        public UInt32 firstLineNumber;
        public PyObject lnotab;

        public override bool load(BinaryReader reader)
        {
            arg_count = reader.ReadUInt32();
            local_count = reader.ReadUInt32();
            stack_size = reader.ReadUInt32();
            flags = reader.ReadUInt32();
            assembly = new Assembly();
            if (!assembly.load(reader))
            {
                return false;
            }
            consts = loadObject(reader);
            names = loadObject(reader);
            varNames = loadObject(reader);
            freeVars = loadObject(reader);
            cellVars = loadObject(reader);
            filename = loadObject(reader);
            name = loadObject(reader);
            firstLineNumber = reader.ReadUInt32();
            lnotab = loadObject(reader);

            return true;
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("{Code:}");
            printer.indentLevel++;
            printer.addLine("arg_count: " + arg_count.ToString());
            printer.addLine("local_count: " + local_count.ToString());
            printer.addLine("stack_size: " + stack_size.ToString());
            printer.addLine("flags: " + flags.ToString("X"));
            printer.addLine("Code:");
            printer.indentLevel++;
            assembly.dump(printer);
            printer.indentLevel--;
            printer.addLine("const:");
            printer.addItem(consts);
            printer.addLine("names:");
            printer.addItem(names);
            printer.addLine("varNames:");
            printer.addItem(varNames);
            printer.addLine("freeVars:");
            printer.addItem(freeVars);
            printer.addLine("cellVars:");
            printer.addItem(cellVars);
            printer.addLine("filename: ");
            printer.addItem(filename);
            printer.addLine("name: ");
            printer.addItem(name);
            printer.addLine("firstLineNumber: " + firstLineNumber.ToString());
            printer.addLine("lnotab:");
            printer.addItem(lnotab);
            printer.indentLevel--;
            printer.addLine("{end Code}");
        }

    }
}
