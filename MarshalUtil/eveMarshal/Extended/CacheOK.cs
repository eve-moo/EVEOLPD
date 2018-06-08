using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class CacheOK : ExtendedObject
    {
        public CacheOK()
        {
        }

        public override void dump(PrettyPrinter printer)
        {
            printer.addLine("[CacheOK]");
        }
    }
}
