using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Pipe leakingPipe;
    public bool valveCanTurn;
    public RectTransform valveOpenSlider;

    public AudioSource leakingSound, explosionSound, fillingSound, countdown, backgroundMusic;

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

    private List<int[,]> levelList, levelPlaceList, playerSpecificLevelList;

    

    // Level 1
    private int[,] level1 =        {{ 1,  1,  1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6, -1, -1,  2,  1,  1,  1},
                                    {-1, -1, -1,  6, -1, -1,  0, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  1,  4, -1, -1, -1}};

    private int[,] level1Place =   {{ 0,  1,  2,  3, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  4, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  5, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, -1, 11, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, 10, -1, -1, -1}};

    private int[,] level2 =        {{ 1,  5, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  6, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  5, -1,  2,  1,  1,  1,  1},
                                    {-1, -1, -1,  6, -1,  0, -1, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1}};

    private int[,] level2Place =   {{ 0,  1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, 11, 12, 13, 14, 15},
                                    {-1, -1, -1,  6, -1, 10, -1, -1, -1, -1},
                                    {-1, -1, -1,  7,  8,  9, -1, -1, -1, -1}};

    private int[,] level3 =        {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    { 1,  1,  5,  2,  5,  2,  5, -1, -1, -1},
                                    {-1, -1,  3,  4,  3,  4,  6, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level3Place =   {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    { 0,  1,  2,  5,  6,  9, 10, -1, -1, -1},
                                    {-1, -1,  3,  4,  7,  8, 11, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level4 =        {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    { 5, -1, -1, -1, -1,  2,  5, -1, -1, -1},
                                    { 3,  1,  1,  5, -1,  0,  6, -1, -1, -1},
                                    {-1, -1, -1,  3,  1,  4,  3,  1,  1,  1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level4Place =   {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    { 0, -1, -1, -1, -1,  9, 10, -1, -1, -1},
                                    { 1,  2,  3,  4, -1,  8, 11, -1, -1, -1},
                                    {-1, -1, -1,  5,  6,  7, 12, 13, 14, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level5 =        {{-1, -1, -1, -1, -1, -1, -1,  2,  1,  1},
                                    {-1,  2,  1,  5, -1, -1, -1,  0, -1, -1},
                                    { 1,  4, -1,  6, -1,  2,  1,  4, -1, -1},
                                    {-1, -1, -1,  3,  1,  4, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level5Place =   {{-1, -1, -1, -1, -1, -1, -1, 13, 14, 15},
                                    {-1,  2,  3,  4, -1, -1, -1, 12, -1, -1},
                                    { 0,  1, -1,  5, -1,  9, 10, 11, -1, -1},
                                    {-1, -1, -1,  6,  7,  8, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level6 =        {{-1, -1, -1, -1, -1, -1, -1, -1, -1,  2},
                                    {-1, -1, -1, -1, -1, -1, -1,  2,  1,  4},
                                    { 5, -1, -1,  2,  1,  1,  1,  4, -1, -1},
                                    { 6, -1, -1,  0, -1, -1, -1, -1, -1, -1},
                                    { 3,  1,  1,  4, -1, -1, -1, -1, -1, -1}};

    private int[,] level6Place =   {{-1, -1, -1, -1, -1, -1, -1, -1, -1, 15},
                                    {-1, -1, -1, -1, -1, -1, -1, 12, 13, 14},
                                    { 0, -1, -1,  7,  8,  9, 10, 11, -1, -1},
                                    { 1, -1, -1,  6, -1, -1, -1, -1, -1, -1},
                                    { 2,  3,  4,  5, -1, -1, -1, -1, -1, -1}};

    private int[,] level7 =        {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1,  2,  5,  2,  1,  1},
                                    {-1, -1, -1,  2,  1,  4,  3,  4, -1, -1},
                                    { 1,  5, -1,  0, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  1,  4, -1, -1, -1, -1, -1, -1}};

    private int[,] level7Place =   {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1, -1, -1,  9, 10, 13, 14, 15},
                                    {-1, -1, -1,  6,  7,  8, 11, 12, -1, -1},
                                    { 0,  1, -1,  5, -1, -1, -1, -1, -1, -1},
                                    {-1,  2,  3,  4, -1, -1, -1, -1, -1, -1}};

    private int[,] level8 =        {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  2,  1,  5, -1, -1, -1, -1, -1,  2},
                                    {-1,  0, -1,  3,  5, -1,  2,  1,  1,  4},
                                    { 1,  4, -1, -1,  3,  1,  4, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level8Place =   {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1,  3,  4,  5, -1, -1, -1, -1, -1, 15},
                                    {-1,  2, -1,  6,  7, -1, 11, 12, 13, 14},
                                    { 0,  1, -1, -1,  8,  9, 10, -1, -1, -1},
                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    private int[,] level9 =        {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  2,  1,  1,  5, -1, -1, -1},
                                    {-1, -1,  2,  4, -1, -1,  3,  5, -1, -1},
                                    {-1,  2,  4, -1, -1, -1, -1,  3,  5, -1},
                                    { 1,  4, -1, -1, -1, -1, -1, -1,  3,  1}};

    private int[,] level9Place =   {{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                                    {-1, -1, -1,  6,  7,  8,  9, -1, -1, -1},
                                    {-1, -1,  4,  5, -1, -1, 10, 11, -1, -1},
                                    {-1,  2,  3, -1, -1, -1, -1, 12, 13, -1},
                                    { 0,  1, -1, -1, -1, -1, -1, -1, 14, 15}};

    private int level1PipeNumber = 16;

    private int level1LeakPipe = 0;

    // Level 2
    private int level2PipeNumber = 16;

    private int level2LeakPipe = 0;

    // Level 3

    private int level3PipeNumber = 16;

    private int level3LeakPipe = 0;

    // Level 4
    private int level4PipeNumber = 16;

    private int level4LeakPipe = 0;

    // Level 5
    private int level5PipeNumber = 16;

    private int level5LeakPipe = 1;

    // Level 6
    private int level6PipeNumber = 16;

    private int level6LeakPipe = 1;

    // Level 7
    private int level7PipeNumber = 16;

    private int level7LeakPipe = 2;

    // Level 8
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

	private int[] restrictionCounts;

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
        // Levels
        levelList = new List<int[,]>();
        levelList.Add(level1);
        levelList.Add(level2);
        levelList.Add(level3);
        levelList.Add(level4);
        levelList.Add(level5);
        levelList.Add(level6);
        levelList.Add(level7);
        levelList.Add(level8);
        levelList.Add(level9);

        levelPlaceList = new List<int[,]>();
        levelPlaceList.Add(level1Place);
        levelPlaceList.Add(level2Place);
        levelPlaceList.Add(level3Place);
        levelPlaceList.Add(level4Place);
        levelPlaceList.Add(level5Place);
        levelPlaceList.Add(level6Place);
        levelPlaceList.Add(level7Place);
        levelPlaceList.Add(level8Place);
        levelPlaceList.Add(level9Place);

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

		//restrictionCounts = new int[levelList.Count];

		//for (int i = 0; i < restrictionCounts.Length; i++)
		//{
		//	restrictionCounts[i] = getRestrictionStatusCount(levelList[i]);
		//}
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
			countdown.Stop();
            endMenu.transform.GetChild(1).GetComponent<Text>().text = "Süreniz Doldu.";
            Time.timeScale = 0;
            State = GameStateBoru.Paused;
        }

        if (currentLevel != null) // Playing a level.
        {
            if (LevelTime < 3.5f && !countdown.isPlaying && !allPlaced)
            {
				backgroundMusic.volume = 0.3f;
                countdown.Play();
            }

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
				if (countdown.isPlaying)
				{
					countdown.Stop();
				}

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
						backgroundMusic.volume = 0.3f;
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

					backgroundMusic.volume = 0.5f;
					timeText.text = "";

					State = GameStateBoru.MainMenu;
                }
            }
            else
            {
                // Update time and check all placed.
                if (Level % 2 == 0)
                {
					timeText.text = "Kalan Süre: " + Mathf.Ceil(LevelTime);
					timeText.text += " (Süre %10 azaltıldı.)";
                }
				if (!Grid.countdowning)
				{
					LevelTime -= Time.deltaTime;
					timer += Time.deltaTime;
				}
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
                else if (CheckPlace((int)grid.transform.position[1] / -2, (int)grid.transform.position[0] / 2) == Place + 1)
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
		backgroundMusic.volume = 0.5f;
		timeText.text = "";
		State = GameStateBoru.MainMenu;
		if (countdown.isPlaying)
		{
			countdown.Stop();
		}
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

		Grid.count = 1;

		//restrictionCounts = new int[levelList.Count];

		//for (int i = 0; i < restrictionCounts.Length; i++)
		//{
		//	restrictionCounts[i] = getRestrictionStatusCount(levelList[i]);
		//}

		//int min = restrictionCounts[0];

		//playerSpecificLevelList = new List<int[,]>();

		//for (int i = 1; i < restrictionCounts.Length; i++)
		//{
		//	if (restrictionCounts[i] < min)
		//	{
		//		min = restrictionCounts[i];
		//	}
		//}

		//for (int i = 0; i < restrictionCounts.Length; i++)
		//{
		//	if (restrictionCounts[i] == min)
		//	{
		//		playerSpecificLevelList.Add(levelList[i]);
		//	}
		//}

		switch (Level)
        {
            case 1:
                rand = Random.Range(0, levelList.Count);
                while (rand == PlayerPrefs.GetInt("Level1Pipes", -1)) { rand = Random.Range(0, levelList.Count); }
                PlayerPrefs.SetInt("Level1Pipes", rand);
                grid = levelList[rand].Clone() as int[,];
                places = levelPlaceList[rand].Clone() as int[,];
                NumberOfPipes = level1PipeNumber;
                ShadowValue = 0;
                LeakPipeNumber = level1LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 2:
                rand = PlayerPrefs.GetInt("Level1Pipes");
                grid = levelList[PlayerPrefs.GetInt("Level1Pipes")].Clone() as int[,];
                places = levelPlaceList[PlayerPrefs.GetInt("Level1Pipes")].Clone() as int[,];
                NumberOfPipes = level2PipeNumber;
                ShadowValue = 0;
                LeakPipeNumber = level2LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level1Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 3:
                rand = Random.Range(0, levelList.Count);
                while (rand == PlayerPrefs.GetInt("Level3Pipes", -1)) { rand = Random.Range(0, levelList.Count); }
                PlayerPrefs.SetInt("Level3Pipes", rand);
                grid = levelList[rand].Clone() as int[,];
                places = levelPlaceList[rand].Clone() as int[,];
                NumberOfPipes = level3PipeNumber;
                ShadowValue = 1;
                LeakPipeNumber = level3LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 4:
                rand = PlayerPrefs.GetInt("Level3Pipes");
                grid = levelList[PlayerPrefs.GetInt("Level3Pipes")].Clone() as int[,];
                places = levelPlaceList[PlayerPrefs.GetInt("Level3Pipes")].Clone() as int[,];
                NumberOfPipes = level4PipeNumber;
                ShadowValue = 1;
                LeakPipeNumber = level4LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level3Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 5:
                rand = Random.Range(0, levelList.Count);
                while (rand == PlayerPrefs.GetInt("Level5Pipes", -1)) { rand = Random.Range(0, levelList.Count); }
                PlayerPrefs.SetInt("Level5Pipes", rand);
                grid = levelList[rand].Clone() as int[,];
                places = levelPlaceList[rand].Clone() as int[,];
                NumberOfPipes = level5PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level5LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 6:
                rand = PlayerPrefs.GetInt("Level5Pipes");
                grid = levelList[PlayerPrefs.GetInt("Level5Pipes")].Clone() as int[,];
                places = levelPlaceList[PlayerPrefs.GetInt("Level5Pipes")].Clone() as int[,];
                NumberOfPipes = level6PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level6LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level5Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 7:
                rand = Random.Range(0, levelList.Count);
                while (rand == PlayerPrefs.GetInt("Level7Pipes", -1)) { rand = Random.Range(0, levelList.Count); }
                PlayerPrefs.SetInt("Level7Pipes", rand);
                grid = levelList[rand].Clone() as int[,];
                places = levelPlaceList[rand].Clone() as int[,];
                NumberOfPipes = level7PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level7LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = 400.0f;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
                valve = currentLevel.transform.GetChild(0).gameObject;
                board = currentLevel.transform.GetChild(7);
                valve.SetActive(false);
                currentLevel.SetActive(true);
                menu.gameObject.SetActive(false);
                game.gameObject.SetActive(true);
                break;
            case 8:
                rand = PlayerPrefs.GetInt("Level7Pipes");
                grid = levelList[PlayerPrefs.GetInt("Level7Pipes")].Clone() as int[,];
                places = levelPlaceList[PlayerPrefs.GetInt("Level7Pipes")].Clone() as int[,];
                NumberOfPipes = level8PipeNumber;
                ShadowValue = 2;
                LeakPipeNumber = level8LeakPipe;
                allPlaced = false;
                allFilled = false;
                levelCompleted = false;
                LevelTime = PlayerPrefs.GetFloat("Level7Time") * 9 / 10;
                currentLevel = Instantiate(levels.GetChild(Level - 1).gameObject, Vector3.zero, Quaternion.identity) as GameObject;
                ChangeBoard(rand);
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

	private int getRestrictionStatusCount(int [,] level)
	{
		int count = 0;
		
		for (int i = 0; i < level.GetLength(1); i++)
		{
			for (int j = 0; j < level.GetLength(0); j++)
			{
				if (level[j, i] != -1)
				{
					Vector2 pipePosition = new Vector2(i * 2 - 9f, j * -2 + 4);

					if (RestrictionMap.findDifficulty(pipePosition) >= 4)
					{
						count++;
					}
				}
			}
		}

		return count;
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

    private void ChangeBoard(int levelIndex)
    {
        if (levelIndex < 2)
        {new Vector2(-770.0f, 440.0f);
			valveOpenSlider.anchoredPosition = new Vector2(-770.0f, 440.0f);
            currentLevel.transform.GetChild(0).position = new Vector2(-2.5f, 0.15f);
            currentLevel.transform.GetChild(7).GetChild(5).position = new Vector2(21f, -4f);
            currentLevel.transform.GetChild(7).GetChild(6).position = new Vector2(-2.5f, 0f);
        }
        else if (levelIndex < 4)
        {
            valveOpenSlider.anchoredPosition = new Vector2(-770.0f, 300.0f);
            currentLevel.transform.GetChild(0).position = new Vector2(-2.5f, -1.85f);
            currentLevel.transform.GetChild(7).GetChild(5).position = new Vector2(21f, -6f);
            currentLevel.transform.GetChild(7).GetChild(6).position = new Vector2(-2.5f, -2f);
        }
        else if (levelIndex < 6)
        {
            valveOpenSlider.anchoredPosition = new Vector2(-770.0f, 160.0f);
            currentLevel.transform.GetChild(0).position = new Vector2(-2.5f, -3.85f);
            currentLevel.transform.GetChild(7).GetChild(5).position = new Vector2(21f, 0f);
            currentLevel.transform.GetChild(7).GetChild(6).position = new Vector2(-2.5f, -4f);
        }
        else if (levelIndex < 8)
        {
            valveOpenSlider.anchoredPosition = new Vector2(-770.0f, 20.0f);
            currentLevel.transform.GetChild(0).position = new Vector2(-2.5f, -5.85f);
            currentLevel.transform.GetChild(7).GetChild(5).position = new Vector2(21f, -2f);
            currentLevel.transform.GetChild(7).GetChild(6).position = new Vector2(-2.5f, -6f);
        }
        else if (levelIndex < 10)
        {
            valveOpenSlider.anchoredPosition = new Vector2(-770.0f, -120.0f);
            currentLevel.transform.GetChild(0).position = new Vector2(-2.5f, -7.85f);
            currentLevel.transform.GetChild(7).GetChild(5).position = new Vector2(21f, -8f);
            currentLevel.transform.GetChild(7).GetChild(6).position = new Vector2(-2.5f, -8f);
        }
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
