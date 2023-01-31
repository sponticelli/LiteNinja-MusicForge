using System;
using UnityEngine;

namespace LiteNinja.MusicForge
{
  [Serializable]
  public class Waveform : IWaveform
  {
    [SerializeField] private WaveformType _type = WaveformType.Sine;
    [SerializeField] private float _offset = 0f;
    [SerializeField] private float _noise = 0.0f;

    public WaveformType Type
    {
      get => _type;
      set => _type = value;
    }

    public float Noise
    {
      get => _noise;
      set => _noise = value;
    }
    
    public float Offset
    {
      get => _offset;
      set => _offset = value;
    }

    /// <summary>
    /// Returns the amplitude of the waveform at a given time.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    public float GetAmplitude(float time)
    {
      time = Mathf.Repeat(time + _offset, 1.0f);
      return _type switch
      {
        WaveformType.Sine => Mathf.Sin(time * Mathf.PI * 2),
        WaveformType.Square => Mathf.Sign(Mathf.Sin(time * Mathf.PI * 2)),
        WaveformType.Sawtooth => 2 * (time - Mathf.Floor(time + 0.5f)),
        WaveformType.Triangle => (Mathf.Abs(0.5f - time) * 4) - 1,
        WaveformType.SineCubed => Mathf.Pow(Mathf.Sin(time * Mathf.PI * 2), 3),
        WaveformType.SemiCircle => time < 0.5f
          ? Mathf.Sqrt(Mathf.Sin((time % 0.5f) * Mathf.PI * 2))
          : -Mathf.Sqrt(Mathf.Sin((time % 0.5f) * Mathf.PI * 2)),
        _ => 0
      };
    }

    /// <summary>
    /// Returns the amplitude of the waveform at a given time with noise.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    public float GetAmplitudeWithNoise(float time)
    {
      return Mathf.Clamp(Mathf.Lerp(GetAmplitude(time), UnityEngine.Random.Range(-2.0f, 2.0f), _noise), -1, 1);
    }
  }
}