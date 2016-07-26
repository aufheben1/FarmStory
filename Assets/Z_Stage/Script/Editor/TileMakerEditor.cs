using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(TileMaker))]
public class TileMakerEditor : Editor{
	TileMaker[] tiles;

	void OnEnable(){
		tiles = Array.ConvertAll (targets, _t => _t as TileMaker);
		
		foreach (TileMaker tile in tiles) {
			tile.sprite = tile.GetComponent<UISprite>();
		}
	}


	public override void OnInspectorGUI(){
		tiles = Array.ConvertAll (targets, _t => _t as TileMaker);

		EditorGUILayout.LabelField("Custom Inspector for TileMaker"); 

		EditorGUILayout.BeginHorizontal ();

		EditorGUILayout.LabelField ("TileType");
		string[] typeName = {"empty", "none", "ob","obs pipe", "start", "end", "in", "out" };
		int[] typeValue = {0, 1, 2, 3, 4, 5, 6, 7};
		tiles[0].tileType = EditorGUILayout.IntPopup(tiles[0].tileType, typeName,typeValue);
			
		EditorGUILayout.EndHorizontal ();

		if (tiles [0].tileType == 0) {
			EditorGUILayout.LabelField ("ArrowInOut");
			if (GUILayout.Button ("UpOut")){
				tiles[0].upOut = !tiles[0].upOut;
				foreach (TileMaker tile in tiles){
					tile.upOut = tiles[0].upOut;
				}
			}

			if (GUILayout.Button ("DownOut")){
				tiles[0].downOut = !tiles[0].downOut;
				foreach (TileMaker tile in tiles){
					tile.downOut = tiles[0].downOut;
				}
			}

			if (GUILayout.Button ("LeftOut")){
				tiles[0].leftOut = !tiles[0].leftOut;
				foreach (TileMaker tile in tiles){
					tile.leftOut = tiles[0].leftOut;
				}
			}

			if (GUILayout.Button ("RigthOut")){
				tiles[0].rightOut = !tiles[0].rightOut;
				foreach (TileMaker tile in tiles){
					tile.rightOut = tiles[0].rightOut;
				}
			}
		}

		if (tiles [0].tileType == 3) {
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("PipeType");
			string[] numName = {"─", "│", "┼", "┐", "┘", "┌", "└" };
			int[] numVal = {0, 1, 2, 3, 4, 5, 6};
			tiles [0].pipeNum = EditorGUILayout.IntPopup (tiles [0].pipeNum, numName, typeValue);

			EditorGUILayout.EndHorizontal ();
		}

		if (tiles [0].tileType >= 4) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Direction");
			string[] dirName = {"\u25C4", "\u25BA", "\u25B2", "\u25BC"};
			int[] dirVal = {0, 1, 2, 3};
			tiles[0].dir = EditorGUILayout.IntPopup (tiles [0].dir, dirName, dirVal);
			EditorGUILayout.EndHorizontal ();
		}

		if (tiles [0].tileType == 6) {
			EditorGUILayout.BeginHorizontal();
			tiles[0].outVec = EditorGUILayout.Vector2Field ("Out Position", tiles[0].outVec);
			EditorGUILayout.EndHorizontal();
		}


		if (tiles [0].tileType == 0 || tiles [0].tileType == 2 || tiles [0].tileType == 3) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("NormalCropName");
			string[] cropName = {"Carrot", "Watermelon", "Pumpkin", "Mushroom"};
			int[] cropVal = {0, 1, 2, 3};
			tiles[0].cropType = EditorGUILayout.IntPopup (tiles[0].cropType, cropName, cropVal);
			EditorGUILayout.EndHorizontal();

			//EditorGUILayout.LabelField ("CropRange");
			EditorGUILayout.BeginHorizontal ();
			tiles[0].cropRange = EditorGUILayout.Vector2Field ("CropRange", tiles[0].cropRange);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("SpecialCropName");
			string[] cropName2 = {"None", "crop2", "crop3", "crop4", "crop5"};
			int[] cropVal2 = {0, 1, 2, 3, 4};
			tiles[0].spCropType = EditorGUILayout.IntPopup (tiles[0].spCropType, cropName2, cropVal2);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal ();
			tiles[0].spCropRange = EditorGUILayout.Vector2Field ("SpecialCropRange", tiles[0].spCropRange);
			EditorGUILayout.EndHorizontal();
		}

		if (GUI.changed) {
			int[] degree = {-1, 1, 2, 0};

			foreach (TileMaker tile in tiles) {
				tile.tileType = tiles[0].tileType;
				tile.upOut = tiles[0].upOut;
				tile.downOut = tiles[0].downOut;
				tile.leftOut = tiles[0].leftOut;
				tile.rightOut = tiles[0].rightOut;
				tile.dir = tiles[0].dir;
				tile.pipeNum = tiles[0].pipeNum;
				tile.cropType = tiles[0].cropType;
				tile.cropRange = tiles[0].cropRange;
				tile.spCropType = tiles[0].spCropType;
				tile.spCropRange = tiles[0].spCropRange;
			}
			foreach(TileMaker tile in tiles){
				tile.transform.rotation = Quaternion.Euler (0,0,0);
				switch (tile.tileType) {
//string[] typeName = {"empty", "none", "ob","obs pipe", "start", "end", "in", "out" };
				case 0 :
					tile.sprite.spriteName = "Empty";
					break;
				case 1 :
					tile.sprite.spriteName = "None";
					break;
				case 2 :
					tile.sprite.spriteName = "Obstacle";
					break;
				case 3 :
					tile.sprite.spriteName = "op_" + tile.pipeNum.ToString();
					break;
				case 4 :
					tile.sprite.spriteName = "Start";
					tile.transform.rotation = Quaternion.Euler (0,0, 90 * degree[tile.dir]);
					break;
				case 5 :
					tile.sprite.spriteName = "End";
					tile.transform.rotation = Quaternion.Euler (0,0, 90 * degree[tile.dir]);
					break;
				case 6 :
					tile.sprite.spriteName = "In";
					tile.transform.rotation = Quaternion.Euler (0,0, 90 * degree[tile.dir]);
					break;
				case 7 :
					tile.sprite.spriteName = "Out";
					tile.transform.rotation = Quaternion.Euler (0,0, 90 * degree[tile.dir]);
					break;

				}
				EditorUtility.SetDirty(tile.gameObject);
			}
		}
	}

}
