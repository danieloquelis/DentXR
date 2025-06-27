using UnityEngine;
using LibCSG;
using System.Collections;
using static UnityEngine.InputSystem.InputAction;

public class DrillController : MonoBehaviour
{
    public static DrillController instance;

    [SerializeField]
    [Tooltip("The tooth to drill")]
    private GameObject currentTooth;

    [SerializeField]
    [Tooltip("Prefab of result")]
    private GameObject resultPrefab;

    [Space(10)]

    [SerializeField]
    [Tooltip("The max distance to the points before the system breaks")]
    private float distanceThreshold;

    [SerializeField]
    [Tooltip("The time to hold the drill in the same point to drill")]
    private float timeThreshold;

    private DrillCollider drillCollider;

    private CSGBrushOperation CSGOp = new CSGBrushOperation();
    private CSGBrush resultBrush;
    private CSGBrush drillColliderBrush;
    private CSGBrush targetToothBrush;

    private Transform currentResultTransform;

    private MeshFilter resultMeshFilter;
    private MeshFilter currentResultMeshFilter;

    private MeshCollider currentResultMeshCollider;

    private bool isActive = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Start the brushes for the drill collider
        drillColliderBrush = new CSGBrush(drillCollider.gameObject);
        drillColliderBrush.build_from_mesh(drillCollider.GetComponent<MeshFilter>().mesh);
    }

#if UNITY_EDITOR

    public void SpaceButtonPressed(CallbackContext context)
    {
        if (!context.performed) return;

        SetCurrentTooth(currentTooth);
        StartDrill();
    }

#endif

    /// <summary>
    /// Changes the tooth to be drilled
    /// </summary>
    /// <param name="currentTooth"> The new current tooth </param>
    public static void SetCurrentTooth(GameObject currentTooth)
    {
        instance._SetCurrentTooth(currentTooth);
    }

    private void _SetCurrentTooth(GameObject currentTooth)
    {
        this.currentTooth = currentTooth;

        // Create a result mesh and place it where the target tooth is
        currentResultTransform = Instantiate(currentTooth).transform;
        currentResultTransform.position = currentTooth.transform.position;
        currentResultTransform.rotation = currentTooth.transform.rotation;

        // Setting the collider to collide with the current result only
        drillCollider.SetTarget(currentResultTransform.gameObject);

        currentTooth.SetActive(false);

        resultMeshFilter = currentResultTransform.GetComponent<MeshFilter>();
        

        // Create the new brushes and operations for the new target
        CSGOp = new CSGBrushOperation();
        targetToothBrush = new CSGBrush(currentResultTransform.gameObject);

        currentResultMeshFilter = currentResultTransform.GetComponent<MeshFilter>();

        targetToothBrush.build_from_mesh(currentResultMeshFilter.mesh);

        resultBrush = new CSGBrush(currentResultTransform.gameObject);
        resultBrush.build_from_mesh(currentResultMeshFilter.mesh);

        currentResultMeshCollider = currentResultTransform.GetComponent<MeshCollider>();
    }

    /// <summary>
    /// Starts the drilling system
    /// </summary>
    public static void StartDrill()
    {
        instance._StartDrill();
    }

    public void _StartDrill()
    {
        isActive = true;
    }

    /// <summary>
    /// Stops the drilling system
    /// </summary>
    public static void StopDrill()
    {
        instance._StopDrill();
    }

    private void _StopDrill()
    {
        isActive = false;

        if (DrillSystem != null)
            StopCoroutine(DrillSystem);
    }

    public void SetCollider(DrillCollider collider)
    {
        drillCollider = collider;
    }

    /// <summary>
    /// Triggered when the drill collider triggered-in the target tooth
    /// </summary>
    /// <param name="other"> The target tooth </param>
    public void ColliderEntered(Collider other)
    {
        if (!isActive) return;

        StartCoroutine(DrillSystem = DrillSystemEnum());
    }

    /// <summary>
    /// Triggered when the drill collider has exited the target tooth collider space
    /// </summary>
    /// <param name="other"></param>
    public void ColliderExited(Collider other)
    {
        if (!isActive) return;

        StopCoroutine(DrillSystem);
    }

    private IEnumerator DrillSystem;
    private IEnumerator DrillSystemEnum()
    {
        Transform targetTransform = currentTooth.transform;
        Transform drillColliderTransform = drillCollider.transform;

        yield return new WaitForSeconds(3f);

        SubtractDrill();
    }

    /// <summary>
    /// Do the subtract operation of removing the drill sphere shape from the target tooth shape
    /// </summary>
    private void SubtractDrill()
    {
        // Do the operation subtration between the cube and the cylinder 
        CSGOp.merge_brushes(Operation.OPERATION_SUBTRACTION, targetToothBrush, drillColliderBrush, ref resultBrush);

        resultMeshFilter.mesh.Clear();

        // Put the mesh result in the mesh give in parameter if you don't give a mesh he return a new mesh with the result
        resultBrush.getMesh(resultMeshFilter.mesh);

        // Put the same mesh on the result so it loads again
        currentResultMeshCollider.sharedMesh = resultMeshFilter.sharedMesh;

        // Reset the brushes on the result
        targetToothBrush = new CSGBrush(currentResultTransform.gameObject);
        targetToothBrush.build_from_mesh(currentResultMeshFilter.mesh);

        resultBrush = new CSGBrush(currentResultTransform.gameObject);
        resultBrush.build_from_mesh(currentResultMeshFilter.mesh);
    }
}
