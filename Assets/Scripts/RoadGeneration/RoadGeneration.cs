using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGeneration : MonoBehaviour
{
  public int segmentIndex = 0;
  public int segmentCount = 20;
  public float roadWidth = 0.5f;
  public float segmentLength = 0.1f;
  public List<RoadSegment> roadSegments;
  [HideInInspector] public MeshFilter roadMeshFilter;

  public CurveSettings curveSettings;
  public SlopeSettings slopeSettings;

  private Curve currentCurve;
  private Slope currentSlope;

  void Start()
  {
    roadSegments = new List<RoadSegment>();
    roadMeshFilter = GetComponent<MeshFilter>();
  }

  private void FixedUpdate()
  {
    roadMeshFilter.mesh = GetRoadMeshSection(segmentIndex, segmentCount);
  }

  public Mesh GetRoadMeshSection(int index, int count)
  {
    if (index < 0 || count < 0)
    {
      Debug.LogError("negative count or index");
      return null;
    }
    if (roadSegments.Count == 0)
    {
      GenerateRoadSegment(0, index + count);
    }
    else if (index + count > roadSegments.Count)
    {
      GenerateRoadSegment(roadSegments.Count, count - ((roadSegments.Count) - index));
    }
    return GetBuiltTerrainMesh(index, count);
  }

  private void GenerateRoadSegment(int index, int count)
  {
    for (int i = index; i < index + count; i++)
    {
      if (i == 0)
      {
        roadSegments.Add(new RoadSegment(null, Vector3.forward, roadWidth, segmentLength));
        continue;
      }
      var prevSeg = roadSegments[i - 1];
      
      roadSegments.Add(new RoadSegment(prevSeg, ApplyCurvature(prevSeg.direction), roadWidth, segmentLength));
    }
  }

  private Mesh GetBuiltTerrainMesh(int index, int count)
  {
    Mesh roadMesh = new Mesh();
    roadMesh.Clear();

    var vertices = new List<Vector3>();
    var triangles = new List<int>();
    for (int i = index; i < index + count; i++)
    {
      vertices.AddRange(roadSegments[i].vertices);
      var addedTriangleIndices = new List<int>();
      foreach (var triangle in roadSegments[i].triangles)
      {
        addedTriangleIndices.Add(triangle + (4 * (i - index)));
      }
      triangles.AddRange(addedTriangleIndices);
    }
    roadMesh.vertices = vertices.ToArray();
    roadMesh.triangles = triangles.ToArray();
    roadMesh.uv = GetUvs(roadMesh.vertices);
    roadMesh.RecalculateNormals();
    return roadMesh;
  }

  public Vector2[] GetUvs(Vector3[] vertices)
  {
    Vector2[] uvs = new Vector2[vertices.Length];
    for (int i = 0; i < uvs.Length; i++)
    {
      uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
    }
    return uvs;
  }

  public Vector3 ApplyCurvature(Vector3 direction)
  {
    // choose either curve or slope
    if (currentCurve == null && currentSlope == null)
    {
      if (Random.value <= curveSettings.Chance)
      {
        currentCurve = new Curve(curveSettings);
      }
      else if (Random.value <= slopeSettings.Chance)
      {
        currentSlope = new Slope(slopeSettings);
      }
    }

    if (currentCurve != null)
    {
      direction = Quaternion.AngleAxis((currentCurve.Positive ? 1 : -1) * currentCurve.Strength, Vector3.up) * direction;
      if (!currentCurve.DoCurvature(segmentLength))
      {
        currentCurve = null;
      }
    }

    if (currentSlope != null)
    {
      var left = Vector3.Cross(direction, Vector3.up);
      // HACK: road leveling out isn't guaranteed.
      if (currentSlope.Length > currentSlope.OriginalLength / 2f)
      {
        // smooth lerp towards steepest part
        direction.y = 0;
        direction = Quaternion.AngleAxis(
          Mathf.SmoothStep(currentSlope.SlopeValue * 2, 0, currentSlope.Length / currentSlope.OriginalLength), 
          left) * direction;
      }
      else
      {
        // smooth lerp back towards zero
        direction.y = 0;
        direction = Quaternion.AngleAxis(
          InvertedSmoothStep(currentSlope.SlopeValue, 0, currentSlope.Length / (currentSlope.OriginalLength / 2)),
          left) * direction;
      }

      if (!currentSlope.DoSlope(segmentLength))
      {
        currentSlope = null;
      }
    }
    
    return direction;
  }

  public static float InvertedSmoothStep(float from, float to, float t)
  {
    t = Mathf.Clamp01(t);
    t = -2.0F * t * t * t + 3.0F * t * t;
    return to * t + from * t;
  }

}
