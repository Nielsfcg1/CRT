using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class GameManager : MonoBehaviour {

    [Header("Drag the corresponding GameObjects here")]
    public GameObject player;
    public GameObject boss;
    public GameObject redLaser;
    public GameObject blueLaser;
    public GameObject yellowLaser;
    public GameObject greenLaser;
    public Transform laserSpawnpoint;
    public GameObject bossLeye;
    public GameObject bossReye;

    //Inspector variables
    public float minScanTime;
    public float maxScanTime;
    public float[] zones;           //In inspector set the amount of zones the game has
    public float distanceToWin;
    public float minReactionTime;
    public float maxReactionTime;
    public float reactionTimer;
    public int amountOfMasks;

    [Header("UI")]
    public Text pressXToContinue;
    public Text wonGame;
    public Text uZone;
    public Text uDistance;

    //Local variables
    float scanTimer = 0.0f;
    
    bool maskSelection;
    int startDistance;

    float distance;
    int currentZone = 99999;
    int reactionZone;

    int currentMask;
    float randomScanTime;

    //Masks
    public GameObject mask;
    public GameObject maskRed;
    public GameObject maskBlue;
    public GameObject maskGreen;
    public GameObject maskYellow;

    public bool correctMask;

    //Script references
    PlayerScript pl;

    void Start ()
    {
        //Calculate distance to boss
        distance = Vector3.Distance(player.transform.position, boss.transform.position);
        startDistance = Mathf.RoundToInt(distance);

        //Calculate the zones with the length of the zones array set in the inspector
        CalculateDifficultyZones(zones.Length);

        //Component reference of player
        pl = player.GetComponent<PlayerScript>();
    }

    void Update ()
    {
        //Zone 0 is the last zone, therefore the most amount of masks
        if (currentZone == 2) { amountOfMasks = 2; }
        if (currentZone == 1) { amountOfMasks = 3; }
        if (currentZone == 0) { amountOfMasks = 4; }

        //Check distance
        distance = Vector3.Distance(player.transform.position, boss.transform.position);

        //UI DEBUGGING
        uDistance.text = "Distance: " + distance.ToString("0.00");
        uZone.text = "Zone " + currentZone;

        //Executing methods in update function
        CheckZonePlayer();
        ReactionTimer();

        //When close enough to the boss
        if (distance <= distanceToWin)
        {
            wonGame.text = "Congratulations! You have won the game.";
            wonGame.gameObject.SetActive(true);

            //Player stops walking
            pl.walk = false;
        }

        //Scan timer
        if (pl.walk)
        {
            scanTimer += Time.deltaTime;

            //Chooses a random time between the minimum and the maximum time
            if (randomScanTime == 0)
            {
                randomScanTime = Random.Range(minScanTime, maxScanTime);
            }


            //Initiate the attacking anmation
            if (scanTimer >= randomScanTime - 1.15f)
            {
                boss.GetComponentInChildren<Animator>().SetTrigger("Attack");
            }

            //If the timer exeeded the random time, show the masks and reset the randomScanTime
            if (scanTimer >= randomScanTime)
            {
                ShowMasks();
                Debug.Log("Attack");



                randomScanTime = 0;
            }
        }

        //if(moveLaser)
        //{

        //}

        //The player must select a mask
        if(maskSelection)
        {
            //Reaction timer
            reactionTimer -= Time.deltaTime;

            //Check input from player
            CheckInput();
        }

        if(correctMask)
        {
            if (reactionTimer <= 0.0f)
            {
                mask.SetActive(false);

                switch (currentMask)
                {
                    case 1:
                        maskRed.SetActive(false);
                        break;
                    case 2:
                        maskGreen.SetActive(false);
                        break;
                    case 3:
                        maskYellow.SetActive(false);
                        break;
                    case 4:
                        maskBlue.SetActive(false);
                        break;
                }

                bossLeye.GetComponent<Light>().color = Color.black;
                bossReye.GetComponent<Light>().color = Color.black;

                //Player starts walking again
                pl.walk = true;

                //Player can't select a mask anymore
                maskSelection = false;

                //A new reaction time will be set corresponding to the current zone of the player
                reactionTimer = (maxReactionTime * (currentZone + 1)) / zones.Length;

                correctMask = false;
            }
        }
	}

    void CheckInput()
    {
        //Setup buttons
        var button1 = Input.GetKeyDown(KeyCode.W);
        var button2 = Input.GetKeyDown(KeyCode.A);
        var button3 = Input.GetKeyDown(KeyCode.S);
        var button4 = Input.GetKeyDown(KeyCode.D);

        //If the correct button has been pressed, execute CorrectMask(). When player is wrong, execute WrongMask().
        if (button1 && currentMask == 1) { CorrectMask(); } else if (button1 && currentMask != 1) { WrongMask(); } 
        if (button2 && currentMask == 2) { CorrectMask(); } else if (button2 && currentMask != 2) { WrongMask(); } 
        if (button3 && currentMask == 3) { CorrectMask(); } else if (button3 && currentMask != 3) { WrongMask(); }
        if (button4 && currentMask == 4) { CorrectMask(); } else if (button4 && currentMask != 4) { WrongMask(); }
    }

    //When the player entered the correct button
    void CorrectMask()
    {
        //Scantimer will reset to zero
        scanTimer = 0.0f;

        //Remove message from screen
        pressXToContinue.gameObject.SetActive(false);

        //Put on mask with corresponding color
        mask.SetActive(true);
        switch(currentMask)
        {
            case 1:
                maskRed.SetActive(true);
                break;
            case 2:
                maskGreen.SetActive(true);
                break;
            case 3:
                maskYellow.SetActive(true);
                break;
            case 4: 
                maskBlue.SetActive(true);
                break;
        }

        correctMask = true;
    }

    //When the player entered the wrong button or ran out of time
    public void WrongMask()
    {
        pressXToContinue.text = "Game Over";
        pressXToContinue.gameObject.SetActive(true);
        maskSelection = false;

        player.GetComponent<PlayerScript>().GetComponentInChildren<Blur>().enabled = true;
    }

    void ShowMasks()
    {
        //Player stops walking
        pl.walk = false;

        //Generate a random mask
        int x = GenerateMask(1, amountOfMasks);

        //Store the mask in a variable
        currentMask = x;

        switch(currentMask)
        {
            case 1: 
                bossLeye.GetComponent<Light>().color = Color.red;
                bossReye.GetComponent<Light>().color = Color.red;
                //Fire laser with reaction time speed.
                GameObject go1 = Instantiate(redLaser, laserSpawnpoint.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
                break;
            case 2:
                bossLeye.GetComponent<Light>().color = Color.green;
                bossReye.GetComponent<Light>().color = Color.green;
                //Fire laser with reaction time speed.
                GameObject go2 = Instantiate(greenLaser, laserSpawnpoint.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
                break;
            case 3:
                bossLeye.GetComponent<Light>().color = Color.yellow;
                bossReye.GetComponent<Light>().color = Color.yellow;
                //Fire laser with reaction time speed.
                GameObject go3 = Instantiate(yellowLaser, laserSpawnpoint.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
                break;
            case 4:
                bossLeye.GetComponent<Light>().color = Color.blue;
                bossReye.GetComponent<Light>().color = Color.blue;
                //Fire laser with reaction time speed.
                GameObject go4 = Instantiate(blueLaser, laserSpawnpoint.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
                break;
        }

        //The reaction timer will be set corresponding to the current one of the player
        //reactionTimer = (maxReactionTime * (currentZone + 1)) / zones.Length;

        //The player can select a mask
        maskSelection = true;

        //Fire laser with reaction time speed.
        //GameObject go = Instantiate(laser, laserSpawnpoint.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
    }

    //Generate a mask. Max value is included this way
    int GenerateMask (int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    //Calculate the zone borders with a variable amount of zones.
    void CalculateDifficultyZones(int numberOfZones)
    {
        for (int i = 0; i < numberOfZones; i++)
        {
            zones[i] = (startDistance - distanceToWin) * i / numberOfZones + distanceToWin;
        }
    }

    //Check in which zone the player is
    void CheckZonePlayer()
    {
        for(int i = 0; i < zones.Length; i++)
        {
            //If the distance is higher than the last zone distance, or if the distance is ....
            if (distance > zones[zones.Length-1] || distance > zones[i] && distance < zones[i + 1])
            {
                currentZone = i;
            }
        }
    }

    displayMonth

    //Set a new reaction time each time a player enters a new zone
    void ReactionTimer()
    {
        //Checks when the player enters a new zone
        if(currentZone != reactionZone)
        {
            //Function will only be called when following variables are not equal
            reactionZone = currentZone;

            //ReactionTimer wil gradually downgrade to the minimum. When the reactiontimer is too far, it wil reset to minReaction time.
            reactionTimer = (maxReactionTime * (currentZone + 1)) / zones.Length;

            if(reactionTimer < minReactionTime)
            {
                reactionTimer = minReactionTime;
            }
        }
    }
}

//var x25 = (((startDistance - distanceToWin) * 0.25f) + distanceToWin);
//print("25% = " + x25);
 