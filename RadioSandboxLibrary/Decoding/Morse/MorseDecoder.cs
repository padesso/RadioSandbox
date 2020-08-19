using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RadioSandboxLibrary.Decoding.Morse
{
    public class MorseDecoder : IDecoder
    {
        private StringBuilder decodedString;

        private double centerFrequency;

        private const int BUFFER_LENGTH = 44100;
        private CircularBuffer signalBuffer = new CircularBuffer(1024);

        private double timeUnitLength;
        private int dotLengthMultiple = 1;
        private int dashLengthMultiple = 3;
        private int intraCharacterMultiple = 1;
        private int interCharacterMultiple = 3;
        private int interWordMultiple = 7;

        public MorseDecoder()
        {
            decodedString = new StringBuilder();
            signalBuffer = new CircularBuffer(BUFFER_LENGTH);
        }

        public string Decode(double[] signal)
        {
            //TODO: process the signal and create text from it...  :)

            //Pack the current signal in to a buffer so we can look back
            for (int i = 0; i < signal.Length; i++)
            {
                signalBuffer.Write(BitConverter.GetBytes(signal[i]), 0, sizeof(double));
            }

            if(signalBuffer.Count < signalBuffer.MaxLength)
            {
                decodedString.Clear();
                decodedString.Append("Analyzing...");
            }
            else
            {
                //TODO
                decodedString.Append("new morse data... ");
            }

            return DecodedString;
        }

        public string DecodedString { get => decodedString.ToString(); }
        public double CenterFrequency { get => centerFrequency; set => centerFrequency = value; }
    }
}
