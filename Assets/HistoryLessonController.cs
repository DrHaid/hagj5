using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryLessonController : MonoBehaviour
{
  public Animator animator;

  void Start()
  {

  }

  void Update()
  {
    if (Input.GetKey(KeyCode.Return))
    {
      animator.speed = 5f;
    }
    else
    {
      animator.speed = 1f;
    }

    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7)
    {
      if (Input.GetKeyDown(KeyCode.Return))
      {
        // load next scene
      }
    }
  }
}
