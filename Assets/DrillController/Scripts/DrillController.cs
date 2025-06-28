using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DrillSystem
{
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
        [Tooltip("The particles to Play when drilling")]
        private ParticleSystem drillParticles;

        [Space(10)]

        [SerializeField]
        private bool automaticStart;

        private DrillCollider drillCollider;

        private bool isActive = false;
        private bool isDrilling = false;

        private WaitForSeconds waitThreshold;

        public enum PieceType {Healthy, Infected};

        /// <summary>
        /// Triggered when a part has been drilled and says if it's healthy or infected part
        /// </summary>
        public static UnityAction<PieceType, GameObject> OnPartDrilled
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

        private UnityAction<PieceType, GameObject> _OnPartDrilled;

        /// <summary>
        /// Triggered when the drilling system has started or ended
        /// </summary>
        public static UnityAction<bool> OnDrillingSystem
        {
            get
            {
                return instance._OnDrillingSystem;
            }

            set
            {
                instance._OnDrillingSystem = value;
            }
        }

        private UnityAction<bool> _OnDrillingSystem;

        /// <summary>
        /// Triggered when the drill is actually dilling the tooth
        /// </summary>
        public static UnityAction<bool> OnDrillingCollision
        {
            get
            {
                return instance._OnDrillingCollision;
            }

            set
            {
                instance._OnDrillingCollision = value;
            }
        }

        private UnityAction<bool> _OnDrillingCollision;

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
            _OnDrillingSystem?.Invoke(true);
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
            _OnDrillingSystem?.Invoke(false);
        }

        /// <summary>
        /// Triggered when drilling a healthy piece
        /// </summary>
        /// <param name="other"></param>
        public void HealthyCollider(GameObject other)
        {
            if (!isActive) return;

            other.gameObject.SetActive(false);
            _OnPartDrilled?.Invoke(PieceType.Healthy, other);
        }

        /// <summary>
        /// Triggered when drilling an infected piece
        /// </summary>
        /// <param name="other"></param>
        public void InfectedCollider(GameObject other)
        {
            if (!isActive) return;

            other.gameObject.SetActive(false);
            _OnPartDrilled?.Invoke(PieceType.Infected, other);
        }

        /// <summary>
        /// Triggered when the drill collider entered a new piece
        /// </summary>
        /// <param name="other"> The piece </param>
        public void PartCollided(Collider other)
        {
            if (PartsList.Contains(other.gameObject)) return;

            if (!isDrilling)
            {
                isDrilling = true;

                _OnDrillingCollision?.Invoke(true);

                if (drillParticles)
                    drillParticles.Play();
            }

            PartsList.Add(other.gameObject);

            StartCoroutine(CheckPartThreshold(other.gameObject));
        }

        /// <summary>
        /// Triggered when the drill collider exited a piece
        /// </summary>
        /// <param name="other"></param>
        public void PartExited(Collider other)
        {
            if (PartsList.Contains(other.gameObject))
                PartsList.Remove(other.gameObject);

            if (PartsList.Count == 0 && isDrilling)
            {
                isDrilling = false;

                _OnDrillingCollision?.Invoke(false);

                if (drillParticles)
                    drillParticles.Stop();
            }
        }

        /// <summary>
        /// Check the threshold and if it's still drilling, remove the part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private IEnumerator CheckPartThreshold(GameObject part)
        {
            yield return waitThreshold;

            if (PartsList.Contains(part))
            {
                if (part.layer == healthyLayer)
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
}