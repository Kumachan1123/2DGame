/*
 *      SoundManager.cs
 *      サウンド管理クラス
 *      シングルトンで実装
 *      BGMは1つ、SEは複数同時再生可能
 *      
*/

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 名前と関連付けられたオーディオクリップで構成されるオーディオデータを表します。
/// </summary>
/// <remarks>このクラスは通常、サウンドエフェクト管理やオーディオ再生システムなど、アプリケーション内で
/// 名前によってオーディオアセットを保存および参照するために使用されます。このクラスはシリアル化可能として
/// マークされており、Unityのシリアライゼーションおよびインスペクターシステムでの使用をサポートします。</remarks>
[System.Serializable]
public class SoundData
{
    [Header("サウンドの名前")]
    public string name;
    [Header("音声ファイル")]
    public AudioClip clip;
}
/// <summary>
/// アプリケーションのバックグラウンドミュージック（BGM）とサウンドエフェクト（SE）の再生および音量制御を管理します。
/// </summary>
/// <remarks>このクラスは、マスター、BGM、SEチャンネルの個別の音量設定を含む、オーディオ再生の一元管理を提供します。
/// シングルトンとして実装されており、アプリケーション内でオーディオを再生および管理するための主要なインターフェースとして
/// 使用されることを意図しています。</remarks>
public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [Header("マスターボリューム")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Header("BGM音量")]
    [Range(0f, 1f)]
    public float bgmVolume = 1f;
    [Header("SE音量")]
    [Range(0f, 1f)]
    public float seVolume = 1f;

    [Header("サウンドデータ")]
    [SerializeField]
    private SoundDatas soundDatas;

    /// <summary>
    /// BGM用のオーディオソース
    /// </summary>
    private AudioSource bgmSource;

    /// <summary>
    /// BGMデータのリスト
    /// </summary>
    private List<SoundData> bgmList;

    /// <summary>
    /// SE用のオーディオソースのプール
    /// </summary>
    private List<AudioSource> seSources;

    /// <summary>
    /// SEデータのリスト
    /// </summary>
    private List<SoundData> seList;

    /// <summary>
    /// SEプールの親となるTransform
    /// </summary>
    private Transform sePoolRoot;

    /// <summary>
    /// スクリプトインスタンスがロードされるときにコンポーネントを初期化します。
    /// </summary>
    /// <remarks>このメソッドは、すべてのStartメソッドの前にUnityによって呼び出されます。このメソッドをオーバーライドして、
    /// ゲーム開始前に発生する必要がある初期化タスクを実行します。オーバーライドする際は、常に基底の実装を呼び出してください。</remarks>
    override protected void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// バックグラウンドミュージック（BGM）とサウンドエフェクト（SE）のソースをセットアップし、
    /// 利用可能なサウンドデータをロードすることで、オーディオシステムを初期化します。
    /// </summary>
    /// <remarks>このメソッドは、BGMおよびSE再生に必要な内部リストとオーディオソースを準備します。
    /// サウンドを再生する前に呼び出す必要があります。サウンドデータの設定が欠落している場合、
    /// 初期化は実行されず、エラーがログに記録されます。</remarks>
    private void Start()
    {
        bgmList = new List<SoundData>();
        seList = new List<SoundData>();
        seSources = new List<AudioSource>();

        if (soundDatas == null)
        {
            Debug.LogError("SoundDatas が設定されていません");
            return;
        }

        // ---------- BGM ----------
        var bgmObj = new GameObject("BGMSource");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;

        foreach (var data in soundDatas.BGMDatas)
        {
            if (data != null && data.clip != null)
            {
                bgmList.Add(data);
            }
        }

        // ---------- SE ----------
        sePoolRoot = new GameObject("SEPool").transform;
        sePoolRoot.SetParent(transform);

        foreach (var data in soundDatas.SEDatas)
        {
            if (data != null && data.clip != null)
            {
                seList.Add(data);
            }
        }

        // SE用AudioSourceをプール生成
        for (int i = 0; i < 16; i++)
        {
            var seObj = new GameObject($"SE_{i}");
            seObj.transform.SetParent(sePoolRoot);

            var source = seObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;

            seSources.Add(source);
        }

        // テスト再生（必要に応じて削除してください）
        //PlayBGM("PlayBGM", true);
    }
    /// <summary>
    /// 現在のマスター音量と個別の音量設定に基づいて、オーディオソースの音量を更新します。
    /// </summary>
    /// <remarks>このメソッドは、マスター音量または個別のバックグラウンドミュージック（BGM）
    /// またはサウンドエフェクト（SE）の音量レベルが変更されたときに呼び出され、
    /// すべてのオーディオソースが最新の設定を反映するようにします。</remarks>
    private void Update()
    {
        bgmSource.volume = masterVolume * bgmVolume;

        foreach (var source in seSources)
        {
            source.volume = masterVolume * seVolume;
        }
    }
    /// <summary>
    /// 指定された名前のサウンドエフェクトを再生します。
    /// </summary>
    /// <remarks>指定された名前のサウンドエフェクトが見つからない場合、サウンドは再生されません。</remarks>
    /// <param name="seName">再生するサウンドエフェクトの名前。サウンドエフェクトリストで利用可能なサウンドエフェクトに対応している必要があります。</param>
    public void PlaySE(string seName)
    {
        var data = seList.Find(x => x.name == seName);
        if (data == null) return;

        var source = GetFreeSESource();
        source.PlayOneShot(data.clip);
    }
    /// <summary>
    /// サウンドエフェクト再生に利用可能なオーディオソースを取得します。
    /// </summary>
    /// <remarks>このメソッドは、サウンドエフェクトの再生に使用されるオーディオソースの内部管理を目的としています。
    /// すべてのソースがビジー状態の場合、最初のソースを再利用すると、再生中のサウンドエフェクトが中断される可能性があります。</remarks>
    /// <returns>現在再生中でない<see cref="AudioSource"/>。すべてのソースが使用中の場合、
    /// コレクション内の最初のオーディオソースを返します。</returns>
    private AudioSource GetFreeSESource()
    {
        foreach (var source in seSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return seSources[0];
    }
    /// <summary>
    /// 指定されたバックグラウンドミュージック（BGM）トラックを再生し、オプションでループさせます。
    /// </summary>
    /// <remarks>指定されたBGMトラックが見つからない場合、音楽は再生されず、エラーがログに記録されます。
    /// 要求されたトラックが既に再生中の場合、このメソッドは何も実行しません。</remarks>
    /// <param name="bgmName">再生するバックグラウンドミュージックトラックの名前。BGMリスト内の有効なトラックに対応している必要があります。</param>
    /// <param name="loop">BGMを連続してループするかどうかを示す値。トラックをループする場合は<see langword="true"/>に設定し、
    /// それ以外の場合は<see langword="false"/>に設定します。</param>
    public void PlayBGM(string bgmName, bool loop = false)
    {
        var data = bgmList.Find(x => x.name == bgmName);
        if (data == null)
        {
            Debug.LogError($"指定されたBGMが見つかりません: {bgmName}");
            return;
        }

        if (bgmSource.clip == data.clip) return;

        bgmSource.clip = data.clip;
        bgmSource.loop = loop;
        bgmSource.volume = masterVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// 全音声の再生を終了
    /// </summary>
    public void StopAllSound()
    {
        bgmSource.Stop();
        foreach (var source in seSources)
        {
            source.Stop();
        }
    }
}


/*
*   外部からの関数の呼び出し方
*   
*   SEの場合（例）
*   SoundTester.cs内にサンプルがあります
*    SoundManager.Instance.PlaySE("Damage");
*                                   ↑
*                                音声の名前
*   BGMの場合（例）
*    SoundManager.Instance.PlayBGM("PlayBGM", true);
*                                     ↑       ↑
*                               音声の名前    ループフラグ
*                               
*                               
*   
*/