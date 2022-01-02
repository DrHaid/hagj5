using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageManager : MonoBehaviour
{
  public GameObject carriagePrefab;
  public List<CarriageBot> carriages;

  void Start()
  {
    carriages = new List<CarriageBot>();
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Space))
    {
      SpawnCarriage();
    }
  }

  [ContextMenu("Spawn Carriage")]
  public void SpawnCarriage()
  {
    var carriage = Instantiate(carriagePrefab);
    var bot = carriage.GetComponent<CarriageBot>();
    bot.InitBot(2f, Random.Range(0, RoadGeneration.instance.laneCount));
    carriages.Add(bot);
  }
}
