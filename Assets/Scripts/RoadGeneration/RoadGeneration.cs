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
  public MeshFilter roadMeshFilter;
  
  public float curveChance;
  public float curveMinStrength;
  public float curveMaxStrength;
  public int curveMinLength;
  public int curveMaxLength;
  
  public float slopeChance;
  public float slopeMinStrength;
  public float slopeMaxStrength;
  public int slopeMinLength;
  public int slopeMaxLength;
               
  private Curve currentSlope;
  private Curve currentCurve;

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
        roadSegments.Add(new RoadSegment(null, Vector3.forward, 0.5f, 0.1f));
        continue;
      }
      var prevSeg = roadSegments[i - 1];
      roadSegments.Add(new RoadSegment(prevSeg, ApplyCurvature(prevSeg.direction), 0.5f, 0.1f));
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
    if (Random.value <= curveChance)
    {
      currentCurve = new Curve(curveMinLength, curveMaxLength, curveMinStrength, curveMaxStrength);
    }

    if (currentCurve != null)
    {
      direction = Quaternion.AngleAxis((currentCurve.Positive ? 1 : -1) * currentCurve.Strength, Vector3.up) * direction;
      currentCurve.DoCurvature(segmentLength);
      if(currentCurve.Length <= 0)
      {
        currentCurve = null;
      }
    }
    return direction;
  }
}
