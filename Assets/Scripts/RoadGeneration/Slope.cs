using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope
{
  public bool Positive { get; set; }
  public float Length { get; set; }
  public float OriginalLength { get; set; }
  public float SlopeValue { get; set; }

  public Slope(SlopeSettings slopeSettings)
  {
    Positive = Random.value <= 0.5f;
    OriginalLength = Random.Range(slopeSettings.MinLength, slopeSettings.MaxLength);
    Length = OriginalLength;
    SlopeValue = Random.Range(slopeSettings.MinSlopeValue, slopeSettings.MaxSlopeValue);
  }

  public bool DoSlope(float segmentLength)
  {
    Length -= segmentLength;
    return Length >= 0;
  }
}
