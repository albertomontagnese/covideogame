﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.SceneManagement;



[AddComponentMenu("")]
public class UIScript : MonoBehaviour
{
	[Header("Configuration")]
	public Players numberOfPlayers = Players.OnePlayer;

	public GameType gameType = GameType.Score;

	// If the scoreToWin is -1, the game becomes endless (no win conditions, but you could do game over)
	public int scoreToWin = 10000000;


	[Header("References (don't touch)")]
	//Right is used for the score in P1 games
	public Text[] numberLabels = new Text[2];
	public Text rightLabel, leftLabel;
	public Text winLabel;
	public GameObject statsPanel, gameOverPanel, winPanel, statsPanelBottom;
	public Transform inventory;
	public GameObject resourceItemPrefab;


	// Internal variables to keep track of score, health, and resources, win state
	public int[] scores = new int[2];
	public int[] playersHealth = new int[2];
	private Dictionary<int, ResourceStruct> resourcesDict = new Dictionary<int, ResourceStruct>(); //holds a reference to all the resources collected, and to their UI
    public bool gameOver = false; //this gets changed when the game is won OR lost
	public Stopwatch timer;
	public int level = 0;
 
 	private void Start()
	{
		//ctr
		timer = new Stopwatch();

		timer.Start();

		if(numberOfPlayers == Players.OnePlayer)
		{
			// No setup needed
		}
		else
		{
			if(gameType == GameType.Score)
			{
				// Show the 2-player score interface
				rightLabel.text = leftLabel.text = "Score";

				// Show the score as 0 for both players
				numberLabels[0].text = numberLabels[1].text = "0";
				scores[0] = scores[1] = 0;
			}
			else
			{
				// Show the 2-player life interface
				rightLabel.text = leftLabel.text = "Life";

				// Life will be provided by the PlayerHealth components
			}
		}
	}

	private void Update() {
		GameObject.Find("LeftNumberBottom").GetComponent<UnityEngine.UI.Text>().text = timer.Elapsed.ToString("hh\\:mm\\:ss\\.ff");
		int newLevel = System.Convert.ToInt32(timer.ElapsedMilliseconds / 30000) + 1;
		if (level < newLevel) {
			level = newLevel;
			GameObject.Find("RightNumberBottom").GetComponent<UnityEngine.UI.Text>().text = newLevel.ToString();
		}

		if (gameOver && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
			{ 
				UnityEngine.Debug.Log("restart");
				SceneManager.LoadScene("AlbertoMainScene"); 
			}
		
	}

	//version of the one below with one parameter to be able to connect UnityEvents
	public void AddOnePoint(int playerNumber)
	{
		AddPoints(playerNumber, 1);
	}


	public void AddPoints(int playerNumber, int amount = 1)
	{
		scores[playerNumber] += amount;

		if(numberOfPlayers == Players.OnePlayer)
		{
			numberLabels[1].text = scores[playerNumber].ToString(); //with one player, the score is on the right
		}
		else
		{
			numberLabels[playerNumber].text = scores[playerNumber].ToString();
		}

		if(gameType == GameType.Score
			&& scores[playerNumber] >= scoreToWin)
		{
			GameWon(playerNumber);
		}
	}

	//currently unused by other Playground scripts
	public void RemoveOnePoint(int playerNumber)
	{
		scores[playerNumber]--;

		if(numberOfPlayers == Players.OnePlayer)
		{
			numberLabels[1].text = scores[playerNumber].ToString(); //with one player, the score is on the right
		}
		else
		{
			numberLabels[playerNumber].text = scores[playerNumber].ToString();
		}
	}



	public void GameWon(int playerNumber)
	{
		// only set game over UI if game is not over
	    if (!gameOver)
	    { 
			timer.Stop();
			gameOver = true;
			// winLabel.text = "Player " + ++playerNumber + " wins!";
			winLabel.text = "Congrats! Way to flatten the curve!!";
			statsPanel.SetActive(false);
			winPanel.SetActive(true);
		}
	}



	public void GameOver(int playerNumber)
	{
        // only set game over UI if game is not over
	    if (!gameOver)
	    {
			timer.Stop();
			gameOver = true;
	        // statsPanel.SetActive(false);
	        gameOverPanel.SetActive(true);

			// string restOfLabel = "\nDo the 5: \n1. HANDS Wash them often \n2. ELBOW Cough into it \n3. FACE Don't touch it \n4. SPACE Keep safe distance \n5. HOME Stay if you can";
			string restOfLabel = "\nStay Home & Play On :) \n Touch anywhere to restart";
			string gameOverLabel = "Score: " + scores[playerNumber].ToString() + restOfLabel ;
			// if (level < 3) {
			// 	gameOverLabel = "Score: " + scores[playerNumber].ToString() + restOfLabel ;
			// } else if (level < 6) {
			// 	gameOverLabel = "";
			// } else {
			// 	gameOverLabel = "";
			// }
			GameObject.Find("GameOverLabel").GetComponent<UnityEngine.UI.Text>().text = gameOverLabel;
	    }
	}



	public void SetHealth(int amount, int playerNumber)
	{
		playersHealth[playerNumber] = amount;
		numberLabels[playerNumber].text = playersHealth[playerNumber].ToString();
	}



	public void ChangeHealth(int change, int playerNumber)
	{
		SetHealth(playersHealth[playerNumber] + change, playerNumber);

		if(gameType != GameType.Endless
			&& playersHealth[playerNumber] <= 0)
		{
			GameOver(playerNumber);
		}

	}



	//Adds a resource to the dictionary, and to the UI
	public void AddResource(int resourceType, int pickedUpAmount, Sprite graphics)
	{
		if(resourcesDict.ContainsKey(resourceType))
		{
			//update the dictionary key
			int newAmount = resourcesDict[resourceType].amount + pickedUpAmount;
			resourcesDict[resourceType].UIItem.ShowNumber(newAmount);
			resourcesDict[resourceType].amount = newAmount;
		}
		else
		{
			//create the UIItemScript and display the icon
			UIItemScript newUIItem = Instantiate<GameObject>(resourceItemPrefab).GetComponent<UIItemScript>();
			newUIItem.transform.SetParent(inventory, false);

			resourcesDict.Add(resourceType, new ResourceStruct(pickedUpAmount, newUIItem));

			resourcesDict[resourceType].UIItem.ShowNumber(pickedUpAmount);
			resourcesDict[resourceType].UIItem.DisplayIcon(graphics);
		}
	}


	//checks if a certain resource is in the inventory, in the needed quantity
	public bool CheckIfHasResources(int resourceType, int amountNeeded = 1)
	{
		if(resourcesDict.ContainsKey(resourceType))
		{
			if(resourcesDict[resourceType].amount >= amountNeeded)
			{
				return true;
			}
			else
			{
				//not enough
				return false;
			}
		}
		else
		{
			//resource not present
			return false;
		}
	}


	//to use only before checking that the resource is in the dictionary
	public void ConsumeResource(int resourceType, int amountNeeded = 1)
	{
		resourcesDict[resourceType].amount -= amountNeeded;
		resourcesDict[resourceType].UIItem.ShowNumber(resourcesDict[resourceType].amount);
	}


	public enum Players
	{
		OnePlayer = 0,
		TwoPlayers
	}

	public enum GameType
	{
		Score = 0,
		Life,
		Endless
	}
}



//just a virtual representation of the resources for the private dictionary
public class ResourceStruct
{
	public int amount;
	public UIItemScript UIItem;

	public ResourceStruct(int a, UIItemScript uiRef)
	{
		amount = a;
		UIItem = uiRef;
	}
}