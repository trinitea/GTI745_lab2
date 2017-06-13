using System;

public class GameModel
{
    static public Difficulty gameDifficulty = Difficulty.Normal;
    static public InputController controllerMode = InputController.VIRTUAL_JOYSTICK;

    static public float accelerometerXAtStart = 0.0f;
    static public float accelerometerYAtStart = 0.0f;
    
    static public float accelerometerSensitivity = 1.0f;
}
