using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarriageManager : MonoBehaviour
{
  public List<Color32> CarriageColors;
  public List<GameObject> CarriagePrefabs;
  public float MaxSpeed;
  public float MinSpeed;
  public List<CarriageBot> CarriageBots;

  public static CarriageManager instance;

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

    if(Input.GetKeyDown(KeyCode.Space))
    {
      SpawnCarriage();
    }
    if (Input.GetKeyDown(KeyCode.C))
    {
      CarriageBots[0].ChangeLane(0);
    }
  }

  private void CarriageCleanup()
  {
    CarriageBot botToDelete = CarriageBots.FirstOrDefault(x => x.State == CarriageBot.ProgressState.OUTRUN);
    if (botToDelete != null)
    {
      Destroy(botToDelete.gameObject ?? null);
      CarriageBots.Remove(botToDelete);
    }
  }

  [ContextMenu("Spawn Carriage")]
  public void SpawnCarriage()
  {
    var index = Random.Range(0, CarriagePrefabs.Count);
    var carriage = Instantiate(CarriagePrefabs[index]);
    var bot = carriage.GetComponent<CarriageBot>();
    bot.InitBot(Random.Range(MinSpeed, MaxSpeed), Random.Range(0, RoadGeneration.instance.laneCount));
    var color = Random.Range(0, CarriageColors.Count);
    carriage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color 
      = CarriageColors[color];
    CarriageBots.Add(bot);
  }
}
