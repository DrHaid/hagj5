using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public float progress;
  public float strafePosition;
  public float speed;
  private int currentSegment;

  void Start()
  {
    
  }

  void Update()
  {
    if (Input.GetKey(KeyCode.UpArrow))
    {
      speed += 0.01f;
    }
    if (Input.GetKey(KeyCode.DownArrow))
    {
      speed -= 0.02f;
    }
    if (Input.GetKey(KeyCode.RightArrow))
    {
      strafePosition += 5f * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      strafePosition -= 5f * Time.deltaTime;
    }
    progress += Time.deltaTime * speed;
    SetPositionFromProgress(progress, strafePosition);
    int index = currentSegment - 3;
    RoadGeneration.instance.segmentIndex = index >= 0 ? index : 0;
  }

  void SetPositionFromProgress(float progress, float strafePosition)
  {
    var segmentLength = RoadGeneration.instance.segmentLength;
    currentSegment = (int)(progress / segmentLength);
    var localProgress = progress % segmentLength;
    var prevPos = RoadGeneration.instance.roadSegments[currentSegment - 1].position;
    gameObject.transform.position = prevPos + RoadGeneration.instance.roadSegments[currentSegment].direction * localProgress;
    gameObject.transform.rotation = Quaternion.LookRotation(RoadGeneration.instance.roadSegments[currentSegment].direction, Vector3.up);
    gameObject.transform.position = gameObject.transform.position + gameObject.transform.right * strafePosition;
  }
}
