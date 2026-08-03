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
        //update
        input_.Tick();
        currentState.Update(this);

        //dir
        if (input_.MoveInput != 0) player_dir = input_.MoveInput;



        //cast
        isGround = Physics2D.BoxCast(transform.position, boxsize, 0f, Vector2.down, 0.1f, block);
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
}
