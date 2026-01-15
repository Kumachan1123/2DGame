
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class BattleInputReceiver : MonoBehaviour
{
    // アクションマップ名
    private static readonly string ACTION_MAP_NAME = "Battle";

    // アクション
    public enum Actions : uint
    {
        SHOW = 0,        //	表示
        UP,             //	上移動
        DOWN,           //	下移動
        LEFT,           //	左移動
        RIGHT,          //	右移動
        OVER_ID      //	最大数
    };

    // アクションの最大数
    public static readonly uint ACTION_COUNT = (int)Actions.OVER_ID;

    // アクション名（上記アクションのインデックスに依存）
    private static readonly string[] ACTION_NAME =
    {
        "Show",     // 表示
        "Up",     // 上移動
        "Down",   // 下移動
        "Left",   // 左移動
        "Right",  // 右移動
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

    // 実行前初期化処理 
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
            m_actions[i] = m_gameActionMap.FindAction(ACTION_NAME[i], true);
        }
    }

    // 入力の取得
    public bool GetInputButton(Actions action, InputType inputType)
    {
        switch (inputType)
        {
            case InputType.PRESSED:// 入力された瞬間
                return m_actions[(int)action].WasPressedThisFrame();
            case InputType.HOLD:// 入力されている間
                return m_actions[(int)action].IsPressed();
            case InputType.RELEASED:// 入力がなくなった瞬間
                return m_actions[(int)action].WasReleasedThisFrame();
            default:// それ以外
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
