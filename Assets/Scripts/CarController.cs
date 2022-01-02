using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public float Progress;
  public float LanePosition;
  public float Speed;

  void Update()
  {
    if (Input.GetKey(KeyCode.UpArrow))
    {
      Speed += 0.01f;
    }
    if (Input.GetKey(KeyCode.DownArrow))
    {
      Speed = Mathf.Clamp(Speed - 0.02f, 0f, 1000f);
    }
    if (Input.GetKey(KeyCode.RightArrow))
    {
      LanePosition += 0.1f * Speed * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      LanePosition -= 0.1f * Speed * Time.deltaTime;
    }

    Progress += Time.deltaTime * Speed;
    LanePosition = ClampLanePosition(LanePosition, gameObject.transform.localScale.x);
    SetTransformFromProgress(gameObject.transform, Progress, LanePosition);
    
    // set position of RoadGeneration
    var segmentLength = RoadGeneration.instance.segmentLength;
    var currentSegment = (int)(Progress / segmentLength);
    int index = currentSegment - 3;
    RoadGeneration.instance.segmentIndex = index >= 0 ? index : 0;
  }

  public static CarriageBot.CarriageBotState SetTransformFromProgress(Transform car, float progress, float lanePosition)
  {
    var segmentLength = RoadGeneration.instance.segmentLength;
    var currentSegment = (int)(progress / segmentLength);
    if (currentSegment < RoadGeneration.instance.segmentIndex)
    {
      return CarriageBot.CarriageBotState.OUTRUN;
    }
    if (currentSegment > RoadGeneration.instance.roadSegments.Count - 1)
    {
      return CarriageBot.CarriageBotState.AHEAD;
    }
    var localProgress = progress % segmentLength;
    var prevPos = RoadGeneration.instance.roadSegments[currentSegment - 1].position;
    car.position = prevPos + RoadGeneration.instance.roadSegments[currentSegment].direction * localProgress;
    car.rotation = Quaternion.LookRotation(RoadGeneration.instance.roadSegments[currentSegment].direction, Vector3.up);
    car.position = car.position + car.right * lanePosition;
    return CarriageBot.CarriageBotState.LEVEL;
  }

  public static float ClampLanePosition(float lanePosition, float carWidth)
  {
    var roadWidth = RoadGeneration.instance.laneCount * RoadGeneration.instance.laneWidth;
    var boundary = (roadWidth / 2f) - (carWidth / 2f);
    return Mathf.Clamp(lanePosition, -boundary, boundary);
  }
}
