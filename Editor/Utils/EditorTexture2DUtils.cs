using System;
using UnityEngine;

namespace LiteNinja.MusicForge.Editor
{
  public static class EditorTexture2DUtils
  {
    public static void Fill(Texture2D texture, Color color)
    {
      for (var i = 0; i < texture.width; i++)
        for (var j = 0; j < texture.height; j++)
          texture.SetPixel(i, j, color);
    }
    
    public static void DrawLine(Texture2D texture, Vector2 lineFrom, Vector2 lineTo, Color colour)
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
    
    public static void DrawWaveform(IWaveform compositeWaveform, Texture2D texture, Color lineColor, Color axysColor, Color backgroundColor)
    {
      var textureWidth = texture.width;
      var textureHeight = texture.height;
      var step = 1f / textureWidth;
    
      Fill(texture, backgroundColor);
      
      var halfHeight = textureHeight / 2;

      var previousPoint = new Vector2(0, halfHeight);

      for (var x = 0; x < textureWidth; x++)
      {
        var t = x * step;
        var amplitude = compositeWaveform.GetAmplitudeWithNoise(t) * 0.5f + 0.5f;
        var y = (int)((textureHeight-2) * amplitude);
        texture.SetPixel(x, halfHeight, axysColor);
        var currentPoint = new Vector2(x, y);
        
        if (t == 0)
        {
          texture.SetPixel(x, y, lineColor);
        }
        else
        {
          DrawLine(texture, previousPoint, currentPoint, lineColor);
        }
        previousPoint = currentPoint;
      }
      
      texture.Apply();
    }
  }
}