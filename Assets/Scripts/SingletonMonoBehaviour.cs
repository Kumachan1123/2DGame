
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///  MonoBehaviourを継承した汎用的なSingleton
/// </summary>
/// <typeparam name="T">継承するクラス</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // インスタンス
    private static T instance;

    // インスタンスのゲッター
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindAnyObjectByType(t);
                if (instance == null)
                {
                    GameObject newobj = new GameObject();
                    newobj.name = typeof(T).Name;
                    newobj.AddComponent<T>();

                    //Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

    /*--------------------------------------------------------------------------------
　　|| 実行前初期化処理
　　--------------------------------------------------------------------------------*/
    /// <summary>
    /// 実行前初期化処理
    /// </summary>
    virtual protected void Awake()
    {
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する。
        CheckInstance();
    }

    /*--------------------------------------------------------------------------------
　　|| インスタンスが存在するのかどうか
　　--------------------------------------------------------------------------------*/
    /// <summary>
    /// インスタンスが存在するのかどうか
    /// </summary>
    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;

            return true;
        }
        else if (Instance == this)
        {
            return true;
        }
        Destroy(gameObject);
        return false;
    }
}
