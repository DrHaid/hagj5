using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnvironmentSettings : ScriptableObject
{
  public string Name;
  public EnvironmentType Type;
  public bool Exclusive;
  public int Weight;
  public int MinRange;
  public int MaxRange;
  public int GapSize;
  public List<GameObject> Prefabs;
}

public enum EnvironmentType
{
  SINGLE, // one object next to road
  GROUP, // collection of objects e.g. town
  REPEATING, // repeating e.g. trees, lamps
  WALL // like repeating but starting object is different
}

