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
        public PyObject dict;

        public UserError(PyDict nDict)
        {
            PyObject msg = nDict.Get("msg");
            if (msg == null)
            {
                throw new InvalidDataException("UserError: No message found.");
            }
            message = msg.StringValue;
            dict = nDict.Get("dict");
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + "    ";
            string pfx2 = pfx1 + "    ";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[UserError]");
            builder.AppendLine(pfx1 + "Message: " + message);
            if (dict != null)
            {
                builder.AppendLine(pfx1 + "Parameters:");
                builder.AppendLine(pfx2 + dict.dump(pfx2));
            }
            else
            {
                builder.AppendLine(pfx1 + "No parameters.");
            }
            return builder.ToString();
        }
    }
}
