using UnityEngine;

namespace PrisonIsland.VFX
{
    /// <summary>
    /// Manages visual effects and particles
    /// </summary>
    public class VisualEffectsManager : MonoBehaviour
    {
        public static VisualEffectsManager Instance { get; private set; }

        [Header("Effect Prefabs")]
        [SerializeField] private GameObject constructionEffectPrefab;
        [SerializeField] private GameObject demolishEffectPrefab;
        [SerializeField] private GameObject escapeAlertEffectPrefab;
        [SerializeField] private GameObject levelUpEffectPrefab;

        [Header("Effect Settings")]
        [SerializeField] private float effectDuration = 2f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (Core.GameEvents.Instance != null)
            {
                Core.GameEvents.Instance.OnBuildingConstructed += PlayConstructionEffect;
                Core.GameEvents.Instance.OnBuildingDemolished += PlayDemolishEffect;
                Core.GameEvents.Instance.OnPrisonerEscaped += PlayEscapeEffect;
            }
        }

        private void PlayConstructionEffect(Buildings.Building building)
        {
            PlayEffectAt(constructionEffectPrefab, building.transform.position);
        }

        private void PlayDemolishEffect(Buildings.Building building)
        {
            PlayEffectAt(demolishEffectPrefab, building.transform.position);
        }

        private void PlayEscapeEffect(Prisoners.Prisoner prisoner)
        {
            PlayEffectAt(escapeAlertEffectPrefab, prisoner.transform.position);
        }

        public void PlayEffectAt(GameObject effectPrefab, Vector3 position)
        {
            if (effectPrefab == null)
            {
                // Create simple particle effect if no prefab
                CreateSimpleParticleEffect(position);
                return;
            }

            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }

        private void CreateSimpleParticleEffect(Vector3 position)
        {
            GameObject effectObj = new GameObject("SimpleEffect");
            effectObj.transform.position = position;

            ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1f;
            main.startLifetime = 0.5f;
            main.startSpeed = 5f;
            main.startSize = 0.2f;
            main.startColor = new Color(1f, 0.8f, 0.3f);

            var emission = ps.emission;
            emission.rateOverTime = 50;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.5f;

            ps.Play();
            Destroy(effectObj, 2f);
        }

        public void PlayConstructionEffectAt(Vector3 position)
        {
            PlayEffectAt(constructionEffectPrefab, position);
        }

        public void PlayDemolishEffectAt(Vector3 position)
        {
            PlayEffectAt(demolishEffectPrefab, position);
        }

        public void PlayLevelUpEffect(Transform target)
        {
            if (levelUpEffectPrefab != null)
            {
                GameObject effect = Instantiate(levelUpEffectPrefab, target.position, Quaternion.identity);
                effect.transform.SetParent(target);
                Destroy(effect, effectDuration);
            }
        }

        public void CreateExplosionEffect(Vector3 position, float radius = 2f)
        {
            GameObject effectObj = new GameObject("ExplosionEffect");
            effectObj.transform.position = position;

            ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.5f;
            main.startLifetime = 1f;
            main.startSpeed = 10f;
            main.startSize = 0.5f;
            main.startColor = new Color(1f, 0.5f, 0f);

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 100) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = radius;

            ps.Play();
            Destroy(effectObj, 2f);
        }

        public void CreateHealEffect(Transform target)
        {
            GameObject effectObj = new GameObject("HealEffect");
            effectObj.transform.position = target.position;
            effectObj.transform.SetParent(target);

            ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1.5f;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 0.3f;
            main.startColor = new Color(0f, 1f, 0.5f);

            var emission = ps.emission;
            emission.rateOverTime = 20;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 10f;

            ps.Play();
            Destroy(effectObj, 2f);
        }
    }
}
