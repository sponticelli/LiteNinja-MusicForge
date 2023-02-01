using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MusicForge.Editor
{
  [CustomPropertyDrawer(typeof(CompositeWaveform))]
  public class CompositeWaveformDrawer : PropertyDrawer
  {
 
    private const int TextureWidth = 256;
    private const int TextureHeight = 64;
    private Texture2D _waveformTexture;
    private CompositeWaveform _compositeWaveform;
    private bool _isChanged;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (_waveformTexture == null)
      {
        _waveformTexture = new Texture2D(TextureWidth, TextureHeight)
        {
          filterMode = FilterMode.Point
        };
      }
      
      if (_compositeWaveform == null)
      {
        _compositeWaveform = new CompositeWaveform();
        _isChanged = true;
      }

      EditorGUI.BeginProperty(position, label, property);
      var weightedWaveformsProperty = property.FindPropertyRelative("_weightedWaveforms");
      var weightedWaveformsRect = new Rect(position.x, position.y, position.width, 
        EditorGUI.GetPropertyHeight(weightedWaveformsProperty));
      EditorGUI.PropertyField(weightedWaveformsRect, weightedWaveformsProperty, true);

      var waveformTextureRect = new Rect(position.x,
        position.y + weightedWaveformsRect.height + EditorGUIUtility.standardVerticalSpacing, position.width,
        TextureHeight);
      
      
      CheckChanges(weightedWaveformsProperty);
      if (_isChanged)
      {
        EditorTexture2DUtils.DrawWaveform(_compositeWaveform, _waveformTexture, Color.yellow, Color.gray, Color.black);
      }

      EditorGUI.DrawTextureTransparent(waveformTextureRect, _waveformTexture, ScaleMode.ScaleToFit);
      EditorGUI.EndProperty();
    }

    private void CheckChanges(SerializedProperty weightedWaveformsProperty)
    {

      if (weightedWaveformsProperty.arraySize != _compositeWaveform.Waveforms.Length)
      {
        // Array size changed
        _isChanged = true;
        
        var weightedWaveforms = new List<CompositeWaveform.WeightedWaveform>();
        for (var i = 0; i < weightedWaveformsProperty.arraySize; i++)
        {
          var weightedWaveformProperty = weightedWaveformsProperty.GetArrayElementAtIndex(i);
          var weightProperty = weightedWaveformProperty.FindPropertyRelative("_weight");
          var waveformProperty = weightedWaveformProperty.FindPropertyRelative("_waveform");
          var weight = weightProperty.floatValue;
          var waveformTypeProperty = waveformProperty.FindPropertyRelative("_type");
          var offsetProperty = waveformProperty.FindPropertyRelative("_offset");
          var noiseAmountProperty = waveformProperty.FindPropertyRelative("_noise");
          
          var weightedWaveform = new CompositeWaveform.WeightedWaveform()
          {
           Weight = weight,
            Waveform = new Waveform()
            {
              Type = (WaveformType) waveformTypeProperty.enumValueIndex,
              Offset = offsetProperty.floatValue,
              Noise = noiseAmountProperty.floatValue
            }
          };
          weightedWaveforms.Add(weightedWaveform);
        }
        _compositeWaveform.Waveforms = weightedWaveforms.ToArray();
      }
      else
      {
        // Check if any of the waveforms changed
        for (var i = 0; i < weightedWaveformsProperty.arraySize; i++)
        {
          var weightedWaveformProperty = weightedWaveformsProperty.GetArrayElementAtIndex(i);
          var waveformProperty = weightedWaveformProperty.FindPropertyRelative("_waveform");
          var weightProperty = weightedWaveformProperty.FindPropertyRelative("_weight");

          var waveformTypeProperty = waveformProperty.FindPropertyRelative("_type");
          var offsetProperty = waveformProperty.FindPropertyRelative("_offset");
          var noiseAmountProperty = waveformProperty.FindPropertyRelative("_noise");
          
          var weight = weightProperty.floatValue;
          if (weight == _compositeWaveform.Waveforms[i].Weight &&
              waveformTypeProperty.enumValueIndex == (int)_compositeWaveform.Waveforms[i].Waveform.Type &&
              offsetProperty.floatValue == _compositeWaveform.Waveforms[i].Waveform.Offset &&
              noiseAmountProperty.floatValue == _compositeWaveform.Waveforms[i].Waveform.Noise) continue;
          
          _isChanged = true;
          _compositeWaveform.Waveforms[i].Weight = weight;
          _compositeWaveform.Waveforms[i].Waveform.Type = (WaveformType) waveformTypeProperty.enumValueIndex;
          _compositeWaveform.Waveforms[i].Waveform.Offset = offsetProperty.floatValue;
          _compositeWaveform.Waveforms[i].Waveform.Noise = noiseAmountProperty.floatValue;
        }
      }
      
      
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_weightedWaveforms")) +
             EditorGUIUtility.standardVerticalSpacing + TextureHeight;
    }
  }
}