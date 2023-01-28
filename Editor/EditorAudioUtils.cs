using UnityEditor;
using UnityEngine;

namespace LiteNinja.MusicForge.Editor
{
  public static class EditorAudioUtils
  {
    private static AudioSource _previewAudioSource;

    /// <summary>
    /// Play an audio clip from the editor by creating a temporary game object with an audio source on it.
    /// </summary>
    public static void PlayAudioClip(int frequency, float[] sampleData, string noAudioDataMessage)
    {
      if (sampleData.Length == 0)
      {
        EditorUtility.DisplayDialog("Preview",
          noAudioDataMessage == "" ? "There is no audio data to play." : noAudioDataMessage, "OK");
        return;
      }

      EditorApplication.update -= OnUpdate;
      EditorApplication.update += OnUpdate;
      EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
      DestroyPreviewAudio();
      var audioSourceGameObject = new GameObject("Temporary Audio Source");
      audioSourceGameObject.hideFlags = HideFlags.HideAndDontSave;
      _previewAudioSource = audioSourceGameObject.AddComponent<AudioSource>();
      _previewAudioSource.hideFlags = HideFlags.HideAndDontSave;
      _previewAudioSource.playOnAwake = false;
      var audioClip = AudioClip.Create("Temporary Audio Clip", sampleData.Length, 1, frequency, false);
      audioClip.SetData(sampleData, 0);
      _previewAudioSource.clip = audioClip;
      _previewAudioSource.Play();
    }

    /// <summary>
    /// Stop the currently playing preview audio.
    /// </summary>
    public static void StopAudioClip()
    {
      if (_previewAudioSource != null) DestroyPreviewAudio();
    }
    
    /// <summary>
    /// Return the position of the preview audio clip in samples, or -1 if no preview is playing.
    /// </summary>
    public static int GetWavePosition()
    {
      return _previewAudioSource != null && _previewAudioSource.isPlaying ? _previewAudioSource.timeSamples : -1;
    }
    
    /// <summary>
    /// Check if the audio source needs destroying.
    /// </summary>
    private static void OnUpdate()
    {
      if (_previewAudioSource != null && !_previewAudioSource.isPlaying) DestroyPreviewAudio();
    }
    
    /// <summary>
    /// Check if entering play mode.
    /// </summary>
    /// <param name="playModeStateChange"></param>
    private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
      DestroyPreviewAudio();
    }

    /// <summary>
    /// Destroy the preview audio source and game object.
    /// </summary>
    private static void DestroyPreviewAudio()
    {
      if (_previewAudioSource == null) return;
      GameObject.DestroyImmediate(_previewAudioSource.clip);
      var previewAudioSourceGameObject = _previewAudioSource.gameObject;
      GameObject.DestroyImmediate(_previewAudioSource);
      GameObject.DestroyImmediate(previewAudioSourceGameObject);
      _previewAudioSource = null;
    }
    
    
  }
}