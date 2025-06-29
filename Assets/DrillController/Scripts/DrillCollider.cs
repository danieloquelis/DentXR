using UnityEngine;

namespace DrillSystem
{
    public class DrillCollider : MonoBehaviour
    {
        private int healthyLayer;
        private int infectedLayer;
        private int nerveLayer;

        public void SetLayers(int healthyLayer, int infectedLayer, int nerveLayer)
        {
            this.healthyLayer = healthyLayer;
            this.infectedLayer = infectedLayer;
            this.nerveLayer = nerveLayer;
        }

        private void OnTriggerEnter(Collider other)
        {
            int layer = other.gameObject.layer;
            if (layer != healthyLayer && layer != infectedLayer && layer != nerveLayer) return;

            DrillController.instance.PartCollided(other);
        }

        private void OnTriggerExit(Collider other)
        {
            int layer = other.gameObject.layer;
            if (layer != healthyLayer && layer != infectedLayer && layer != nerveLayer) return;

            DrillController.instance.PartExited(other);
        }
    }
}