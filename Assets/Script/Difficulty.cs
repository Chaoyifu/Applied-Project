﻿/// <summary>
/// This is an enum to track the different difficulty levels
/// in the game. You probably do not want to modify this file.
/// If you want to change the number of difficulty levels,
/// this is the place to do it. If you want to change the actual
/// difficulty, see initDiffulcty in EmotionWrapper.
/// </summary>

public enum Difficulty
{
    One = 1,
    Two,
    Three,
    Four,
    Five
}

public static class DifficultyManagement {
    public static Difficulty currentDifficulty { get; private set; }

    public static void setDifficulty(Difficulty diff)
    {
        currentDifficulty = diff;
    }
}