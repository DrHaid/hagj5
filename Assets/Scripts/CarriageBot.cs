using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageBot : MonoBehaviour
{
  public float Progress;
  public float LanePosition;
  public float Speed;
  public bool SlowToStop;

  public CarriageBotState State;

  public enum CarriageBotState
  {
    AHEAD,
    LEVEL,
    OUTRUN
  }

  void Update()
  {
    if (changingLane)
    {
      LanePosition = Mathf.SmoothStep(oldLanePosition, newLanePosition, changingProgress);
      changingProgress += Time.deltaTime / (Mathf.Abs(newLanePosition - oldLanePosition)) * 0.7f;

      if (changingProgress >= 1f)
      {
        changingLane = false;
        LanePosition = newLanePosition;
      }
      if (GetCurrentLaneIndex() < 0 || GetCurrentLaneIndex() > RoadGeneration.instance.laneCount - 1)
      {
        SlowToStop = true;
      }
    }
    if (SlowToStop)
    {
      Speed = Mathf.Clamp(Speed - Time.deltaTime, 0, float.MaxValue);
      if (Speed == 0)
      {
        SlowToStop = false;
      }
    }

    State = CarController.SetTransformFromProgress(gameObject.transform, Progress, LanePosition);
    if(State == CarriageBotState.OUTRUN)
    {
      // wait for destruction by CarriageManager
      return;
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
    LanePosition = GetLanePosition(startingLane);
  }

  private float GetLanePosition(int laneIndex)
  {
    var posFromLeft = laneIndex * RoadGeneration.instance.laneWidth + (RoadGeneration.instance.laneWidth / 2);
    return -((RoadGeneration.instance.laneWidth * RoadGeneration.instance.laneCount) / 2) + posFromLeft;
  }

  public int GetCurrentLaneIndex()
  {
    var rightShift = ((RoadGeneration.instance.laneWidth * RoadGeneration.instance.laneCount) / 2);
    return (int)((LanePosition + rightShift) / RoadGeneration.instance.laneWidth);
  }

  private bool changingLane = false;
  private float oldLanePosition;
  private float newLanePosition;
  private float changingProgress = 0f;
  public void ChangeLane(int newLaneIndex)
  {
    changingLane = true;
    oldLanePosition = LanePosition;
    newLanePosition = GetLanePosition(newLaneIndex);
    changingProgress = 0f;
  }
}
