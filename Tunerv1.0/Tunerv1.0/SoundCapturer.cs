﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Media;
using Android.Util;

namespace Tunerv1._0
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
        private Queue<Sound> _queue;
        private AutoResetEvent _soundCaptureEvent;
        private AutoResetEvent _displayFrequencyEvent;
        private Thread _soundCaptureThread;
        private Thread _displayFrequencyThread;
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
        private List<Sound> _data;
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
            Encoding audioFormat = Encoding.PcmFloat;
            _bufferSize = AudioRecord.GetMinBufferSize(SampleRate, channelConfig, audioFormat);
            _audioRecord = new AudioRecord(
                AudioSource.Mic,
                SampleRate,
                channelConfig,
                audioFormat,
                _bufferSize);

            _buffer = new float[_bufferSize];
            _queue = new Queue<Sound>();
            _data = new List<Sound>();
            _audioBuffer = new double[FramesPerBuffer];
            _analyzer = new SoundAnalyzer(_audioBuffer,SampleRate);
            _soundCaptureEvent = new AutoResetEvent(false);
            _displayFrequencyEvent = new AutoResetEvent(false);
            _soundCaptureThread = new Thread(SoundCaptureThreadLoop);
            //_displayFrequencyThread = new Thread(DisplayFrequencyThreadLoop);

            _isCapturing = true;
            _audioRecord.StartRecording();
            _soundCaptureThread.Start();
            _soundCaptureEvent.Set();
            //_displayFrequencyThread.Start();
        }

        private static List<Sound> FindLongestSubsequence(List<Sound> values, double delta)
        {
            List<KeyValuePair<int, Sound>> indexedValues = new List<KeyValuePair<int, Sound>>();
            for (int i = 0; i < values.Count; i++)
            {
                indexedValues.Add(new KeyValuePair<int, Sound>(i, values[i]));
            }

            indexedValues.Sort((x, y) => x.Value.Frequency.CompareTo(y.Value.Frequency));

            List<KeyValuePair<int, Sound>> longestSubsequence = new List<KeyValuePair<int, Sound>>();
            for (int i = 0; i < indexedValues.Count; i++)
            {
                List<KeyValuePair<int, Sound>> curSubsequence = new List<KeyValuePair<int, Sound>>();
                curSubsequence.Add(indexedValues[i]);
                double maxValue = indexedValues[i].Value.Frequency + delta;

                for (int j = i + 1; j < indexedValues.Count; j++)
                {
                    if (indexedValues[j].Value.Frequency <= maxValue)
                    {
                        curSubsequence.Add(indexedValues[j]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (curSubsequence.Count > longestSubsequence.Count)
                {
                    longestSubsequence = curSubsequence;
                }
            }

            longestSubsequence.Sort((x, y) => x.Key.CompareTo(y.Key));

            return longestSubsequence.Select(kv => kv.Value).ToList();
        }
        private void SoundCaptureThreadLoop()
        {
            try
            {
                Queue<double> newFreq = new Queue<double>();
                Queue<double> prevFreq = new Queue<double>();
                int lowBorder = 20;
                int highBorder = 20000;
                int step = 10;
                int[] freqCount = new int[(highBorder - lowBorder) / step];
                while (_isCapturing)
                {
                    
                    _audioRecord.Read(_buffer, 0, _buffer.Length, (int)AudioRecordReadOptions.NonBlocking);

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
                    
                    newFreq.Enqueue(newSound.Frequency);
                    freqCount[(int)((newSound.Frequency - lowBorder) / step)]++;
                    
                    if (newFreq.Count() >= 5)
                    {
                        double freq = newFreq.Dequeue();
                        int rangeIdx = (int)((freq - lowBorder) / step);
                        int count = 0;
                        count += freqCount[rangeIdx];
                        count += rangeIdx > 0 ? freqCount[rangeIdx - 1] : 0;
                        count += rangeIdx > freqCount.Length ? freqCount[rangeIdx - 1] : 0;

                        if (count >= 6)
                        {
                            int halfRangeIdx = (int)((freq / 2 - lowBorder) / step);
                            if (!(freq >= 130 && freq <= 160 && freqCount[halfRangeIdx] >= 3))
                            {
                                OnFrequencyDetected(new FrequencyDetectedEventArgs(freq));
                            }
                            
                        }
                        
                        prevFreq.Enqueue(freq);
                        
                        if (prevFreq.Count() > 5)
                        {
                            double removedFreq = prevFreq.Dequeue();
                            rangeIdx = (int)((removedFreq - lowBorder) / step);
                            freqCount[rangeIdx]--;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("SoundCapturer", $"Error occurred: {e.Message}");
                _audioRecord.Stop();
            }
        }


        /*private void DisplayFrequencyThreadLoop()
        {
            while (_isCapturing)
            {
                if (_queue.Count > 0)
                {
                    Sound elem;
                    lock (_lockObject)
                    {
                        elem = _queue.Dequeue();
                    }

                    if (elem.Volume > MicThreshold)
                    {
                        OnFrequencyDetected(new FrequencyDetectedEventArgs(elem.Frequency));
                    }
                }

            }
        }*/
    }
}