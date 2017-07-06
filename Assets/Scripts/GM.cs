using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {

    public GameObject drums, levels, endMenu;
    public List<AudioSource> sounds;
    public List<GameObject> LevelList; 
    public Text timeText, LevelText;
    public int level, partNo = 0, partNo2 = 0;
    public bool levelStarted = false ,notePlayed = true, firstTap = true;
    public float levelTime = 0f, levelDuration = 120f, remainingTime = 120f;
    public int[] level3Notes = {0, 0, 6, 4, 4, 5, 7};
    public int[] level6notes = {1, 2, 3, 4, 9, 10, 5};
    public int level3count = 0, level6count = 0;
	public float maxResponseTime, minResponseTime;
	public List<AudioSource> veryFastOnes, fastOnes, slowOnes, verySlowOnes;
    



    public List<GameObject> drumParts, glowParts;
	// Use this for initialization
	void Start ()
    {
		//PlayerPrefs.DeleteAll ();
        OpenLevels();

		// Değerlendirme testi kontrolü

		if (PlayerPrefs.GetFloat ("CurrentMaxTime") == 0) {
			PlayerPrefs.SetFloat ("CurrentMaxTime", 5);
		}
		if (PlayerPrefs.GetFloat ("CurrentMinTime") == 0) {
			PlayerPrefs.SetFloat ("CurrentMinTime", 2);
		}

		maxResponseTime = PlayerPrefs.GetFloat ("CurrentMaxTime") * 1.5f;
		minResponseTime = PlayerPrefs.GetFloat ("CurrentMinTime") * 1.5f;

		//end
	}
	
	// Update is called once per frame
	void Update () {

        if (levelStarted )
        {
            timeText.text = "" + (int)remainingTime + "sn";
            levelTime += Time.deltaTime;
            remainingTime = levelDuration - levelTime;
            switch (level)
            {
                case 1:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote1());
                    }
                    break;
                case 2:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote2());
                    }
                    break;
                case 3:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote3());
                    }
                    break;
                case 4:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote4());
                    }
                    break;
                case 5:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote5());
                    }
                    break;
                case 6:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote6());
                    }
                    break;
                case 7:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote7());
                    }
                    break;
                case 8:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote8());
                    }
                    break;
                case 9:
                    if (notePlayed)
                    {
                        StartCoroutine(PlayNote9());
                    }
                    break;




                default:
                    break;
            }
        }
        if(remainingTime <= 0)
        {
            
            FinishLevel2();
            if (level != 9 && PlayerPrefs.GetInt("Level" + (level+1) + "Opened") == 0)
            {
                endMenu.SetActive(true);
            }
            else
            {
                FinishLevel();
            }
            LevelDone(level);
        }
        
    }
    public void OpenLevels()
    {
        for (int i = 1; i < LevelList.Count; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i + "Opened") == 1)
            {
                LevelList[i].SetActive(true);
            }
        }
    }
    public void LevelDone(int flevel)
    {
        if (level != 9)
        {
            PlayerPrefs.SetInt("Level" + (flevel + 1) + "Opened", 1);
            LevelList[flevel + 1].SetActive(true);
        }
    }
    public IEnumerator PlayNote1()
    {
        notePlayed = false;
        if (partNo < 7)
        {
            partNo++;
        }
        else
        {
            partNo = 0;
        }
        
        TurnPartOn(partNo);
		yield return new WaitForSeconds(maxResponseTime * 1.1f);
        TurnPartOff(partNo);
        notePlayed = true;
    }
    public IEnumerator PlayNote2()
    {
        notePlayed = false;
        partNo = Random.Range(0, 8);
        TurnPartOn(partNo);
		yield return new WaitForSeconds(maxResponseTime);
        TurnPartOff(partNo);
        notePlayed = true;
    }

    public IEnumerator PlayNote3()
    {
        notePlayed = false;
        partNo = level3Notes[level3count];
        TurnPartOn(partNo);
		yield return new WaitForSeconds(maxResponseTime * 0.9f);
        TurnPartOff(partNo);
        notePlayed = true;
        if (level3count == 6)
        {
            level3count = 0;
        }
        else
        {
            level3count++;
        }
    }

    public IEnumerator PlayNote4()
    {
        notePlayed = false;
        partNo = Random.Range(0,14);
        glowParts[partNo].SetActive(true);
		yield return new WaitForSeconds(maxResponseTime * 0.8f);
        glowParts[partNo].SetActive(false);
        notePlayed = true;
    }
    public IEnumerator PlayNote5()
    {
        notePlayed = false;
        partNo = level3Notes[level3count];
        TurnPartOn(partNo);
		yield return new WaitForSeconds(minResponseTime * 1.2f);
        TurnPartOff(partNo);
        notePlayed = true;
        if (level3count == 6)
        {
            level3count = 0;
        }
        else
        {
            level3count++;
        }
    }

    public IEnumerator PlayNote6()
    {
        notePlayed = false;
        partNo = level6notes[level6count];
        glowParts[partNo].SetActive(true);
		yield return new WaitForSeconds(minResponseTime * 1.1f);
        glowParts[partNo].SetActive(false);
        notePlayed = true;
        if (level6count == 6)
        {
            level6count = 0;
        }
        else
        {
            level6count++;
        }
    }

    public IEnumerator PlayNote7()
    {
        notePlayed = false;
        partNo = Random.Range(0, 8);
        partNo2 = Random.Range(0,8);
		while(partNo2 == partNo)
		{
			partNo2 = Random.Range(0,8);
		}

        TurnPartOn(partNo);
        TurnPartOn(partNo2);
		yield return new WaitForSeconds(minResponseTime);
        TurnPartOff(partNo);
        TurnPartOff(partNo2);
        notePlayed = true;
    }
    public IEnumerator PlayNote8()
    {
        notePlayed = false;
        partNo = Random.Range(0, 8);
        partNo2 = Random.Range(0, 8);

		while(partNo2 == partNo)
		{
			partNo2 = Random.Range(0,8);
		}
        TurnPartOn(partNo);
        TurnPartOn(partNo2);
		yield return new WaitForSeconds(minResponseTime * 0.9f);
        TurnPartOff(partNo);
        TurnPartOff(partNo2);
        notePlayed = true;
    }
    public IEnumerator PlayNote9()
    {
        notePlayed = false;
        partNo = Random.Range(0, 8);
        partNo2 = Random.Range(0, 8);

		while(partNo2 == partNo)
		{
			partNo2 = Random.Range(0,8);
		}
        TurnPartOn(partNo);
        TurnPartOn(partNo2);
		yield return new WaitForSeconds(minResponseTime * 0.8f);
        TurnPartOff(partNo);
        TurnPartOff(partNo2);
        notePlayed = true;
    }


    public void TurnPartOn(int partNo)
    {
        if (partNo == 0)
        {
            glowParts[0].SetActive(true);
        }
        else if(partNo == 1)
        {
            glowParts[10].SetActive(true);
        }
        else if (partNo == 2)
        {
            glowParts[1].SetActive(true);
            glowParts[2].SetActive(true);
        }
        else if (partNo == 3)
        {
            glowParts[3].SetActive(true);
            glowParts[4].SetActive(true);
        }
        else if (partNo == 4)
        {
            glowParts[11].SetActive(true);
            glowParts[12].SetActive(true); 
            glowParts[13].SetActive(true);
        }
        else if (partNo == 5)
        {
            glowParts[5].SetActive(true);
            glowParts[6].SetActive(true);
        }
        else if (partNo == 6)
        {
            glowParts[8].SetActive(true);
            glowParts[9].SetActive(true);
        }
        else
        {
            glowParts[7].SetActive(true);
        }
    }

    public void TurnPartOff(int partNo)
    {
        if (partNo == 0)
        {
            glowParts[0].SetActive(false);
        }
        else if (partNo == 1)
        {
            glowParts[10].SetActive(false);
        }
        else if (partNo == 2)
        {
            glowParts[1].SetActive(false);
            glowParts[2].SetActive(false);
        }
        else if (partNo == 3)
        {
            glowParts[3].SetActive(false);
            glowParts[4].SetActive(false);
        }
        else if (partNo == 4)
        {
            glowParts[11].SetActive(false);
            glowParts[12].SetActive(false);
            glowParts[13].SetActive(false);
        }
        else if (partNo == 5)
        {
            glowParts[5].SetActive(false);
            glowParts[6].SetActive(false);
        }
        else if (partNo == 6)
        {
            glowParts[8].SetActive(false);
            glowParts[9].SetActive(false);
        }
        else
        {
            glowParts[7].SetActive(false);
        }
    }
    public int secondTap = 0;
    public void UserPlayNote(int playedPart)
    {
        
        int playedPartMod = 0;
        if ((level < 4 && level > 0) || level == 5 )
        {
            if (playedPart == 1)
            {
                playedPartMod = 0;
            }
            else if (playedPart == 2 || playedPart == 3)
            {
                playedPartMod = 2;
            }
            else if (playedPart == 4 || playedPart == 5)
            {
                playedPartMod = 3;
            }
            else if (playedPart == 6 || playedPart == 7)
            {
                playedPartMod = 5;
            }
            else if (playedPart == 8)
            {
                playedPartMod = 7;
            }
            else if (playedPart == 9 || playedPart == 10)
            {
                playedPartMod = 6;
            }
            else if (playedPart == 11)
            {
                playedPartMod = 1;
            }
            else if (playedPart == 12 || playedPart == 13 || playedPart == 14)
            {
                playedPartMod = 4;
            }

            if (partNo == playedPartMod)
            {
                sounds[partNo].Play();
                StopAllCoroutines();
                TurnPartOff(partNo);
                notePlayed = true;
                if (level == 3 || level == 5)
                {
                    if (level3count == 6)
                    {
                        level3count = 0;
                    }
                    else
                    {
                        level3count++;
                    }
                }
            }
        }
        else if (level == 0)
        {
            sounds[playedPart].Play();
        }
        else if(level == 4 || level == 6)
        {
            if (playedPart-1 == partNo)
            {
                sounds[partNo].Play();
                StopAllCoroutines();
                notePlayed = true;
                glowParts[partNo].SetActive(false);
                if (level == 6)
                {
                    if (level6count == 6)
                    {
                        level6count = 0;
                    }
                    else
                    {
                        level6count++;
                    }
                }
            }
        }

        else if (level == 7 || level == 8 || level == 9)
        {
            if (playedPart == 1)
            {
                playedPartMod = 0;
            }
            else if (playedPart == 2 || playedPart == 3)
            {
                playedPartMod = 2;
            }
            else if (playedPart == 4 || playedPart == 5)
            {
                playedPartMod = 3;
            }
            else if (playedPart == 6 || playedPart == 7)
            {
                playedPartMod = 5;
            }
            else if (playedPart == 8)
            {
                playedPartMod = 7;
            }
            else if (playedPart == 9 || playedPart == 10)
            {
                playedPartMod = 6;
            }
            else if (playedPart == 11)
            {
                playedPartMod = 1;
            }
            else if (playedPart == 12 || playedPart == 13 || playedPart == 14)
            {
                playedPartMod = 4;
            }
            if (firstTap)
            {
                if (playedPartMod == partNo)
                {
                    secondTap = partNo2;
                }
                else if (playedPartMod == partNo2)
                {
                    secondTap = partNo;
                }
            }
            else
            {
                if (playedPartMod == secondTap)
                {
                    sounds[partNo].Play();
                    sounds[partNo2].Play();
                    TurnPartOff(partNo);
                    TurnPartOff(partNo2);
                    StopAllCoroutines();
                    notePlayed = true;
                }
            }
        }
        if (firstTap)
        {
            firstTap = false;
        }
        else
        {
            firstTap = true;
        }
    }
    public void returnToMainMenu()
    {
        PlayerPrefs.SetInt("ReturningFromGame", 1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void StartLevel(int levelNumber)
    {
        level6count = 0;
        level3count = 0;
        levels.SetActive(false);
        drums.SetActive(true);
        level = levelNumber;
        levelStarted = true;
        if (level == 0)
        {
            levelDuration = 60f;
            remainingTime = 60f;
        }
        else
        {
            levelDuration = 90f;
            remainingTime = 90f;
        }
		LevelText.text = "Seviye: " + level;
		startRhythms ();

    }

    public void StartNextLevel()
    {
        level6count = 0;
        level3count = 0;
        levels.SetActive(false);
        drums.SetActive(true);
        levelStarted = true;
        level++;

		LevelText.text = "Seviye: " + level;
        if (level == 0)
        {
            levelDuration = 60f;
            remainingTime = 60f;
        }
        else
        {
            levelDuration = 90f;
            remainingTime = 90f;
        }
		startRhythms ();
    }

    public void FinishLevel()
    {
        levelTime = 0f;
        levelDuration = 90f;
        remainingTime = 90f;
        drums.SetActive(false);
        levels.SetActive(true);
        levelStarted = false;
        TurnPartOff(partNo2);
		stopRhythms ();
    }

    public void FinishLevel2()
    {
        levelTime = 0f;
        levelDuration = 90f;
        remainingTime = 90f;
        levelStarted = false;
        TurnPartOff(partNo2);
		stopRhythms ();
    }

	public void stopRhythms(){
		for (int i = 0; i < 4; i++) {
			veryFastOnes [i].Stop ();
		}

		for (int i = 0; i < 4; i++) {
			fastOnes [i].Stop ();
		}

		for (int i = 0; i < 4; i++) {
			slowOnes [i].Stop ();
		}

		for (int i = 0; i < 4; i++) {
			verySlowOnes [i].Stop ();
		}
	}
	public void startRhythms(){
		switch (level) {
		case 1:
			if (maxResponseTime * 1.1f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (maxResponseTime * 1.1f >= 3 && maxResponseTime * 1.1f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (maxResponseTime * 1.1f >= 4 && maxResponseTime * 1.1f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 2:
			if (maxResponseTime  < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (maxResponseTime  >= 3 && maxResponseTime < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (maxResponseTime >= 4 && maxResponseTime < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 3:
			if (maxResponseTime * 0.9f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (maxResponseTime * 0.9f >= 3 && maxResponseTime * 0.9f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (maxResponseTime * 0.9f >= 4 && maxResponseTime * 0.9f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 4:
			if (maxResponseTime * 0.8f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (maxResponseTime * 0.8f >= 3 && maxResponseTime * 0.8f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (maxResponseTime * 0.8f >= 4 && maxResponseTime * 0.8f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 5:
			if (minResponseTime * 1.2f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			} else if (minResponseTime * 1.2f >= 3 && minResponseTime * 1.2f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			} else if (minResponseTime * 1.2f >= 4 && minResponseTime * 1.2f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}
			break;
		case 6:
			if (minResponseTime * 1.1f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (minResponseTime * 1.1f >= 3 && minResponseTime * 1.1f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (minResponseTime * 1.1f >= 4 && minResponseTime * 1.1f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 7:
			if (minResponseTime  < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (minResponseTime  >= 3 && minResponseTime  < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (minResponseTime  >= 4 && minResponseTime  < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 8:
			if (minResponseTime * 0.9f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (minResponseTime * 0.9f >= 3 && minResponseTime * 0.9f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (minResponseTime * 0.9f >= 4 && minResponseTime * 0.9f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		case 9:
			if (minResponseTime * 0.8f < 3) {
				veryFastOnes [Random.Range (0, veryFastOnes.Count)].Play ();
			}
			else if (minResponseTime * 0.8f >= 3 && minResponseTime * 0.8f < 4) {

				fastOnes [Random.Range (0, fastOnes.Count)].Play ();
			}
			else if (minResponseTime * 0.8f >= 4 && minResponseTime * 0.8f < 5) {

				slowOnes [Random.Range (0, slowOnes.Count)].Play ();
			} 
			else {
				verySlowOnes [Random.Range (0, verySlowOnes.Count)].Play ();
			}

			break;
		default:
			break;
		}
	}

}
