using UnityEngine;
using System.Collections;

public class Warehouse : MonoBehaviour {
    public UILabel currentCrop;
    public UILabel goalCrop;

	void Start () {
        Initialize();
	}

    public void Initialize()
    {
        currentCrop.effectStyle = UILabel.Effect.None;
        int sumCrop = 0;
        foreach (string cropName in PipeTools.cropNameMapper)
        {
            if (PlayerPrefs.HasKey(cropName))
            {
                sumCrop += PlayerPrefs.GetInt(cropName);
            }
        }
        currentCrop.text = sumCrop.ToString();

        int section = 0;
        if (PlayerPrefs.HasKey("Section"))
        {
            section = PlayerPrefs.GetInt("Section");
        }
        goalCrop.text = "/ " + PipeTools.goalCropPerSection[section].ToString();

        if (sumCrop >= PipeTools.goalCropPerSection[section])
        {
            currentCrop.color = goalCrop.color;
            currentCrop.effectStyle = goalCrop.effectStyle;
            currentCrop.effectDistance = goalCrop.effectDistance;
            currentCrop.effectColor = goalCrop.effectColor;
        }
    }
}
