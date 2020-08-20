using NAudio.Dsp;
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
        private const float RISING_EDGE_THRESHOLD = 8000f; //TODO: use better values
        private const float FALLING_EDGE_THRESHOLD = 2000f; //TODO: use better values

        private int sRate;
        private int bufferLength;
        private double secondsPerSample;
        private CircularBuffer signalBuffer;

        private StringBuilder decodedString;

        private float centerFrequency;
        private BiQuadFilter cwFilter;

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

            centerFrequency = 15.0f; //TODO: use a real default value
            cwFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, centerFrequency, 1.0f);

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

            //TODO: walk through and find rising edges
            float lastReading = 0;
            for (int index = 0; index < currentBytes.Length; index += 2)
            {
                short sample = (short)((currentBytes[index + 1] << 8) |
                                        currentBytes[index + 0]);
                // to floating point
                var sample32 = sample / 32768f;
                // absolute value 
                if (sample32 < 0) sample32 = -sample32;
                // is this the max value?
                //if (sample32 > max) max = sample32;

                if(sample32 > lastReading) //Rising
                {
                    if(sample32 > RISING_EDGE_THRESHOLD)
                    {
                        //TODO
                    }
                }
                else if (sample32 < lastReading) //Falling
                {
                    if (sample32 < FALLING_EDGE_THRESHOLD)
                    {
                        //TODO
                    }
                }
                else
                {
                    //TODO: Not sure what we need to do here
                }

                lastReading = sample32;
            }

            return tempUnitLength;
        }

        public string DecodedString { get => decodedString.ToString(); }

        public float CenterFrequency { get => centerFrequency; set => centerFrequency = value; }
    }
}
