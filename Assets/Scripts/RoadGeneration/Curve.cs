using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
  public bool Positive { get; set; }
  public float Length { get; set; }
  public float Strength { get; set; }

  public Curve(float minLength, float maxLength, float minStrength, float maxStrength)
  {
    Positive = Random.value <= 0.5f;
    Length = Random.Range(minLength, maxLength);
    Strength = Random.Range(minStrength, maxStrength);
  }

  public bool DoCurvature(float segmentLength)
  {
    Length -= segmentLength;
    return Length >= 0;
  }
}
