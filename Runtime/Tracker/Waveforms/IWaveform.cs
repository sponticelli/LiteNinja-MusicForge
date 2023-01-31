namespace LiteNinja.MusicForge
{
  public interface IWaveform
  {
    float GetAmplitude(float time);
    float GetAmplitudeWithNoise(float time);
  }
}