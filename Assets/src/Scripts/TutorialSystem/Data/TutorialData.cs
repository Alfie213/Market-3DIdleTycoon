using UnityEngine;

/// <summary>
/// Stores localized text strings for the tutorial system.
/// </summary>
[CreateAssetMenu(fileName = "TutorialData", menuName = "Tycoon/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    [Header("Messages")]
    [TextArea(2, 5)] public string welcomeMessage = "Welcome! Buy the Cashier and the Meat Stall to open your market.";
    [TextArea(2, 5)] public string shopOpenedMessage = "Great job! The shop is OPEN. Customers are coming...";
    [TextArea(2, 5)] public string firstProfitMessage = "First profit made! Tap on a building and BUY an upgrade to boost efficiency!";
    [TextArea(2, 5)] public string completionMessage = "You are a professional businessman now! Remember: the more upgrades you buy, the more customers will come! Good luck!";
    [TextArea(2, 5)] public string ticketFoundMessage = "You found a GOLDEN TICKET! Tap on the ticket icon to claim your reward.";
    [TextArea(2, 5)] public string ticketUsedMessage = "The Golden Ticket gave you money! Bigger shop = More tickets!";
}