﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour
{
    Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        ScoreManager.score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = ScoreManager.score.ToString();
    }
}
