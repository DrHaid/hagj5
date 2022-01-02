using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonPositioner : MonoBehaviour
{
  private Transform horizon;
  private Canvas canvas;

  void Start()
  {
    horizon = gameObject.transform.GetChild(0);
    canvas = gameObject.transform.GetComponent<Canvas>(); 
  }

  void Update()
  {
    // make horizon stick to last RoadSegment
    var roadSegment = 
      RoadGeneration.instance.roadSegments[RoadGeneration.instance.roadSegments.Count - 1];
    Vector2 viewport = Camera.main.WorldToViewportPoint(roadSegment.position);
    Ray canvasRay = canvas.worldCamera.ViewportPointToRay(viewport);
    var point = canvasRay.GetPoint(canvas.planeDistance);
    var pos = horizon.position;
    pos.y = point.y;
    horizon.position = pos;
  }
}
