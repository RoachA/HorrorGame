using System;
using UnityEngine;
using DG.Tweening;

namespace Game.World.Objects
{
    [RequireComponent(typeof(Light))]
    public class WorldLightHandler : MonoBehaviour, IHaveIdentity, IInteractable
    {
        private LightType LightType => m_light.type;
        [SerializeField] private Vector2 IntensityAndRange => new Vector2(m_light.intensity, m_light.range);

        [Header("JitterParams")]
        [SerializeField] private bool _startWithJitter = false;
        [SerializeField] private Vector2 _minMaxIntensity = new Vector2(0, 1);
        [SerializeField] private float _jitterFrequency = 1;
        
        private Sequence _jitterSeq;
        private bool m_isOn = true;
        private Light m_light;
        private InteractionMethod m_interactionType;
        private bool m_isActive;
        private InteractionStat m_endStat;
        private InteractionStat m_startStat;
        public int Id { get; set; }
        
        public void GenerateUniqueId()
        {
            Id = UniqueIDHelper.GenerateUniqueId(this);
        }

        InteractionMethod IInteractable.InteractionType
        {
            get => m_interactionType;
            set => m_interactionType = value;
        }
        bool IInteractable.IsActive
        {
            get => m_isActive;
            set => m_isActive = value;
        }
        InteractionStat IInteractable.EndStat
        {
            get => m_endStat;
            set => m_endStat = value;
        }
        InteractionStat IInteractable.StartStat
        {
            get => m_startStat;
            set => m_startStat = value;
        }

        private void Start()
        {
            if (m_light == null) m_light = GetComponent<Light>();
            if (_startWithJitter) StartJitter();
        }

        public GameObject GetInteractionGameObject()
        {
            return this.gameObject;
        }

        public void InteractStart(InteractionStat stat, Action callback = null)
        {
            m_isOn = !m_isOn;
            SetLightState(m_isActive);
        }

        public void InteractEnd(InteractionStat stat, Action callback = null)
        {
        }

        public void SetLightState(bool isActive)
        {
            m_light.intensity = isActive ? IntensityAndRange.x : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"> x : intensity, y. range</param>
        public void SetLightValues(Vector2 param)
        {
            m_light.intensity = param.x;
            m_light.range = param.y;
        }


        [Sirenix.OdinInspector.Button]
        public void StartJitter()
        {
            _jitterSeq?.Kill(true);
            _jitterSeq = DOTween.Sequence().SetLoops(-1);

            var s = 0;

            _jitterSeq.Append(DOTween.To(() => s, x => s = x, 1, _jitterFrequency)
                .OnStepComplete(() =>
                {
                    float randomIntensity = m_isOn ? UnityEngine.Random.Range(_minMaxIntensity.x, _minMaxIntensity.y) : 0;
                    m_light.intensity = randomIntensity;
                }));
        }

        [Sirenix.OdinInspector.Button]
        public void StopJitter()
        {
            _jitterSeq?.Kill(true);
            m_light.intensity = IntensityAndRange.x;
        }
    }
}