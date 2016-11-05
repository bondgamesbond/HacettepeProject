using UnityEngine;
using System.Collections;

public class RestrictionMap : MonoBehaviour
{
    private static int[][] difficultyValues;
    private static float mapTextureHeight = 1600;
    private static float mapTextureWidth = 900;
    private static float pixelRange = 4;

    private static string prefName = "CurrentDifficultyArea";

    void Start()
    {
        difficultyValues = new int[(int)(mapTextureHeight/ pixelRange)][];
    }

    public static int findDifficulty(Vector2 position)
    {
        for (int i = 0; i < mapTextureHeight / pixelRange; i++)
        {
            difficultyValues[i] = PlayerPrefsX.GetIntArray(prefName + i);
        }

        float pxX = 800 + 100 * position.x;
        float pxY = 450 + 100 * position.y;

        return difficultyValues[(int)(pxY / pixelRange)][(int)(pxX / pixelRange)];
    }
}
