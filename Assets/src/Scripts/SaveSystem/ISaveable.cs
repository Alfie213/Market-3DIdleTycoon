public interface ISaveable
{
    // Метод, который говорит объекту: "Заполни эти данные своим состоянием"
    void PopulateSaveData(GameSaveData saveData);

    // Метод, который говорит объекту: "Вот твои данные, восстанови состояние"
    void LoadFromSaveData(GameSaveData saveData);
}