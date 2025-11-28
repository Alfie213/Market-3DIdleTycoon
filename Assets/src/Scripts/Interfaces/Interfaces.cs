// Интерфейс для объектов, с которыми можно взаимодействовать (кликом)
public interface IInteractable
{
    void Interact();
}

// Интерфейс для системы сохранений
public interface ISaveable
{
    void PopulateSaveData(GameSaveData saveData);
    void LoadFromSaveData(GameSaveData saveData);
}

// Интерфейс для UI окон (Новый)
public interface IView
{
    void Show();
    void Hide();
}