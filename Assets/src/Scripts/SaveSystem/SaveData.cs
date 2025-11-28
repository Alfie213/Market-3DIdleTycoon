using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    // Глобальные настройки
    public int money;
    public int tickets;

    // Состояние туториала
    public int tutorialStepIndex;
    public bool isTutorialInventoryReady;

    // Состояние инвентаря (флаги первых билетов)
    public bool isFirstTicketDropped;
    public bool isFirstTicketUsed;

    // Список зданий. Используем List, так как Dictionary не сериализуется Unity JsonUtility
    public List<BuildingSaveData> buildings = new List<BuildingSaveData>();
}

[Serializable]
public class BuildingSaveData
{
    public string id;           // Уникальный ID здания (чтобы знать, какое это здание)
    public bool isBuilt;
    public int speedLevel;
    public int unlockedWorkers; // Сколько работников открыто
}