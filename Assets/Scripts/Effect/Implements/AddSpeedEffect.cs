using UnityEngine;

public class AddSpeedEffect : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload)
    {
        // TODO : 플레이어에 대한 부분 추후 GameManager 연동 후 등록
        PlayerView playerCharacter = null;
        float increasingSpeed = payload._values[0];

        playerCharacter.AddMoveSpeed(increasingSpeed);
    }
    public void RemoveEffect(EffectPayload payload)
    {
        PlayerView playerCharacter = null;
        float increasingSpeed = payload._values[0];
        playerCharacter.AddMoveSpeed(-increasingSpeed);
    }
}