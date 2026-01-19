using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ActionPlayerInputReceiver : MonoBehaviour
{
    // アクションマップ名
    private static readonly string ACTION_MAP_NAME = "Player";
    // アクション
    public enum Actions : uint
    {
        MOVE = 0,        //	移動
        JUMP,           //  ジャンプ
        OVER_ID      //	最大数
    };
    // アクションの最大数
    public static readonly uint ACTION_COUNT = (int)Actions.OVER_ID;
    // アクション名（上記アクションのインデックスに依存）
    private static readonly string[] ACTION_NAME =
    {
        "Move",     // 移動
        "Jump",     // ジャンプ
    };
    // 入力タイプ
    public enum InputType
    {
        PRESSED,        //	入力された瞬間
        HOLD,           //	入力されている間
        RELEASED,       //	入力がなくなった瞬間
    }
    // コンポーネント
    private PlayerInput m_playerInput;
    // アクションマップ
    private InputActionMap m_gameActionMap;
    // 各アクション
    private InputAction[] m_actions;

    void Start()
    {
        //	コンポーネントの取得
        m_playerInput = GetComponent<PlayerInput>();
        //	アクションマップの取得
        m_gameActionMap = m_playerInput.actions.FindActionMap(ACTION_MAP_NAME);
        //	各アクションの取得
        m_actions = new InputAction[ACTION_COUNT];
        for (int i = 0; i < ACTION_COUNT; i++)
        {
            m_actions[i] = m_gameActionMap.FindAction(ACTION_NAME[i]);
        }
    }
    // 指定したアクションが指定した入力タイプかどうかを取得
    public bool GetActionInput(Actions action, InputType inputType)
    {
        switch (inputType)
        {
            case InputType.PRESSED:
                return m_actions[(int)action].WasPressedThisFrame();
            case InputType.HOLD:
                return m_actions[(int)action].IsPressed();
            case InputType.RELEASED:
                return m_actions[(int)action].WasReleasedThisFrame();
            default:
                return false;
        }
    }
    // 任意の型での入力を取得
    public T GetInputValue<T>(Actions action)
        where T : struct
    {
        return m_actions[(uint)action].ReadValue<T>();
    }
}
