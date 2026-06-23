using System;
using System.Collections.Generic;

public class AddBuffEffect : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload)
    {
        PlayerView playerCharacter = null;
        float duration = payload._values[1];
        string EffectTypeString = payload._stringValues[0];
        if (Enum.TryParse<EffectType>(EffectTypeString, true, out var effectType) == false)
        {
            return;
        }

        BuffBase buff = new BuffBase();

        //예를 들어 이속증가 버프를 부여한다 치면...
        // EffectType : AddBuff
        // String : AddSpeed
        // Value : 10:10
        // 이렇게 적으면 AddSpeed(10)을 10초 동안 적용하는 버프가 된다.

        // 점프력증가 버프는
        // EffectType : AddBuff
        // String : AddJump
        // Value : 10:10

        List<EffectPayload> effects = new List<EffectPayload>();
        EffectPayload effect = new EffectPayload
        {
            _effectType = effectType,
            _values = payload._values
        };
        effects.Add(effect);
        buff.InitBuff(effects, duration);
        //playerCharacter.addBuff(buff)
    }
    public void RemoveEffect(EffectPayload payload) { }
}
