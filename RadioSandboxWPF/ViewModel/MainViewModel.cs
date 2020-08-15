using GalaSoft.MvvmLight;
using NAudio.Wave;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Text;

namespace RadioSandboxWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Colormap[] cmaps;
        private Spectrogram.Spectrogram spec;
        private Listener listener;

        private List<WaveInCapabilities> deviceCapabilities;
        private WaveInCapabilities selectedDevice;
        private List<string> fftSizes;
        private string selectedFftSize;

        public MainViewModel()
        {
            //Setup the audio input
            if (NAudio.Wave.WaveIn.DeviceCount == 0)
            {
                //TODO: set NoDevice app state
            }
            else
            {
                DeviceCapabilities.Clear();
                for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
                    DeviceCapabilities.Add(NAudio.Wave.WaveIn.GetCapabilities(i));
                SelectedDevice = DeviceCapabilities[0];
            }

            //Setup FFT size
            FftSizes = new List<string>() { "512", "1024", "2048", "4096", "8192", "16384", "32768"};
            selectedFftSize = FftSizes[1];


        }

        public List<WaveInCapabilities> DeviceCapabilities 
        { 
            get => deviceCapabilities;
            set{ deviceCapabilities = value; RaisePropertyChanged("DeviceCapabilites"); } 
        }

        public WaveInCapabilities SelectedDevice 
        { 
            get => selectedDevice;
            set { selectedDevice = value; RaisePropertyChanged("SelectedDevice"); }
        }

        public List<string> FftSizes 
        { 
            get => fftSizes;
            set { fftSizes = value; RaisePropertyChanged("FftSizes"); }
        }

        public string SelectedFftSize 
        { 
            get => selectedFftSize;
            set { selectedFftSize = value; RaisePropertyChanged("SelectedFftSize"); }
        }
    }
}
