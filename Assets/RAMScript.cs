using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using KModkit;

public class RAMScript : MonoBehaviour {

	public KMSelectable clear;
	public KMBomb Bomb;
	public TextMesh displayCurrentDigit, displayCurrentUnit, displayLimitDigit, displayLimitUnit;
	int increasedPercentage;
	int increasedDigit;
	int currentDigit;
	int unit;
	string currentUnit;
	int limitDigit;
	bool solvedState = false;
	bool readytoSolve = false;

    private static int moduleIdCounter = 1;
    private int moduleId = 0;
	private string[] unitList = new string [5] {"B", "KB", "MB", "GB", "TB"};
	// Use this for initialization
	void Awake () {
		moduleId = moduleIdCounter++;
 		clear.OnInteract += clearRAM;

		unit = Random.Range(0, 4);
		currentUnit = unitList[unit];
		limitDigit = Random.Range(100, 1000);
		displayCurrentUnit.text = currentUnit.ToString();
		displayLimitUnit.text = currentUnit.ToString();
		displayLimitDigit.text = limitDigit.ToString();
		currentDigit = limitDigit / 3;
		displayCurrentDigit.text = currentDigit.ToString();
	}
	private bool clearRAM ()
		{
			if (solvedState == false)
			{	
			Debug.LogFormat("[Random Access Memory #{0}]: Clear button has been pressed!", moduleId);
			currentDigit = limitDigit * 4 / 10;
			displayCurrentDigit.text = currentDigit.ToString();
			}
       		return false;
		}

	IEnumerator RAMUsage () {
		yield return new WaitForSeconds(10f);
		increasedPercentage = Random.Range (5, 12);
		increasedDigit = (increasedPercentage * limitDigit / 100); 
		displayCurrentDigit.text = currentDigit.ToString();
	}

	
}
