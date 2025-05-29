using System;
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
    
    public static event Action<string> OnLocalPlayerRequestedDeal;

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
        
        // Subscribe to button trigger
        OnLocalPlayerRequestedDeal += HandleDealRequested;
    }
    
    private void HandleDealRequested(string _)
    {
        Show();
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
        _dealerController.RpcRequestDealCards(PlayerName);
    }

    public static void OnOnLocalPlayerRequestedDeal(string obj)
    {
        OnLocalPlayerRequestedDeal?.Invoke(obj);
    }
}