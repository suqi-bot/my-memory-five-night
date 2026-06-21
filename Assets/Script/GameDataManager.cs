using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    public static GameDataManager Instance => instance;
    private static GameDataManager instance = new GameDataManager();

    private GameDataManager()
    {

    }
}
