using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;
using RadioSandboxLibrary.Decoding.Morse;
using Spectrogram;
using System;
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
                //Assert.AreEqual(16, reader.WaveFormat.BitsPerSample, "Only works with 16 bit audio");
                //byte[] buffer = new byte[reader.Length];
                //int read = reader.Read(buffer, 0, buffer.Length);
                //short[] sampleBuffer = new short[read];
                //Buffer.BlockCopy(buffer, 0, sampleBuffer, 0, read);

                //MorseDecoder morseDecoder = new MorseDecoder(reader.WaveFormat.SampleRate);
                //string decodedText = morseDecoder.Decode(sampleBuffer);

                ////TODO: manually decode the morse 
                //Assert.AreEqual(decodedText, "I'm not sure what this text is...");
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
                1024,
                32); //TODO: better defaults

            //Do the processing
            spec.Add(audio, process: false);
            double[][] fftsProcessed = spec.Process();

            //Setup some filtering data
            float centerFrequency = 1500f; //Toner freq in Hz
            int centerBandIndex = (int)((centerFrequency / (spec.FreqMax - spec.FreqMin)) * fftsProcessed[0].Length) - 1;

            bool isHigh = false;
            int risingEdges = 0;
            int fallingEdges = 0;

            //Iiterate through the processed data in the frequency bands we care about and decode morse //TODO: include the side bands
            for (int processedIndex = 0; processedIndex < fftsProcessed.Length; processedIndex++) //Iterate all readings
            {
                if (fftsProcessed[processedIndex][centerBandIndex] > 0.4) //TODO: better thresholds
                {
                    if (!isHigh)
                    {
                        risingEdges++;
                        isHigh = true;
                    }
                }
                else
                {
                    if (isHigh)
                    {
                        fallingEdges++;
                        isHigh = false;
                    }
                }
            }

            //The sample audio should rise and fall 12 times each
            Assert.AreEqual(12, risingEdges);
            Assert.AreEqual(12, fallingEdges);
        }
    }
}