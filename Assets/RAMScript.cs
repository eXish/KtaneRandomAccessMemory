using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using KModkit;

public class RAMScript : MonoBehaviour {
	public string[] ignoredModules;
	public KMSelectable clear;
	private int solvables;
	public KMBombInfo Bomb;
	public TextMesh displayCurrentDigit, displayCurrentUnit, displayLimitDigit, displayLimitUnit, progressbar, progressbarlimit;
	int increasedDuration;
	int increasedPercentage;
	int increasedDigit;
	int currentDigit;
	int unit;
	string currentUnit;
	int limitDigit;
	bool solvedState = false;
	bool inputMode = false;
	string bars1;
	string bars2;
	int numofbars1;
	int numofbars2;
	int resetPercentage;
	int percentageToSolve;
    private static int moduleIdCounter = 1;
    private int moduleId = 0;
	private string[] unitList = new string [5] {"B", "KB", "MB", "GB", "TB"};

	// Use this for initialization
	void Awake () {
		moduleId = moduleIdCounter++;
		//Clear button interact
 		clear.OnInteract += clearRAM;
		//Picking units
		unit = Random.Range(0, 4);
		currentUnit = unitList[unit];
		//Randomizing RAM Limit
		limitDigit = Random.Range(100, 1000);
		//Initial RAM = 30%
		currentDigit = limitDigit / 4;
		//Display all of them on the module
		displayCurrentUnit.text = currentUnit.ToString();
		displayLimitUnit.text = currentUnit.ToString();
		displayLimitDigit.text = limitDigit.ToString();
		displayCurrentDigit.text = currentDigit.ToString();
		UpdateProgressBar();
		percentageToSolve = Random.Range (40, 60);
	}
	void Start () {
		//Activates the module
		StartCoroutine(RAMUsage());
	}
	void UpdateProgressBar()
		{
		//70 Bars
		numofbars1 = currentDigit * 70 / limitDigit;
		numofbars2 = (70 - numofbars1);
		Debug.LogFormat("[Random Access Memory #{0}]: Numbers of bars = {1},{2}!", moduleId, numofbars1, numofbars2);
		for (int i = 1; i <= numofbars1; i++)
		{
			bars1 += "|";
		}
		for (int i = 1; i <= numofbars2; i++)
		{
			bars2 += "|";
		}
		progressbar.text = bars1;
		progressbarlimit.text = bars2;
		bars1 = "";
		bars2 = "";
		}
	private bool clearRAM ()
		{
			if (solvedState == false && inputMode == true)
			{	
			Debug.LogFormat("[Random Access Memory #{0}]: Clear button has been pressed!", moduleId);
			delayForResponse();
			resetPercentage = Random.Range (20, 30);
			currentDigit = resetPercentage * limitDigit / 100;
			displayCurrentDigit.text = currentDigit.ToString();
			UpdateProgressBar();
			}
       		return false;
		}
	IEnumerator delayForResponse () {
		yield return new WaitForSeconds(1f);
	}
	IEnumerator RAMUsage () {
		ignoredModules = GetComponent<KMBossModule>().GetIgnoredModules("Random Access Memory", new string[]{
				"Random Access Memory",
				"14"
            });		
		//For the module to not autosolve, temporarily add 1 in next line below:
		solvables = Bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Count();
		if (solvables == 0) {
			yield return new WaitForSeconds(3f);
			GetComponent<KMBombModule>().HandlePass();
			Debug.LogFormat("[Random Access Memory #{0}]: There are no non-ignored modules, forcing module to be solved.", moduleId);
			StopCoroutine(RAMUsage());
			solvedState = true;
			inputMode = false;
			}
		else while (solvedState == false)
		{
		inputMode = true;
		increasedDuration = Random.Range (5, 10);
		for ( int i = 1; i < increasedDuration; i++ ) {
			yield return new WaitForSeconds(1f);
		}
		increasedPercentage = Random.Range (5, 12);
		increasedDigit = (increasedPercentage * limitDigit / 100);
		currentDigit = currentDigit + increasedDigit; 
		displayCurrentDigit.text = currentDigit.ToString();
		UpdateProgressBar();
		Debug.LogFormat("[Random Access Memory #{0}]: Increased Percentage = {2}%, Increased Amount = {3} Current RAM Amount is {1}", moduleId, currentDigit, increasedPercentage, increasedDigit);			
			if (currentDigit > limitDigit)
			{
				inputMode = false;
				//*Add bomb freeze here
				StopCoroutine(RAMUsage());
				yield return new WaitForSeconds(3f);
				GetComponent<KMBombModule>().HandleStrike(); 
				Debug.LogFormat("[Random Access Memory #{0}]: RAM Exceeded Maximum, module striked.", moduleId, currentDigit, increasedPercentage, increasedDigit);
				currentDigit = limitDigit * 4 / 10;
				displayCurrentDigit.text = currentDigit.ToString();
				UpdateProgressBar();
				StartCoroutine(RAMUsage());
				inputMode = true;
			}
			if ( Bomb.GetSolvedModuleNames().Count() * 100 / solvables >= percentageToSolve)
			{
				GetComponent<KMBombModule>().HandlePass();
				Debug.LogFormat("[Random Access Memory #{0}]: Battery depleted, module solved!", moduleId);
				solvedState = true;
				inputMode = false;
			}
		}
	}

	#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} clear [Clears the memory]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
			if(command.Equals("clear"))       
		{
			yield return null;
			clear.OnInteract();
			yield break;
        }
    }
 }
