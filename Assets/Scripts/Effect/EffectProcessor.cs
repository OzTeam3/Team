using System.Collections.Generic;

public static class EffectProcessor
{
    private static Dictionary<EffectType, IEffectLogic> _effectLogicList = new Dictionary<EffectType, IEffectLogic>()
    {
        { EffectType.None, new AddSpeedEffect() },
        { EffectType.AddSpeed, new AddSpeedEffect() }
    };
    public static void ApplyEffect(EffectPayload effect)
    {
        _effectLogicList[effect._effectType].ApplyEffect(effect);
    }
    public static void RemoveEffect(EffectPayload effect)
    {
        _effectLogicList[effect._effectType].RemoveEffect(effect);
    }
}