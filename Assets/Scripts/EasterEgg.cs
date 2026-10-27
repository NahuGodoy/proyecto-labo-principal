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
    [SerializeField] private bool modoExhibicion;
    [SerializeField] private Sprite iconoNotificacion;

    private Renderer[] renderers;
    private Collider[] colliders;

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

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CollectibleChanged += HandleCollectibleChanged;
        }
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CollectibleChanged -= HandleCollectibleChanged;
        }
    }

    private void Start()
    {
        ApplyVisibility(FueRecogido());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (modoExhibicion ||
            other.GetComponentInParent<PlayerInventory>() == null)
        {
            return;
        }

        SaveManager manager = SaveManager.Instance;
        if (manager == null || manager.ActiveSave == null)
        {
            return;
        }

        manager.SetCollectible(SaveId, true, iconoNotificacion);
    }

    private void HandleCollectibleChanged(string id, bool recogido)
    {
        if (id == SaveId)
        {
            ApplyVisibility(recogido);
        }
    }

    private void ApplyVisibility(bool recogido)
    {
        bool visible = modoExhibicion ? recogido : !recogido;

        foreach (Renderer item in renderers)
        {
            item.enabled = visible;
        }

        foreach (Collider item in colliders)
        {
            item.enabled = visible && !modoExhibicion;
        }
    }

    private bool FueRecogido()
    {
        SaveManager manager = SaveManager.Instance;

        return manager != null &&
               manager.ActiveSave != null &&
               manager.ActiveSave.HasCollectible(SaveId);
    }
}