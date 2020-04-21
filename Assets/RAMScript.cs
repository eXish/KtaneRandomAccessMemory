using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using KModkit;

public class RAMScript : MonoBehaviour {
	public string[] ignoredModules;
	public Material[] colours;
	public Renderer[] buttons;
	public TextMesh[] texts;
	public KMSelectable clear;
	private int nonIgnored;
	public KMBombInfo Bomb;
	public TextMesh displayCurrentDigit, displayCurrentUnit, displayLimitDigit, displayLimitUnit, progressbar, progressbarlimit;
	int increasedDuration;
	int increasedPercentage;
	int currentDigit, limitDigit;
	int unit;
	string currentUnit;
	int appclearedcount;
	string appclearedtext;
	bool solvedState = false, inputMode = false, safeMode = false;
	int numofbars1, numofbars2;
	string bars1, bars2;
	public Color normal, warning, shutdown, normaltext;
	int resetPercentage;
	int percentageToSolve;
    private static int moduleIdCounter = 1;
    private int moduleId = 0;
	private string[] unitList = new string [6] {"B", "KB", "MB", "GB", "TB", "PB"};
	private Coroutine RAMactive;
	// Use this for initialization
	void Awake () {
		moduleId = moduleIdCounter++;
		//Clear button interact
 		clear.OnInteract += clearRAM;
		//Picking units
		unit = Random.Range(0, 6);
		currentUnit = unitList[unit];
		//Randomizing RAM Limit
		limitDigit = Random.Range(100, 1000);
		//Initial RAM = 40%
		currentDigit = limitDigit / 4;
		//Display all of them on the module
		displayCurrentDigit.text = currentDigit.ToString();
		displayCurrentUnit.text = currentUnit.ToString();
		displayLimitDigit.text = limitDigit.ToString();
		displayLimitUnit.text = currentUnit.ToString();
		UpdateProgressBar();
		percentageToSolve = Random.Range (40, 60);
		texts[11].text = string.Empty;
		ignoredModules = GetComponent<KMBossModule>().GetIgnoredModules("Random Access Memory", new string[]{
				"Random Access Memory",
				"14"
            });		
		nonIgnored = Bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Count();
		//nonIgnored = nonIgnored + 2; Debug.LogFormat("[Random Access Memory #{0}]: Please notify creator because he forgot he added 2 on non-Ignored modules for debugging!", moduleId); //For debugging 
	}
	void Start () {
		//Activates the module
		StartCoroutine(checkForceSolve());
		RAMactive = StartCoroutine(RAMUsage());
		if (RAMactive != null)
		StopCoroutine(RAMactive);
		//StartCoroutine(solveDelay()); //Debugging
	}
	void UpdateProgressBar()
		{
		//70 Bars
		numofbars1 = currentDigit * 70 / limitDigit;
		numofbars2 = (70 - numofbars1);
		//Debug.LogFormat("[Random Access Memory #{0}]: Numbers of bars = {1},{2}!", moduleId, numofbars1, numofbars2);
		for (int i = 1; i <= numofbars1; i++)
		{
			bars1 += "|";
		}
		for (int i = 1; i <= numofbars2; i++)
		{
			bars2 += "|";
		}
		if (currentDigit * 100 / limitDigit >= 70)
		{
			progressbar.color = warning;
			for (int i = 3; i <= 7; i++)
			texts[i].color = warning;
		}
		else
		{
			progressbar.color = normal;
			for (int i = 3; i <= 7; i++)
			texts[i].color = normaltext;
		}
		progressbar.text = bars1;
		progressbarlimit.text = bars2;
		bars1 = "";
		bars2 = "";
		}
	private bool clearRAM ()
		{
			if (solvedState == false && inputMode == true && (currentDigit * 100 / limitDigit) >= 30)
			{	
			//Debug.LogFormat("[Random Access Memory #{0}]: Clear button has been pressed!", moduleId);
			StartCoroutine(clearAnimation());
			
			}
			else if (solvedState == false && inputMode == true)
			{
			StartCoroutine(clearAnimationlite());
			}
       		return false;
		}	
	
