namespace LiteNinja.MusicForge
{
  /// <summary>
  /// Enumeration of waveforms that can be generated by the instrument.
  /// </summary>
  public enum Waveform
  {
    /// <summary>
    /// A sawtooth waveform is a type of waveform that starts at 0, increases linearly to a maximum value, then drops back to 0 and starts again.
    /// </summary>
    Sawtooth,
    /// <summary>
    /// A sine waveform is a type of waveform that oscillates between a minimum and maximum value, following the shape of a sine curve.
    /// </summary>
    Sine,
    /// <summary>
    /// A square waveform is a type of waveform that alternates abruptly between a minimum and maximum value.
    /// </summary>
    Square,
    /// <summary>
    /// A triangle waveform is a type of waveform that starts at a minimum value, increases linearly to a maximum value, then decreases linearly back to the minimum value.
    /// </summary>
    Triangle,
    /// <summary>
    /// A sine cubed waveform is a type of waveform that oscillates between a minimum and maximum value, following the shape of a sine curve, but with the added effect of cubing the value.
    /// </summary>
    SineCubed,
    /// <summary>
    /// A triangle cubed waveform is a type of waveform that starts at a minimum value, increases linearly to a maximum value, then decreases linearly back to the minimum value, but with the added effect of cubing the value.
    /// </summary>
    TriangleCubed,
    /// <summary>
    /// A semi-circle waveform is a type of waveform that oscillates between a minimum and maximum value, following the shape of a semi-circle.
    /// </summary>
    SemiCircle
  }
}