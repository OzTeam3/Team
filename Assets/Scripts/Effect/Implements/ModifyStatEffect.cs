public class ModifyStatEffect : IEffectLogic
{
    private StatType _statType;
    public ModifyStatEffect(StatType statType)
    {
        _statType = statType;
    }
    public void ApplyEffect(EffectPayload payload)
    {
        // 스탯 관련한거는... 일단 보류.
        // 후에 플레이어 쪽에 StatEnum타입으로 받아서 수정하는 걸 만들면
        // AddSpeed, JumpForce같은 이펙트도 이걸로 통합 가능할 듯...

        // TODO : 플레이어에 대한 부분 추후 GameManager 연동 후 등록
        PlayerView playerCharacter = null;
        float amount = payload._values[0];
        //playerCharacter.AddStat(statType, amount);
    }
    public void RemoveEffect(EffectPayload payload) { }
}