	IEnumerator clearAnimation () {
		inputMode = false;
		appclearedcount = Random.Range(100 , 150) * ((currentDigit * 100 / limitDigit) - 20) / 100;
		appclearedtext = appclearedcount + " applications cleared.";
		buttons[10].material = colours[1];
		texts[11].text = appclearedtext;
		resetPercentage = Random.Range (20, 30);
		currentDigit = resetPercentage * limitDigit / 100;
		yield return new WaitForSeconds(1.5f);
		buttons[10].material = colours[2];
		texts[11].text = string.Empty;
		inputMode = true;
		yield return new WaitForSeconds (1f);
		displayCurrentDigit.text = currentDigit.ToString();
		UpdateProgressBar();
	}
	IEnumerator clearAnimationlite () {
		inputMode = false;
		appclearedtext = "0 applications cleared.";
		buttons[10].material = colours[1];
		texts[11].text = appclearedtext;
		yield return new WaitForSeconds(1.5f);
		buttons[10].material = colours[2];
		texts[11].text = string.Empty;
		inputMode = true;
		yield return new WaitForSeconds (1f);
	}
	IEnumerator checkForceSolve () {
		if (nonIgnored == 0) {
			yield return new WaitForSeconds(3f);
			GetComponent<KMBombModule>().HandlePass();
			Debug.LogFormat("[Random Access Memory #{0}]: There are no non-ignored modules, forcing module to be solved.", moduleId);
			solvedState = true;
			inputMode = false;
			displayCurrentDigit.text = "---";
			displayLimitDigit.text = "---";
			yield return new WaitForSeconds (2f);
			foreach(Renderer b in buttons)
       		{
            b.material = colours[0];
			}
			foreach (TextMesh thing in texts)
         	{
            thing.text = string.Empty;
            }
		}
		else {StartCoroutine(RAMUsage());};
	}

	IEnumerator RAMUsage () {
		if (solvedState == false && safeMode == false)
		{
		inputMode = true;
		increasedDuration = Random.Range (6, 15);
		for ( int i = 1; i < increasedDuration; i++ ) {
			yield return new WaitForSeconds(1f);
		}
		increasedPercentage = Random.Range (5, 12);
		currentDigit = currentDigit + (increasedPercentage * limitDigit / 100); 
		//Debug.LogFormat("[Random Access Memory #{0}]: Increased Percentage = {2}%, Current RAM Amount is {1}", moduleId, currentDigit, increasedPercentage);			
		displayCurrentDigit.text = currentDigit.ToString();
		UpdateProgressBar();
		StartCoroutine(checkSolveStrike());
		}
	}
	IEnumerator checkSolveStrike () {
			if ( Bomb.GetSolvedModuleNames().Count() * 100 / nonIgnored >= percentageToSolve)
			{
				safeMode = true;
				StartCoroutine(solveDelay());
			}
			else if (currentDigit > limitDigit)
			{
				inputMode = false;
				currentDigit = limitDigit;
				displayCurrentDigit.text = currentDigit.ToString();
				UpdateProgressBar();
				//*Add bomb freeze here
				yield return new WaitForSeconds(3f);
				GetComponent<KMBombModule>().HandleStrike(); 
				Debug.LogFormat("[Random Access Memory #{0}]: RAM Exceeded Maximum, module striked.", moduleId);
				currentDigit = limitDigit * 4 / 10;
				StartCoroutine(RAMUsage());
				inputMode = true;
			}
			else StartCoroutine(RAMUsage());
	}

	IEnumerator solveDelay () {
		StopCoroutine(RAMUsage());
		RAMactive = null;
		yield return new WaitForSeconds (1f);
		GetComponent<KMAudio>().PlaySoundAtTransform("4beeps", transform);
		inputMode = false;
		buttons[10].material = colours[1];
		Debug.LogFormat("[Random Access Memory #{0}]: Initiated safe mode!", moduleId);
		progressbar.color = shutdown;
		bars1 = "";
		bars2 = "||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||";
		numofbars1 = 69;
		numofbars2 = 69;
		progressbar.text = bars1;
		progressbarlimit.text = bars2;
		displayCurrentDigit.text = "---";
		displayLimitDigit.text = "---";
		for ( int i = 1; i <= 70; i++)
		{
			yield return new WaitForSeconds(.1005f);
			bars1 += "|";
			bars2 = bars2.Remove(numofbars2);
			numofbars2--;
			progressbar.text = bars1;
			progressbarlimit.text = bars2;
		}
		yield return new WaitForSeconds (1.2857f);
		for ( int i = 1; i <= 70; i++)
		{
			yield return new WaitForSeconds(1.2857f);
			bars1 = bars1.Remove(numofbars1);
			bars2 += "|";
			numofbars1--;
			progressbar.text = bars1;
			progressbarlimit.text = bars2;
		}
			GetComponent<KMBombModule>().HandlePass();
			GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
			Debug.LogFormat("[Random Access Memory #{0}]: Battery depleted, module solved!", moduleId);
			yield return new WaitForSeconds(2f);
			solvedState = true;
			foreach(Renderer b in buttons)
       		{
            b.material = colours[0];
			}
			foreach (TextMesh thing in texts)
         	{
            thing.text = string.Empty;
            }
	}

	#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} clear [Clears the memory]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
		if (Regex.IsMatch(command, @"^\s*clear\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*slap clear now\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))		
		{
			yield return null;
			clear.OnInteract();
			yield break;
        }
    }
 }
