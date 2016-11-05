using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Pipe leakingPipe;
    public bool valveCanTurn;

    public AudioSource leakingSound, explosionSound, fillingSound;

    public Canvas menu;
    public Canvas game;
    public Canvas endMenu;
    public Text timeText;

    private GameObject valve;
    public Transform levels;
    public Transform board;

    public GameStateBoru State { get; set; }

    public int NumberOfPipes { get; set; }
    public int Level { get; set; }
    public int ShadowValue { get; set; }
    public float LevelTime { get; set; }
    public int LeakPipeNumber { get; set; }
    public int Place { get; set; }
    private int[,] grid;
    private int[,] places;
    private GameObject currentLevel;
    SkeletonAnimation tutorialAnimation;
    Transform tutorial;

    private List<int[,]> level1List, level1PlaceList,
                         level2List, level2PlaceList,
                         level3List, level3PlaceList,
                         level4List, level4PlaceList,
                         level5List, level5PlaceList,
                         level6List, level6PlaceList,
                         level7List, level7PlaceList,
                         level8List, level8PlaceList;

    

    // Level 1
    private int[,] level1_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level1_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level1_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level1_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level1_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level1_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level1PipeNumber = 16;

    private int level1LeakPipe = 0;

    // Level 2
    private int[,] level2_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level2_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level2_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level2_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level2_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level2_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level2PipeNumber = 16;

    private int level2LeakPipe = 0;

    // Level 3
    private int[,] level3_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level3_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level3_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level3_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level3_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level3_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level3PipeNumber = 16;

    private int level3LeakPipe = 0;

    // Level 4
    private int[,] level4_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level4_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level4_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level4_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level4_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level4_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level4PipeNumber = 16;

    private int level4LeakPipe = 0;

    // Level 5
    private int[,] level5_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level5_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level5_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level5_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level5_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level5_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level5PipeNumber = 16;

    private int level5LeakPipe = 1;

    // Level 6
    private int[,] level6_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level6_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level6_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level6_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level6_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level6_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level6PipeNumber = 16;

    private int level6LeakPipe = 1;

    // Level 7
    private int[,] level7_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level7_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level7_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level7_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level7_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level7_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level7PipeNumber = 16;

    private int level7LeakPipe = 2;

    // Level 8
    private int[,] level8_1 =      {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level8_1Place = {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level8_2 =      {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level8_2Place = {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level8_3 =      {{ 5, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    { 6, -1, -1,  0, -1, -1,  6, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level8_3Place = {{ 0, -1, -1,  7,  8,  9, 10, -1, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int level8PipeNumber = 16;

    private int level8LeakPipe = 2;

    private List<Pipe> pipeList;

    private Vector3 pipePosition;

    private float timer;
    private float timePassed;

    private bool allPlaced;
    private bool allFilled;
    private bool levelCompleted;
    private bool tutorialBool;
    private int levelsPassed;

    // Singleton
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }

            return instance;
        }
    }


    void Start()
    {
        // Level 1
        level1List = new List<int[,]>(); level1PlaceList = new List<int[,]>();
        level1List.Add(level1_1); level1List.Add(level1_2); level1List.Add(level1_3);
        level1PlaceList.Add(level1_1Place); level1PlaceList.Add(level1_2Place); level1PlaceList.Add(level1_3Place);

        // Level 2
        level2List = new List<int[,]>(); level2PlaceList = new List<int[,]>();
        level2List.Add(level2_1); level2List.Add(level2_2); level2List.Add(level2_3);
        level2PlaceList.Add(level2_1Place); level2PlaceList.Add(level2_2Place); level2PlaceList.Add(level2_3Place);

        // Level 3
        level3List = new List<int[,]>(); level3PlaceList = new List<int[,]>();
        level3List.Add(level3_1); level3List.Add(level3_2); level3List.Add(level3_3);
        level3PlaceList.Add(level3_1Place); level3PlaceList.Add(level3_2Place); level3PlaceList.Add(level3_3Place);

        // Level 4
        level4List = new List<int[,]>(); level4PlaceList = new List<int[,]>();
        level4List.Add(level4_1); level4List.Add(level4_2); level4List.Add(level4_3);
        level4PlaceList.Add(level4_1Place); level4PlaceList.Add(level4_2Place); level4PlaceList.Add(level4_3Place);

        // Level 5
        level5List = new List<int[,]>(); level5PlaceList = new List<int[,]>();
        level5List.Add(level5_1); level5List.Add(level5_2); level5List.Add(level5_3);
        level5PlaceList.Add(level5_1Place); level5PlaceList.Add(level5_2Place); level5PlaceList.Add(level5_3Place);

        // Level 6
        level6List = new List<int[,]>(); level6PlaceList = new List<int[,]>();
        level6List.Add(level6_1); level6List.Add(level6_2); level6List.Add(level6_3);
        level6PlaceList.Add(level6_1Place); level6PlaceList.Add(level6_2Place); level6PlaceList.Add(level6_3Place);

        // Level 7
        level7List = new List<int[,]>(); level7PlaceList = new List<int[,]>();
        level7List.Add(level7_1); level7List.Add(level7_2); level7List.Add(level7_3);
        level7PlaceList.Add(level7_1Place); level7PlaceList.Add(level7_2Place); level7PlaceList.Add(level7_3Place);

        // Level 8
        level8List = new List<int[,]>(); level8PlaceList = new List<int[,]>();
        level8List.Add(level8_1); level8List.Add(level8_2); level8List.Add(level8_3);
        level8PlaceList.Add(level8_1Place); level8PlaceList.Add(level8_2Place); level8PlaceList.Add(level8_3Place);
        
        levelsPassed = PlayerPrefs.GetInt("LevelsPassed");

        tutorial = transform.GetChild(0);
        tutorialAnimation = transform.GetChild(1).GetComponent<SkeletonAnimation>();

        for (int i = 1; i < 9; i++)
        {
            menu.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = "%" + PlayerPrefs.GetFloat("Level" + i + "Percentage");
            if (i <= levelsPassed)
            {
                menu.transform.GetChild(i + 1).gameObject.SetActive(true);
            }
        }

        pipePosition = new Vector2(0.0f, -11.0f);
    }



    void Update()
    {
        if (State == GameStateBoru.FillWater && valve.GetComponent<Valve>().Opened)
        {
            leakingSound.Stop();
            if (!fillingSound.isPlaying)
            {
                fillingSound.Play();
            }
        }
        else if(State == GameStateBoru.Leaking)
        {
            fillingSound.Stop();
            if (!leakingSound.isPlaying)
            {
                leakingSound.volume = 1;
                leakingSound.Play();
            }
        }
        if (LevelTime < 0 && State != GameStateBoru.MainMenu)
        {
            endMenu.gameObject.SetActive(true);
            endMenu.transform.GetChild(1).GetComponent<Text>().text = "Süreniz Doldu.";
            Time.timeScale = 0;
            State = GameStateBoru.Paused;
        }

        if (currentLevel != null) // Playing a level.
        {
            if (PlayerPrefs.GetFloat("Level" + Level + "Percentage") < Place * 100 / NumberOfPipes)
            {
                if (Place * 100 / NumberOfPipes > 100)
                {
                    PlayerPrefs.SetFloat("Level" + Level + "Percentage", 100);
                }
                else
                {
                    PlayerPrefs.SetFloat("Level" + Level + "Percentage", Place * 100 / NumberOfPipes);
                    menu.transform.GetChild(Level).GetChild(2).GetComponent<Text>().text = "%" + PlayerPrefs.GetFloat("Level" + Level + "Percentage");
                }
            }

            if (allPlaced)
            {
                if (Level == 1 && !tutorialBool)
                {
                    tutorial.gameObject.SetActive(true);
                    transform.GetChild(1).gameObject.SetActive(true);
                    currentLevel.SetActive(false);
                    game.gameObject.SetActive(false);
                    tutorialAnimation.AnimationName = "level5_valveOpening";
                }

                if (!allFilled && valve.GetComponent<Valve>().Opened)
                {
                    if (State != GameStateBoru.MainMenu)
                    {
                        StartWater();
                    }
                }
                if (levelCompleted)
                {
                    Destroy(currentLevel);
                    menu.gameObject.SetActive(true);
                    game.transform.GetChild(4).gameObject.SetActive(false);
                    game.transform.GetChild(4).GetComponent<Slider>().value = 0;
                    game.gameObject.SetActive(false);
                    fillingSound.Stop();

                    if (State != GameStateBoru.MainMenu)
                    {
                        if (PlayerPrefs.GetInt("LevelsPassed") < 10)
                        {
                            levelsPassed++;
                            PlayerPrefs.SetInt("LevelsPassed", levelsPassed);

                            menu.transform.GetChild(levelsPassed + 1).gameObject.SetActive(true);
                        }
                    }

                    State = GameStateBoru.MainMenu;
                }
            }
            else
            {
                // Update time and check all placed.
                timeText.text = "Kalan Süre: " + Mathf.Ceil(LevelTime);
                if (Level % 2 == 0)
                {
                    timeText.text += " (Süre %10 azaltıldı.)";
                }
                LevelTime -= Time.deltaTime;
                timer += Time.deltaTime;
                allPlaced = AllPipesPlaced();
            }
        }
    }

    public Vector3 getPipePosition()
    {
        return pipePosition;
    }

    public int CheckGrid(int y, int x)
    {
        return grid[y, x];
    }

    public int CheckPlace(int y, int x)
    {
        return places[y, x];
    }

    public bool CurrentPlace(int y, int x)
    {
        return places[y, x] == Place;
    }

    public void PlaceToGrid(int y, int x)
    {
        grid[y, x] = -1;
    }

    public void MoveFromGrid(int y, int x, int type)
    {
        grid[y, x] = type;
    }

    public bool AllPipesPlaced()
    {
        // Check all pipes are placed.
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != -1)
                {
                    return false;
                }
            }
        }

        // If placed finish the level and start valve mechanic.
        timePassed = timer;
        PlayerPrefs.SetFloat("Level" + Level + "Time", timePassed);
        valve.SetActive(true);
        game.transform.GetChild(4).gameObject.SetActive(true);

        // Determine the leaked pipes.
        for (int i = 0; i < LeakPipeNumber; i++)
        {
            int randVal = Random.Range(0, pipeList.Count - 1);

            if (!pipeList[randVal].Leak)
            {
                pipeList[randVal].Leak = true;
            }
        }

        State = GameStateBoru.FillWater;

        return true;
    }

    public void AddPipe(int position, Pipe pipe)
    {
        pipeList.Insert(position, pipe);
        pipeList.RemoveAt(position + 1);
    }

    public void StartWater()
    {
        foreach (Pipe pipe in pipeList)
        {
            if (pipe.Leak)
            {
                leakingPipe = pipe;
                valveCanTurn = pipe.holding;

                if (pipe.transform.GetChild(4).gameObject.activeInHierarchy == false)
                {
                    State = GameStateBoru.Leaking;
                    pipe.transform.GetChild(4).gameObject.SetActive(true);
                    explosionSound.Play();
                    StartCoroutine(ShowLeakingHint(10));

                    if (Level == 5)
                    {
                        explosionSound.Stop();
                        tutorial.gameObject.SetActive(true);
                        transform.GetChild(1).gameObject.SetActive(true);
                        tutorialAnimation.AnimationName = "level5_pipeLeak";
                        currentLevel.SetActive(false);
                        game.gameObject.SetActive(false);
                    }
                }
                return;
            }
            if (!pipe.Filled)
            {
                pipe.Filled = pipe.FillWater();
                break;
            }
            if (pipe == pipeList[pipeList.Count - 1])
            {
                allFilled = true;
                levelCompleted = true;
            }
        }
    }
    
    public int pipeTypeAtPosition(Vector2 position)
    {
        int yPos = (int)(position[1] / -2);
        int xPos = (int)(position[0] / 2);
        return grid[yPos, xPos];
    }

    public void ShowHint()
    {
        GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");

        foreach (GameObject grid in grids)
        {
            if (!grid.GetComponent<Grid>().ShowShadow && grid.GetComponent<Grid>().InGame && !grid.GetComponent<Grid>().Placed)
            {
                if (CheckPlace((int)grid.transform.position[1] / -2, (int)grid.transform.position[0] / 2) == Place)
                {
                    grid.GetComponent<Grid>().HintShadow(3);
                    break;
                }
            }
        }
    }

    IEnumerator ShowLeakingHint(int seconds)
    {
        game.transform.GetChild(5).gameObject.SetActive(true);

        yield return new WaitForSeconds(seconds);

        game.transform.GetChild(5).gameObject.SetActive(false);
    }

    public void StartLevel (int level)
    {
        Level = level;

        if (level == 1 || level == 7)
        {
            
            menu.gameObject.SetActive(false);

            tutorial.gameObject.SetActive(true);
            tutorialAnimation.gameObject.SetActive(true);
            
            if (level == 1)
            {
                print("sdafgshd");
                tutorialAnimation.AnimationName = "";
                tutorialAnimation.AnimationName = "level1_placement";
                //tutorialAnimation.state.SetAnimation(0, "", true);

                //tutorialAnimation.state.SetAnimation(0, "level1_placement", true);
            }
            else
            {
                tutorialAnimation.AnimationName = "";
                tutorialAnimation.AnimationName = "level8_turnPipe";
                //tutorialAnimation.state.SetAnimation(0, "", true);
                //tutorialAnimation.state.SetAnimation(0, "level8_turnPipe", true);
            }
        }
        else
        {
            PlayLevel();
        }
    }

    public void ContinueGame()
    {
        endMenu.gameObject.SetActive(false);
        State = GameStateBoru.Idle;
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        State = GameStateBoru.MainMenu;
        currentLevel.SetActive(false);
        game.transform.GetChild(4).gameObject.SetActive(false);
        game.transform.GetChild(4).GetComponent<Slider>().value = 0;
        tutorialBool = false;
        game.transform.GetChild(5).gameObject.SetActive(false);
        tutorial.gameObject.SetActive(false);
        game.gameObject.SetActive(false);
        endMenu.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
        allPlaced = false;
        allFilled = false;
        levelCompleted = false;
        currentLevel = null;
        Time.timeScale = 1;
    }

    public void GetMenu()
    {
        endMenu.gameObject.SetActive(true);
        endMenu.transform.GetChild(1).GetComponent<Text>().text = "Oyun Durduruldu.";
        Time.timeScale = 0;
        State = GameStateBoru.Paused;
    }

    public void PlayLevel()
    {
        if ((Level == 1 && GameManager.Instance.State == GameStateBoru.FillWater) || (Level == 5 && GameManager.Instance.State == GameStateBoru.Leaking))
        {
            tutorial.gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            currentLevel.SetActive(true);
            game.gameObject.SetActive(true);
            tutorialBool = true;
            return;
        }

        State = GameStateBoru.Idle;
        leakingSound.Stop();
        fillingSound.Stop();


        int rand;
        Place = 0;
        timer = 0;
        Time.timeScale = 1;

        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        if (tutorial != null)
        {
            tutorial.gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }

        endMenu.gameObject.SetActive(false);

        switch(Level)
        {
            case 1:
                rand = Random.Range(0, level1List.Count);
                while (rand == PlayerPrefs.GetInt("RandomLevelNumber", -1)) { rand = Random.Range(0, level1List.Count); }
                PlayerPrefs.SetInt("RandomLevelNumber", rand);
                grid = level1List[rand].Clone() as int[,];
                places = level1PlaceList[rand].Clone() as int[,];
                NumberOfPipes = level1PipeNumber;
                ShadowValue = 0;
                LeakPipeNumber = level1LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 2:
                grid = level2List[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                places = level2PlaceList[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                NumberOfPipes = level2PipeNumber;
                ShadowValue = 0;
                LeakPipeNumber = level2LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level1Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 3:
                rand = Random.Range(0, level1List.Count);
                while (rand == PlayerPrefs.GetInt("RandomLevelNumber", -1)) { rand = Random.Range(0, level1List.Count); }
                PlayerPrefs.SetInt("RandomLevelNumber", rand);
                grid = level3List[rand].Clone() as int[,];
                places = level3PlaceList[rand].Clone() as int[,];
                NumberOfPipes = level3PipeNumber;
                ShadowValue = 1;
                LeakPipeNumber = level3LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 4:
                grid = level4List[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                places = level4PlaceList[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                NumberOfPipes = level4PipeNumber;
                ShadowValue = 1;
                LeakPipeNumber = level4LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level3Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 5:
                rand = Random.Range(0, level1List.Count);
                while (rand == PlayerPrefs.GetInt("RandomLevelNumber", -1)) { rand = Random.Range(0, level1List.Count); }
                PlayerPrefs.SetInt("RandomLevelNumber", rand);
                grid = level5List[rand].Clone() as int[,];
                places = level5PlaceList[rand].Clone() as int[,];
                NumberOfPipes = level5PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level5LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 6:
                grid = level6List[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                places = level6PlaceList[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                NumberOfPipes = level6PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level6LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level5Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 7:
                rand = Random.Range(0, level1List.Count);
                while (rand == PlayerPrefs.GetInt("RandomLevelNumber", -1)) { rand = Random.Range(0, level1List.Count); }
                PlayerPrefs.SetInt("RandomLevelNumber", rand);
                grid = level7List[rand].Clone() as int[,];
                places = level7PlaceList[rand].Clone() as int[,];
                NumberOfPipes = level7PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level7LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 8:
                grid = level8List[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                places = level8PlaceList[PlayerPrefs.GetInt("RandomLevelNumber")].Clone() as int[,];
                NumberOfPipes = level8PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level8LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level7Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
        }

        pipeList = new List<Pipe>();
        for (int i = 0; i < NumberOfPipes; i++)
        {
            pipeList.Add(null);
        }
    }
    
    public void ReturnMenu()
    {
        game.transform.GetChild(4).gameObject.SetActive(false);
        game.transform.GetChild(4).GetComponent<Slider>().value = 0;
        game.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("Babuş", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}

public enum GameStateBoru
{
    Idle,
    Paused,
    FillWater,
    Leaking,
    MainMenu
};
