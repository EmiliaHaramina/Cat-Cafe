using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatGrabAnimation : MonoBehaviour  
{

    public RuntimeAnimatorController grabAnimatorController;
    public RuntimeAnimatorController letGoAnimatorController;
    public Animator catAnimator;

    public void OnGrabAnimation()
    {
        Debug.Log("Hi");
        catAnimator.runtimeAnimatorController = grabAnimatorController;
        catAnimator.Play("Entry");
    }

    public void OnLetGoAnimation()
    {
        Debug.Log("Bye");
        catAnimator.runtimeAnimatorController = letGoAnimatorController;
        catAnimator.Play("Entry");
    }
}
