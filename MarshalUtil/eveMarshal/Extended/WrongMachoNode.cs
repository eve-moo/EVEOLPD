using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    class WrongMachoNode : ExtendedObject
    {
        public Int64 correctNode;

        public WrongMachoNode(PyDict obj)
        {
            if(obj.Contains("payload"))
            {
                PyObject value = obj.Get("payload");
                correctNode = value.IntValue;
            }
        }

        public override string dump(string prefix)
        {
            return "[WrongMachoNode: correct node = " + correctNode + "]";
        }
    }
}
