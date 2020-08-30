using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;
using RadioSandboxLibrary.Decoding.Morse;
using RadioSandboxLibrary.Providers;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RadioSandboxTests
{
    [TestClass]
    public class MorseDecoderTests
    {
        [TestMethod]
        public void DecodeFileTest()
        {
            //Load wav file contianing morse code
            using (WaveFileReader reader = new WaveFileReader(@"Media\LS2-beacon.wav"))
            {
                int bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                var samples = reader.Length / (bytesPerSample);
                var stepSize = 128;
                var width = 3200; //TODO: this should be figured based on some estimated max morse code speed
  
                //Apply RMS peak provider to only get smoothed curve
                RmsPeakProvider rmsPeakProvider = new RmsPeakProvider(256); //TODO: get the right value here
                rmsPeakProvider.Init(reader.ToSampleProvider(), stepSize);

                //Get all the peaks into a list
                int x = 0;
                List<float> maxPeaks = new List<float>();
                maxPeaks.Add(rmsPeakProvider.GetNextPeak().Max);
                while (x < width)
                {
                    maxPeaks.Add(rmsPeakProvider.GetNextPeak().Max);
                    x++;
                }

                //Edge detection
                bool isHigh = false;
                List<int> risingEdgeIndices = new List<int>();
                List<int> fallingEdgeIndices = new List<int>();

                for(int peakIndex = 0; peakIndex < maxPeaks.Count; peakIndex++)
                {
                    if(maxPeaks[peakIndex] > 0.2f) //TODO: expose this threshold to a UI
                    { 
                        if (!isHigh)
                        {
                            isHigh = true;
                            risingEdgeIndices.Add(peakIndex);
                        }
                    }
                    else
                    {
                        if (isHigh)
                        {
                            fallingEdgeIndices.Add(peakIndex);
                            isHigh = false;
                        }
                    }
                }

                Assert.AreEqual(12, risingEdgeIndices.Count);
                Assert.AreEqual(12, fallingEdgeIndices.Count);

                //TODO: compare where rising/falling indices are in relation to total file and decode

                //How many 
                List<int> highLengths = new List<int>();

                //Figure out how many indices between rising and falling edges
                //Assumes equal number of rising and falling edges - ie: we didn't start or end analyzing on a high edge
                for (int risingEdgeIndex = 0; risingEdgeIndex < risingEdgeIndices.Count; risingEdgeIndex++)
                {
                    highLengths.Add(fallingEdgeIndices[risingEdgeIndex] - risingEdgeIndices[risingEdgeIndex]);
                }

                Console.WriteLine("wait");
                
            }
        }

        [TestMethod]
        public void FileFFTTest()
        {
            //Load wav file contianing morse code, tone is 1000Hz
            (int sampleRate, double[] audio) = WavFile.ReadMono(@"Media\LS2-beacon.wav");

            //Create the spectrogram to process the sample file
            Spectrogram.Spectrogram spec = new Spectrogram.Spectrogram(
                sampleRate,
                2048,
                32); //TODO: better defaults

            //Do the processing
            spec.Add(audio, process: false);
            double[][] fftsProcessed = spec.Process();

            //Setup some filtering data
            float centerFrequency = 1500f; //Toner freq in Hz
            int centerBandIndex = (int)((centerFrequency / (spec.FreqMax - spec.FreqMin)) * fftsProcessed[0].Length) - 1;  
        }
    }
}