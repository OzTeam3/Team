using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemData
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
    private static Dictionary<string, ItemData> _itemList = new Dictionary<string, ItemData>();
    public static List<ItemData> GetAllItemData(this DataManager dataManager)
    {
        return _itemList.Values.ToList();
    }
    public static ItemData GetItemData(this DataManager dataManager, string itemId)
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
        foreach (var effectDTO in dataManager.GetAllData<ItemEffectData>())
        {
            if (_itemList.ContainsKey(effectDTO.ItemID) == false)
            {
                Debug.LogWarning($"{effectDTO.ItemID} Is not in ItemMaster Database");
                continue;
            }
            _itemList[effectDTO.ItemID].EffectList.Add(GetPayLoad(effectDTO));
        }
    }
    private static EffectPayload GetPayLoad(ItemEffectData effectData)
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
            ItemData itemData = new ItemData();
            itemData.InitData(item);
            _itemList[itemData.ID] = itemData;
        }
    }
}

