using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRatBrain : MonoBehaviour
{
    Animator fakeRatAnimator;

    private void Awake()
    {
        fakeRatAnimator = GetComponent<Animator>();
    }

    private void Cleaning()
    {
        fakeRatAnimator.SetFloat("Blend", 0f);
    }

    private void Walking()
    {
        fakeRatAnimator.SetFloat("Blend", 0.25f);
    }

    private void Running()
    {
        fakeRatAnimator.SetFloat("Blend", 0.5f);
    }


}
