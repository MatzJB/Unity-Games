using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Linq;
using Assets;
using UnityEditor;
using static CreateCards;
using UnityEngine.UIElements;
using System;
using UnityEditor.SceneManagement;


/* This class contains the game state of the game, level data, bonuses and penalties et cetera */
public class MyGameState : MonoBehaviour
{
    public List<LevelState> levels;
    public int stage=-1;

    // add state for menu, pause, running and end, replay


    void Start()
    {
        // loading level data
        stage = 0;
        levels = LevelDataReader.Load("LevelData");

    }

    // Rotates the cards around the center of the cloud
    void Tornado()
    {
    }

    // Reveals the cards by lighting a light bulb
    void Idea()
    {
    }

    void NextLevel()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }
}
