using System;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Tuner
{
    public class SoundAnalyzer
    {
        private readonly int _audioBufferLength;
        private readonly int _fs;
        private readonly int _fftBufferLength;
        private readonly Complex32[] _fftIn;
        private readonly Complex32[] _fftOut;
        private readonly double[] _nsdf;
        private readonly double[] _pow;
        private readonly double[] _tmp;
        private readonly double[] _audioBuffer;

        public SoundAnalyzer(double[] audioBuffer, int fs)
        {
            _audioBuffer = audioBuffer;
            _audioBufferLength = audioBuffer.Length;
            _fs = fs;
            _fftBufferLength = _audioBufferLength * 2;
            _fftIn = new Complex32[_fftBufferLength];
            _fftOut = new Complex32[_fftBufferLength];
            _nsdf = new double[_audioBufferLength];
            _pow = new double[_audioBufferLength];
            _tmp = new double[_audioBufferLength];
        }

        void CalculateNsdFunction()
        {
            int i;
            for (i = 0; i < _audioBufferLength; i++)
            {
                _fftIn[i] = new Complex32((float)_audioBuffer[i], 0);
            }
            
            for (; i < _fftBufferLength; i++)
            {
                _fftIn[i] = new Complex32(0, 0);
            }

            Array.Copy(_fftIn, _fftOut, _fftBufferLength);
            Fourier.Forward(_fftOut, FourierOptions.NumericalRecipes);
            for (i = 0; i < _fftBufferLength; i++)
                _fftOut[i] *= _fftOut[i].Conjugate();
            Array.Copy(_fftOut, _fftIn, _fftBufferLength);
            Fourier.Inverse(_fftIn, FourierOptions.NumericalRecipes);

            _tmp[0] = 0;
            for (i = 0; i < _audioBufferLength; i++) {
                _pow[i] = _audioBuffer[i] * _audioBuffer[i];	    
                _tmp[0] += _pow[i] + _pow[i];			                            
            }
            
            int l = 0, r = _audioBufferLength - 1;
            for (i = 1; i < _audioBufferLength; i++) {
                _tmp[i] = _tmp[i - 1] - _pow[l] - _pow[r];
                _nsdf[i] = _fftIn[i].Real / _audioBufferLength / _tmp[i];
                l++; r--;
            }

            _nsdf[0] = _fftIn[0].Real / _audioBufferLength / _tmp[0];
        }
        
        double FitStationaryPntPoly2(int x0, int x1, int x2, double y0, double y1, double y2)
        {
            double f01 = (y1 - y0) / (x1 - x0),
                f12 = (y2 - y1) / (x2 - x1),
                f02 = (f12 - f01) / (x2 - x0);
            return (x0 + x1 - f01 / f02) / 2;
        }
        
        public double CalculateFrequency()
        {
            CalculateNsdFunction();
            var i = 0;
            while (i < _audioBufferLength && _nsdf[i] >= 0.0) i++;
            while (i < _audioBufferLength && _nsdf[i] <=  0.0) i++;

            int maxArg = i;
            while (i < _audioBufferLength && _nsdf[i] >= 0.0) {
                if (_nsdf[maxArg] < _nsdf[i] ) 
                    maxArg = i;
                i++;
            }

            while (i < _audioBufferLength) {
                if (_nsdf[maxArg] < _nsdf[i] - 0.03)
                    maxArg = i;
                i++;
            }

            if (maxArg == _nsdf.Length)
                maxArg--;
            
            if (maxArg == _nsdf.Length - 1)
            {
                return _fs / FitStationaryPntPoly2(
                    maxArg - 1, maxArg, maxArg, 
                    _nsdf[maxArg - 1], _nsdf[maxArg], _nsdf[maxArg]
                );
            }
            
            if (maxArg == 0)
            {
                return _fs / FitStationaryPntPoly2(
                    maxArg, maxArg, 1, 
                    _nsdf[maxArg], _nsdf[maxArg], _nsdf[1]
                );
            }

            return _fs / FitStationaryPntPoly2(
                maxArg - 1, maxArg, maxArg + 1, 
                _nsdf[maxArg - 1], _nsdf[maxArg], _nsdf[maxArg + 1]
            );
        }
    }
}