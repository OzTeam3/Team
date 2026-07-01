using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

//안쓰는 유징 정리해주세요
//에러문
public class ItemEntity : MonoBehaviour
{
    //매시 필터는 겟컴포넌트로 뺴기
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private string _itemDataId;

    private ItemBase _item;

    private void OnEnable()
    {
        InitItem();
    }

    //업데이트에서 해주세요
    private void FixedUpdate()
    {
        transform.Rotate(0, 60 * Time.fixedDeltaTime, 0);
    }

    //메서드명 수정
    public async UniTask InitItem()
    {
        ItemData itemData = DataManager.Instance.GetData<ItemData>(_itemDataId);
        if(itemData == null)
        {
            Debug.LogWarning($"[ItemEntity] Can't find {_itemDataId} in DataManager");
            return;
        }
       
        //변수명을 성공했는지?
        bool isVariableType = Enum.TryParse(itemData.ItemType, out ItemType itemtype);
       
        //에러체크
        if(!isVariableType)
        {
            itemtype = ItemType.None;
        }

        switch (itemtype)
        {
            case ItemType.StatUp:
                _item = new StatUpItem();
                break;
            default:
                return;
        }

        //_Item초기화 하는 메서드

        //위로 가도될듯
        Mesh itemMesh = await ResourceManager.Instance.GetAssetAsync<Mesh>(itemData.MeshId);
        _meshFilter.mesh = itemMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == false)
        {
            return;
        }

        if(other.transform.TryGetComponent(out Player player) == false)
        {
            Debug.LogWarning("PlayerTag Object does not have a PlayerView componenet");
            return;
        }

        //유즈 아이템
        AddItem(player);
    }

    //삭제될수도
    private void AddItem(Player character)
    {
        _item.UseItem(character);
        //character.AddItem(_item);
        Destroy(this.gameObject);
    }
}
