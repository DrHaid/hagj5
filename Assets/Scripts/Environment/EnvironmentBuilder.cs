using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBuilder : MonoBehaviour
{
  public float DistanceFromRoad;
  public float EnvironmentFrequency;
  public int EnvironmentDeletionBuffer;

  public List<EnvironmentSettings> EnvironmentTemplates = new List<EnvironmentSettings>();
  public List<EnvironmentInstance> ActiveEnvironments = new List<EnvironmentInstance>();

  private int lastSegmentIndex = 0;

  void Start()
  {

  }

  void Update()
  {
    // check unckecked roadSegments if environment object should be spawned
    for (int i = lastSegmentIndex; i < RoadGeneration.instance.roadSegments.Count; i++)
    {
      var finished = new List<EnvironmentInstance>();
      foreach (var activeEnv in ActiveEnvironments)
      {
        activeEnv.Range--;
        if (activeEnv.Range == 0)
        {
          activeEnv.LastSegmentIndex = i;
          finished.Add(activeEnv);
        }

        // place repeating object
        if (activeEnv.Settings.Type == EnvironmentType.REPEATING
          || activeEnv.Settings.Type == EnvironmentType.WALL)
        {
          if (activeEnv.Range % activeEnv.Settings.GapSize == 0)
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
        break;
      }

      var freeRoadSide = GetFreeRoadSide();
      if (freeRoadSide == null)
      {
        break;
      }

      var envObj = new EnvironmentInstance(PickEnvironmentObject(), freeRoadSide);
      PlaceObject(i, envObj);
      ActiveEnvironments.Add(envObj);
    }
    lastSegmentIndex = RoadGeneration.instance.roadSegments.Count;
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

  private void PlaceObject(int segmentIndex, EnvironmentInstance inst, bool initialize = true)
  {
    var roadSegment = RoadGeneration.instance.roadSegments[segmentIndex];
    var distToCenter = (RoadGeneration.instance.laneCount * RoadGeneration.instance.laneWidth) + DistanceFromRoad;
    var left = Vector3.Cross(roadSegment.direction, Vector3.up);
    var pos = roadSegment.position + (left * (inst.RoadSideRight ? -1 : 1)) * distToCenter;
    var dir = new Vector3(roadSegment.direction.x, 0f, roadSegment.direction.z);
    switch (inst.Settings.Type)
    { 
      case EnvironmentType.SINGLE:
        if (inst.Settings.Prefabs.Count < 1)
        {
          Debug.LogError("No prefabs in EnvironmentTemplate");
          return;
        }
        var single = Instantiate(inst.Settings.Prefabs[0], pos, Quaternion.LookRotation(dir, Vector3.up));
        single.GetComponent<EnvironmentCleaner>().SetCleanerParams(inst, EnvironmentDeletionBuffer);
        single.transform.parent = gameObject.transform;
        break;

      case EnvironmentType.REPEATING:
        if (inst.Settings.Prefabs.Count < 1)
        {
          Debug.LogError("No prefabs in EnvironmentTemplate");
          return;
        }
        var repeat = Instantiate(inst.Settings.Prefabs[0], pos, Quaternion.LookRotation(dir, Vector3.up));
        repeat.GetComponent<EnvironmentCleaner>().SetCleanerParams(inst, EnvironmentDeletionBuffer);
        repeat.transform.parent = gameObject.transform;
        break;

      case EnvironmentType.WALL:
        if (inst.Settings.Prefabs.Count < 2)
        {
          Debug.LogError("Minimum prefab count (2) not met EnvironmentTemplate");
          return;
        }
        var wall = Instantiate(initialize ? inst.Settings.Prefabs[0] : inst.Settings.Prefabs[1], pos, Quaternion.LookRotation(dir, Vector3.up));
        wall.GetComponent<EnvironmentCleaner>().SetCleanerParams(inst, EnvironmentDeletionBuffer);
        wall.transform.parent = gameObject.transform;
        break;

      case EnvironmentType.GROUP:
        if (inst.Objects.Count < 1)
        {
          Debug.LogError("No Objects found in Group");
          return;
        }

        // HACK: to get around vector maths
        var tempParent = new GameObject();
        tempParent.transform.parent = gameObject.transform;
        tempParent.transform.position = pos;
        foreach (var groupObj in inst.Objects)
        {
          var o = Instantiate(groupObj.Item2, pos + groupObj.Item1, Quaternion.identity, tempParent.transform);
          o.GetComponent<EnvironmentCleaner>().SetCleanerParams(inst, EnvironmentDeletionBuffer);
        }
        tempParent.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        tempParent.transform.DetachChildren();
        Destroy(tempParent);
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
  public int LastSegmentIndex = -1;

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
