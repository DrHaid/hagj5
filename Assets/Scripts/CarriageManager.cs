using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarriageManager : MonoBehaviour
{
  public List<Color32> CarriageColors;
  public List<GameObject> CarriagePrefabs;
  public float SpawnRate;
  public float MaxSpeed;
  public float MinSpeed;
  public List<CarriageBot> CarriageBots;

  public static CarriageManager instance;

  private const int noiseLength = 2048;
  private int lastSegmentIndex = 0;

  private void Awake()
  {
    instance = this;
  }

  void Start()
  {
    CarriageBots = new List<CarriageBot>();
  }

  void Update()
  {
    CarriageCleanup();
    for (int i = lastSegmentIndex; i < RoadGeneration.instance.roadSegments.Count; i += 5)
    {
      if (Random.value > SpawnRate)
      {
        continue;
      }

      var noise = Mathf.PerlinNoise(0, Mathf.Repeat(RoadGeneration.instance.roadSegments.Count, noiseLength) / noiseLength * 100);
      if (noise > 0.5f)
      {
        SpawnCarriage(i);
      }
    }
    lastSegmentIndex = RoadGeneration.instance.roadSegments.Count;
  }

  private void CarriageCleanup()
  {
    CarriageBot botToDelete = CarriageBots.FirstOrDefault(x => 
      x.State == CarriageBot.ProgressState.OUTRUN || x.State == CarriageBot.ProgressState.AHEAD);
    if (botToDelete != null)
    {
      Destroy(botToDelete.gameObject ?? null);
      CarriageBots.Remove(botToDelete);
    }
  }

  [ContextMenu("Spawn Carriage")]
  public void SpawnCarriage(int startingIndex)
  {
    var index = Random.Range(0, CarriagePrefabs.Count);
    var carriage = Instantiate(CarriagePrefabs[index]);
    var bot = carriage.GetComponent<CarriageBot>();
    bot.InitBot(Random.Range(MinSpeed, MaxSpeed), startingIndex, Random.Range(0, RoadGeneration.instance.laneCount));
    var color = Random.Range(0, CarriageColors.Count);
    var sprite = carriage.transform.GetChild(0);
    sprite.GetChild(0).GetComponent<SpriteRenderer>().color
      = CarriageColors[color];

    CarriageBots.Add(bot);
  }

  public bool IsLanePositionFree(float newLanePosition, CarriageBot carriageBot, float range)
  {
    foreach (var bot in CarriageBots)
    {
      if (bot.LanePosition == newLanePosition)
      {
        if (carriageBot.Progress < bot.Progress + range && carriageBot.Progress > bot.Progress - range)
        {
          return false;
        }
      }
    }
    return true;
  }
}
