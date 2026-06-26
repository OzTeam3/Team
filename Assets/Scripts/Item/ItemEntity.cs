using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class ItemEntity : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    private ItemBase _item;

    private void Awake()
    {

    }
    private void Start()
    {
        // ItemEntity.InitItem(ItemId)
        // 이렇게 하면 해당 아이디의 아이템으로 자동으로 초기화 됩니다.

        //DelayTest().Forget();
    }
    private void FixedUpdate()
    {
        transform.Rotate(0, 60 * Time.fixedDeltaTime, 0);
    }
    public void SetMesh(Mesh mesh)
    {
        _meshFilter.mesh = mesh;
    }
    public async void InitItem(ItemBase item)
    {
        _item = item;
        Mesh itemMesh = await ResourceManager.Instance.GetAssetAsync<Mesh>(item.MeshId);
        SetMesh(itemMesh);
    }
    public async void InitItem(string itemId)
    {
        ItemData itemData = DataManager.Instance.GetData<ItemData>(itemId);
        if(itemData == null)
        {
            Debug.LogWarning($"[ItemEntity] Can't find {itemId} in DataManager");
            return;
        }
        ItemType itemtype;
        ItemBase item = null;
        bool isVariableType = Enum.TryParse<ItemType>(itemData.ItemType, out itemtype);
        if(!isVariableType)
        {
            itemtype = ItemType.None;
        }
        switch(itemtype)
        {
            case ItemType.StatUp:
                item = new StatUpItem();
                break;
        }
        if(item == null)
        {
            return;
        }
        item.InitItem(itemId);
        _item = item;
        Mesh itemMesh = await ResourceManager.Instance.GetAssetAsync<Mesh>(item.MeshId);
        SetMesh(itemMesh);
    }
    private async UniTask DelayTest()
    {
        await UniTask.Delay(3500);
        //StatUpItem statUpItem = new StatUpItem();
        //statUpItem.InitItem("Item_HastePotion");
        InitItem("Item_HastePotion");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == false)
        {
            return;
        }
        if(other.transform.TryGetComponent<PlayerView>(out var player) == false)
        {
            Debug.LogWarning("PlayerTag Object does not have a PlayerView componenet");
            return;
        }
        AddItem(player);
    }
    private void AddItem(PlayerView character)
    {
        _item.UseItem(character);
        //character.AddItem(_item);
        Destroy(this.gameObject);
    }
}
