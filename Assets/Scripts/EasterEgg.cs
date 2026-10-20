using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    public enum CollectibleId
    {
        Vik,
        Nahu,
        Enzo,
        Vicu,
        Leo,
        Juan
    }

    [SerializeField] private CollectibleId collectibleId;

    private string SaveId => collectibleId switch
    {
        CollectibleId.Vik => "vik",
        CollectibleId.Nahu => "nahu",
        CollectibleId.Enzo => "enzo",
        CollectibleId.Vicu => "vicu",
        CollectibleId.Leo => "leo",
        CollectibleId.Juan => "juan",
        _ => string.Empty
    };

    private void Start()
    {
        SaveManager manager = SaveManager.Instance;

        if (manager != null &&
            manager.ActiveSave != null &&
            manager.ActiveSave.HasCollectible(SaveId))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerInventory>() == null)
        {
            return;
        }

        SaveManager manager = SaveManager.Instance;
        if (manager == null || manager.ActiveSave == null)
        {
            return;
        }

        manager.SetCollectible(SaveId);
        gameObject.SetActive(false);
    }
}