// モンスターの行動を制御する純粋C#
using UnityEngine;

public class Move
{
    // モンスターが実際に使うときの技データ
    // 使いやすいようにするためPPも持つ

    // Monster.csが参照するのでpublicにする
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    // コンストラクタ
    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        PP = moveBase.MaxPP;
    }
}
