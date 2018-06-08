using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class DefaultDict : ExtendedObject
    {
        public Dictionary<PyRep, PyRep> Dictionary { get; private set; }

        /*
        * [PyObjectEx Type1]
        *   header:
        *     [PyToken "collections.defaultdict"]
        *     [PyTuple 1]
        *       [PyToken __builtin__.set]
        *   dict:
        *     Dictionary
        * create with: new DefaultDict();
        */
        public DefaultDict(Dictionary<PyRep, PyRep> dict)
        {
            Dictionary = dict;
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[DefaultDict]");
            if (Dictionary != null)
            {
                printer.indentLevel++;
                printer.addLine("Dictionary:");
                printer.indentLevel++;
                foreach (var kvp in Dictionary)
                {
                    printer.addLine("Key:");
                    printer.addItem(kvp.Key);
                    if (kvp.Value == null)
                    {
                        printer.addLine("Value: <nullptr>");
                    }
                    else
                    {
                        printer.addLine("Value:");
                        printer.addItem(kvp.Value);
                    }
                }
                printer.indentLevel--;
                printer.indentLevel--;
            }
        }
    }
}
