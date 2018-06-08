using System;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    class CallMachoBindObject : ExtendedObject
    {
        PyRep bindArgs;
        long characterID = 0;
        long locationID = 0;
        long locationGroupID = 0;
        string callMethod;
        PyTuple callTuple;
        PyDict callDict;

        public CallMachoBindObject(PyTuple payload)
        {
            if (payload == null)
            {
                throw new InvalidDataException("CallMachoBindObject: null payload.");
            }
            if (payload.Items.Count != 2)
            {
                throw new InvalidDataException("CallMachoBindObject: Invalid tuple size expected 2 got" + payload.Items.Count);
            }
            bindArgs = payload.Items[0];
            PyTuple tupleArgs = bindArgs as PyTuple;
            if(tupleArgs != null && tupleArgs.Items.Count == 2 && tupleArgs.Items[0].isIntNumber && tupleArgs.Items[1].isIntNumber)
            {
                locationID = tupleArgs.Items[0].IntValue;
                locationGroupID = tupleArgs.Items[1].IntValue;
                bindArgs = null;
            }
            else if(bindArgs.isIntNumber)
            {
                characterID = bindArgs.IntValue;
                bindArgs = null;
            }
            PyTuple tuple = payload.Items[1] as PyTuple;
            if (tuple != null)
            {
                if (tuple.Items.Count != 3)
                {
                    throw new InvalidDataException("CallMachoBindObject: Invalid tuple size expected 3 got" + tuple.Items.Count);
                }
                if (!(tuple.Items[0] is PyString) || !(tuple.Items[1] is PyTuple) || !(tuple.Items[2] is PyDict))
                {
                    throw new InvalidDataException("CallMachoBindObject: Invalid call structure, expected PyString, PyTuple, PyDict.  Got " +
                        tuple.Items[0].Type + ", " + tuple.Items[1].Type + "," + tuple.Items[2].Type);
                }
                callMethod = tuple.Items[0].StringValue;
                callTuple = tuple.Items[1] as PyTuple;
                callDict = tuple.Items[2] as PyDict;
            }
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[CallMachoBindObject]:");
            printer.indentLevel++;
            printer.addLine("Bind Arguments:");
            if (bindArgs != null)
            {
                printer.addItem(bindArgs);
            }
            else
            {
                printer.indentLevel++;
                if (locationID == 0)
                {
                    if (characterID == 0)
                    {
                        printer.addLine("<nullptr>");
                    }
                    else
                    {
                        printer.addLine("characterID: " + characterID);
                    }
                }
                else
                {
                    printer.addLine("LocationID: " + locationID);
                    printer.addLine("LocationGourpID: " + locationGroupID);
                }
                printer.indentLevel--;
            }
            printer.addLine("Call: '" + callMethod + "'");
            printer.indentLevel++;
            printer.addLine("Call Arguments:");
            printer.addItem(callTuple);
            printer.addLine("Call Named Arguments:");
            printer.addItem(callDict);
            printer.indentLevel--;
            printer.indentLevel--;
        }
    }
}
