using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MimiJson
{
#if !NET45
    public class MyMemoryStream : MemoryStream
    {
        public override void Close() { }

        public void ReallyClose()
        {
            base.Close();
        }
    }
#endif
}
