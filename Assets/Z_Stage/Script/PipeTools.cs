using UnityEngine;
using System.Collections;

public static class PipeTools {
	public static int tileSize = 126;
	public static string[] cropNameMapper = {"Carrot", "Watermelon", "Pumpkin", "Mushroom"};
    public static int[] NumOfStagePerSection = {7, 8, 16, 15, 16, 19, 18};
	public static int[] goalCropPerSection = {136, 238, 930, 1390, 1900, 3200, 4360};

	public static int ReverseDir(int dir){
		return ((dir + 1) % 2 + (dir < 2 ? 0 : 2));
	}

	public static Vector2 GetPosition(int x, int y){
		return new Vector2 ((-4.5f + x) * tileSize, (-3.5f + y) * tileSize);
	}

	public static Vector2 GetPosition(float x, float y){
		return new Vector2 ((-4.5f + x) * tileSize, (-3.5f + y) * tileSize);
	}

	public static Vector2 BUIGetPosition(float x, float y){
		return GetPosition (x, y) / 540f;
	}

	public static Vector2 NGUIPositionToBUIPosition(int x, int y){
		return new Vector2(x, y) / 540f;
	}

    public static int GetIndex(int total, int count)
    {
        int returnVal = 0;
        for (int i = 0; i < total; i++)
        {
            returnVal += i;
        }
        return (returnVal + count);
    }
}
