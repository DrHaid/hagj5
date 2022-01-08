using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageBot : MonoBehaviour
{
  public float Progress;
  public float LanePosition;
  public float Speed;
  public bool SlowToStop;

  public ProgressState State;

  public enum ProgressState
  {
    LEVEL,
    AHEAD,
    OUTRUN
  }

  void FixedUpdate()
  {
    if (changingLane)
    {
      LanePosition = Mathf.SmoothStep(oldLanePosition, newLanePosition, changingProgress);
      changingProgress += (Time.fixedDeltaTime) / (Mathf.Abs(newLanePosition - oldLanePosition)) * 0.3f;
      if (slowDown)
      {
        Speed = prevSpeed * 0.75f;
      }

      if (changingProgress >= 1f)
      {
        float randomTime = Random.Range(5, 10);
        Invoke("ChangeLaneRoutine", randomTime);
        slowDown = false;
        Speed = prevSpeed;
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
      Speed = Mathf.Clamp(Speed - Time.fixedDeltaTime, 0, float.MaxValue);
      if (Speed == 0)
      {
        SlowToStop = false;
      }
    }

    State = CarController.SetTransformFromProgress(gameObject.transform, Progress, LanePosition);
    if (State == ProgressState.OUTRUN || State == ProgressState.AHEAD)
    {
      // wait for destruction by CarriageManager
      return;
    }
    Progress += Time.fixedDeltaTime * Speed;
  }

  public void InitBot(float speed, int startingIndex, int startingLane)
  {
    State = ProgressState.LEVEL;
    Speed = speed;
    Progress = (RoadGeneration.instance.roadSegments.Count - 5) * RoadGeneration.instance.segmentLength;
    LanePosition = GetLanePosition(startingLane);
    Invoke("ChangeLaneRoutine", Random.Range(2, 5));
  }

  public float GetLanePosition(int laneIndex)
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
  private float prevSpeed;
  private bool slowDown;
  private float oldLanePosition;
  private float newLanePosition;
  private float changingProgress = 0f;
  public void ChangeLane(int newLaneIndex, bool slowDown = false)
  {
    changingLane = true;
    prevSpeed = Speed;
    this.slowDown = slowDown;
    oldLanePosition = LanePosition;
    newLanePosition = GetLanePosition(newLaneIndex);
    changingProgress = 0f;
  }

  public void ChangeLaneRoutine()
  {
    int targetIndex;
    bool slowDown;
    var rand = Random.value > 0.5f;
    var newLane = GetCurrentLaneIndex() + (rand ? 1 : -1);
    var altLane = GetCurrentLaneIndex() + (!rand ? 1 : -1);
    if (newLane >= 0 && newLane < RoadGeneration.instance.laneCount)
    {
      targetIndex = newLane;
      var lanePos = GetLanePosition(newLane);
      if (CarriageManager.instance.IsLanePositionFree(lanePos, this, 0.4f))
      {
        slowDown = false;
      }
      else
      {
        slowDown = true;
      }
    }
    else
    {
      targetIndex = altLane;
      var lanePos = GetLanePosition(altLane);
      if (CarriageManager.instance.IsLanePositionFree(lanePos, this, 0.4f))
      {
        slowDown = false;
      }
      else
      {
        slowDown = true;
      }
    }
    ChangeLane(targetIndex, slowDown);
  }
}
