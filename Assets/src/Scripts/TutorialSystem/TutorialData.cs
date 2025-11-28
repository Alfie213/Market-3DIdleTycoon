using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Tycoon/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    [Header("Phase 1: Start")]
    [TextArea(2, 5)] 
    public string welcomeMessage = "Welcome! Buy the Cashier and the Meat Stall to open your market.";

    [Header("Phase 2: Shop Open")]
    [TextArea(2, 5)]
    public string shopOpenedMessage = "Great job! The shop is OPEN. Customers are coming...";

    [Header("Phase 3: First Profit")]
    [TextArea(2, 5)]
    public string firstProfitMessage = "First profit made! Tap on a building and BUY an upgrade to boost efficiency!";

    [Header("Phase 4: Completion")]
    [TextArea(2, 5)]
    public string completionMessage = "You are a professional businessman now! Remember: the more upgrades you buy, the more customers will come! Good luck!";
}