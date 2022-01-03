using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarriageManager : MonoBehaviour
{
  public GameObject CarriagePrefab;
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
    Destroy(botToDelete);
    CarriageBots.Remove(botToDelete); 
  }

  [ContextMenu("Spawn Carriage")]
  public void SpawnCarriage()
  {
    var carriage = Instantiate(CarriagePrefab);
    var bot = carriage.GetComponent<CarriageBot>();
    bot.InitBot(Random.Range(MinSpeed, MaxSpeed), Random.Range(0, RoadGeneration.instance.laneCount));
    CarriageBots.Add(bot);
  }
}
