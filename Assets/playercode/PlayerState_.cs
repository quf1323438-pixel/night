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
    }

    public void FixedUpdate(Player Player)
    {
        //플레이어 수평이동
        Player.rb.linearVelocity = new Vector2(Player.playerSpeed * Player.input_.MoveInput, 
        Player.rb.linearVelocity.y);

        // 가변 점프
        if (Player.rb.linearVelocity.y < 0f) Player.rb.gravityScale = Player.gravity_down;
        else if (Player.rb.linearVelocity.y > 0f && !Player.input_.JumpHeld) Player.rb.gravityScale = Player.gravity_cut;
        else Player.rb.gravityScale = Player.gravity_up;
    }

    public void Exit(Player Player) {}
}
