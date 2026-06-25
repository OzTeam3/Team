using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemEntity : MonoBehaviour
{
    [SerializeField] private Material _material;

    private MeshRenderer _mesh;
    private ItemBase _item;

    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        DelayTest().Forget();
    }
    public void SetMaterial(Material material)
    {
        _mesh.material= material;
    }
    public void SetMaterial(string materialPath)
    {
        // Resource매니저를 통해서 메테리얼 설정.
    }
    public void InitItem(ItemBase item)
    {
        _item= item;
    }
    private async UniTask DelayTest()
    {
        await UniTask.Delay(4000);
        SetMaterial(_material);
        _item = new StatUpItem();
        _item.InitItem("Item_HastePotion");
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
