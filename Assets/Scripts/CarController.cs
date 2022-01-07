using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public int Lives = 3;
  public float MaxSpeed = 15f;
  public Animator Animator;
  public GameObject SmokeParticles;
  public bool CarBroke = false;
  public bool CarStopped = false;

  [HideInInspector] public float Progress;
  [HideInInspector] public float LanePosition;
  [HideInInspector] public float Speed;
  [HideInInspector] public int actualDistance;

  [HideInInspector] public float initProgress;
  private float collisionTimeout;

  public static CarController instance;


  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    AudioController.instance.StartMotor();
    initProgress = Progress;
  }

  void Update()
  {
    if (CarStopped)
    {
      Speed = HorizonPositioner.Remap(
        actualDistance, RoadGeneration.pforzheimDistance - 100, RoadGeneration.pforzheimDistance - 5, MaxSpeed, 0);
    }
    if (CarBroke)
    {
      Speed = Mathf.Clamp(Speed - 5 * Time.deltaTime, 0, float.MaxValue);
      SmokeParticles.SetActive(true);
    }
    collisionTimeout = Mathf.Clamp(collisionTimeout - Time.deltaTime, 0, float.MaxValue);

    if (!CarBroke && !CarStopped)
    {
      if (Input.GetKey(KeyCode.UpArrow))
      {
        Speed = Mathf.Clamp(Speed + 0.02f, 0f, MaxSpeed); ;
      }
      if (Input.GetKey(KeyCode.DownArrow))
      {
        Speed = Mathf.Clamp(Speed - 0.02f, 0f, float.MaxValue);
      }
      if (Input.GetKey(KeyCode.RightArrow))
      {
        LanePosition += 0.15f * Speed * Time.deltaTime;
      }
      if (Input.GetKey(KeyCode.LeftArrow))
      {
        LanePosition -= 0.15f * Speed * Time.deltaTime;
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
    actualDistance = (int)((Progress - initProgress) * 2.5f);
    SetTransformFromProgress(gameObject.transform, Progress, LanePosition);
    if (IsLanePositionOffroad(LanePosition))
    {
      CarBroke = true;
    }
    if (actualDistance == RoadGeneration.pforzheimDistance - 50)
    {
      CarStopped = true;
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
    Lives--;
    collisionTimeout = 1f;
    if (Lives == 0)
    {
      CarBroke = true;
    }
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
    var boundary = (roadWidth / 2f) + 0.2f;
    return lanePosition < -boundary || lanePosition > boundary;
  }
}
