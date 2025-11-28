using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int money;
    public int tickets;
    public int tutorialStepIndex;
    public bool isTutorialInventoryReady;
    public bool isFirstTicketDropped;
    public bool isFirstTicketUsed;
    public List<BuildingSaveData> buildings = new List<BuildingSaveData>();
}

[Serializable]
public class BuildingSaveData
{
    public string id;
    public bool isBuilt;
    public int speedLevel;
    public int unlockedWorkers;
}