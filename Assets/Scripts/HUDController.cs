using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
  public TextMeshProUGUI DistanceTravelled;
  public TextMeshProUGUI Speed;
  public TextMeshProUGUI Lives;
  public TextMeshProUGUI GameOver;
  public TextMeshProUGUI GameWon;
  public TextMeshProUGUI LeftToPforzheim;
  public TextMeshProUGUI YouWon;
  public TextMeshProUGUI EnterToRestart;

  private Vector3 lastPos = Vector3.zero;

  private int lastLifeCount = 3;
  private bool gameOver;
  private bool gameWon;
  private bool showEnter;


  private void Update()
  {
    if (showEnter)
    {
      if (Input.GetKeyDown(KeyCode.Return))
      {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }
    }
  }

  void FixedUpdate()
  {
    if (!gameOver && !gameWon)
    {
      DistanceTravelled.text = $"Distance Travelled: {CarController.instance.actualDistance} m";
      var dist = Vector3.Distance(lastPos, CarController.instance.gameObject.transform.position);
      int speed = (int)(((dist * 2.5f) / Time.fixedDeltaTime) * 3.6f);
      speed = RoundDown(speed);
      Speed.text = $"Speed: {speed} km/h";
      lastPos = CarController.instance.gameObject.transform.position;
      if(lastLifeCount != CarController.instance.Lives)
      {
        Lives.text = $"Lives: {CarController.instance.Lives}";
        var anim = Lives.transform.GetComponent<Animator>();
        anim.SetTrigger("BlinkLives");
        lastLifeCount = CarController.instance.Lives;
      }
    }

    if (CarController.instance.CarStopped)
    {
      gameWon = true;
      Invoke("ShowCongratulations", 2f);
    }

    if (CarController.instance.CarBroke)
    {
      gameOver = true;
      Invoke("ShowGameOver", 1.5f);
    }
  }

  void ShowCongratulations()
  {
    GameWon.gameObject.SetActive(true);
    Invoke("ShowYouWon", 0.7f);
  }

  void ShowYouWon()
  {
    YouWon.gameObject.SetActive(true);
    Invoke("ShowEnterToRestart", 0.7f);
  }

  void ShowGameOver()
  {
    GameOver.gameObject.SetActive(true);
    Invoke("ShowLeftToPforzheim", 0.7f);
  }

  void ShowLeftToPforzheim()
  {
    LeftToPforzheim.gameObject.SetActive(true);
    LeftToPforzheim.text = $"{105000 - CarController.instance.actualDistance} m left until Pforzheim";
    Invoke("ShowEnterToRestart", 0.7f);
  }

  void ShowEnterToRestart()
  {
    EnterToRestart.gameObject.SetActive(true);
    showEnter = true;
  }

  int RoundDown(int toRound)
  {
    return toRound - toRound % 10;
  }
}
