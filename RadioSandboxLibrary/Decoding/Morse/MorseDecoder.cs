using System;
using System.Collections.Generic;
using System.Text;

namespace RadioSandboxLibrary.Decoding.Morse
{
    public class MorseDecoder : IDecoder
    {
        private StringBuilder decodedString;

        public MorseDecoder()
        {
            decodedString = new StringBuilder();
        }

        public string Decode(double[] signal)
        {
            decodedString.Append("new morse data... ");

            //TODO: process the signal and create text from it...  :)

            return DecodedString;
        }

        public string DecodedString { get => decodedString.ToString(); }
    }
}
