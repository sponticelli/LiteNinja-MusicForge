using System;
using System.Collections;
using System.Collections.Generic;
using LiteNinja.MusicForge.Utils;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace LiteNinja.MusicForge
{
  /// <summary>
  /// This class is used to play a random selection of audio tracks with fading in and out functionality.
  /// The user can adjust the number of tracks playing at a time, the fade in and out duration, and the average time between track operations through the inspector.
  /// Additionally, the user can choose to play the tracks automatically on start or manually through the Play() method.
  /// </summary>
  public class RandomTracksPlayer : MonoBehaviour
  {
    [Header("Master")] 
    [SerializeField] [Range(0f, 1f)] private float _masterVolume = 0.8f;

    [SerializeField] private float _masterFadeDuration = 1f;
    [SerializeField] private bool _playOnStart = true;

    [Header("Playing Tracks")] 
    [SerializeField] private int _avgPlayingTracks = 3;
    [SerializeField] private int _playingTracksVariance = 1;

    [Header("Fade Time")] 
    [SerializeField] private float _fadeInTime = 1f;
    [SerializeField] private float _fadeOutTime = 1f;
    
    [Header("Pitch Randomization")]
    [SerializeField] private float _averagePitch = 1f;
    [SerializeField] private float _pitchVariance = 0.1f;

    [Header("Time before next track operation")] 
    [SerializeField] private float _averageTimeBetweenOperations = 5f;
    [SerializeField] private float _timeBetweenOperationsVariance = 2f;

    [Header("Audio Clips")] [SerializeField]
    private AudioClip[] _audioClips;

    private AudioSource[] _audioSources;
    private Coroutine _generatorCoroutine;
    private bool _isPlaying;
    private List<AudioSource> _playingAudioSources = new List<AudioSource>();
    private List<AudioSource> _idleAudioSources = new List<AudioSource>();

    private void Awake()
    {
      // Create audio sources for each audio clip
      CreateAudioSources();
    }

    private void Start()
    {
      // Play music if play on start is true
      if (_playOnStart)
      {
        Play();
      }
    }

    /// <summary>
    /// Play the audio clips.
    /// </summary>
    public void Play()
    {
      if (_isPlaying) Stop(false);
      _isPlaying = true;
      _idleAudioSources.Clear();  
      _idleAudioSources.AddRange(_audioSources);
      
      // Determine the number of tracks to play
      float numberOfTracks = _avgPlayingTracks + Random.Range(-_playingTracksVariance, _playingTracksVariance);
      
      // Extract the audio sources that will be used and add them to a playing list
      _playingAudioSources.Clear();
      
      for (var i = 0; i < numberOfTracks; i++)
      {
        var randomIndex = Random.Range(0, _idleAudioSources.Count);
        _playingAudioSources.Add(_idleAudioSources[randomIndex]);
        _idleAudioSources.RemoveAt(randomIndex);
      }
      
      // Fade in the playing audio sources
      foreach (var audioSource in _playingAudioSources)
      {
        audioSource.volume = 0f;
        audioSource.Play();
        audioSource.pitch = _averagePitch + Random.Range(-_pitchVariance, _pitchVariance);
        StartCoroutine(FadeIn(audioSource, _masterFadeDuration));
      }

      //shuffle the idle audio sources
      _idleAudioSources.Shuffle();
      
      _generatorCoroutine = StartCoroutine(Generate());
    }
    
    /// <summary>
    /// Stop the audio clips.
    /// </summary>
    /// <param name="fadeOut">If true, audio clips will fade out before stopping. If false, audio clips will stop immediately.</param>
    public void Stop(bool fadeOut = true)
    {
      
      StopGenerator();

      if (fadeOut)
      {
        StartCoroutine(FadeOutAndStopAll(_masterFadeDuration));
      }
      else
      {
        // Stop all audio sources
        foreach (var audioSource in _audioSources)
        {
          audioSource.Stop();
          audioSource.volume = 0f;
        }
        _isPlaying = false;
        _playingAudioSources.Clear();
      }
    }


    /// <summary>
    /// Create audio sources for each audio clip
    /// </summary>
    private void CreateAudioSources()
    {
      _audioSources = new AudioSource[_audioClips.Length];
      for (var i = 0; i < _audioClips.Length; i++)
      {
        _audioSources[i] = gameObject.AddComponent<AudioSource>();
        _audioSources[i].clip = _audioClips[i];
        _audioSources[i].volume = 0f;
        _audioSources[i].loop = true;
      }
    }

    /// <summary>
    /// Coroutine to play audio tracks.
    /// Every _averageTimeBetweenOperations seconds, it determines the number of tracks to play.
    /// if the number of tracks to play is greater than the number of tracks currently playing, it will fade in  random tracks.
    /// if the number of tracks to play is less than the number of tracks currently playing, it will fade out random tracks.
    /// </summary>
    private IEnumerator Generate()
    {
      while (_isPlaying)
      {
        var waitTime = _averageTimeBetweenOperations + Random.Range(-_timeBetweenOperationsVariance, _timeBetweenOperationsVariance);
        yield return new WaitForSeconds(waitTime);
        
        // Determine if a track should be added or removed
        float numberOfTracks = _avgPlayingTracks + Random.Range(-_playingTracksVariance, _playingTracksVariance);
        var numberOfTracksToChange = Mathf.RoundToInt(numberOfTracks - _playingAudioSources.Count);

        switch (numberOfTracksToChange)
        {
          // If the number of tracks to change is positive, add tracks
          case > 0:
            AddTracks(numberOfTracksToChange);
            break;
          case < 0:
            RemoveTracks(numberOfTracksToChange);
            break;
        }
        
      }
    }

    /// <summary>
    /// Remove tracks from the playing audio sources list.
    /// </summary>
    private void RemoveTracks(int numberOfTracksToChange)
    {
      for (var i = 0; i < Mathf.Abs(numberOfTracksToChange); i++)
      {
        if (_playingAudioSources.Count == 0) break;
        var randomIndex = Random.Range(0, _playingAudioSources.Count);
        var audioSource = _playingAudioSources[randomIndex];
        _playingAudioSources.RemoveAt(randomIndex);
        _idleAudioSources.Add(audioSource);
        StartCoroutine(FadeOutAndStop(audioSource, _fadeOutTime));
      }
    }
    
    /// <summary>
    /// Add new tracks to the playing audio sources list.
    /// </summary>
    private void AddTracks(int numberOfTracksToChange)
    {
      for (var i = 0; i < numberOfTracksToChange; i++)
      {
        if (_idleAudioSources.Count == 0) break;
        //get the first idle audio source
        var audioSource = _idleAudioSources[0];
        _idleAudioSources.RemoveAt(0);
        _playingAudioSources.Add(audioSource);
        audioSource.volume = 0f;
        audioSource.pitch = _averagePitch + Random.Range(-_pitchVariance, _pitchVariance);
        audioSource.Play();
        StartCoroutine(FadeIn(audioSource, _fadeInTime));
      }
    }

    private void StopGenerator()
    {
      if (_generatorCoroutine == null) return;
      StopCoroutine(_generatorCoroutine);
      _generatorCoroutine = null;
    }

    /// <summary>
    /// Fade in an audio source over a specified duration.
    /// </summary>
    /// <param name="audioSource">The audio source to fade in.</param>
    /// <param name="duration">The duration of the fade in effect.</param>
    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
      var startVolume = audioSource.volume;
      var startTime = Time.time;
      while (audioSource.volume < _masterVolume)
      {
        var t = (Time.time - startTime) / duration;
        audioSource.volume = Mathf.Lerp(startVolume, _masterVolume, t);
        yield return null;
      }
    }

    /// <summary>
    /// Fade out all audio sources and stop them.
    /// </summary>
    /// <param name="duration">The duration of the fade out effect.</param>
    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
      if (audioSource.volume == 0f || !audioSource.isPlaying) yield break;

      var startVolume = audioSource.volume;
      var startTime = Time.time;
      while (audioSource.volume > 0f && audioSource.isPlaying)
      {
        var t = (Time.time - startTime) / duration;
        audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
        yield return null;
      }
      
      audioSource.Stop();
      //if audio source is not in the playing list, add it to the idle list
      if (_playingAudioSources.Contains(audioSource))
      {
        _playingAudioSources.Remove(audioSource);
      }
      _idleAudioSources.Add(audioSource);
    }

    private IEnumerator FadeOutAndStop(AudioSource audioSource, float duration)
    {
      yield return FadeOut(audioSource, duration);
      audioSource.Stop();
      audioSource.volume = 0f;
    }

    private IEnumerator FadeOutAndStopAll(float duration)
    {
      foreach (var audioSource in _audioSources)
      {
        yield return FadeOut(audioSource, duration);
        audioSource.Stop();
        audioSource.volume = 0f;
      }
      _isPlaying = false;
      _playingAudioSources.Clear();
    }
  }
}