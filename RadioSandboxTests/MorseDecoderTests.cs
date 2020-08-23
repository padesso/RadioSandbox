using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;
using RadioSandboxLibrary.Decoding.Morse;
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
                Assert.AreEqual(16, reader.WaveFormat.BitsPerSample, "Only works with 16 bit audio");
                byte[] buffer = new byte[reader.Length];
                int read = reader.Read(buffer, 0, buffer.Length);
                short[] sampleBuffer = new short[read];
                Buffer.BlockCopy(buffer, 0, sampleBuffer, 0, read);

                MorseDecoder morseDecoder = new MorseDecoder(reader.WaveFormat.SampleRate);
                string decodedText = morseDecoder.Decode(sampleBuffer);

                //TODO: manually decode the morse 
                Assert.AreEqual(decodedText, "I'm not sure what this text is...");
            }  
        }
    }
}
