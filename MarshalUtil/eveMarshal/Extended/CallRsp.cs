using System;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    public class CallRsp : ExtendedObject
    {
        bool subStream = false;
        PyRep response;

        /*
        * [PyTuple 1]
        *   [PyTuple] response
        * or
        * [PyTuple 1]
        *   [PySubStream]
        *     response
        * -------------
        * response = return from PyCallable::Call();
        */
        public CallRsp(PyTuple payload)
        {
            if(payload == null)
            {
                throw new InvalidDataException("CallRsp: null payload.");
            }
            if (payload.Items.Count != 1)
            {
                throw new InvalidDataException("CallRsp: Invalid tuple size expected 1 got" + payload.Items.Count);
            }
            if (payload.Items[0] is PyTuple)
            {
                response = payload.Items[0];
            }
            else
            {
                if (!(payload.Items[0] is PySubStream))
                {
                    throw new InvalidDataException("CallRsp: No PySubStreeam.");
                }
                subStream = true;
                PySubStream sub = payload.Items[0] as PySubStream;
                response = sub.Data;
            }
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[CallRsp " + (subStream ? "PySubStream" : "PyTuple") + "]");
            printer.addItem(response);
        }
    }
}
