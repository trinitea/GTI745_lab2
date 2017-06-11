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

    // Accelerometer
    public Slider AccelerometerSlider;

    // Use this for initialization
    void Start()
    {
        switch (GameModel.gameDifficulty)
        {
            case Difficulty.Easy:
                SelectDifficultySettings(1);
                break;

            case Difficulty.Normal:
                SelectDifficultySettings(2);
                break;

            default:
                SelectDifficultySettings(3);
                break;
        }

        switch(GameModel.controllerMode)
        {
            case InputController.VIRTUAL_JOYSTICK:
                ControllerModeSettings(1);
                break;

            case InputController.PAD:
                ControllerModeSettings(2);
                break;

            default:
                ControllerModeSettings(3);
                break;
        }

        AccelerometerSlider.value = GameModel.accelerometerSensitivity;
    }

    public void SelectDifficultySettings(int diffSettings)
    {
        EasyButton.interactable = true;
        NormalButton.interactable = true;
        HardButton.interactable = true;

        Difficulty difficulty = Difficulty.Easy;

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
        }

        GameModel.gameDifficulty = difficulty;
    }

    

    public void ControllerModeSettings(int controllerSettings)
    {
        TactileButton.interactable = true;
        AccelerometerButton.interactable = true;

        InputController controller = InputController.VIRTUAL_JOYSTICK;

        switch (controllerSettings)
        {
            case 1:
                controller = InputController.VIRTUAL_JOYSTICK;
                TactileButton.interactable = false;
                break;

            case 2:
                controller = InputController.PAD;
                TactileButton.interactable = false;
                break;

            case 3:
                controller = InputController.ACCELEROMETER;
                AccelerometerButton.interactable = false;
                break;
        }

        GameModel.controllerMode = controller;
    }

    public void CalibrateAccelerometer()
    {
        GameModel.accelerometerXAtStart = Input.acceleration.x;
        GameModel.accelerometerYAtStart = Input.acceleration.y;
    }

    public void SetAccelerometerSensitivity()
    {
        GameModel.accelerometerSensitivity = AccelerometerSlider.value;
    }
}
