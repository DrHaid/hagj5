using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBuilder : MonoBehaviour
{
  public List<EnvironmentSettings> EnvironmentSettings = new List<EnvironmentSettings>();
  public List<EnvironmentInstance> EnvironmentInstances = new List<EnvironmentInstance>();

  void Start()
  {
    
  }

  void Update()
  {

  }
}

public class EnvironmentInstance
{
  public EnvironmentSettings Settings;
  public bool RoadSideRight;
  public float Length;
  public List<Tuple<Vector3, GameObject>> Objects;

  public EnvironmentInstance(EnvironmentSettings settings)
  {
    Settings = settings;
    RoadSideRight = UnityEngine.Random.value > 0.5f;
    Length = UnityEngine.Random.Range(Settings.MinLength, Settings.MaxLength);
    if (Settings.Type == EnvironmentType.GROUP)
    {
      PlaceObjects();
    }
  }

  private void PlaceObjects()
  {
    Objects = new List<Tuple<Vector3, GameObject>>();
    foreach(var envObj in Settings.Prefabs)
    {
      // TODO: assemble group of environment gameobjects
    }
  }
}
