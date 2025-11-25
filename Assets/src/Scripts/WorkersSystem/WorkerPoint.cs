using UnityEngine;

public class WorkerPoint : MonoBehaviour
{
    [SerializeField] private GameObject workerModel;
    [SerializeField] private WorldProgressBar progressBar;

    private bool _isUnlocked = false;
    private bool _isBusy = false;

    public bool IsUnlocked => _isUnlocked;
    public bool IsBusy => _isBusy;

    public void SetUnlocked(bool state)
    {
        _isUnlocked = state;
        if (workerModel) workerModel.SetActive(state);
        // Если место заблокировано или работник свободен - бар скрыт
        if (!state) progressBar.Hide();
    }

    public void SetBusy(bool state)
    {
        _isBusy = state;
        if (!state) progressBar.Hide();
    }

    public void UpdateProgress(float progress)
    {
        progressBar.SetProgress(progress);
    }
}