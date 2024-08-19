using System;
using UnityEngine;
using UnityEngine.UI;
using State = Define.EntityState;

public class UIButtonPanel : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button endTurnButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(() => Managers.Game.Player.ReadyGame());
        moveButton.onClick.AddListener(() => Managers.Game.Player.ChangeState(State.ShowRange | State.Move));
        attackButton.onClick.AddListener(() => Managers.Game.Player.ChangeState(State.ShowRange | State.Attack));
        endTurnButton.onClick.AddListener(() => Managers.Game.Player.EndTurn());
    }
}
