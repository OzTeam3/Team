using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

//안쓰는 유징 정리해주세요
//에러문
public class ItemEntity : MonoBehaviour
{
    [SerializeField] private string _itemDataId;

    private ItemBase _item;

    private SphereCollider _sphereCollider;
    private MeshFilter _meshFilter;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _meshFilter = GetComponentInChildren<MeshFilter>();
        if(_sphereCollider == null)
        {
            Debug.LogWarning($"[ItemEntity:Awake] SphereCollider 컴포넌트 없음");
        }
        if(_meshFilter == null)
        {
            Debug.LogWarning($"[ItemEntity:Awake] MeshFilter 컴포넌트 없음");
        }
    }
    private void OnEnable()
    {
        InitItemEntity().Forget();
    }

    private void Update()
    {
        RotateEntity();
    }

    private void RotateEntity()
    {
        transform.Rotate(0, 60 * Time.fixedDeltaTime, 0);
    }

    public async UniTask InitItemEntity()
    {
        ItemData itemData = DataManager.Instance.GetData<ItemData>(_itemDataId);
        if(itemData == null)
        {
            Debug.LogWarning($"[ItemEntity:InitItemEntity] 데이터 매니저에서 아이템ID 찾을 수 없음");
            return;
        }

        //위로 가도될듯
        Mesh itemMesh = await ResourceManager.Instance.GetAssetAsync<Mesh>(itemData.MeshId);
        _meshFilter.mesh = itemMesh;

        bool isItemParsed = Enum.TryParse(itemData.ItemType, out ItemType itemtype);
        if (!isItemParsed)
        {
            Debug.LogWarning($"[ItemEntity:InitItemEntity] 아이템 타입 파싱 실패");
            return;
        }

        switch (itemtype)
        {
            case ItemType.StatUp:
                _item = new StatUpItem();
                break;
            default:
                return;
        }
        _item.InitItem(_itemDataId);
    }

    private async UniTask RespawnItemDelay()
    {
        _sphereCollider.enabled = false;
        _meshFilter.gameObject.SetActive(false);
        bool isCancel = await UniTask.Delay(TimeSpan.FromSeconds(4f)).SuppressCancellationThrow();
        if(isCancel)
        {
            Debug.LogWarning("[ItemEntity:RespawnItemDelay] 비동기 처리 중 관련 오브젝트 파괴 됨");
            return;
        }
        _sphereCollider.enabled = true;
        _meshFilter.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == false)
        {
            return;
        }

        if(other.transform.TryGetComponent(out Player player) == false)
        {
            Debug.LogWarning("[ItemEntity:OnTriggerEnter] 플레이어 태그 오브젝트에 Player컴포넌트 없음");
            return;
        }

        UseItem(player);
    }

    private void UseItem(Player player)
    {
        _item.UseItem(player);
        RespawnItemDelay().Forget();
    }
}
