using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        private SoundCapturer _soundCapturer;
        
        public Form1()
        {
            InitializeComponent();
        }
        
        private void SoundCapturer_FrequencyDetected(object sender, FrequencyDetectedEventArgs e)
        {
            label1.Text = e.Freq.ToString(CultureInfo.CurrentCulture);
        }
        private void startButton_Click(object sender, EventArgs e)
        {
            _soundCapturer = new SoundCapturer();
            _soundCapturer.StartCapture();
            _soundCapturer.FrequencyDetected += SoundCapturer_FrequencyDetected;
        }

        private void ReleaseResources(object sender, EventArgs e)
        {
            _soundCapturer.ReleaseResources();
        }
        
    }
}