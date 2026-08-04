using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput_ : IPlayerInput_
{
    private const int BufferFrames = 6;   //쪽지 유효기간 = 6프레임 (약 0.1초)
    private Dictionary<InputKey, int> buffer = new Dictionary<InputKey, int>();

    public float MoveInput {get; private set;}           //플레이어 이동 감지
    public bool RunHeld { get; private set;}             //달리기 키를 누르고 있는가
    public bool JumpHeld { get; private set;}            //점프 키를 누르고 있는가 (가변점프용)
    public bool ShieldHeld {get; private set;}           //방패 키를 누르고 있는가
    

    public void Tick() //update와 동일한 존재
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        //플레이어 이동 입력
        MoveInput = 0f;
        if (kb.dKey.isPressed) MoveInput = 1f;
        else if (kb.aKey.isPressed) MoveInput = -1f;
        
        if (kb.kKey.wasPressedThisFrame) buffer[InputKey.Jump] = BufferFrames;
        else if (kb.jKey.wasPressedThisFrame) buffer[InputKey.Attack] = BufferFrames;
        else if (kb.lKey.wasPressedThisFrame) buffer[InputKey.Dash] = BufferFrames;
        else if (kb.shiftKey.wasPressedThisFrame) buffer[InputKey.Roll] = BufferFrames;


        RunHeld = kb.shiftKey.isPressed;
        JumpHeld = kb.kKey.isPressed;
        ShieldHeld = kb.iKey.isPressed;


        foreach (var key in new List<InputKey>(buffer.Keys))
        {
            if (--buffer[key] <= 0) buffer.Remove(key);
        }

    }

    public void Consume(InputKey action) => buffer.Remove(action);
    public bool IsBuffered(InputKey action) => buffer.ContainsKey(action);
}
