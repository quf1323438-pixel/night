using UnityEngine;
public enum InputKey { Jump, Attack, Dash }
public interface IPlayerInput_ 
{
    public float MoveInput {get;} //플레이어 이동 감지
    bool RunHeld { get;}             //달리기 키를 누르고 있는가
    bool JumpHeld { get;}            //점프 키를 누르고 있는가 (가변점프용)

    void Tick(); //버퍼갱신 
    void Consume(InputKey action);      //사용한 입력 지우기 (중복 발동 방지)
    public bool IsBuffered(InputKey action);
}
