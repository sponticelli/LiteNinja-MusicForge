using System;
using UnityEngine;

namespace LiteNinja.MusicForge.Instruments
{
  [Serializable]
  public class Instrument : IInstrument
  {
    
    
    [SerializeField] private CompositeWaveform _waveform;
    
    
    // getters and setters
    public CompositeWaveform Waveform
    {
      get => _waveform;
      set => _waveform = value;
    }
    
    public float[] GetNoteWave(int samplingRate, int note, float length)
    {
      var samples = (int)(length * samplingRate);
      var wave = new float[samples];
        
      for (var i = 0; i < samples; i++)
      {
        var t = (float)i / samplingRate;
        wave[i] = _waveform.GetAmplitudeWithNoise(t);
      }
        
      return wave;
    }
  }
}