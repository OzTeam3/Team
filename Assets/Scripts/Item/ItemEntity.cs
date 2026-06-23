using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    private MeshRenderer _mesh;
    private ItemBase _item;

    // 데이터 드리븐으로 뺄 수 있으면...
    // DataManager에서 Item데이터를 갖고 있다. (무슨 효과인지, 얼만큼의 Value인지 Data클래스에 저장되어 있음)
    // ItemId를 키값으로 DataManager를 통해서 GetData가 가능하기는 한데...
    // 효과 자체도 사실... Data클래스 내부에서 구현되어있는게 아니라
    // Data는 효과의 타입/Value쌍만 가지고 있는건 어떨까.
    // 효과를 적용하는 효과처리하는 것들만 모아두는 클래스/다른 무언가를 만들거나.

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
        //character.AddItem(_item);
        Destroy(this.gameObject);
    }
}
