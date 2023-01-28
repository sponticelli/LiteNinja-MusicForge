using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MusicForge
{
  [CreateAssetMenu(menuName = "LiteNinja/Music Forge/Create Instrument", fileName = "InstrumentSO", order = 0)]
  [Serializable]
  public class InstrumentSO : AInstrumentSO
  {
    [SerializeField] private WaveformData _waveformData = new WaveformData();
    public WaveformData WaveformInfo => _waveformData;

    [Header("Volume")] [SerializeField] private bool _useVolume;
    [SerializeField] private VolumeData _volumeData;
    public VolumeData VolumeInfo => _volumeData;
    public bool UseVolume => _useVolume;

    [Header("Pitch")] [SerializeField] private bool _usePitch;
    [SerializeField] private PitchData _pitchData;
    public PitchData PitchInfo => _pitchData;
    public bool UsePitch => _usePitch;

    [Header("Ornaments")] [SerializeField] private bool _useOrnaments;
    [SerializeField] private OrnamentData _ornamentData;
    public OrnamentData OrnamentInfo => _ornamentData;
    public bool UseOrnaments => _useOrnaments;

    [Header("Effects - Chorus")] [SerializeField]
    private bool _useChorus;

    [SerializeField] private ChorusData _chorusData;
    public ChorusData ChorusInfo => _chorusData;
    public bool UseChorus => _useChorus;


    [Header("Effects - Echo")] [SerializeField]
    private bool _useEcho;

    [SerializeField] private EchoData _echoData;
    public EchoData EchoInfo => _echoData;
    public bool UseEcho => _useEcho;


    [Header("Preview")] [SerializeField] private PreviewData _previewData;
    public PreviewData PreviewInfo => _previewData;

    private List<int> _cachedNotePitches;
    private List<float> _cachedNoteLengths;
    private List<float[]> _cachedNoteData;

    public override void ClearCache()
    {
      _cachedNotePitches = null;
      _cachedNoteLengths = null;
      _cachedNoteData = null;
    }

    public override float GetWaveformAmplitude(float time, bool applyNoise, bool firstWaveform)
    {
      var thisWaveform = firstWaveform ? _waveformData.Waveform1 : _waveformData.Waveform2;
      float amplitude;
      switch (thisWaveform)
      {
        case Waveform.Square:
          amplitude = time < 0.5 ? 1 : -1;
          break;
        case Waveform.Sawtooth:
          amplitude = (time * 2) - 1;
          break;
        case Waveform.Sine:
          amplitude = Mathf.Sin(time * Mathf.PI * 2);
          break;
        case Waveform.Triangle:
          amplitude = (Mathf.Abs(0.5f - time) * 4) - 1;
          break;
        case Waveform.SineCubed:
        {
          var sine = Mathf.Sin(time * Mathf.PI * 2);
          return sine * sine * sine;
        }
        case Waveform.TriangleCubed:
        {
          var triangle = (Mathf.Abs(0.5f - time) * 4) - 1;
          return triangle * triangle * triangle;
        }
        case Waveform.SemiCircle:
        {
          var sine = Mathf.Sin((time % 0.5f) * Mathf.PI * 2);
          return time < 0.5f ? Mathf.Sqrt(sine) : -Mathf.Sqrt(sine);
        }
        default:
          Debug.LogError("Waveform not found!");
          return 0;
      }

      return applyNoise
        ? Mathf.Clamp(Mathf.Lerp(amplitude, UnityEngine.Random.Range(-2.0f, 2.0f), _waveformData.Noise), -1, 1)
        : amplitude;
    }

    public override float GetWaveformVolumeAmplitudeMultiplier(float timeInSecondsThroughWaveform)
    {
      if (!_useVolume) return 1;
      return _volumeData.GetWaveformVolumeAmplitudeMultiplier(timeInSecondsThroughWaveform);
    }

    private float GetPitchWaveform(int samples, int pitchWaveformSamples)
    {
      switch (_pitchData.Waveform)
      {
        case Waveform.Sine:
          return Mathf.Sin(((float)(samples % pitchWaveformSamples) / pitchWaveformSamples) * Mathf.PI * 2) *
                 _pitchData.WaveformAmplitude;
        case Waveform.Square:
          return samples % pitchWaveformSamples < pitchWaveformSamples / 2
            ? _pitchData.WaveformAmplitude
            : -_pitchData.WaveformAmplitude;
        case Waveform.Sawtooth:
          return (((float)(samples % pitchWaveformSamples) / pitchWaveformSamples) - 0.5f) *
                 _pitchData.WaveformAmplitude * 2;
        case Waveform.Triangle:
          return (Mathf.Abs(0.5f - ((float)(samples % pitchWaveformSamples) / pitchWaveformSamples)) - 0.25f) *
                 _pitchData.WaveformAmplitude * 4;
        case Waveform.SineCubed:
        {
          var sine = Mathf.Sin(((float)(samples % pitchWaveformSamples) / pitchWaveformSamples) * Mathf.PI * 2);
          return sine * sine * sine * _pitchData.WaveformAmplitude;
        }
        case Waveform.TriangleCubed:
        {
          var triangle = Mathf.Abs(0.5f - ((float)(samples % pitchWaveformSamples) / pitchWaveformSamples)) - 0.25f;
          return triangle * triangle * triangle * _pitchData.WaveformAmplitude * 64;
        }
        case Waveform.SemiCircle:
        default:
        {
          if (_volumeData.Waveform != Waveform.SemiCircle) return 0;
          var sine = Mathf.Sin(((float)(samples % (pitchWaveformSamples / 2)) / pitchWaveformSamples) * Mathf.PI * 2) *
                     _pitchData.WaveformAmplitude * _pitchData.WaveformAmplitude;
          return samples % pitchWaveformSamples < pitchWaveformSamples / 2 ? Mathf.Sqrt(sine) : -Mathf.Sqrt(sine);
        }
      }
    }
    
    public override float GetPitchChange(int noteSamples, int samples, bool applyOrnament, int pitchAttackSamples,
      int pitchReleaseSamples,
      int pitchWaveformSamples, int ornamentSamples)
    {
      return (_usePitch
               ? (
                 //Apply the initial/release pitch change.
                 (samples < pitchAttackSamples ? _pitchData.Attack * (1 - (samples / (float)pitchAttackSamples))
                   : (samples > noteSamples && pitchReleaseSamples > 0
                     ? _pitchData.Release * ((samples - noteSamples) / (float)pitchReleaseSamples) : 0)) +
                 //Apply the pitch waveform.
                 (pitchWaveformSamples > 0 ? GetPitchWaveform(samples, pitchWaveformSamples) : 0)) : 0) +
             //Apply the ornament.
             (_useOrnaments && applyOrnament && ornamentSamples > 0 && _ornamentData.SemitoneOffsets.Count > 0
               ? _ornamentData.SemitoneOffsets[(int)((samples % ornamentSamples) * (1 / (float)ornamentSamples) * 
                                                     _ornamentData.SemitoneOffsets.Count)]
               : 0);
    }

    public override float[] GetNoteWave(int frequency, int note, float length, bool useCache)
    {
      //Cache notes if required - e.g., when getting the waveform of an entire song to speed it up.
      if (useCache)
      {
        if (_cachedNotePitches == null || _cachedNoteLengths == null || _cachedNoteData == null)
        {
          _cachedNotePitches = new List<int>();
          _cachedNoteLengths = new List<float>();
          _cachedNoteData = new List<float[]>();
        }
        else
        {
          for (var i = 0; i < _cachedNotePitches.Count; i++)
          {
            if (_cachedNotePitches[i] == note && Mathf.Abs(_cachedNoteLengths[i] - length) < 0.0001f)
              return _cachedNoteData[i];
          }
        }
      }

      //Calculate the length of the note, taking into account attack, decay, sustain, release and echo.
      var attackLength = _useVolume ? (int)(_volumeData.AttackTime * frequency) : 0;
      var decayLength = _useVolume ? (int)(_volumeData.DecayTime * frequency) : 0;
      var entireNoteLength = (int)(length * frequency);
      var releaseLength = _useVolume ? (int)(_volumeData.ReleaseTime * frequency) : 0;
      var echoLength = _useEcho ? Mathf.CeilToInt(_echoData.EchoTimes * _echoData.EchoTimeDelay * frequency) : 0;
      var pitchAttackSamples = Math.Max((int)(_pitchData.AttackTime * frequency), 1);
      var pitchReleaseSamples = Math.Max((int)(_pitchData.ReleaseTime * frequency), 1);
      var pitchWaveformSamples = (int)(_pitchData.WaveformWavelength * frequency);
      var ornamentSamples = (int)(_ornamentData.Time * frequency);

      //Set up an array of samples for the note.
      var wave = new float[entireNoteLength + releaseLength + echoLength + 1];
      Array.Clear(wave, 0, wave.Length);

      //Get the overall volume of the note - the sustain level if volumes are being used or 1 otherwise.
      //If using chorus, divide the volume by three because the chorus consists of the wave being replicated three times.
      var sustainLevel = _useVolume ? _volumeData.SustainLevel : 1;
      var volume = _useChorus ? 1f / 3f : 1;

      //Set the echo offsets and volumes.
      var echoOffsets = new int[_useEcho ? _echoData.EchoTimes : 0];
      var echoVolumes = new float[_useEcho ? _echoData.EchoTimes : 0];
      if (_useEcho)
        for (var i = 0; i < _echoData.EchoTimes; i++)
        {
          echoOffsets[i] = Mathf.RoundToInt((i + 1) * _echoData.EchoTimeDelay * frequency) + 1;
          echoVolumes[i] = 1 - ((float)(i + 1) / (_echoData.EchoTimes + 1));
        }

      //Loop three times if using chorus, once if not.
      float numerator = _volumeData.ReleaseDropOff switch
      {
        ReleaseDropOff.ShallowCurve => (frequency / 3),
        ReleaseDropOff.MediumCurve => (frequency / 12),
        _ => frequency / 60
      };
      var applyNoise = _waveformData.Noise > 0.0001f;
      float attackLengthFloat = attackLength, decayLengthFloat = decayLength, releaseLengthFloat = releaseLength;
      var oneMinusSustainLevel = 1 - sustainLevel;
      var trackerFrequencyAdjusted = frequency / 440f;
      float timeInSecondsThroughWaveform = 0;
      var oneOverWaveformMultipliedByFrequency = 1 / (_volumeData.WaveformWavelength * frequency);
      var attackLengthPlusDecayLength = attackLength + decayLength;
      for (var j = _useChorus ? -1 : 0; j < (_useChorus ? 2 : 1); j++)
      {
        //Loop over the note samples and get the amplitude.
        var waveAmplitude = _waveformData.Waveform1Offset;
        var wave2Amplitude = _waveformData.Waveform2Offset;
        var thisNote = note + (j * _chorusData.PitchChange) - 57;
        for (var i = 0; i < entireNoteLength + releaseLength; i++)
        {
          float amplitude;
          if (i < attackLength)
            amplitude = i / attackLengthFloat;
          else if (i < attackLengthPlusDecayLength)
            amplitude = 1 - (((i - attackLengthFloat) / decayLengthFloat) * oneMinusSustainLevel);
          else if (i < entireNoteLength)
            amplitude = sustainLevel;
          else if (_volumeData.ReleaseDropOff == ReleaseDropOff.Linear)
            amplitude = sustainLevel - (((i - entireNoteLength) / releaseLengthFloat) * sustainLevel);
          else
            amplitude = (numerator / (i - entireNoteLength + numerator)) * sustainLevel *
                        (1 - ((i - entireNoteLength) / releaseLengthFloat));
          var waveformAmplitude = GetWaveformAmplitude(waveAmplitude, applyNoise, true);
          if (_waveformData.NumberOfWaveforms == WaveformData.WaveformsCount.Two)
            waveformAmplitude = (waveformAmplitude * (1 - _waveformData.BlendAmount)) +
                                (GetWaveformAmplitude(wave2Amplitude, applyNoise, false) *
                                 _waveformData.BlendAmount);
          amplitude *= GetWaveformVolumeAmplitudeMultiplier(timeInSecondsThroughWaveform) * waveformAmplitude * volume;
          timeInSecondsThroughWaveform += oneOverWaveformMultipliedByFrequency;
          if (timeInSecondsThroughWaveform > 1)
            timeInSecondsThroughWaveform--;
          wave[i + 1] += amplitude;
          if (_useEcho)
          {
            for (var k = 0; k < _echoData.EchoTimes; k++)
            {
              wave[i + echoOffsets[k]] += amplitude * echoVolumes[k];
            }
          }

          var waveAmplitudeChange = Mathf.Pow(1.0594630943592952645618252949463f, thisNote +
            GetPitchChange(entireNoteLength, i, true, pitchAttackSamples, pitchReleaseSamples,
              pitchWaveformSamples, ornamentSamples)) / trackerFrequencyAdjusted;
          waveAmplitude += waveAmplitudeChange;
          if (waveAmplitude > 1) waveAmplitude -= 1;
          if (_waveformData.NumberOfWaveforms == WaveformData.WaveformsCount.Two)
          {
            wave2Amplitude += waveAmplitudeChange;
            if (wave2Amplitude > 1) wave2Amplitude -= 1;
          }
        }
      }

      //Cache the waveform data if required.
      if (useCache)
      {
        _cachedNotePitches.Add(note);
        _cachedNoteLengths.Add(length);
        _cachedNoteData.Add(wave);
      }

      //Return the wave.
      return wave;
    }


    [Serializable]
    public class WaveformData 
    {
      [Tooltip("Choose a single waveform or blend two waveforms to create unique instrument sounds. Blending options include waveform shape, offsets, and blend amount.")]
      [SerializeField] private WaveformsCount _numberOfWaveforms = WaveformsCount.One;
      [Tooltip("Adjust the blend ratio between the first and second waveforms to shape the final sound.")]
      [SerializeField][Range(0f, 1f)] private float _blendAmount = 0.5f;

      [Header("Waveform 1")] 
      [Tooltip("Choose a waveform for your instrument. Experiment with different options for unique sounds. Preview in graph.")]
      [SerializeField] private Waveform _waveform1 = Waveform.Square;
      [Tooltip("Adjust waveform offset to shape blended sound when using multiple waveforms.")]
      [SerializeField][Range(0f, 1f)] private float _waveform1Offset;

      [Header("Waveform 2")] 
      [Tooltip("Choose a waveform for your instrument. Experiment with different options for unique sounds. Preview in graph.")]
      [SerializeField] private Waveform _waveform2 = Waveform.Square;
      [Tooltip("Adjust waveform offset to shape blended sound when using multiple waveforms.")]
      [SerializeField][Range(0f, 1f)] private float _waveform2Offset;

      [Header("Noise")] 
      [Tooltip("Noise simulates static background sound, like an old TV. It can be used to create drumbeats, and at 100% it sounds like a snare drum. Use it for percussion sounds, but set to 0% for instruments that play different pitches.")]
      [SerializeField][Range(0f, 1f)] private float _noise;

      public WaveformsCount NumberOfWaveforms => _numberOfWaveforms;
      public float BlendAmount => _blendAmount;

      public Waveform Waveform1 => _waveform1;
      public float Waveform1Offset => _waveform1Offset;

      public Waveform Waveform2 => _waveform2;
      public float Waveform2Offset => _waveform2Offset;

      public float Noise => _noise;

      public enum WaveformsCount
      {
        One,
        Two
      };
      
    }

    [Serializable]
    public class VolumeData
    {
      [SerializeField] private float _attackTime = 0.05f;
      [SerializeField] private float _decayTime;
      [SerializeField] [Range(0f, 1f)] private float _sustainLevel = 1.0f;
      [SerializeField] private float _releaseTime = 0.1f;
      [SerializeField] private ReleaseDropOff _releaseDropOff = ReleaseDropOff.MediumCurve;
      [SerializeField] private Waveform _waveform = Waveform.Sine;
      [SerializeField] [Range(0f, 1f)] private float _waveformAmplitude;
      [SerializeField] private float _waveformWavelength;

      public float AttackTime => _attackTime;
      public float DecayTime => _decayTime;
      public float SustainLevel => _sustainLevel;
      public float ReleaseTime => _releaseTime;
      public ReleaseDropOff ReleaseDropOff => _releaseDropOff;
      public Waveform Waveform => _waveform;
      public float WaveformAmplitude => _waveformAmplitude;
      public float WaveformWavelength => _waveformWavelength;

      public float GetWaveformVolumeAmplitudeMultiplier(float timeInSecondsThroughWaveform)
      {
        if (_waveformAmplitude < 0.0001f || _waveformWavelength < 0.0001f)
          return 1;
        switch (_waveform)
        {
          case Waveform.Square:
            return timeInSecondsThroughWaveform < 0.5f ? (_waveformAmplitude + 1) : (1 - _waveformAmplitude);
          case Waveform.Sine:
            return (Mathf.Sin(timeInSecondsThroughWaveform * Mathf.PI * 2) * _waveformAmplitude) + 1;
          case Waveform.Sawtooth:
            return ((timeInSecondsThroughWaveform - 0.5f) * _waveformAmplitude * 2) + 1;
          case Waveform.Triangle:
            return ((Mathf.Abs(0.5f - timeInSecondsThroughWaveform) - 0.25f) * _waveformAmplitude * 4) + 1;
          case Waveform.SineCubed:
          {
            var sine = Mathf.Sin(timeInSecondsThroughWaveform * Mathf.PI * 2);
            return (sine * sine * sine * _waveformAmplitude) + 1;
          }
          case Waveform.TriangleCubed:
          {
            var triangle = Mathf.Abs(0.5f - timeInSecondsThroughWaveform) - 0.25f;
            return (triangle * triangle * triangle * _waveformAmplitude * 64) + 1;
          }
          case Waveform.SemiCircle:
          {
            var sine = Mathf.Sin((timeInSecondsThroughWaveform % 0.5f) * Mathf.PI * 2) * _waveformAmplitude *
                       _waveformAmplitude;
            return (timeInSecondsThroughWaveform < 0.5f ? Mathf.Sqrt(sine) : -Mathf.Sqrt(sine)) + 1;
          }
          default:
            Debug.LogError("Volume waveform not found!");
            return 1;
        }
      }
    }

    [Serializable]
    public class PitchData
    {
      [SerializeField] [Range(-36, 36)] private float _attack;
      [SerializeField] private float _attackTime;
      [SerializeField] [Range(-36, 36)] private float _release;
      [SerializeField] private float _releaseTime;
      
      [SerializeField] private Waveform _waveform = Waveform.Sine;
      [SerializeField] [Range(0,36)] private float _waveformAmplitude;
      [SerializeField] private float _waveformWavelength;

      public float Attack => _attack;
      public float AttackTime => _attackTime;
      public float Release => _release;
      public float ReleaseTime => _releaseTime;
      
      public Waveform Waveform => _waveform;
      public float WaveformAmplitude => _waveformAmplitude;
      public float WaveformWavelength => _waveformWavelength;
    }

    [Serializable]
    public class OrnamentData
    {
      [SerializeField] private float _time = 0.1f;
      [SerializeField] private List<float> _semitoneOffsets = new();

      public float Time => _time;
      public List<float> SemitoneOffsets => _semitoneOffsets;
    }

    [Serializable]
    public class ChorusData
    {
      [SerializeField] private float _pitchChange = 0.05f;

      public float PitchChange => _pitchChange;
    }

    [Serializable]
    public class EchoData
    {
      [SerializeField] private int _echoTimes = 10;
      [SerializeField] private float _echoTimeDelay = 0.1f;

      public int EchoTimes => _echoTimes;
      public float EchoTimeDelay => _echoTimeDelay;
    }

    [Serializable]
    public class PreviewData
    {
      [SerializeField] private int _note;
      
      [SerializeField] [Range(0,8)] private int _noteOctave = 4;
      [SerializeField] private float _noteLength = 1;
      [SerializeField] private int _frequency = 44100;
      
      public int Note => _note;
      public int NoteOctave => _noteOctave;
      public float NoteLength => _noteLength;
      
      public int Frequency => _frequency;
    }
  }
}