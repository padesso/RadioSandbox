using System;
using System.Collections.Generic;
using System.Text;

namespace RadioSandboxLibrary.Decoding
{
    public interface IDecoder
    {
        string Decode(double[] signal);
    }
}
