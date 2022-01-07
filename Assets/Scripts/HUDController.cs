using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
  public TextMeshProUGUI DistanceTravelled;
  public TextMeshProUGUI Speed;
  public TextMeshProUGUI Lives;

  private Vector3 lastPos = Vector3.zero;
  private float initProgress;
  private int actualDistance;

  private void Start()
  {
    initProgress = CarController.instance.Progress;
  }

  void FixedUpdate()
  {
    actualDistance = (int)((CarController.instance.Progress - initProgress) * 2.5f);
    DistanceTravelled.text = $"Distance Travelled: {actualDistance} m";
    var dist = Vector3.Distance(lastPos, CarController.instance.gameObject.transform.position);
    int speed = (int)(((dist * 2.5f) / Time.fixedDeltaTime) * 3.6f);
    speed = RoundDown(speed);
    Speed.text = $"Speed: {speed} km/h";
    lastPos = CarController.instance.gameObject.transform.position;
    Lives.text = $"Lives: {CarController.instance.Lives}";
  }

  int RoundDown(int toRound)
  {
    return toRound - toRound % 10;
  }
}
