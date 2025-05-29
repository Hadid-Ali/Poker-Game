using UnityEngine;
using Fusion;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Transform spawnPoint;
    [Networked] private string PlayerName { get; set; }
    [Networked] public int PlayerID { get; set; }

    private DealerController _dealerController;
    private NetworkRunner _networkRunner;

    public override void Spawned()
    {
        if(!HasInputAuthority) return;
        _networkRunner = FindObjectOfType<NetworkRunner>();
        
        // Update the display
        UpdatePlayerName();
        
        // Find dealer (can also be cached centrally if needed)
        _dealerController = FindObjectOfType<DealerController>();
        if (_dealerController == null)
        {
            Debug.LogError("DealerController not found in scene.");
        }
    }
    
    public override void Render()
    {
        UpdatePlayerName();
    }

    private void UpdatePlayerName()
    {
        if (feedbackText == null) return;
        
        feedbackText.text = $"Player {PlayerID}";
        PlayerName = $"Player {PlayerID}";
    }

    public override void FixedUpdateNetwork()
    {
        if(!HasInputAuthority) return;

        if (!GetInput<NetworkInputData>(out var input)) return;

        if (input.EKeyDown)
        {
            //Show();
        }
    }
    
    private void Show()
    {
        if (_dealerController.IsPlaying)
        {
            _dealerController.ShowMessage("Dealer is already playing animation. Request ignored.");
            return;
        }
        _dealerController.RpcRequestDealCards(PlayerName);
    }
}