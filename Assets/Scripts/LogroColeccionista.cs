using System.Collections;
using UnityEngine;

public class LogroColeccionista : MonoBehaviour
{
private const int CantidadMeta = 6;

    [SerializeField] private float delayDesbloqueo = 2.5f;

    private Coroutine corrutinaDesbloqueo;

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CollectibleChanged += OnCollectibleChanged;
        }
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CollectibleChanged -= OnCollectibleChanged;
        }

        if (corrutinaDesbloqueo != null)
        {
            StopCoroutine(corrutinaDesbloqueo);
            corrutinaDesbloqueo = null;
        }
    }

    private void Start()
    {
        ComprobarLogro(conDelay: false);
    }

    private void OnCollectibleChanged(string id, bool recogido)
    {
        if (recogido)
        {
            ComprobarLogro(conDelay: true);
        }
    }

    private void ComprobarLogro(bool conDelay)
    {
        if (SaveManager.Instance == null || SaveManager.Instance.ActiveSave == null)
            return;

        int recolectados = SaveManager.Instance.ActiveSave.ContarEasterEggs();

        if (recolectados == CantidadMeta)
        {
            if (AchievementSystem.Instance != null &&
                AchievementSystem.Instance.IsUnlocked(AchievementSystem.TodosLosColeccionablesId))
            {
                return;
            }

            if (conDelay)
            {
                if (corrutinaDesbloqueo == null)
                {
                    corrutinaDesbloqueo = StartCoroutine(DesbloquearConEspera());
                }
            }
            else
            {
                DesbloquearInmediato();
            }
        }
    }

    private IEnumerator DesbloquearConEspera()
    {

        yield return new WaitForSecondsRealtime(delayDesbloqueo);

        DesbloquearInmediato();
        corrutinaDesbloqueo = null;
    }

    private void DesbloquearInmediato()
    {
        AchievementSystem.Instance?.Unlock(AchievementSystem.TodosLosColeccionablesId);
    }
}
