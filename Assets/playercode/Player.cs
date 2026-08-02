using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //컴포넌트 관리
    public Rigidbody2D rb;
    public BoxCollider2D col;
    public IPlayerInput_ input_;

    //수평 이동
    public float playerSpeed;

    //점프
    public float jumpforce; //점프 높이
    public float gravity_down; // 추락할 때 중력값
    public float gravity_cut; // 위로 올라갈 때  중력값
    public float gravity_up; //평소상태의 중력값

    //바닥 감지
    public Vector2 boxsize;
    public LayerMask block;
    public bool isGround;


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
    }

    void Start()
    {
        
    }

    void Update()
    {
        //update
        input_.Tick();
        currentState.Update(this);

        //cast
        isGround = Physics2D.BoxCast(transform.position, boxsize, 0f, Vector2.down, 0.1f, block);
        Debug.Log(isGround);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate(this);
    }
}
