using GalaSoft.MvvmLight;
using NAudio.Wave;
using RadioSandboxWPF.Helpers;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;

namespace RadioSandboxWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Spectrogram.Spectrogram spec;
        private Listener listener;

        private List<WaveInCapabilities> deviceCapabilities;
        private WaveInCapabilities selectedDevice;
        private List<int> fftSizes;
        private int selectedFftSize;

        private List<Colormap> colorMaps;
        private Colormap selectedColorMap;

        private DispatcherTimer timer;

        private bool decibels;
        private bool roll;

        private int brightness;
        private int spectrogramHeight;
        private ImageSource spectrogamImageSource;
        private ImageSource verticalScaleImageSource;

        private string totalTime;
        private string fftsProcessed;
        private string renderTime;
        private string peak;

        private int amplitude;

        public MainViewModel()
        {
            //Setup the audio input
            if (NAudio.Wave.WaveIn.DeviceCount == 0)
            {
                //TODO: set NoDevice app state
            }
            else
            {
                DeviceCapabilities = new List<WaveInCapabilities>();
                for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
                    DeviceCapabilities.Add(NAudio.Wave.WaveIn.GetCapabilities(i));
                SelectedDevice = DeviceCapabilities[0];
            }

            //Setup FFT size
            FftSizes = new List<int>() { 512, 1024, 2048, 4096, 8192, 16384, 32768 };
            selectedFftSize = FftSizes[1];

            ColorMaps = Colormap.GetColormaps().ToList();
            SelectedColorMap = ColorMaps.Where(c => c.Name == "Viridis").FirstOrDefault();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.IsEnabled = true;
            timer.Tick += Timer_Tick;

            Brightness = 5;

            //TODO: move this somewhere else?
            StartListening();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double[] newAudio = listener.GetNewAudio();
            spec.Add(newAudio, process: false);

            double multiplier = Brightness / 20.0; 

            if (spec.FftsToProcess > 0)
            {
                Stopwatch sw = Stopwatch.StartNew();
                spec.Process();
                //if (SpectrogamImageSource != null)
                spec.SetFixedWidth(1024);// (int)SpectrogamImageSource.Width); 

                Bitmap bmpSpec = new Bitmap(spec.Width, spec.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                using (var bmpSpecIndexed = spec.GetBitmap(multiplier, Decibels, Roll))
                using (var gfx = Graphics.FromImage(bmpSpec))
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.White))
                {
                    gfx.DrawImage(bmpSpecIndexed, 0, 0);
                    if (Roll)
                    {
                        gfx.DrawLine(pen, spec.NextColumnIndex, 0, spec.NextColumnIndex, SpectrogramHeight);
                    }
                }
                sw.Stop();

                //SpectrogamImageSource.Dispose();
                SpectrogamImageSource = ImageHelpers.BitmapToImageSource(bmpSpec);
                VerticalScaleImageSource = ImageHelpers.BitmapToImageSource(spec.GetVerticalScale(75));

                RenderTime = $"Render time: {sw.ElapsedMilliseconds:D2} ms";
                Peak = $"Peak (Hz): {spec.GetPeak().freqHz:N0}";
            }

            TotalTime = $"Time: {listener.TotalTimeSec:N3} sec";
            FftsProcessed = $"FFTs processed: {spec.FftsProcessed:N0}";

            //Default max on the progressbar is 100 so hardcoding it here for now
            Amplitude = (int)(listener.AmplitudeFrac * 100);

            SpectrogramHeight = spec.Height;

            VerticalScaleImageSource = null;
            VerticalScaleImageSource = ImageHelpers.BitmapToImageSource(spec.GetVerticalScale(75)); //TODO: not hardcode the wid
        }

        private void StartListening()
        {
            int sampleRate = 6000;
            int fftSize = SelectedFftSize;
            int stepSize = fftSize / 20;

            SpectrogamImageSource = null;
            listener?.Dispose();
            listener = new Listener(0, sampleRate); //TODO: support changes to devices... cbDevice.SelectedIndex, sampleRate);
            spec = new Spectrogram.Spectrogram(sampleRate, fftSize, stepSize);
            spec.SetWindow(FftSharp.Window.Rectangular(fftSize));
            SpectrogramHeight = spec.Height;

            VerticalScaleImageSource = null;
            VerticalScaleImageSource = ImageHelpers.BitmapToImageSource(spec.GetVerticalScale(75)); //TODO: not hardcode the width
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

        public List<int> FftSizes
        {
            get => fftSizes;
            set { fftSizes = value; RaisePropertyChanged("FftSizes"); }
        }

        public int SelectedFftSize
        {
            get => selectedFftSize;
            set 
            { 
                selectedFftSize = value;
                StartListening();
                RaisePropertyChanged("SelectedFftSize"); 
            }
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

        public string TotalTime 
        { 
            get => totalTime;
            set { totalTime = value; RaisePropertyChanged("TotalTime"); }
        }

        public string FftsProcessed
        {
            get => fftsProcessed;
            set { fftsProcessed = value; RaisePropertyChanged("FftsProcessed"); }
        }

        public string RenderTime
        {
            get => renderTime;
            set { renderTime = value; RaisePropertyChanged("RenderTime"); }
        }

        public string Peak 
        { 
            get => peak;
            set { peak = value; RaisePropertyChanged("Peak"); }
        }

        public Colormap SelectedColorMap 
        { 
            get => selectedColorMap;
            set { 
                selectedColorMap = value; 
                if(spec != null)
                    spec.SetColormap(selectedColorMap); 
                RaisePropertyChanged("SelectedColorMap"); 
            }
        }

        public List<Colormap> ColorMaps 
        { 
            get => colorMaps;
            set { colorMaps = value; RaisePropertyChanged("ColorMaps"); }
        }

        public int Amplitude 
        { 
            get => amplitude;
            set { amplitude = value; RaisePropertyChanged("Amplitude"); }
        }

        public ImageSource VerticalScaleImageSource 
        { 
            get => verticalScaleImageSource;
            set { verticalScaleImageSource = value; RaisePropertyChanged("VerticalScaleImageSource"); }
        }

        public int SpectrogramHeight 
        { 
            get => spectrogramHeight;
            set { spectrogramHeight = value; RaisePropertyChanged("SpectrogramHeight"); }
        }

        public int Brightness 
        { 
            get => brightness;
            set { brightness = value; RaisePropertyChanged("Brightness"); }
        }
    }
}
