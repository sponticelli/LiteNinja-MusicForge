namespace LiteNinja.MusicForge
{
  /// <summary>
  /// Interface for an instrument class, which defines the basic functionality for generating musical notes.
  /// </summary>
  public interface IInstrument
  {
    /// <summary>
    /// The name of the instrument.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Clears any cached data stored by the instrument.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Return the amplitude of a wave at a given time, from 0 to 1.
    /// </summary>
    /// <param name="time">The time in seconds through the waveform.</param>
    /// <param name="applyNoise">A flag indicating whether to apply noise to the amplitude.</param>
    /// <param name="firstWaveform">A flag indicating whether the waveform is the first one to be played.</param>
    /// <returns>The amplitude of the waveform at the specified time.</returns>
    float GetWaveformAmplitude(float time, bool applyNoise, bool firstWaveform);

    /// <summary>
    /// Return the amount to adjust the amplitude of a wave based on the volume wave settings.
    /// </summary>
    /// <param name="timeInSecondsThroughWaveform">The time in seconds through the waveform.</param>
    /// <returns>The volume amplitude multiplier for the waveform at the specified time.</returns>
    float GetWaveformVolumeAmplitudeMultiplier(float timeInSecondsThroughWaveform);

    /// <summary>
    /// Returns the pitch change for a specific note.
    /// </summary>
    /// <param name="noteSamples">The number of samples for the note.</param>
    /// <param name="samples">The total number of samples for the instrument.</param>
    /// <param name="applyOrnament">A flag indicating whether to apply an ornament to the note.</param>
    /// <param name="pitchAttackSamples">The number of samples for the pitch attack.</param>
    /// <param name="pitchReleaseSamples">The number of samples for the pitch release.</param>
    /// <param name="pitchWaveformSamples">The number of samples for the pitch waveform.</param>
    /// <param name="ornamentSamples">The number of samples for the ornament.</param>
    /// <returns>The pitch change for the specified note.</returns>
    float GetPitchChange(int noteSamples, int samples, bool applyOrnament, int pitchAttackSamples,
      int pitchReleaseSamples, int pitchWaveformSamples, int ornamentSamples);

    /// <summary>
    /// Construct a wave for a given note played with this instrument.
    /// </summary>
    /// <param name="frequency">The frequency of the note in hertz.</param>
    /// <param name="note">The note number to generate the waveform for. (e.g. C4 = 60, A4 = 69, etc.)</param>
    /// <param name="length">The length of the note in seconds.</param>
    /// <param name="useCache">Determines whether or not to use a cached version of the waveform if it has been previously generated.</param>
    /// <returns>A float array representing the waveform for the given note and length.</returns
    float[] GetNoteWave(int frequency, int note, float length, bool useCache);
  }
}