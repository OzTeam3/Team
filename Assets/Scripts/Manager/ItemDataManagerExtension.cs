using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 기존 데이터 드리븐 작업물에 ItemData 클래스가 존재해서 Temp로 임시 명명했습니다.
// TODO : 추후 수정 필요
public class ItemDataTemp
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MaterialPath { get; set; }
    public List<EffectPayload> EffectList = new List<EffectPayload>();
    public void InitData(ItemMasterData masterData)
    {
        ID = masterData.Id;
        Name = masterData.Name;
        Description = masterData.Description;
        MaterialPath = masterData.MaterialPath;
    }
}

public static class ItemDataManagerExtension
{
    private static Dictionary<string, ItemDataTemp> _itemList = new Dictionary<string, ItemDataTemp>();
    public static List<ItemDataTemp> GetAllItemData(this DataManager dataManager)
    {
        return _itemList.Values.ToList();
    }
    public static ItemDataTemp GetItemData(this DataManager dataManager, string itemId)
    {
        if (_itemList.ContainsKey(itemId) == false)
        {
            Debug.LogWarning($"{itemId} is not Contained To itemList");
            return null;
        }
        return _itemList[itemId];
    }
    public static void LoadItemData(this DataManager dataManager)
    {
        if (_itemList.Count > 0)
        {
            return;
        }
        InitMasterData(dataManager);
        foreach (var effectDTO in dataManager.GetAllData<ItemEffectDTO>())
        {
            if (_itemList.ContainsKey(effectDTO.ItemID) == false)
            {
                Debug.LogWarning($"{effectDTO.ItemID} Is not in ItemMaster Database");
                continue;
            }
            _itemList[effectDTO.ItemID].EffectList.Add(GetPayLoad(effectDTO));
        }
    }
    private static EffectPayload GetPayLoad(ItemEffectDTO effectData)
    {
        EffectPayload payload = new EffectPayload();
        EffectType effectType;
        if (Enum.TryParse<EffectType>(effectData.EffectType, out effectType) == false)
        {
            effectType = EffectType.None;
        }
        payload._effectType = effectType;
        payload._stringValues = effectData.StringValues;
        payload._values = effectData.FloatValues;
        return payload;
    }
    private static void InitMasterData(this DataManager dataManager)
    {
        List<ItemMasterData> masterData = dataManager.GetAllData<ItemMasterData>();
        foreach (var item in masterData)
        {
            if (_itemList.ContainsKey(item.Id))
            {
                continue;
            }
            ItemDataTemp itemData = new ItemDataTemp();
            itemData.InitData(item);
            _itemList[itemData.ID] = itemData;
        }
    }
}

