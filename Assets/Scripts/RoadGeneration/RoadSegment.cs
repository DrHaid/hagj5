using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment
{
  public Vector3 position { get; set; }
  public Vector3 direction { get; set; }
  public List<Vector3> vertices { get; set; }
  public List<int> triangles { get; set; }

  public RoadSegment(RoadSegment prevSegment, Vector3 direction, float width, float length)
  {
    triangles = new List<int>
    {
      0, 1, 2,
      1, 3, 2
    };

    vertices = new List<Vector3>();
    vertices.Add((prevSegment == null) ? Vector3.zero : prevSegment.vertices[2]);
    vertices.Add((prevSegment == null) ? Vector3.zero : prevSegment.vertices[3]);

    this.direction = direction.normalized;
    position = (prevSegment == null ? Vector3.zero : prevSegment.position) + this.direction * length;
    var left = Vector3.Cross(this.direction, Vector3.up);
    var a = position + left.normalized * (width / 2);
    var b = position - left.normalized * (width / 2);
    vertices.Add(b);
    vertices.Add(a);
  }
}
