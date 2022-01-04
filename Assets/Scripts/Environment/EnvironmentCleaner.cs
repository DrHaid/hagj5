using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCleaner : MonoBehaviour
{
  private EnvironmentInstance environmentInstance;
  private int deletionBuffer;

  void Update()
  {
    if (environmentInstance == null)
    {
      Debug.Log("No Cleaner parameters set.");
      return;
    }
    if (environmentInstance.LastSegmentIndex == -1)
    {
      // Object still active
      return;
    }

    if (RoadGeneration.instance.segmentIndex > environmentInstance.LastSegmentIndex + deletionBuffer)
    {
      Destroy(gameObject);
    }
  }

  public void SetCleanerParams(EnvironmentInstance inst, int deletionBuffer)
  {
    this.deletionBuffer = deletionBuffer;
    environmentInstance = inst;
  }
}
