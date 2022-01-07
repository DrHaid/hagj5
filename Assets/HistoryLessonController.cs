using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HistoryLessonController : MonoBehaviour
{
  public Animator Animator;
  public GameObject StartTrip;

  void Start()
  {

  }

  void Update()
  {
    if (Input.GetKey(KeyCode.Return))
    {
      Animator.speed = 5f;
    }
    else
    {
      Animator.speed = 1f;
    }

    if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7)
    {
      StartTrip.SetActive(true);
      if (Input.GetKeyDown(KeyCode.Return))
      {
        SceneManager.LoadScene(2);
      }
    }
  }
}
