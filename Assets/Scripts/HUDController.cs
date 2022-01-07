using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
  public TextMeshProUGUI DistanceTravelled;
  public TextMeshProUGUI Speed;
  public TextMeshProUGUI Lives;

  private float lastProgress = 0;

  void FixedUpdate()
  {
    DistanceTravelled.text = $"Distance Travelled: {(int)CarController.instance.Progress} m";
    int speed = (int)(((CarController.instance.Progress - lastProgress) / Time.fixedDeltaTime) * 3.6f);
    speed = RoundDown(speed);
    Speed.text = $"Speed: {speed} km/h";
    lastProgress = CarController.instance.Progress;
    Lives.text = $"Speed: {CarController.instance.Lives}";
  }

  int RoundDown(int toRound)
  {
    return toRound - toRound % 10;
  }
}
