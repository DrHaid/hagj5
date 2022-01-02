using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageBot : MonoBehaviour
{
  public float Progress;
  public float LanePosition;
  public float Speed;

  public CarriageBotState State;

  public enum CarriageBotState
  {
    AHEAD,
    LEVEL,
    OUTRUN
  }

  void Update()
  {
    LanePosition = CarController.ClampLanePosition(LanePosition, gameObject.transform.localScale.x);
    State = CarController.SetTransformFromProgress(gameObject.transform, Progress, LanePosition);
    if(State == CarriageBotState.OUTRUN)
    {
      Destroy(gameObject);
    }
    if(State != CarriageBotState.AHEAD)
    {
      Progress += Time.deltaTime * Speed;
    }
  }

  public void InitBot(float speed, int startingLane)
  {
    Speed = speed;
    Progress = RoadGeneration.instance.roadSegments.Count * RoadGeneration.instance.segmentLength;
    var posFromLeft = startingLane * RoadGeneration.instance.laneWidth + (RoadGeneration.instance.laneWidth / 2);
    LanePosition = -((RoadGeneration.instance.laneWidth * RoadGeneration.instance.laneCount) / 2) + posFromLeft;
  }
}
