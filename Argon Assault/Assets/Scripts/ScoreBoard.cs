using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    TMP_Text scoreText;

    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (score == 0)
        {
            scoreText.text = "0";
        }
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        string stringScore = score.ToString();
        scoreText.text = stringScore;
    }
}
