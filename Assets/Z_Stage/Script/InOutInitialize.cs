using UnityEngine;
using System.Collections;

public class InOutInitialize : Pipe {
    public UISprite baseTile;
    public UISprite enter;
    public UISprite text;
    public UISprite arrow;
    public UISlider water;

    private int arrowIndex;

	// Use this for initialization
    public void Initialize(int dir, int count, bool isIn)
    {
        baseTile.spriteName = "InOut_" + count.ToString();
        enter.spriteName = "Enter_" + dir.ToString();
        UISprite tempSprite = water.GetComponentInChildren<UISprite>();
        tempSprite.spriteName = "IO_Water_" + dir.ToString();
        
        if (dir < 2)
        {
            tempSprite.fillDirection = UISprite.FillDirection.Horizontal;            
        }
        else
        {
            tempSprite.fillDirection = UISprite.FillDirection.Vertical;
        }

        tempSprite.invert = !((dir % 3 != 0) ^ isIn);

        if (isIn)
        {
            text.spriteName = "Text_In_0";
        }
        else
        {
            text.spriteName = "Text_Out_" + (dir < 2 ? "1" : "0");
        }

        int[] xOffSet = { -1, 1, 0, 0 };
        int[] yOffSet = { 0, 0, 1, -1 };
        int[] rOffSet = { 270, 90, 180, 0};

        text.transform.localPosition = new Vector2(xOffSet[PipeTools.ReverseDir(dir)], yOffSet[PipeTools.ReverseDir(dir)]) * 20;
        arrow.transform.localPosition = new Vector2(xOffSet[dir], yOffSet[dir]) * 35;
        arrow.transform.rotation = Quaternion.Euler(0,0, rOffSet[dir] + (isIn? 0 : 180));

        water.sliderValue = 0;
        enter.MakePixelPerfect();
        text.MakePixelPerfect();
        tempSprite.MakePixelPerfect();

        StartCoroutine(ArrowAnimation());
    }

    IEnumerator ArrowAnimation()
    {
        while (true){
            yield return new WaitForSeconds(0.5f);
            arrowIndex = (arrowIndex + 1) % 3;
            arrow.spriteName  = "Arrow_" + arrowIndex.ToString();
        }
    }

}
