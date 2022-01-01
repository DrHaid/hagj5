using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlopeSettings : ScriptableObject
{
  public float Chance;
  public float MinSlopeValue;
  public float MaxSlopeValue;
  public int MinLength;
  public int MaxLength;
}
