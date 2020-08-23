using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;
using RadioSandboxLibrary.Decoding.Morse;
using Spectrogram;
using System;

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

            Spectrogram.Spectrogram spec = new Spectrogram.Spectrogram(
                sampleRate,
                2048,
                2048 / 16); //TODO: better defaults

            spec.Add(audio, process: false);
            double[][] fftsProcessed = spec.Process();

            //TODO: iterate through the samples and process FFT
            for (int processedIndex = 0; processedIndex < fftsProcessed.Length; processedIndex++)
            {
                for (int sampleIndex = 0; sampleIndex < fftsProcessed[processedIndex].Length; sampleIndex++)
                {
                    if(fftsProcessed[processedIndex][sampleIndex] > 1)
                    {
                        Console.WriteLine("Found a reading: " + fftsProcessed[processedIndex][sampleIndex]);
                    }
                }
            }

            MorseDecoder morseDecoder = new MorseDecoder(sampleRate);
            string decodedText = morseDecoder.Decode(fftsProcessed);
        }
    }
}
