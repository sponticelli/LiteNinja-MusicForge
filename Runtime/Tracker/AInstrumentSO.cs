using System;
using UnityEngine;

namespace LiteNinja.MusicForge
{
  [Serializable]
  public abstract class AInstrumentSO : ScriptableObject, IInstrument
  {
    [SerializeField] private string _name;

    public string Name => _name;
    
    public abstract void ClearCache();
    public abstract float GetWaveformAmplitude(float time, bool applyNoise, bool firstWaveform);
    public abstract float GetWaveformVolumeAmplitudeMultiplier(float timeInSecondsThroughWaveform);

    public abstract float GetPitchChange(int noteSamples, int samples, bool applyOrnament, int pitchAttackSamples, int pitchReleaseSamples,
      int pitchWaveformSamples, int ornamentSamples);
    
    public abstract float[] GetNoteWave(int frequency, int note, float length, bool useCache);
  }
}