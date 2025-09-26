using System;
using System.Collections.Generic;
using UnityEngine;

public class RepetitionTracker
{
    private Dictionary<string, int> positionCounts = new Dictionary<string, int>();
    private const int MaxPositions = 500;


    public void RecordPosition(string positionKey)
    {
        if (positionCounts.ContainsKey(positionKey))
            positionCounts[positionKey]++;
        else
            positionCounts[positionKey] = 1;

        if (positionCounts[positionKey] >= 3)
        {
            Debug.Log("Threefold repetition - draw");
            GameManager.Instance.TriggerThirdRepetition();
        }

        if (positionCounts.Count > MaxPositions)
        {
            var firstKey = new List<string>(positionCounts.Keys)[0];
            positionCounts.Remove(firstKey);
        }
    }

    public void Reset()
    {
        positionCounts.Clear();
    }
}
