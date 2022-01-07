using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
  public GameObject Info;
  public GameObject PressStart;
  public GameObject MusicPlayer;
  private bool infoShown;

  private void Start()
  {
    DontDestroyOnLoad(MusicPlayer);
  }

  public void ShowInfo()
  {
    Info.SetActive(true);
    PressStart.SetActive(false);
    infoShown = true;
  }

  public void HideInfo()
  {
    Info.SetActive(false);
    PressStart.SetActive(true);
    infoShown = false;
  }

  void Update()
  {
    if (infoShown)
    {
      return;
    }
    if (Input.GetKeyDown(KeyCode.Return))
    {
      SceneManager.LoadScene(1);
    }
  }
}
