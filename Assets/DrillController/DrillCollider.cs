using UnityEngine;

public class DrillCollider : MonoBehaviour
{
    private GameObject target;

    void Start()
    {
        DrillController.instance.SetCollider(this);
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != target) return;

        DrillController.instance.ColliderEntered(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != target) return;

        DrillController.instance.ColliderExited(other);
    }
}
