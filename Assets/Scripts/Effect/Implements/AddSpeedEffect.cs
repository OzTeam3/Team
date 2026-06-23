using UnityEngine;

public class AddSpeedEffect : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload)
    {
        PlayerView playerCharacter = null;
        // 얘를 이제 매니저를 통해 받든 어떻게 하든 할 예정.
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
