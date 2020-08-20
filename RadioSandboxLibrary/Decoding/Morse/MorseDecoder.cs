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
        private int sRate;
        private int bufferLength;
        private double secondsPerSample;
        private CircularBuffer signalBuffer;

        private StringBuilder decodedString;

        private double centerFrequency;

        private double timeUnitLength;
        private int dotLengthMultiple = 1;
        private int dashLengthMultiple = 3;
        private int intraCharacterMultiple = 1;
        private int interCharacterMultiple = 3;
        private int interWordMultiple = 7;

        public MorseDecoder(int sampleRate)
        {
            sRate = sampleRate;
            bufferLength = sRate;
            secondsPerSample = 1.0 / sampleRate;

            decodedString = new StringBuilder();
            signalBuffer = new CircularBuffer(bufferLength);
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
                byte[] currentBytes = new byte[signalBuffer.Count];
                signalBuffer.Read(currentBytes, 0, signalBuffer.Count);

                timeUnitLength = CalculateUnitLength(currentBytes);
                string currentMessage = DecodeMessage(timeUnitLength, currentBytes);

                decodedString.Append(currentMessage);
            }

            return DecodedString;
        }

        private string DecodeMessage(double timeUnitLength, byte[] currentBytes)
        {
            //TODO: now that we know how the time unit, let's get a message
            string currentMessage = "";

            return currentMessage;
        }

        private double CalculateUnitLength(byte[] currentBytes)
        {
            //TODO: iterate through the bytes and find the unit length
            double tempUnitLength = -1;

            return tempUnitLength;
        }

        public string DecodedString { get => decodedString.ToString(); }
        public double CenterFrequency { get => centerFrequency; set => centerFrequency = value; }
    }
}
