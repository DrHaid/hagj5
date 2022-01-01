using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CurveSettings : ScriptableObject
{
  public float Chance;
  public float MinStrength;
  public float MaxStrength;
  public int MinLength;
  public int MaxLength;
}
