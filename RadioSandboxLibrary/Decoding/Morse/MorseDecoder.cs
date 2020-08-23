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
        private const short FALLING_EDGE_THRESHOLD = 50; //TODO: use better values
        private const float HIGH_SIGNAL_THRESHOLD_PCT = 0.9f;

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
            //TODO: this approach to timing doesn't work when looking at fft data, need to figure that out
            sRate = sampleRate;
            bufferLength = sRate;
            secondsPerSample = 1.0 / sampleRate;

            centerFrequency = 400f; //TODO: adjustable via UI; preferably from clicking on spectrogram
            cwFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, centerFrequency, 1.0f);

            decodedString = new StringBuilder();
            signalBuffer = new CircularBuffer(bufferLength);
        }

        public string Decode(double[][] signal)
        {
            //TODO: filter, then process the signal and create text from it...  :)

            //Pack the current signal in to a buffer so we can look back
            //for (int i = 0; i < signal.Length; i++)
            //{
            //    signalBuffer.Write(BitConverter.GetBytes(signal[i]), 0, sizeof(short));
            //}

            //if(signalBuffer.Count < signalBuffer.MaxLength)
            //{
            //    decodedString.Clear();
            //    decodedString.Append("Analyzing...");
            //}
            //else
            //{
            //    timeUnitLength = CalculateUnitLength(signal);
            //    string currentMessage = DecodeMessage(timeUnitLength, signal);

            //    decodedString.Append(currentMessage);
            //}

            timeUnitLength = CalculateUnitLength(signal);

            return DecodedString;
        }

        private string DecodeMessage(double timeUnitLength, double[][] currentSamples)
        {
            //TODO: now that we know how the time unit, let's get a message
            string currentMessage = "";

            return currentMessage;
        }

        private double CalculateUnitLength(double[][] currentSamples)
        { 
            bool isHigh = false;

            //TODO: iterate through the bytes and find the unit length
            double tempUnitLength = -1;
            List<int> peakStartIndices = new List<int>();
            List<int> peakEndIndices = new List<int>();

            //TODO: walk through the specified frequency and bands in specified bandwith and find rising and falling edges
            

            return tempUnitLength;
        }

        public string DecodedString { get => decodedString.ToString(); }

        public float CenterFrequency { get => centerFrequency; set => centerFrequency = value; }
    }
}
