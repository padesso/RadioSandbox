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
            }
        }

        [TestMethod]
        public void DecodeFileFFTTest()
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
            
            bool isHigh = false;
            List<int> risingEdgeIndices = new List<int>();
            List<int> fallingEdgeIndices = new List<int>();

            //TODO: this needs to average several bands...
            //Iiterate through the processed data in the frequency bands we care about and decode morse //TODO: include the side bands
            for (int processedIndex = 0; processedIndex < fftsProcessed.Length; processedIndex++) //Iterate all readings
            {
                //TODO: make this more flexible
                double averagedReading = 0;
                for (int bandpassOffset = -10; bandpassOffset <= 10; bandpassOffset++)
                {
                    averagedReading += fftsProcessed[processedIndex][centerBandIndex + bandpassOffset];
                }
                averagedReading /= 3.0;

                if (averagedReading > 0.5) //TODO: better thresholds
                {
                    if (!isHigh)
                    {
                        risingEdgeIndices.Add(processedIndex);
                        isHigh = true;
                    }
                }
                else
                {
                    if (isHigh)
                    {
                        fallingEdgeIndices.Add(processedIndex);
                        isHigh = false;
                    }
                }
            }

            //TODO: calculate some times
            //string decodedText = "";
            //List<double> highTimes = new List<double>();
            //for (int highIndex = 0; highIndex < risingEdgeIndices.Count; highIndex++)
            //{
            //    highTimes.Add(spec.SecPerPx * (highIndex / fftsProcessed.Length));
            //}

            //The sample audio should rise and fall 12 times each
            Assert.AreEqual(12, risingEdgeIndices.Count);
            Assert.AreEqual(12, fallingEdgeIndices.Count);

            //Assert.AreEqual(decodedText, "LS2");
        }
    }
}