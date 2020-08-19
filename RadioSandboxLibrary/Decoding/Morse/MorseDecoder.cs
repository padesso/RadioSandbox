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

        private double[] signalBuffer;
        private int bufferPointer = 0;

        private double timeUnitLength;
        private int dotLengthMultiple = 1;
        private int dashLengthMultiple = 3;
        private int intraCharacterMultiple = 1;
        private int interCharacterMultiple = 3;
        private int interWordMultiple = 7;

        public MorseDecoder()
        {
            decodedString = new StringBuilder();
            signalBuffer = new double[44100];  //TODO: about a second of samples, maybe allow this to be set from UI?
        }

        public string Decode(double[] signal)
        {
            //Pack the current signal in to a buffer so we can look back
            for(int i = 0; i < signal.Length; i++)
            {
                signalBuffer[bufferPointer + i] = signal[i];
            }
            bufferPointer += signal.Length;


            decodedString.Append("new morse data... ");

            //TODO: process the signal and create text from it...  :)

            return DecodedString;
        }

        public string DecodedString { get => decodedString.ToString(); }
    }
}
