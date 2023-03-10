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

    private Waveform _waveform;
    private bool _changed;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (_waveformTexture == null)
      {
        _waveformTexture = new Texture2D(TextureWidth, TextureHeight)
        {
          filterMode = FilterMode.Point
        };
      }

      if (_waveform == null)
      {
        _waveform = new Waveform();
        _changed = true;
      }

      EditorGUI.BeginProperty(position, label, property);
      var waveformTypeProperty = property.FindPropertyRelative("_type");
      var offsetProperty = property.FindPropertyRelative("_offset");
      var noiseAmountProperty = property.FindPropertyRelative("_noise");

      var waveformTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
      var offsetRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width,
        EditorGUIUtility.singleLineHeight);
      var noiseAmountRect = new Rect(position.x,
        position.y + 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
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


      if (_waveform.Type != (WaveformType)waveformTypeProperty.intValue ||
          _waveform.Offset != offsetProperty.floatValue ||
          _waveform.Noise != noiseAmountProperty.floatValue)
      {
        _waveform.Type = (WaveformType)waveformTypeProperty.intValue;
        _waveform.Offset = offsetProperty.floatValue;
        _waveform.Noise = noiseAmountProperty.floatValue;
        _changed = true;
      }


      if (_changed)
        EditorTexture2DUtils.DrawWaveform(_waveform, _waveformTexture, Color.green, Color.gray, Color.black);
      EditorGUI.DrawTextureTransparent(waveformTextureRect, _waveformTexture, ScaleMode.ScaleToFit);

      EditorGUI.EndProperty();
      _changed = false;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + TextureHeight;
    }
  }
}