/// <summary>
/// Implemented by objects that respond to player clicks (Buildings, ATMs, etc.).
/// </summary>
public interface IInteractable
{
    void Interact();
}

/// <summary>
/// Implemented by systems that need to persist data.
/// </summary>
public interface ISaveable
{
    void PopulateSaveData(GameSaveData saveData);
    void LoadFromSaveData(GameSaveData saveData);
}

/// <summary>
/// Standard interface for UI Windows.
/// </summary>
public interface IView
{
    void Show();
    void Hide();
}