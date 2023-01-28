using UnityEditor;
using UnityEngine;

namespace LiteNinja.MusicForge.Editor
{
  /// <summary>
  /// EditorAudioUtils is a static class that provides methods for playing and stopping audio clips in the Unity editor.
  /// It also includes methods for getting the current position of a playing audio clip in samples, and for destroying a
  /// preview audio source and its associated game object.
  /// </summary>
  public static class EditorAudioUtils
  {
    private static AudioSource _previewAudioSource;

    /// <summary>
    /// Play an audio clip from the editor by creating a temporary game object with an audio source on it.
    /// </summary>
    /// <param name="frequency">The frequency of the audio clip to be played.</param>
    /// <param name="sampleData">The sample data of the audio clip to be played.</param>
    /// <param name="noAudioDataMessage">A message to display if there is no audio data to play.</param>
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
    /// Check if the audio source needs to be destroyed.
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