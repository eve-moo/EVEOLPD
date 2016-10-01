using System;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    public class CallRsp : PyObject
    {
        PyObject response;

        public CallRsp(PyTuple payload)
            : base(PyObjectType.Extended)
        {
            if(payload == null)
            {
                throw new InvalidDataException("CallRsp: null payload.");
            }
            if (payload.Items.Count != 1)
            {
                throw new InvalidDataException("CallRsp: Invalid tuple size expected 1 got" + payload.Items.Count);
            }
            if (!(payload.Items[0] is PySubStream))
            {
                throw new InvalidDataException("CallRsp: No PySubStreeam.");
            }
            PySubStream sub = payload.Items[0] as PySubStream;
            response = sub.Data;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Response:");
            PrettyPrinter.Print(builder, pfx1, response);
            return builder.ToString();
        }
    }
}
