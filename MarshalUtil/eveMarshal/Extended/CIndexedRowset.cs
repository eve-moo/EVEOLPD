using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class CIndexedRowset : ExtendedObject
    {
        string columnName;
        DBRowDescriptor descriptor;
        Dictionary<PyRep, PyRep> rows;

        /*
        * [PyObjectEx Type2]
        *   header:
        *     [PyTuple 1]
        *       [PyToken "carbon.common.script.sys.crowset.CIndexedRowset"]
        *     [PyDict]
        *       Key=header
        *       Value=[DBRowDescriptor]
        *       key=columnName
        *       value=[PyString columnName]
        *   dict:
        *     rows
        * create with: DBResultToCIndexedRowset()
        */
        public CIndexedRowset(PyDict dict, Dictionary<PyRep, PyRep> nRows)
        {
            rows = nRows;
            if(rows == null)
            {
                rows = new Dictionary<PyRep, PyRep>();
            }
            descriptor = dict.Get("header") as DBRowDescriptor;
            if (descriptor == null)
            {
                throw new InvalidDataException("CIndexedRowSet: Invalid DBRowDescriptor.");
            }
            PyRep name = dict.Get("columnName");
            if(name == null)
            {
                throw new InvalidDataException("CIndexedRowSet: Could not find index name.");
            }
            columnName = name.StringValue;
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[CIndexedRowSet]" + PrettyPrinter.PrintRawData(this));
            printer.indentLevel++;
            printer.addLine("index: " + columnName);
            printer.addItem(descriptor);
            printer.addLine("Rows:");
            printer.indentLevel++;
            foreach (var item in rows)
            {
                printer.addLine("Key:");
                printer.addItem(item.Key);
                printer.addLine("Value:");
                printer.addItem(item.Value);
            }
            printer.indentLevel--;
            printer.indentLevel++;
        }

    }
}
