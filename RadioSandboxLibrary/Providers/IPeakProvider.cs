using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace RadioSandboxLibrary.Providers
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);
        PeakInfo GetNextPeak();
    }
}
