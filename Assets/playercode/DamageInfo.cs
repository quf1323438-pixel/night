using UnityEngine;

// 데미지 정보를 담아 나르는 "봉투". 로직 없음, 값만 들어있음.
public readonly struct DamageInfo
{
    public int Amount { get; }        // 데미지 양
    public Vector2 From { get; }      // 공격이 날아온 위치
    public bool Unblockable { get; }  // 가드로 못 막는 공격인가
    public float Knockback { get; }   // 막았을 때 밀려나는 세기

    public DamageInfo(int amount, Vector2 from, bool unblockable = false, float knockback = 3f)
    {
        Amount      = amount;
        From        = from;
        Unblockable = unblockable;
        Knockback   = knockback;
    }
}