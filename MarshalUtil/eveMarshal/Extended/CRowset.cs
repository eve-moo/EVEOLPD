using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    public class CRowSet : ExtendedObject
    {
        public DBRowDescriptor descriptor;
        public List<PyRep> rows;

        /*
        * [PyObjectEx Type2]
        *   header:
        *     [PyTuple 1]
        *       [PyToken "carbon.common.script.sys.crowset.CRowset"]
        *     [PyDict]
        *       Key=header
        *       Value=[DBRowDescriptor]
        *   list:
        *     rows
        * create with: DBResultToCRowset
        */
        public CRowSet(PyDict dict, List<PyRep> list)
        {
            rows = list;
            if(rows == null)
            {
                rows = new List<PyRep>();
            }
            descriptor = dict.Get("header") as DBRowDescriptor;
            if (descriptor == null)
            {
                throw new InvalidDataException("CRowSet: Invalid DBRowDescriptor.");
            }
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[CRowSet]" + PrettyPrinter.PrintRawData(this));
            printer.addItem(descriptor);
            printer.indentLevel++;
            printer.addLine("Rows:");
            foreach (var item in rows)
            {
                printer.addItem(item);
            }
            printer.indentLevel--;
        }

    }
}
