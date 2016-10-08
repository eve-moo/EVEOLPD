using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class DefaultDict : ExtendedObject
    {
        public DefaultDict()
        {

        }

        public override string dump(string prefix)
        {
            return "[DefaultDict]";
        }
    }
}
