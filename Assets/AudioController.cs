using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
  public AudioSource carMotor;
  public AudioSource carMotorStarting;
  public AudioSource collision;

  public static AudioController instance;

  private void Awake()
  {
    instance = this;
  }

  public void StartMotor()
  {
    carMotor.PlayDelayed(2);
    carMotorStarting.Play();
  }

  public void PlayCollisionSound()
  {
    collision.Play();
  }
}
