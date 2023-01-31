using System;
using System.Linq;
using UnityEngine;

namespace LiteNinja.MusicForge
{
  [Serializable]
  public class CompositeWaveform : IWaveform
  {
    [SerializeField] private WeightedWaveform[] _weightedWaveforms = Array.Empty<WeightedWaveform>();

    public WeightedWaveform[] Waveforms
    {
      get => _weightedWaveforms;
      set => _weightedWaveforms = value;
    }
    
    /// <summary>
    /// Returns the amplitude of the waveform at a given time.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    public float GetAmplitude(float time)
    {
      var totalWeight = _weightedWaveforms.Sum(weightedWaveform => weightedWaveform.Weight);
      if (totalWeight == 0)
      {
        return 0;
      }
      return _weightedWaveforms.Sum(t => t.Waveform.GetAmplitude(time) * t.Weight / totalWeight);
    }

    /// <summary>
    /// Returns the amplitude of the waveform at a given time with noise.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    public float GetAmplitudeWithNoise(float time)
    {
      var totalWeight = _weightedWaveforms.Sum(weightedWaveform => weightedWaveform.Weight);
      if (totalWeight == 0)
      {
        return 0;
      }
      return _weightedWaveforms.Sum(t => t.Waveform.GetAmplitudeWithNoise(time) * t.Weight / totalWeight);
    }
    
    [Serializable]
    public class WeightedWaveform
    {
      [SerializeField] private Waveform _waveform;
      [SerializeField] private float _weight = 1f;

      public Waveform Waveform
      {
        get => _waveform;
        set => _waveform = value;
      }

      public float Weight
      {
        get => _weight;
        set => _weight = value;
      }
    }
  }
  
  
}