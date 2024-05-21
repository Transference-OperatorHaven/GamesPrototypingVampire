using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    [SerializeField]int enemyCount = 11;
    [SerializeField] Canvas canvas;
    [SerializeField]TextMeshProUGUI timeText;

    private void Start()
    {

    }

    public void ReduceEnemyCount()
    {
        enemyCount--;
        if (enemyCount == 0)
        {
            Victory();
        }
    }

    void Victory()
    {
        canvas.gameObject.SetActive(true);
        timeText.text = Time.timeSinceLevelLoad.ToString();
        Time.timeScale = 0;

        
    }

}
