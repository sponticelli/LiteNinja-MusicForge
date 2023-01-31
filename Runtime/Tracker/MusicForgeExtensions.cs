namespace LiteNinja.MusicForge.Instruments
{
  public static class MusicForgeExtensions
  {
    /// <summary>
    /// Gets the numerical value of an octave
    /// </summary>
    /// <param name="octave">The octave to convert</param>
    /// <returns>The numerical value of the octave</returns>
    public static int GetOctaveNumber(this Octave octave)
    {
      return (int)octave;
    }
    
    /// <summary>
    /// Gets the numerical value of a base note
    /// </summary>
    /// <param name="note">The base note to convert</param>
    /// <returns>The numerical value of the base note</returns>
    public static int GetNoteNumber(this BaseNote note)
    {
      return (int)note;
    }
    
    /// <summary>
    /// Gets the numerical value of a note with a given base note and octave
    /// </summary>
    /// <param name="note">The base note of the note</param>
    /// <param name="octave">The octave of the note</param>
    /// <returns>The numerical value of the note</returns>
    public static int GetNote(this BaseNote note, Octave octave)
    {
      return (int)note + (int)octave * 12;
    }
  }

}