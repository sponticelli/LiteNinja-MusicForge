using LiteNinja.MusicForge.Instruments;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MusicForge.Editor
{
  [CustomPropertyDrawer(typeof(Instrument))]
  public class InstrumentDrawer : PropertyDrawer
  {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      var waveformProperty = property.FindPropertyRelative("_waveform");
      var waveformRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(waveformProperty));
      
      EditorGUI.PropertyField(waveformRect, waveformProperty, true);

      EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_waveform")) +
             EditorGUIUtility.standardVerticalSpacing;
    }
  }
}