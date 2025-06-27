using UnityEngine;

public class DrillCollider : MonoBehaviour
{
    private int healthyLayer;
    private int infectedLayer;

    public void SetLayers(int healthyLayer, int infectedLayer)
    {
        this.healthyLayer = healthyLayer;
        this.infectedLayer = infectedLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != healthyLayer && other.gameObject.layer != infectedLayer) return;

        DrillController.instance.PartCollided(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != healthyLayer && other.gameObject.layer != infectedLayer) return;

        DrillController.instance.PartExited(other);
    }
}
