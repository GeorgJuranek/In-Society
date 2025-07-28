using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class CountDisplay : MonoBehaviour
{
    private enum ECounts
    {
        current,
        total
    }

    [SerializeField]
    TextMeshProUGUI areaCountHeader;

    [SerializeField] GameObject ratCount;
    TextMeshProUGUI count;
    int maxCount;

    [SerializeField] GameObject ratAreaCount;
    TextMeshProUGUI areaCount;

    //[SerializeField] MovementKing ratKing;

    EAreas playerBelonging = EAreas.Error;

    Dictionary<EAreas,int> areaBook = new Dictionary<EAreas,int>();
    Dictionary<EAreas,int> totalsAreaBook = new Dictionary<EAreas,int>();

    void Awake()
    {
        for(int i = 0; i < Enum.GetValues(typeof(EAreas)).Length; i++)
        {
            areaBook.Add((EAreas)i,0);
            totalsAreaBook.Add((EAreas)i, 0);
        }

        count = ratCount.GetComponent<TextMeshProUGUI>();
        areaCount = ratAreaCount.GetComponent<TextMeshProUGUI>();

        RatBrain[] rats = FindObjectsOfType<RatBrain>();

        foreach (RatBrain rat in rats)
        {
            totalsAreaBook[rat.AreaBelonging]++;
        }

        maxCount = rats.Length;
        count.text = $"0/{maxCount}";
    }

    void OnEnable()
    {
        PlayerController.OnCountChange += HandleCountUpdate;
        PlayerController.OnAreaCountChange += HandleAreaCountUpdate;
        PlayerController.OnAreaChange += AreaInfoUpdate;
    }

    void OnDisable()
    {
        PlayerController.OnCountChange -= HandleCountUpdate;
        PlayerController.OnAreaCountChange -= HandleAreaCountUpdate;
    }

    public void HandleCountUpdate(int newCount)
    {
        count.text = $"{newCount}/{maxCount}";
    }

    public void HandleAreaCountUpdate(EAreas belonging, bool hasNewRat)
    {
        if (hasNewRat)
        {
            areaBook[belonging]++;
        }
        else // if a rat was lost
        {
            areaBook[belonging]--;
        }

        if (belonging != playerBelonging) //out of boundaries reach and first scenario
        {
            playerBelonging = belonging;
            areaCountHeader.text = $"{belonging}";
            areaCount.text = $"{areaBook[belonging]}/{totalsAreaBook[belonging]}";
        }
        else
        {
            areaCount.text = $"{areaBook[playerBelonging]}/{totalsAreaBook[playerBelonging]}";
        }
    }

    public void AreaInfoUpdate(EAreas newPlayerBelonging)
    {
        playerBelonging = newPlayerBelonging;

        areaCountHeader.text = $"{newPlayerBelonging}";
        areaCount.text = $"{areaBook[newPlayerBelonging]}/{totalsAreaBook[newPlayerBelonging]}";
    }
}
