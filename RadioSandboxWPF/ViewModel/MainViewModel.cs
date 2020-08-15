﻿using GalaSoft.MvvmLight;
using NAudio.Wave;
using RadioSandboxWPF.Helpers;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

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

        private DispatcherTimer timer;

        private bool decibels;
        private bool roll;

        private ImageSource spectrogamImageSource;

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
            FftSizes = new List<string>() { "512", "1024", "2048", "4096", "8192", "16384", "32768" };
            selectedFftSize = FftSizes[1];

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.IsEnabled = true;
            timer.Tick += Timer_Tick;

            //TODO: move this somewhere else?
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double[] newAudio = listener.GetNewAudio();
            spec.Add(newAudio, process: false);

            double multiplier = 1 / 20.0; //TODO tbBrightness.Value / 20.0;

            if (spec.FftsToProcess > 0)
            {
                Stopwatch sw = Stopwatch.StartNew();
                spec.Process();
                spec.SetFixedWidth((int)SpectrogamImageSource.Width);
                Bitmap bmpSpec = new Bitmap(spec.Width, spec.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                using (var bmpSpecIndexed = spec.GetBitmap(multiplier, Decibels, Roll))
                using (var gfx = Graphics.FromImage(bmpSpec))
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.White))
                {
                    gfx.DrawImage(bmpSpecIndexed, 0, 0);
                    if (Roll)
                    {
                        gfx.DrawLine(pen, spec.NextColumnIndex, 0, spec.NextColumnIndex, (int)SpectrogamImageSource.Height);
                    }
                }
                sw.Stop();

                //SpectrogamImageSource.Dispose();
                SpectrogamImageSource = ImageHelpers.BitmapToImageSource(bmpSpec);
                //lblStatus3.Text = $"Render time: {sw.ElapsedMilliseconds:D2} ms";
                //lblStatus4.Text = $"Peak (Hz): {spec.GetPeak().freqHz:N0}";
            }

            //lblStatus1.Text = $"Time: {listener.TotalTimeSec:N3} sec";
            //lblStatus2.Text = $"FFTs processed: {spec.FftsProcessed:N0}";
            //pbAmplitude.Value = (int)(listener.AmplitudeFrac * pbAmplitude.Maximum);
        }

        public List<WaveInCapabilities> DeviceCapabilities
        {
            get => deviceCapabilities;
            set { deviceCapabilities = value; RaisePropertyChanged("DeviceCapabilites"); }
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

        public bool Decibels
        {
            get => decibels;
            set { decibels = value; RaisePropertyChanged("Decibels"); }
        }

        public bool Roll
        {
            get => roll;
            set { roll = value; RaisePropertyChanged("Roll"); }
        }

        public ImageSource SpectrogamImageSource
        {
            get => spectrogamImageSource;
            set { spectrogamImageSource = value; RaisePropertyChanged("SpectrogamImageSource"); }
    }
    }
}
