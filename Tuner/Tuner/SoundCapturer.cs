using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Media;

namespace Tuner
{
    public class Sound
    {
        public Sound(double volume, double frequency)
        {
            Volume = volume;
            Frequency = frequency;
        }

        public double Frequency { get; set; }
        public double Volume { get; set; }
    }

    public class FrequencyDetectedEventArgs : EventArgs
    {
        public double Freq { get; }

        public FrequencyDetectedEventArgs(double freq)
        {
            Freq = freq;
        }
    }

    public sealed class SoundCapturer
    {
        private AudioRecord _audioRecord;
        private int _bufferSize;
        private AutoResetEvent _soundCaptureEvent;
        private AutoResetEvent _displayFrequencyEvent;
        private Thread _soundCaptureThread;
        private SoundAnalyzer _analyzer;
        private double[] _audioBuffer;
        private bool _isCapturing;
        private double _freq;
        private float[] _buffer;
        private const double MaxAmplitude = 1.0f;
        private const double MicThreshold = 0.05f;
        private const int SampleRate = 44100;
        private const int DeltaMills = 30;
        private const int FramesPerBuffer = SampleRate * DeltaMills / 1000;
        public event EventHandler<FrequencyDetectedEventArgs> FrequencyDetected;

        private void OnFrequencyDetected(FrequencyDetectedEventArgs e)
        {
            FrequencyDetected?.Invoke(this, e);
        }

        public void ReleaseResources()
        {
            if (_isCapturing)
            {
                _isCapturing = false;
                _soundCaptureEvent.Dispose();
                _displayFrequencyEvent.Dispose();
                _soundCaptureThread.Join();
            }
        }

        public void StartCapture()
        {
            if (_isCapturing)
            {
                return;
            }
            
            ChannelIn channelConfig = ChannelIn.Stereo;
            Encoding audioFormat = Encoding.PcmFloat;
            _bufferSize = AudioRecord.GetMinBufferSize(SampleRate, channelConfig, audioFormat);
            _audioRecord = new AudioRecord(
                AudioSource.Mic,
                SampleRate,
                channelConfig,
                audioFormat,
                _bufferSize);

            _buffer = new float[_bufferSize];
            _audioBuffer = new double[FramesPerBuffer];
            _analyzer = new SoundAnalyzer(_audioBuffer, SampleRate);
            _soundCaptureEvent = new AutoResetEvent(false);
            _displayFrequencyEvent = new AutoResetEvent(false);
            _soundCaptureThread = new Thread(SoundCaptureThreadLoop);

            _isCapturing = true;
            _audioRecord.StartRecording();
            _soundCaptureThread.Start();
            _soundCaptureEvent.Set();
        }

        private void SoundCaptureThreadLoop()
        {
            Queue<double> inputFreq = new Queue<double>();
            int lowBorder = 20;
            int highBorder = 20000;
            int step = 10;
            int[] freqCount = new int[(highBorder - lowBorder) / step];
            while (_isCapturing)
            {

                _audioRecord.Read(_buffer, 0, _buffer.Length, (int)AudioRecordReadOptions.Blocking);

                for (int i = 0; i < FramesPerBuffer; i++)
                {
                    _audioBuffer[i] = (double)(_buffer[i * 2] + _buffer[i * 2 + 1]) / 2;
                }

                _freq = _analyzer.CalculateFrequency();


                Sound newSound = new Sound(_audioBuffer.Max() / MaxAmplitude, _freq);

                if (newSound.Volume < MicThreshold)
                {
                    continue;
                }


                inputFreq.Enqueue(newSound.Frequency);
                freqCount[(int)((newSound.Frequency - lowBorder) / step)]++;

                if (inputFreq.Count >= 10) 
                {
                    double freq = inputFreq.Dequeue();

                    int rangeIdx = (int)((freq - lowBorder) / step);
                    int count = 0;
                    count += freqCount[rangeIdx];
                    count += rangeIdx > 0 ? freqCount[rangeIdx - 1] : 0;
                    count += rangeIdx < freqCount.Length - 1 ? freqCount[rangeIdx + 1] : 0;

                    if (count >= 6)
                    {
                        OnFrequencyDetected(new FrequencyDetectedEventArgs(freq));
                    }

                    freqCount[rangeIdx]--;
                    
                }
            }
        }
    }
}

