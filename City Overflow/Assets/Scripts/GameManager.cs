using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public WaterLevelController waterLevelCity_1;
    public WaterLevelController waterLevelCity_2;
    public Text UIText;
    public GameObject UITextObject;


    private enum Scenarios {None, BothFull, City_1_Flooded, City_2_Flooded, Both_Flooded}
    private Scenarios scenario = Scenarios.None;
    private int scenarioChecks = 0;
    private float countdownTimer = 10f;

    void Awake()
    {
        Instantiate();
    }

    private void Instantiate()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public IEnumerator UpdateWaterLevel(float time)
    {
        yield return new WaitForSeconds(time);

        if (waterLevelCity_1.waterLevelPercentage == 1f && waterLevelCity_2.waterLevelPercentage == 1f)
        {
            scenario = Scenarios.BothFull;
        } else if (waterLevelCity_1.waterLevelPercentage > 1f && waterLevelCity_2.waterLevelPercentage == 1f)
        {
            scenario = Scenarios.City_1_Flooded;
        } else if (waterLevelCity_1.waterLevelPercentage == 1f && waterLevelCity_2.waterLevelPercentage >= 1f)
        {
            scenario = Scenarios.City_2_Flooded;
        } else if (waterLevelCity_1.waterLevelPercentage > 1f && waterLevelCity_2.waterLevelPercentage > 1f) 
        {
            scenario = Scenarios.Both_Flooded;
        }

        /*
        Game logic:

        When a scenario gets triggered a timer of x seconds starts.
        This is because the player needs to be in a scenario for x seconds for it to count as a valid state.

        If the "win scenario" was held the game ends in a win. if in the mean time a city got flooded we end in a game over state.
        */

        // A scenario was reached, check again in x seconds to confirm and end game
        if (scenario != Scenarios.None)
        {
            if (scenarioChecks < 1)
            {
                // Check scenario again in x seconds
                scenarioChecks ++;
                StartCoroutine(UpdateWaterLevel(countdownTimer));
                StartCoroutine(UpdateCountdown(0f));
            }
        }
    }

    private IEnumerator UpdateCountdown(float time)
    {
        yield return new WaitForSeconds(time);
        UITextObject.SetActive(true);
        UIText.text = Mathf.Floor(countdownTimer).ToString();

        countdownTimer -= 1;
        if (countdownTimer > 0)
        {
            StartCoroutine(UpdateCountdown(1f));
        } else {
            GameStateCheck();
        }
    }

    private void GameStateCheck()
    {
        Debug.Log("Checking");
        Debug.Log(scenario);
        if (scenario == Scenarios.BothFull)
        {
            // both cities full
            UIText.text = "Game won!";
        } else if (scenario == Scenarios.City_1_Flooded)
        {
            // City 1 overflowed when city 2 is full
            UIText.text = "Gameover city 1 is flooded!";
        } else if (scenario == Scenarios.City_2_Flooded)
        {
            // City 2 overflowed when city 1 is full
            UIText.text = "Gameover city 2 is flooded!";
        } else if (scenario == Scenarios.Both_Flooded)
        {
            // Both cities overflowed
            UIText.text = "Gameover both cities are flooded!";
        }
    }
}
