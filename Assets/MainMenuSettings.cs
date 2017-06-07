using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSettings : MonoBehaviour {

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
        Difficulty difficulty = Difficulty.Easy;
        switch (diffSettings)
        {
            case 1:
                difficulty = Difficulty.Easy;
                break;

            case 2:
                difficulty = Difficulty.Normal;
                break;

            case 3:
                difficulty = Difficulty.Hard;
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
        switch (controllerSettings)
        {
            case 1:
                break;

            case 2:
                break;
        }
    }
}
