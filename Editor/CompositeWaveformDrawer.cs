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

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (_waveformTexture == null)
      {
        _waveformTexture = new Texture2D(TextureWidth, TextureHeight)
        {
          filterMode = FilterMode.Point
        };
      }

      EditorGUI.BeginProperty(position, label, property);
      var weightedWaveformsProperty = property.FindPropertyRelative("_weightedWaveforms");
      var weightedWaveformsRect = new Rect(position.x, position.y, position.width, 
        EditorGUI.GetPropertyHeight(weightedWaveformsProperty));
      EditorGUI.PropertyField(weightedWaveformsRect, weightedWaveformsProperty, true);

      var waveformTextureRect = new Rect(position.x,
        position.y + weightedWaveformsRect.height + EditorGUIUtility.standardVerticalSpacing, position.width,
        TextureHeight);
      

      var compositeWaveform = new CompositeWaveform();
      var weightedWaveforms = new List<CompositeWaveform.WeightedWaveform>();
      for(var i=0; i<weightedWaveformsProperty.arraySize; i++)
      {
        var weightedWaveformProperty = weightedWaveformsProperty.GetArrayElementAtIndex(i);
        var weightProperty = weightedWaveformProperty.FindPropertyRelative("_weight");
        var waveformProperty = weightedWaveformProperty.FindPropertyRelative("_waveform");

        var waveform = new Waveform()
        {
          Type = (WaveformType)waveformProperty.FindPropertyRelative("_type").enumValueIndex,
          Noise = waveformProperty.FindPropertyRelative("_noise").floatValue,
          Offset = waveformProperty.FindPropertyRelative("_offset").floatValue,
        };
        
        weightedWaveforms.Add(new CompositeWaveform.WeightedWaveform
        {
          Weight = weightProperty.floatValue,
          Waveform = waveform
        });
      }
      compositeWaveform.Waveforms = weightedWaveforms.ToArray();

      EditorTexture2DUtils.DrawWaveform(compositeWaveform, _waveformTexture, Color.yellow, Color.gray, Color.black);
      EditorGUI.DrawTextureTransparent(waveformTextureRect, _waveformTexture, ScaleMode.ScaleToFit);

      EditorGUI.EndProperty();
    }

    

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_weightedWaveforms")) +
             EditorGUIUtility.standardVerticalSpacing + TextureHeight;
    }
  }
}