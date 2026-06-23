using System;
using UnityEngine;

public class MainHUD : UIBase
{
    private Rigidbody _playerRigidbody;

    private void Start()
    {

        if (GameManager.Instance.GetPlayerRigidbody() == null)
        {
            GameManager.Instance.PlayerCreated += BindPlayer;
        }
        else
        {
            BindPlayer(GameManager.Instance.GetPlayerRigidbody());
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCreated -= BindPlayer;
        }
    }
    private void BindPlayer(Rigidbody playerRigidbody)
    {
        _playerRigidbody = playerRigidbody;
    }


}