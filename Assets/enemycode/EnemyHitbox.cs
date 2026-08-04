using UnityEngine;

// 적의 공격 판정. 트리거 콜라이더가 달린 오브젝트에 붙임.
public class EnemyHitbox : MonoBehaviour
{
    public int damage = 10;
    public bool unblockable = false;   // 인스펙터에서 체크하면 가드 불가 공격

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null) return;    // 플레이어가 아니면 무시

        // ② 봉투를 만들어서  ③ 건넴
        player.TakeDamage(new DamageInfo(damage, transform.position, unblockable));
    }
}