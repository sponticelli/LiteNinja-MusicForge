namespace LiteNinja.MusicForge.Instruments
{
  /// <summary>
  /// Interface for an instrument class, which defines the basic functionality for generating musical notes.
  /// </summary>
  public interface IInstrument
  {
    /// <summary>
    /// Generates the wave for a given note played by this instrument.
    /// </summary>
    /// <param name="samplingRate">The sampling rate of the generated wave, in samples per second.</param>
    /// <param name="note">The note to be played by this instrument.</param>
    /// <param name="length">The length of the note, in seconds.</param>
    /// <returns>The generated wave as an array of floats representing the amplitude at each sample.</returns>
    float[] GetNoteWave(int samplingRate, int note, float length);
  }
}