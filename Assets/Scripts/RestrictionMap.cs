using UnityEngine;
using System.Collections;

public class RestrictionMap : MonoBehaviour
{
    private static float mapTextureHeight = 1600;
    private static float mapTextureWidth = 900;
    private static float pixelRange = 4;
    private static int[][] difficultyValues = new int[(int)(mapTextureHeight / pixelRange)][];
    static int redCount, yellowCount, greenCount, blueCount, cyanCount;
    public static string mostAvailableArea;
    public static float redRatio, yellowRatio, greenRatio, blueRatio, cyanRatio;
    static float[] ratioList = new float[5];
    static float tempRatio;

    private static string prefName = "CurrentDifficultyArea";

    public static int findDifficulty(Vector2 position)
    {
        float pxX = 800 + 100 * position.x;
        float pxY = 450 + 100 * position.y;

		for (int i = 0; i < mapTextureHeight / pixelRange; i++)
		{
			difficultyValues[i] = PlayerPrefsX.GetIntArray(prefName + i);
		}

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

    public static void getDifficultyRatios()
    {
        for (int i = 0; i < mapTextureHeight / pixelRange; i++)
        {
            difficultyValues[i] = PlayerPrefsX.GetIntArray(prefName + i);
            for (int j = 0; j < mapTextureWidth / pixelRange; j++)
            {
                if (difficultyValues[i][j] == 0)
                    yellowCount++;
                else if (difficultyValues[i][j] == 1)
                    greenCount++;
                else if (difficultyValues[i][j] == 2)
                    cyanCount++;
                else if (difficultyValues[i][j] == 3)
                    blueCount++;
                else if (difficultyValues[i][j] >= 4)
                    redCount++;
            }
        }
        redRatio= redCount / ((mapTextureWidth / pixelRange) * (mapTextureHeight / pixelRange)) *100;
        blueRatio = blueCount / ((mapTextureWidth / pixelRange) * (mapTextureHeight / pixelRange)) * 100;
        cyanRatio = cyanCount / ((mapTextureWidth / pixelRange) * (mapTextureHeight / pixelRange)) * 100;
        greenRatio = greenCount / ((mapTextureWidth / pixelRange) * (mapTextureHeight / pixelRange)) * 100;
        yellowRatio = yellowCount / ((mapTextureWidth / pixelRange) * (mapTextureHeight / pixelRange)) * 100;
        ratioList[0] = redRatio;
        ratioList[1] = blueRatio;
        ratioList[2] = cyanRatio;
        ratioList[3] = greenRatio;
        ratioList[4] = yellowRatio;
        tempRatio = redRatio;
        for (int i = 0; i < ratioList.Length; i++)
        {
            if (ratioList[i] >= tempRatio)
                tempRatio = ratioList[i];
        }
        if (tempRatio == redRatio)
            mostAvailableArea = "Red";
        else if (tempRatio == blueRatio)
            mostAvailableArea = "Blue";
        else if (tempRatio == cyanRatio)
            mostAvailableArea = "Cyan";
        else if (tempRatio == greenRatio)
            mostAvailableArea = "Green";
        else if (tempRatio == yellowRatio)
            mostAvailableArea = "Yellow";

        redCount = 0;
        blueCount = 0;
        cyanCount = 0;
        greenCount = 0;
        yellowCount = 0;
    }
}
