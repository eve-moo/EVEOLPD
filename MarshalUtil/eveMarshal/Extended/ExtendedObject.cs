using System;
using System.IO;

namespace eveMarshal.Extended
{
    public abstract class ExtendedObject : PyObject
    {
        public ExtendedObject() : base(PyObjectType.Extended)
        {
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }
    }
}
