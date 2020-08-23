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
        private const short RISING_EDGE_THRESHOLD = 15000; //TODO: use better values
        private const short FALLING_EDGE_THRESHOLD = 1000; //TODO: use better values

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

        public string Decode(short[] signal)
        {
            //TODO: filter, then process the signal and create text from it...  :)

            //Pack the current signal in to a buffer so we can look back
            for (int i = 0; i < signal.Length; i++)
            {
                signalBuffer.Write(BitConverter.GetBytes(signal[i]), 0, sizeof(short));
            }

            if(signalBuffer.Count < signalBuffer.MaxLength)
            {
                decodedString.Clear();
                decodedString.Append("Analyzing...");
            }
            else
            {
                timeUnitLength = CalculateUnitLength(signal);
                string currentMessage = DecodeMessage(timeUnitLength, signal);

                decodedString.Append(currentMessage);
            }

            return DecodedString;
        }

        private string DecodeMessage(double timeUnitLength, short[] curretnSamples)
        {
            //TODO: now that we know how the time unit, let's get a message
            string currentMessage = "";

            return currentMessage;
        }

        private double CalculateUnitLength(short[] currentSamples)
        {
            bool isHigh = false;

            //TODO: iterate through the bytes and find the unit length
            double tempUnitLength = -1;
            List<int> peakStartIndices = new List<int>();
            List<int> peakEndIndices = new List<int>();

            //TODO: walk through and find rising edges
            double lastSample = 0;
            for (int index = 0; index < currentSamples.Length; index += 2)
            {
                var sample = currentSamples[index];
                // absolute value 
                if (sample < 0) 
                    sample = (short)-sample;

                //Find the peaks
                if (sample > RISING_EDGE_THRESHOLD) //High
                {
                    if (!isHigh)
                    {
                        isHigh = true;
                        peakStartIndices.Add(index);
                    }
                }
                else if (sample < FALLING_EDGE_THRESHOLD)//Low
                {
                    if (isHigh)
                    {
                        isHigh = false;
                        peakEndIndices.Add(index);
                    }
                }

                lastSample = sample;
            }

            return tempUnitLength;
        }

        public string DecodedString { get => decodedString.ToString(); }

        public float CenterFrequency { get => centerFrequency; set => centerFrequency = value; }
    }
}
