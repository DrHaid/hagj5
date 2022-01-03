using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBuilder : MonoBehaviour
{
  public float DistanceFromRoad;
  public float EnvironmentFrequency;

  public List<EnvironmentSettings> EnvironmentTemplates = new List<EnvironmentSettings>();
  public List<EnvironmentInstance> ActiveEnvironments = new List<EnvironmentInstance>();

  private int lastSegmentIndex = 0;

  void Start()
  {
    
  }

  void Update()
  {
    //if(ActiveEnvironments.Count == 0)
    //{
    //  if(UnityEngine.Random.value < EnvironmentFrequency)
    //  {
    //    var setting = PickEnvironmentObject();
    //    ActiveEnvironments.Add(new EnvironmentInstance(setting));
    //  }
    //}

    // check unckecked roadSegments if environment object should be spawned
    for (int i = lastSegmentIndex; i < RoadGeneration.instance.roadSegments.Count; i++)
    {
      // TODO: oh boi
      /*
       update all active environments here because range and roadside could still be occupied by something else.
      once range is free, do the whole "pick environment" routine. 
      basically do all updating in here, not in Update() because update could happen every other segment.
       */
    }
    // lastSegmentIndex = RoadGeneration.instance.roadSegments.Count - 1;
  }

  public EnvironmentSettings PickEnvironmentObject()
  {
    List<int> weights = new List<int>();
    int weightTotal = 0;

    foreach (var settings in EnvironmentTemplates)
    {
      weights.Add(settings.Weight);
      weightTotal += settings.Weight;
    }

    int randVal = UnityEngine.Random.Range(0, weightTotal + 1);
    int total = 0;
    int result;
    for (result = 0; result < weights.Count; result++)
    {
      total += weights[result];
      if (total >= randVal) break;
    }
    return EnvironmentTemplates[result];
  }
}

public class EnvironmentInstance
{
  public EnvironmentSettings Settings;
  public bool RoadSideRight;
  public int Range;
  public List<Tuple<Vector3, GameObject>> Objects;

  public EnvironmentInstance(EnvironmentSettings settings, bool? roadSideRight = null)
  {
    Settings = settings;
    RoadSideRight = roadSideRight ?? UnityEngine.Random.value > 0.5f;
    Range = UnityEngine.Random.Range(Settings.MinRange, Settings.MaxRange);
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
      var pos = new Vector3(
          UnityEngine.Random.Range(0, 10 * (RoadSideRight ? 1 : -1)),
          0,
          UnityEngine.Random.Range(-(Range / 2), (Range / 2))
        );
      Objects.Add(new Tuple<Vector3, GameObject>(pos, envObj));
    }
  }
}
