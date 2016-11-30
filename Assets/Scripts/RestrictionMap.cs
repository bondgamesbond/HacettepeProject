using UnityEngine;
using System.Collections;

public class RestrictionMap : MonoBehaviour
{
    private static float mapTextureHeight = 1600;
    private static float mapTextureWidth = 900;
    private static float pixelRange = 4;
    private static int[][] difficultyValues = new int[(int)(mapTextureHeight / pixelRange)][];
    static int redCount;

    private static string prefName = "CurrentDifficultyArea";

    public static int findDifficulty(Vector2 position)
    {
        for (int i = 0; i < mapTextureHeight / pixelRange; i++)
        {
            difficultyValues[i] = PlayerPrefsX.GetIntArray(prefName + i);
            if (PlayerPrefs.GetFloat("RedAreaRatio") == 0)
            {
                for (int j = 0; j < mapTextureWidth / pixelRange; j++)
                {
                    if (difficultyValues[i][j] >= 4)
                        redCount++;
                }
            }
        }
        if (PlayerPrefs.GetFloat("RedAreaRatio") == 0)
        {
            Debug.Log(redCount);
            PlayerPrefs.SetFloat("RedAreaRatio", (redCount / ((mapTextureWidth / pixelRange) * (mapTextureHeight / pixelRange))) * 100);
        }
        float pxX = 800 + 100 * position.x;
        float pxY = 450 + 100 * position.y;

        if (pxX >= mapTextureHeight)
        {
            pxX = mapTextureHeight - 100;
        }
        else if (pxX <= 0)
        {
            pxX = 100;
        }

        if (pxY >= mapTextureWidth)
        {
            pxY = mapTextureWidth - 100;
        }
        else if (pxY <= 0)
        {
            pxY = 100;
        }

        return difficultyValues[(int)(pxY / pixelRange)][(int)(pxX / pixelRange)];
    }
}
