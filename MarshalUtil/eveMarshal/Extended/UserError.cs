using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class UserError : ExtendedObject
    {
        public string message;
        public PyDict dict;

        public UserError(PyTuple tuple, PyDict match)
        {
            if (match == null)
            {
                throw new InvalidDataException("UserError: No matching set found.");
            }
            if(tuple.Items.Count > 2 || tuple.Items.Count < 1)
            {
                throw new InvalidDataException("UserError: Invalid tuple size expected 1 or 2 got " + tuple.Items.Count);
            }
            PyString msgString = tuple.Items[0] as PyString;
            if (msgString == null)
            {
                throw new InvalidDataException("UserError: No message found.");
            }
            message = msgString.Value;
            if (tuple.Items.Count == 2)
            {
                dict = tuple.Items[1] as PyDict;
                if(dict == null)
                {
                    throw new InvalidDataException("UserError: Invalid dictionary.");
                }
            }
            msgString = match.Get("msg") as PyString;
            if (msgString == null || msgString.StringValue != message)
            {
                throw new InvalidDataException("UserError: Message name mismatch.");
            }
            PyRep matchRep = match.Get("dict");
            if (dict != null)
            {
                PyDict matchDict = matchRep as PyDict;
                if (matchDict == null)
                {
                    throw new InvalidDataException("UserError: No matching dictionary.");
                }
                if (matchDict.Dictionary.Count != dict.Dictionary.Count)
                {
                    throw new InvalidDataException("UserError: Dictionary size mismatch.");
                }
                //foreach(var kvp in dict.Dictionary)
                //{
                // To-DO: compare each entry.
                //}
            }
            else
            {
                if(matchRep != null && !(matchRep is PyNone))
                {
                    throw new InvalidDataException("UserError: No Dictionary to match.");
                }
            }
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[UserError]");
            printer.indentLevel++;
            printer.addLine("Message: " + message);
            if (dict != null)
            {
                printer.addLine("Parameters:");
                printer.addItem(dict);
            }
            else
            {
                printer.addLine("No parameters.");
            }
            printer.indentLevel--;
        }
    }
}
