using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //컴포넌트 관리
    public Rigidbody2D rb;
    public BoxCollider2D col;
    public IPlayerInput_ input_;

    //수평 이동
    public float playerSpeed;
    public float playerSpeed_Standard;
    public float playerSpeed_Jump;


    //점프
    public float jumpforce; //점프 높이
    public float gravity_down; // 추락할 때 중력값
    public float gravity_cut; // 위로 올라갈 때  중력값
    public float gravity_up; //평소상태의 중력값
    public float gravity_apex;    // 정점 부근 중력 (작게)
    public float apexThreshold;   // 정점으로 칠 속도 범위

    //구르기
    public float rolltimer; // 구르는 시간
    public float rolltimer_Reset; // 구르는 시간_초기화
    public float rolltimer_stop; // 구르기 딜레이
    public bool roll_performable = true;
    public float rollspeed; // 구르는 속도

    //대쉬
    public float dashtimer; // 구르는 시간
    public float dashtimer_Reset; // 구르는 시간_초기화
    public float dashtimer_stop; // 구르기 딜레이
    public bool dash_performable = true;
    public float dashspeed; // 구르는 속도

    //바닥 감지
    public Vector2 boxsize;
    public LayerMask block;
    public bool isGround;

    //레이어 관리
    public int layer_default;
    public int layer_ghost;

    //방패
    public float player_dir;
    public float shield_dir;
    public int hp = 100;
    public float stamina = 50f; //방패가 버틸 수 있는 내구도
    public float guardStartTime = -999f; //가드를 올린 시각
    public const float ParryWindow = 0.15f;  
    //방향 전환 가능 여부 (각 상태가 제어)
    public bool canTurn = true;

    //상태머신
    public IPlayer_ currentState;

    void Awake() //게임 시작 전 초기화
    {
        // 컴포넌트
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        input_ = new PlayerInput_();
        currentState = new Player_idel();

        //변수 초기화
        boxsize = col.bounds.size;

        //레이어 초기화
        layer_default = gameObject.layer;
        layer_ghost   = LayerMask.NameToLayer("Ghost");   // 인스펙터에서 만든 레이어 이름  
    }

    void Update()
    {
        //cast
        isGround = Physics2D.BoxCast(transform.position, boxsize, 0f, Vector2.down, 0.1f, block);
        //update
        input_.Tick();
        currentState.Update(this);

        if (currentState == new Player_Shield()) return;

        //dir
        if (input_.MoveInput != 0) player_dir = input_.MoveInput;

        //방향 오브젝트 표시
        if (input_.MoveInput != 0 && canTurn)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * player_dir;
            transform.localScale = s;
        }
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate(this);
    }

    public void changeState(IPlayer_ next)
    {
        currentState.Exit(this);
        currentState = next;
        currentState.Enter(this);
    }

    public void TakeDamage(DamageInfo Dmg)
    {
        // 공격자가 내 왼쪽이면 -1, 오른쪽이면 +1
        float attackSide = Mathf.Sign(Dmg.From.x - transform.position.x);

        // 내가 그쪽을 보고 있으면 정면
        bool fromFront = Mathf.Approximately(attackSide, Mathf.Sign(shield_dir));

        // 못 막는 경우 → 그냥 맞음
        if (Dmg.Unblockable || !input_.ShieldHeld || !fromFront)
        {
            hp -= Dmg.Amount;
            Debug.Log($"피격!  hp = {hp}");
            if (hp <= 0) Die();
            return;
        }

        // 막음
        stamina -= Dmg.Amount;
        rb.linearVelocity = new Vector2(-attackSide * Dmg.Knockback, rb.linearVelocity.y);
        Debug.Log($"가드!  stamina = {stamina}");

        if (stamina <= 0) GuardBreak();
    }

    private void GuardBreak()
    {
        Debug.Log("가드 브레이크!");
        stamina = 0f;
        // 나중에: 경직 상태로 전환
    }

    private void Die()
    {
        Debug.Log("사망");
    }
}
