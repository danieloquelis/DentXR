using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrillController : MonoBehaviour
{
    public static DrillController instance;

    [SerializeField]
    private int healthyLayer;

    [SerializeField]
    private int infectedLayer;

    [Space(10)]

    [SerializeField]
    [Tooltip("How long the drill has to be drilling a part to remove it")]
    private float timeThreshold;

    [Space(10)]

    [SerializeField]
    private bool automaticStart;

    private DrillCollider drillCollider;

    private bool isActive = false;

    private WaitForSeconds waitThreshold;

    /// <summary>
    /// Triggered when a part has been drilled and says if it's healthy or infected part
    /// </summary>
    public static UnityAction<bool> OnPartDrilled
    {
        get
        {
            return instance._OnPartDrilled;
        }

        set
        {
            instance._OnPartDrilled = value;
        }
    }

    private UnityAction<bool> _OnPartDrilled;

    private List<GameObject> PartsList = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    
    void Start()
    {
        drillCollider = GetComponentInChildren<DrillCollider>();
        drillCollider.SetLayers(healthyLayer, infectedLayer);

        waitThreshold = new WaitForSeconds(timeThreshold);

        if (automaticStart)
            StartDrill();
    }

    /// <summary>
    /// Starts the drill system
    /// </summary>
    public static void StartDrill()
    {
        instance._StartDrill();
    }

    private void _StartDrill()
    {
        isActive = true;
    }

    /// <summary>
    /// Stops the drill system
    /// </summary>
    public static void StopDrill()
    {
        instance._StopDrill();
    }

    private void _StopDrill()
    {
        isActive = false;
    }

    public void HealthyCollider(GameObject other)
    {
        if (!isActive) return;

        other.gameObject.SetActive(false);
        _OnPartDrilled?.Invoke(true);
    }

    public void InfectedCollider(GameObject other)
    {
        if (!isActive) return;

        other.gameObject.SetActive(false);
        _OnPartDrilled?.Invoke(false);
    }

    public void PartCollided(Collider other)
    {
        if (PartsList.Contains(other.gameObject)) return;

        PartsList.Add(other.gameObject);
        
        StartCoroutine(CheckPartThreshold(other.gameObject));
    }

    public void PartExited(Collider other)
    {
        if (PartsList.Contains(other.gameObject))
            PartsList.Remove(other.gameObject);
    }

    private IEnumerator CheckPartThreshold(GameObject part)
    {
        yield return waitThreshold;

        if (PartsList.Contains(part))
        {
            if(part.layer == healthyLayer)
            {
                HealthyCollider(part);
            }
            else
            {
                InfectedCollider(part);
            }
        }
    }
}
