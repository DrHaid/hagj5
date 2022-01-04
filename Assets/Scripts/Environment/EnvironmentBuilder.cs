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
      var finished = new List<EnvironmentInstance>();
      foreach (var activeEnv in ActiveEnvironments)
      {
        activeEnv.Range--;
        if (activeEnv.Range == 0)
        {
          finished.Add(activeEnv);
        }

        // place repeating object
        if (activeEnv.Settings.Type == EnvironmentType.REPEATING
          || activeEnv.Settings.Type == EnvironmentType.WALL)
        {
          if (i % activeEnv.Range == 0)
          {
            PlaceObject(i, activeEnv, false);
          }
        }
      }

      // clean up environment objects that finished generating
      foreach (var instance in finished)
      {
        ActiveEnvironments.Remove(instance);
      }

      // choose if environment should be generated
      if ( UnityEngine.Random.value > EnvironmentFrequency)
      {
        return;
      }

      var freeRoadSide = GetFreeRoadSide();
      if (freeRoadSide == null)
      {
        return;
      }

      var envObj = new EnvironmentInstance(PickEnvironmentObject(), freeRoadSide);
      PlaceObject(i, envObj);
      ActiveEnvironments.Add(envObj);
    }
    lastSegmentIndex = RoadGeneration.instance.roadSegments.Count - 1;
  }

  private bool? GetFreeRoadSide()
  {
    bool? roadSide = null;
    if (ActiveEnvironments.Count == 0)
      return UnityEngine.Random.value > 0.5f;
    foreach (var inst in ActiveEnvironments)
    {
      if (roadSide == (inst.RoadSideRight && inst.Settings.Exclusive))
        return null;
      roadSide = !(inst.RoadSideRight && inst.Settings.Exclusive);
    }
    return roadSide;
  }

  private void PlaceObject(int segmentIndex, EnvironmentInstance inst, bool initalize = true)
  {
    var roadSegment = RoadGeneration.instance.roadSegments[segmentIndex];
    var distToCenter = (RoadGeneration.instance.laneCount * RoadGeneration.instance.laneWidth) + DistanceFromRoad;
    var left = Vector3.Cross(roadSegment.direction, Vector3.up);
    var pos = roadSegment.position + (left * (inst.RoadSideRight ? -1 : 1)) * distToCenter;
    switch (inst.Settings.Type)
    {
      case EnvironmentType.SINGLE:
        break;
      case EnvironmentType.REPEATING:
        break;
      case EnvironmentType.WALL:
        break;
      case EnvironmentType.GROUP:
        break;
    }
  }

  public EnvironmentSettings PickEnvironmentObject()
  {
    List<int> weights = new List<int>();
    int weightTotal = 0;

    foreach (var template in EnvironmentTemplates)
    {
      weights.Add(template.Weight);
      weightTotal += template.Weight;
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
    foreach (var envObj in Settings.Prefabs)
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
