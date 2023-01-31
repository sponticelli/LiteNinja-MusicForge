using UnityEditor;
using UnityEngine;

namespace LiteNinja.MusicForge.Editor
{
  [CustomPropertyDrawer(typeof(Waveform))]
  public class WaveformDrawer : PropertyDrawer
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
      var waveformTypeProperty = property.FindPropertyRelative("_type");
      var offsetProperty = property.FindPropertyRelative("_offset");
      var noiseAmountProperty = property.FindPropertyRelative("_noise");

      var waveformTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
      var offsetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width,
        EditorGUIUtility.singleLineHeight);
      var noiseAmountRect = new Rect(position.x,
        position.y + 2*(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
        position.width, EditorGUIUtility.singleLineHeight);
      var waveformTextureRect = new Rect(position.x,
        position.y + 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
        TextureWidth, TextureHeight);


      var waveformNames = System.Enum.GetNames(typeof(WaveformType));
      waveformTypeProperty.intValue =
        EditorGUI.Popup(waveformTypeRect, label.text, waveformTypeProperty.intValue, waveformNames);
      offsetProperty.floatValue = 
        EditorGUI.Slider(offsetRect, "Offset", offsetProperty.floatValue, 0f, 1f);
      noiseAmountProperty.floatValue =
        EditorGUI.Slider(noiseAmountRect, "Noise", noiseAmountProperty.floatValue, 0f, 1f);


      var waveform =  new Waveform()
      {
        Type = (WaveformType)waveformTypeProperty.intValue,
        Noise = noiseAmountProperty.floatValue,
        Offset = offsetProperty.floatValue
      };
      EditorTexture2DUtils.DrawWaveform(waveform, _waveformTexture, Color.green, Color.gray, Color.black);
      EditorGUI.DrawTextureTransparent(waveformTextureRect, _waveformTexture, ScaleMode.ScaleToFit);

      EditorGUI.EndProperty();
    }

    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + TextureHeight;
    }
  }
}



