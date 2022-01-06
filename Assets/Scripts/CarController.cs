using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public Animator Animator;
  public GameObject SmokeParticles;
  public bool CarBroke = false;

  [HideInInspector] public float Progress;
  [HideInInspector] public float LanePosition;
  [HideInInspector] public float Speed;

  private void Start()
  {
    AudioController.instance.StartMotor();
  }

  void Update()
  {
    if (CarBroke)
    {
      Speed = Mathf.Clamp(Speed - 5 * Time.deltaTime, 0, float.MaxValue);
      SmokeParticles.SetActive(true);
    }

    if (!CarBroke)
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
        LanePosition += 0.2f * Speed * Time.deltaTime;
      }
      if (Input.GetKey(KeyCode.LeftArrow))
      {
        LanePosition -= 0.2f * Speed * Time.deltaTime;
      }
    }

    if (Speed == 0)
    {
      Animator.speed = 0;
    }
    else
    {
      Animator.speed = 1;
    }

    Progress += Time.deltaTime * Speed;
    SetTransformFromProgress(gameObject.transform, Progress, LanePosition);
    if (IsLanePositionOffroad(LanePosition))
    {
      CarBroke = true;
    }

    // set position of RoadGeneration
    var segmentLength = RoadGeneration.instance.segmentLength;
    var currentSegment = (int)(Progress / segmentLength);
    int index = currentSegment - 3;
    RoadGeneration.instance.segmentIndex = index >= 0 ? index : 0;
  }

  private void OnTriggerEnter(Collider other)
  {
    var bot = other.transform.GetComponent<CarriageBot>();
    HandleCollision(bot);
    AudioController.instance.PlayCollisionSound();
  }

  private void HandleCollision(CarriageBot bot)
  {
    Speed = bot.Speed - 2f;
    var collisionFromLeft = LanePosition < bot.LanePosition;
    bot.ChangeLane(bot.GetCurrentLaneIndex() + (collisionFromLeft ? 1 : -1));
  }

  public static CarriageBot.ProgressState SetTransformFromProgress(Transform car, float progress, float lanePosition)
  {
    var segmentLength = RoadGeneration.instance.segmentLength;
    var currentSegment = (int)(progress / segmentLength);
    if (currentSegment < RoadGeneration.instance.segmentIndex)
    {
      return CarriageBot.ProgressState.OUTRUN;
    }
    if (currentSegment > RoadGeneration.instance.roadSegments.Count - 1)
    {
      return CarriageBot.ProgressState.AHEAD;
    }
    var localProgress = progress % segmentLength;
    var prevPos = RoadGeneration.instance.roadSegments[currentSegment - 1].position;
    car.position = prevPos + RoadGeneration.instance.roadSegments[currentSegment].direction * localProgress;
    car.rotation = Quaternion.LookRotation(RoadGeneration.instance.roadSegments[currentSegment].direction, Vector3.up);
    car.position = car.position + car.right * lanePosition;
    return CarriageBot.ProgressState.LEVEL;
  }

  public bool IsLanePositionOffroad(float lanePosition)
  {
    var roadWidth = RoadGeneration.instance.laneCount * RoadGeneration.instance.laneWidth;
    var boundary = (roadWidth / 2f) + 0.25f;
    return lanePosition < -boundary || lanePosition > boundary;
  }
}
