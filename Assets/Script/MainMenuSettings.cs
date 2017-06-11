using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ControllerMode
{
    Tactile,
    Accelerometer,
    Keyboard,
    KeyboardAndMouse
}

public class MainMenuSettings : MonoBehaviour {

    ControllerMode cm;
    Difficulty difficulty;

    // Use this for initialization
    void Start()
    {
        SelectDifficultySettings(1);    //Tactile
        ControllerModeSettings(1);      //Easy
        //cm = ControllerMode.Tactile;
        //difficulty = Difficulty.Easy;

    }

    //Difficulty Buttons
    public Button EasyButton;
    public Button NormalButton;
    public Button HardButton;
    //public Button ImpossibruButton;

    //Game Mode Buttons
    public Button TactileButton;
    public Button AccelerometerButton;


    public void SelectDifficultySettings(int diffSettings)
    {
        EasyButton.interactable = true;
        NormalButton.interactable = true;
        HardButton.interactable = true;
        switch (diffSettings)
        {
            case 1:
                difficulty = Difficulty.Easy;
                EasyButton.interactable = false;
                break;

            case 2:
                difficulty = Difficulty.Normal;
                NormalButton.interactable = false;
                break;

            case 3:
                difficulty = Difficulty.Hard;
                HardButton.interactable = false;
                break;
                /*
            case 4:
                difficulty = Difficulty.Impossibru;
                break;
                */
        }
        GameModel.gameDifficulty = difficulty;
    }

    

    public void ControllerModeSettings(int controllerSettings)
    {
        TactileButton.interactable = true;
        AccelerometerButton.interactable = true;
        switch (controllerSettings)
        {
            case 1:
                cm = ControllerMode.Tactile;
                TactileButton.interactable = false;
                break;

            case 2:
                cm = ControllerMode.Accelerometer;
                AccelerometerButton.interactable = false;
                break;
                /*
            case 3:
                cm = ControllerMode.Keyboard;
                break;
                /*
            case 4:
                cm = ControllerMode.KeyboardAndMouse;
                break;
                */
        }
        GameModel.controllerMode = cm;
    }
}
