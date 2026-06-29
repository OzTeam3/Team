using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : UIBase
{
    [SerializeField] private Text Text_Time;

    private Rigidbody _playerRigidbody;
    private float _elapsedTime = 0f;

    private void Start()
    {
        //if (GameManager.Instance.GetPlayerRigidbody() == null)
        //{
        //    GameManager.Instance.PlayerCreated += BindPlayer;
        //}
        //else
        //{
        //    BindPlayer(GameManager.Instance.GetPlayerRigidbody());
        //}
    }

    private void Update()
    {
        OnClickEscape();
    }

    private void OnDestroy()
    {
        //if (GameManager.Instance != null)
        //{
        //    GameManager.Instance.PlayerCreated -= BindPlayer;
        //}
    }

    private void BindPlayer(Rigidbody playerRigidbody)
    {
        //_playerRigidbody = playerRigidbody;
    }

    private void OnClickEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenEscapePopup();
        }
    }

    private void OpenEscapePopup()
    {
        UIManager.Instance.OpenPopupUI(UIType.ESCPopupUI);
    }
}