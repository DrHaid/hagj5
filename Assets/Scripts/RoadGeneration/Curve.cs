using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
  public bool Positive { get; set; }
  public float Length { get; set; }
  public float Strength { get; set; }

  public Curve(CurveSettings curveSettings)
  {
    Positive = Random.value <= 0.5f;
    Length = Random.Range(curveSettings.MinLength, curveSettings.MaxLength);
    Strength = Random.Range(curveSettings.MinStrength, curveSettings.MaxStrength);
  }

  public bool DoCurvature(float segmentLength)
  {
    Length -= segmentLength;
    return Length >= 0;
  }
}
