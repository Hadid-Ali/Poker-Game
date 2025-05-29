using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button dealButton;
    [SerializeField] private DealerController dealerController;
    [SerializeField] private string playerName = "Player"; // Customize if needed

    private void Start()
    {
        // Subscribe to the dealer's state change event
        if (dealerController == null) return;
        
        dealerController.onDealerPlayingStateChanged.AddListener(SetInteractable);
        SetInteractable(!dealerController.IsPlaying);
    }

    private void OnEnable()
    {
        dealButton.onClick.AddListener(OnDealButtonPressed);
    }

    private void OnDisable()
    {
        dealButton.onClick.RemoveListener(OnDealButtonPressed);
    }

    private void OnDealButtonPressed()
    {
        if (dealerController == null) return;

        if (dealerController.IsPlaying)
        {
            dealerController.ShowMessage("Dealer is busy. Please wait.");
            return;
        }

        dealerController.RpcRequestDealCards(playerName);
    }

    private void SetInteractable(bool enabled)
    {
        if (dealButton != null)
        {
            dealButton.interactable = enabled;
        }
    }
}
