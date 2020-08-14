﻿using Spectrogram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RadioSandboxWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Colormap[] cmaps;

        private Spectrogram.Spectrogram spec;
        private Listener listener;

        private System.Timers.Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            if (NAudio.Wave.WaveIn.DeviceCount == 0)
            {
                Console.WriteLine("Error intializing wave in device.");
                Close();
            }
            else
            {
                //cbDevice.Items.Clear();
                //for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
                //    cbDevice.Items.Add(NAudio.Wave.WaveIn.GetCapabilities(i).ProductName);
                //cbDevice.SelectedIndex = 0;
            }

            //for (int i = 9; i < 16; i++)
            //    cbFftSize.Items.Add($"2^{i} ({1 << i:N0})");
            //cbFftSize.SelectedIndex = 1;

            //cmaps = Colormap.GetColormaps();
            //foreach (Colormap cmap in cmaps)
            //    cbColormap.Items.Add(cmap.Name);
            //cbColormap.SelectedIndex = cbColormap.Items.IndexOf("Viridis");

            timer = new System.Timers.Timer(500);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            spectrographImage.Source = BitmapToImageSource(spec.GetBitmap());
            StartListening();
        }

        private void StartListening()
        {
            int sampleRate = 6000;
            int fftSize = 1 << (9 + cbFftSize.SelectedIndex);
            int stepSize = fftSize / 20;

            pbSpectrogram.Image?.Dispose();
            pbSpectrogram.Image = null;
            listener?.Dispose();
            listener = new Listener(cbDevice.SelectedIndex, sampleRate);
            spec = new Spectrogram(sampleRate, fftSize, stepSize);
            //spec.SetWindow(FftSharp.Window.Rectangular(fftSize));
            pbSpectrogram.Height = spec.Height;

            pbScaleVert.Image?.Dispose();
            pbScaleVert.Image = spec.GetVerticalScale(pbScaleVert.Width);
            pbScaleVert.Height = spec.Height;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double[] newAudio = listener.GetNewAudio();
            spec.Add(newAudio, process: false);

            double multiplier = tbBrightness.Value / 20.0;

            if (spec.FftsToProcess > 0)
            {
                Stopwatch sw = Stopwatch.StartNew();
                spec.Process();
                spec.SetFixedWidth(pbSpectrogram.Width);
                Bitmap bmpSpec = new Bitmap(spec.Width, spec.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                using (var bmpSpecIndexed = spec.GetBitmap(multiplier, cbDecibels.Checked, cbRoll.Checked))
                using (var gfx = Graphics.FromImage(bmpSpec))
                using (var pen = new Pen(Color.White))
                {
                    gfx.DrawImage(bmpSpecIndexed, 0, 0);
                    if (cbRoll.Checked)
                    {
                        gfx.DrawLine(pen, spec.NextColumnIndex, 0, spec.NextColumnIndex, pbSpectrogram.Height);
                    }
                }
                sw.Stop();
                pbSpectrogram.Image?.Dispose();
                pbSpectrogram.Image = bmpSpec;
                lblStatus3.Text = $"Render time: {sw.ElapsedMilliseconds:D2} ms";
                lblStatus4.Text = $"Peak (Hz): {spec.GetPeak().freqHz:N0}";
            }

            lblStatus1.Text = $"Time: {listener.TotalTimeSec:N3} sec";
            lblStatus2.Text = $"FFTs processed: {spec.FftsProcessed:N0}";
            pbAmplitude.Value = (int)(listener.AmplitudeFrac * pbAmplitude.Maximum);
        }


        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        
    }
}
