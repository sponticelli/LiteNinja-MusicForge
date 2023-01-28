using System;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace LiteNinja.MusicForge.Editor
{
  [CustomEditor(typeof(InstrumentSO))]
  public class InstrumentSOEditor : UnityEditor.Editor
  {
#region serialized properties

    private SerializedObject _serializedObject;

    private SerializedProperty _waveformDataProperty;

    private SerializedProperty _useVolumeProperty;
    private SerializedProperty _volumeDataProperty;

    private SerializedProperty _usePitchProperty;
    private SerializedProperty _pitchDataProperty;

    private SerializedProperty _useOrnamentsProperty;
    private SerializedProperty _ornamentDataProperty;

    private SerializedProperty _useChorusProperty;
    private SerializedProperty _chorusDataProperty;

    private SerializedProperty _useEchoProperty;
    private SerializedProperty _echoDataProperty;

    private SerializedProperty _previewDataProperty;

#endregion

#region textures
    private Texture2D _waveformTexture;
#endregion
    

    private Random _random;
    private InstrumentSO _instrument;

    private void OnEnable()
    {
      _serializedObject = new SerializedObject(target);
      _waveformDataProperty = _serializedObject.FindProperty("_waveformData");
      _useVolumeProperty = _serializedObject.FindProperty("_useVolume");
      _volumeDataProperty = _serializedObject.FindProperty("_volumeData");
      _usePitchProperty = _serializedObject.FindProperty("_usePitch");
      _pitchDataProperty = _serializedObject.FindProperty("_pitchData");
      _useOrnamentsProperty = _serializedObject.FindProperty("_useOrnaments");
      _ornamentDataProperty = _serializedObject.FindProperty("_ornamentData");
      _useChorusProperty = _serializedObject.FindProperty("_useChorus");
      _chorusDataProperty = _serializedObject.FindProperty("_chorusData");
      _useEchoProperty = _serializedObject.FindProperty("_useEcho");
      _echoDataProperty = _serializedObject.FindProperty("_echoData");
      _previewDataProperty = _serializedObject.FindProperty("_previewData");

      if (_waveformTexture != null)
      {
        DestroyImmediate(_waveformTexture);
        _waveformTexture = null;
      }

      _random = new System.Random(1);
    }

    public override void OnInspectorGUI()
    {
      _serializedObject.Update();
      _instrument = _serializedObject.targetObject as InstrumentSO;
      DrawWaveform();
      DrawVolume();
      DrawPitch();
      DrawOrnaments();
      DrawChorus();
      DrawEcho();

      DrawPreviewData();


      _serializedObject.ApplyModifiedProperties();
    }


    private void DrawWaveform()
    {
      EditorGUILayout.PropertyField(_waveformDataProperty); 
      DrawWaveformTexture();
      
    }

    private void DrawWaveformTexture()
    {
      var instrument = (InstrumentSO) target;
      var waveformData = _instrument.WaveformInfo;
      

      var numberOfWaveforms = waveformData.NumberOfWaveforms;

      var textureRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 100, GUILayout.ExpandWidth(true));
      _waveformTexture = new Texture2D((int)textureRect.width, (int)textureRect.height, TextureFormat.ARGB32, false);
      _waveformTexture.hideFlags = HideFlags.HideAndDontSave;
      _waveformTexture.filterMode = FilterMode.Point;
      FillTexture(_waveformTexture, Color.black);

      var previousPoint = Vector2.zero;
      var twoWaveforms = numberOfWaveforms == InstrumentSO.WaveformData.WaveformsCount.Two;
      for (var k = 0; k < (twoWaveforms ? 3 : 1); k++)
      {
        var offsetX = k == 1 ? _waveformTexture.width / 2 + 8 : 0;
        var offsetY = !twoWaveforms || k < 2 ? _waveformTexture.height / 2 + (twoWaveforms ? 8 : 0) : 0;
        var width = _waveformTexture.width;
        if (twoWaveforms && k < 2)
          width = width / 2 - 8;
        var height = _waveformTexture.height / 2 - (twoWaveforms ? 8 : 0);
        var repeats = twoWaveforms && k < 2 ? 2 : 4;
        for (var i = 0; i < width; i++)
        {
          _waveformTexture.SetPixel(offsetX + i, offsetY + height / 2, Color.gray);
        }

        for (var j = 0; j < height; j++)
        {
          _waveformTexture.SetPixel(offsetX, offsetY + j, Color.gray);
        }

        for (var i = 1; i < width; i++)
        {
          var waveOffset = twoWaveforms && k != 1 ? waveformData.Waveform1Offset : 0;
          var waveformX = (i + waveOffset * (width / repeats)) * repeats % width / width;
          var waveformAmplitude = _instrument.GetWaveformAmplitude(waveformX, false, k != 1);
          if (k == 2)
          {
            waveformX = (i + waveformData.Waveform2Offset * (width / repeats)) * repeats % width / width;
            waveformAmplitude = waveformAmplitude * (1 - waveformData.BlendAmount) +
                                _instrument.GetWaveformAmplitude(waveformX, false, false) *
                                waveformData.BlendAmount;
          }

          var currentPoint = new Vector2(offsetX + i, (int)(waveformAmplitude * (height / 2)) + height / 2);
          currentPoint.y = offsetY + Mathf.Clamp(Mathf.Lerp(currentPoint.y, height / 2, waveformData.Noise) +
                                              (float)(_random.NextDouble() * waveformData.Noise * 2 -
                                                      waveformData.Noise) * height, 0, height - 1);
          if (i > 1)
          {
            DrawLineOnTexture(_waveformTexture, previousPoint, currentPoint, Color.green);
          }
          previousPoint = currentPoint;
        }
      }

      _waveformTexture.Apply(false);
      EditorGUI.DrawTextureTransparent(textureRect, _waveformTexture, ScaleMode.ScaleToFit);
    }

    private static void DrawLineOnTexture(Texture2D texture, Vector2 lineFrom, Vector2 lineTo, Color colour)
    {
      var points = (int)(lineTo - lineFrom).magnitude * 2;
      var increment = (lineTo - lineFrom) / points;
      for (var i = 0; i < points; i++)
      {
        texture.SetPixel(Math.Max(Math.Min((int)lineFrom.x, texture.width - 1), 0),
          Math.Max(Math.Min((int)lineFrom.y, texture.height - 1), 0),
          colour);
        lineFrom += increment;
      }
    }

    private void DrawPreviewData()
    {
      EditorGUILayout.PropertyField(_previewDataProperty);
      if (GUILayout.Button("Play"))
      {
        var _previewData = _instrument.PreviewInfo;
        EditorAudioUtils.PlayAudioClip(_instrument.PreviewInfo.Frequency, 
          _instrument.GetNoteWave(_previewData.Frequency, (_previewData.NoteOctave * 12) + _previewData.Note,
          _previewData.NoteLength, false), "");
      }
    }

    private void DrawVolume()
    {
      DrawConditionalProperty(_volumeDataProperty, _useVolumeProperty);
    }

    private void DrawPitch()
    {
      DrawConditionalProperty(_pitchDataProperty, _usePitchProperty);
    }

    private void DrawOrnaments()
    {
      DrawConditionalProperty(_ornamentDataProperty, _useOrnamentsProperty);
    }

    private void DrawChorus()
    {
      DrawConditionalProperty(_chorusDataProperty, _useChorusProperty);
    }

    private void DrawEcho()
    {
      DrawConditionalProperty(_echoDataProperty, _useEchoProperty);
    }

    private void DrawConditionalProperty(SerializedProperty property, SerializedProperty conditionProperty)
    {
      EditorGUILayout.PropertyField(conditionProperty);
      if (conditionProperty.boolValue)
      {
        property.isExpanded = true;
        EditorGUILayout.PropertyField(property);
      }
      else
      {
        property.isExpanded = false;
      }
    }

    private static void FillTexture(Texture2D texture, Color color)
    {
      for (var i = 0; i < texture.width; i++)
        for (var j = 0; j < texture.height; j++)
          texture.SetPixel(i, j, color);
    }
  }
}