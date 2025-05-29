using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DealerController : NetworkBehaviour
{
    private static readonly int Talk = Animator.StringToHash("Talk");
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dealClip;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private AnimationClip _animationClip;

    [Networked] public bool IsPlaying { get; set; }

    public UnityEvent<bool> onDealerPlayingStateChanged;
    

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RpcRequestDealCards(string playerName)
    {
        if (IsPlaying)
        {
            Debug.Log("Dealer is already playing. Ignoring request.");
            return;
        }

        IsPlaying = true;
        RpcBroadcastDealCards(playerName);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RpcBroadcastDealCards(string playerName)
    {
        if (messageText != null)
            messageText.text = $"{playerName} Clicked Deal Cards!";

        if (animator != null)
            animator.SetTrigger(Talk);

        if (audioSource != null && dealClip != null)
            audioSource.PlayOneShot(dealClip);
        
        onDealerPlayingStateChanged?.Invoke(false);
        IsPlaying = true;

        if (!HasStateAuthority) return;
        
        var duration = GetClipDuration();
        StartCoroutine(ResetDealerAfter(duration));
    }

    private IEnumerator ResetDealerAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        RpcChangeState();
    }
    
    
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RpcChangeState()
    {
        onDealerPlayingStateChanged?.Invoke(true);
        IsPlaying = false;
        messageText.text = "";
    }

    private float GetClipDuration()
    {
        return _animationClip.length;
    }

    public void ShowMessage(string msg)
    {
        if (messageText != null)
            messageText.text = msg;
    }
}
