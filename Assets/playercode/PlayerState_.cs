using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player_idel : IPlayer_
{
    public void Enter(Player Player) {}

    public void Update(Player Player)
    {
        //플레이어 점프
        if (Player.input_.IsBuffered(InputKey.Jump) && Player.isGround)
        {
            Player.input_.Consume(InputKey.Jump);
            Player.rb.linearVelocity = new Vector2(Player.rb.linearVelocity.x, Player.jumpforce);
        }

        //공중에서 플레이어 속도 조절
        if (!Player.isGround) Player.playerSpeed = Player.playerSpeed_Jump;
        else Player.playerSpeed = Player.playerSpeed_Standard;      

        if (Player.input_.IsBuffered(InputKey.Roll) && Player.isGround)
        {
            Player.input_.Consume(InputKey.Roll);
            Player.changeState(new Player_roll());
        }

        if (Player.input_.IsBuffered(InputKey.Dash) && Player.isGround)
        {
            Player.input_.Consume(InputKey.Dash);
            Player.changeState(new Player_dash());
        }

        //방패 들기
        if (Player.input_.ShieldHeld && Player.isGround)
        {
            Player.changeState(new Player_Shield());
            return;
        }
    }

    public void FixedUpdate(Player Player)
    {
        //플레이어 수평이동
        Player.rb.linearVelocity = new Vector2(Player.playerSpeed * Player.input_.MoveInput, 
        Player.rb.linearVelocity.y);

        // 가변 점프
        if (!Player.isGround && Mathf.Abs(Player.rb.linearVelocity.y) < Player.apexThreshold)
            Player.rb.gravityScale = Player.gravity_apex;
        else if (Player.rb.linearVelocity.y < 0f) Player.rb.gravityScale = Player.gravity_down;
        else if (Player.rb.linearVelocity.y > 0f && !Player.input_.JumpHeld) Player.rb.gravityScale = Player.gravity_cut;
        else Player.rb.gravityScale = Player.gravity_up;
    }

    public void Exit(Player Player) {}
}

public class Player_roll : IPlayer_
{
    float rolldir;
    bool recovering;
    public void Enter(Player Player)
    {
        //지속시간 초기화
        Player.rolltimer = Player.rolltimer_Reset;

        //이전 속도 초기화
        Player.rb.linearVelocity = Vector2.zero;

        //구르기 방향 설정
        rolldir = Player.player_dir;

        //변수 초기화
        recovering = false;

        //레이어 변경
        Player.gameObject.layer = Player.layer_ghost;
        Debug.Log("roll");
    }

    public void Update(Player Player)
    {
        //타이머 시작
        Player.rolltimer -= Time.deltaTime;
        if (Player.rolltimer > 0f) return;

        if (!recovering)
        {
            recovering = true;
            Player.rolltimer = Player.rolltimer_stop;
            Player.gameObject.layer = Player.layer_default;
        } 
        else
        {
            Player.changeState(new Player_idel());
        }
    }

    public void FixedUpdate(Player Player)
    {   
        float vx = recovering ? 0f : Player.rollspeed * rolldir;

        Player.rb.linearVelocity = new Vector2(vx, Player.rb.linearVelocity.y);
    }

    public void Exit(Player Player) 
    {
        Player.rb.linearVelocity = Vector2.zero;
    }
}

public class Player_dash : IPlayer_
{
    float dashdir;
    bool recovering;
    public void Enter(Player Player)
    {
        //지속시간 초기화
        Player.dashtimer = Player.dashtimer_Reset;

        //이전 속도 초기화
        Player.rb.linearVelocity = Vector2.zero;

        //구르기 방향 설정
        dashdir = Player.player_dir;

        //변수 초기화
        recovering = false;

        //레이어 변경
        Player.gameObject.layer = Player.layer_ghost;
    }

    public void Update(Player Player)
    {
        //타이머 시작
        Player.dashtimer -= Time.deltaTime;
        if (Player.dashtimer > 0f) return;

        if (!recovering)
        {
            recovering = true;
            Player.dashtimer = Player.dashtimer_stop;
            Player.gameObject.layer = Player.layer_default;
        } 
        else
        {
            Player.changeState(new Player_idel());
        }
    }

    public void FixedUpdate(Player Player)
    {   
        float vx = recovering ? 0f : Player.dashspeed * dashdir;

        Player.rb.linearVelocity = new Vector2(vx, Player.rb.linearVelocity.y);
    }

    public void Exit(Player Player) 
    {
        Player.rb.linearVelocity = Vector2.zero;
    }
}

public class Player_Shield : IPlayer_
{
    public void Enter(Player Player)
    {
        Player.canTurn = false;      // ← 방향 고정
        Player.rb.linearVelocity = new Vector2(0f, Player.rb.linearVelocity.y);
        Player.shield_dir = Player.player_dir;
        Debug.Log("방패 가동중!");
    }

    public void Update(Player Player)
    {

        //퇴장 조건 
        if (!Player.input_.ShieldHeld)
        {
            Player.changeState(new Player_idel());
            return;
        }
        //구르면
        if (Player.input_.IsBuffered(InputKey.Roll) && Player.isGround)
        {
            Player.input_.Consume(InputKey.Roll);
            Player.changeState(new Player_roll());
        }
    }

    public void FixedUpdate(Player Player) 
    {
        Player.rb.linearVelocity = new Vector2(0f, Player.rb.linearVelocity.y);
    }

    public void Exit(Player Player) { Player.canTurn = true;}
}
