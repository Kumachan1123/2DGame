/*
*   SoundDatas.cs
*   音声のNameとAudioClipを保存するScriptableObject
*/
using UnityEngine;
[CreateAssetMenu(fileName = "SoundDatas", menuName = "SoundDatas")]
public class SoundDatas : ScriptableObject
{
    // サウンドデータの配列
    [Header("BGM用サウンドデータ")]
    public SoundData[] BGMDatas;
    [Header("SE用サウンドデータ")]
    public SoundData[] SEDatas;


}
