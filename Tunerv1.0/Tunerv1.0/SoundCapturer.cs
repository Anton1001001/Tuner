using System;
using System.Threading;
using Android.Media;

namespace Tunerv1._0
{
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
        private Thread _displayFrequencyThread;
        private SoundAnalyzer _analyzer;
        private int _baseGraph;
        private int _lastSample;
        private double[] _audioBuffer;
        private float _threshold;
        private float[] _graphBuffer;
        private bool _isCapturing;
        private double _freq;

        private const int RefreshPeriod = 30;
        private const int NSecGraph = 5;
        private const float MaxAmplitude = 32767.0f;
        private const int SamplePerRefresh = 20;
        private const int GraphBuffSize = NSecGraph * 1000 / RefreshPeriod * SamplePerRefresh;
        private const int BarSteps = 30;
        private const float MicThreshold = BarSteps * 0.05f;
        private const int SampleRate = 44100;
        private const int DeltaMills = 30;
        private const int NFrames = (SampleRate * DeltaMills / 1000);
        private const int FramesPerBuffer = NFrames;
        private readonly object _lockObject = new object();
        
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
                _displayFrequencyThread.Join();
            }
        }
        
        public void StartCapture()
        {
            if (_isCapturing)
            {
                return;
            }
            
            ChannelIn channelConfig = ChannelIn.Stereo;
            Encoding audioFormat = Encoding.Pcm16bit;
            _bufferSize = AudioRecord.GetMinBufferSize(SampleRate, channelConfig, audioFormat);
            _audioRecord = new AudioRecord(
                AudioSource.Mic,
                SampleRate,
                channelConfig,
                audioFormat,
                _bufferSize);
            
            _threshold = 0;
            _baseGraph = 0;
            _lastSample = 0;
            _graphBuffer = new float[GraphBuffSize + 1];
            _audioBuffer = new double[FramesPerBuffer];
            _analyzer = new SoundAnalyzer(_audioBuffer, SampleRate);
            _soundCaptureEvent = new AutoResetEvent(false);
            _displayFrequencyEvent = new AutoResetEvent(false);
            _soundCaptureThread = new Thread(SoundCaptureThreadLoop);
            _displayFrequencyThread = new Thread(DisplayFrequencyThreadLoop);
            
            _isCapturing = true;
            _audioRecord.StartRecording();
            _soundCaptureThread.Start();
            _soundCaptureEvent.Set();
            _displayFrequencyThread.Start();
        }
        
        private void SoundCaptureThreadLoop()
        {
            try
            {
                while (_isCapturing)
                {
                    _soundCaptureEvent.WaitOne();
                    byte[] buffer = new byte[_bufferSize];
                    int bytesRead = _audioRecord.Read(buffer, 0, buffer.Length);
                    float[] samples = new float[bytesRead / 2];
                    
                    for (int i = 0, j = 0; i < bytesRead; i += 2, j++)
                    {
                        samples[j] = BitConverter.ToInt16(buffer, i);
                    }

                    for (int i = 0; i < FramesPerBuffer; i++)
                    {
                        lock (_lockObject)
                        {
                            _audioBuffer[i] = (double)(samples[i * 2] + samples[i * 2 + 1]) / 2;
                        }
                    }

                    lock (_lockObject)
                    {
                        _freq = _analyzer.CalculateFrequency();
                    }
                    
                    _displayFrequencyEvent.Set();
                }
            }
            finally
            {
                _audioRecord.Stop();
            }
        }

        float GetSample(int i)
        {
            if (i > FramesPerBuffer) return 0;
            float sampleValue;
            lock (_lockObject)
            {
                sampleValue = (float)Math.Abs(_audioBuffer[i]);
            }

            return sampleValue;
        }

        private void DisplayFrequencyThreadLoop()
        {
            while (_isCapturing)
            {
                _displayFrequencyEvent.WaitOne();
                _baseGraph = (_baseGraph + SamplePerRefresh) % (2 * GraphBuffSize + 1);
                for (int i = 0, j = 0; i < SamplePerRefresh; i++, j += NFrames / SamplePerRefresh)
                {
                    _graphBuffer[(_lastSample + i) % (GraphBuffSize + 1)] = GetSample(j);
                }

                _lastSample = (_lastSample + SamplePerRefresh) % (2 * GraphBuffSize + 1);
                _threshold = 0f;

                for (int i = 0; i < SamplePerRefresh; i++)
                {
                    var tmpThreshold = _graphBuffer[(_lastSample + GraphBuffSize - i) % (GraphBuffSize + 1)] /
                        MaxAmplitude * BarSteps;
                    if (tmpThreshold > _threshold)
                    {
                        _threshold = tmpThreshold;
                    }
                }

                if (_threshold > MicThreshold)
                {
                    lock (_lockObject)
                    {
                        OnFrequencyDetected(new FrequencyDetectedEventArgs(_freq));
                    }
                }

                _soundCaptureEvent.Set();

            }
        }
    }
}