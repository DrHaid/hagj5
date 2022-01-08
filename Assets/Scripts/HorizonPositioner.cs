using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonPositioner : MonoBehaviour
{
  private Transform horizonPart;
  private Canvas canvas;
  private Camera mainCam;
  private Vector3 initDir;

  void Start()
  {
    horizonPart = gameObject.transform.GetChild(0).GetChild(0);
    canvas = gameObject.transform.GetComponent<Canvas>();
    mainCam = Camera.main;
    initDir = mainCam.transform.forward;
    initDir.y = 0;
  }

  void Update()
  {
    var dir = mainCam.transform.forward;
    dir.y = 0;
    var angle = Vector3.SignedAngle(initDir, dir, Vector3.up);
    var xPos = Remap(angle, -180, 180, 0, -4096);

    // make horizon stick to last RoadSegment
    if (RoadGeneration.instance.roadSegments.Count == 0)
    {
      return;
    }
    var roadSegment = 
      RoadGeneration.instance.roadSegments[RoadGeneration.instance.roadSegments.Count - 1];
    Vector2 viewport = Camera.main.WorldToViewportPoint(roadSegment.position);
    Ray canvasRay = canvas.worldCamera.ViewportPointToRay(viewport);
    var point = canvasRay.GetPoint(canvas.planeDistance);
    var pos = horizonPart.parent.position;
    pos.y = point.y;
    horizonPart.parent.position = pos;
    horizonPart.localPosition = new Vector3(xPos, 0, 0);
  }

  public static float Remap(float value, float from1, float to1, float from2, float to2)
  {
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
  }
}
