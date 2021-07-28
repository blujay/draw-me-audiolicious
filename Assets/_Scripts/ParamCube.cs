using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioVisualizer
{
    public class ParamCube : MonoBehaviour
    {

        [SerializeField] private int m_Band;
        [SerializeField] private float m_StartScale;
        [SerializeField] private float m_ScaleMultiplier;
        [SerializeField] private bool m_UseBuffer;
        [SerializeField] private Material m_Material;

        private void Awake()
        {
            m_Material = GetComponent<MeshRenderer>().materials[0];
        }

        private void Start()
        {
            transform.localScale = new Vector3(
                    transform.localScale.x,
                    m_StartScale,
                    transform.localScale.z);
        }

        void Update()
        {
            if (m_UseBuffer)
            {
                float scaleY = m_StartScale;
                float auxScale = (AudioPeer.AudioBandBuffer[m_Band] * m_ScaleMultiplier) + m_StartScale;
                if (!float.IsNaN(auxScale)) scaleY = auxScale;

                transform.localScale = new Vector3(
                    transform.localScale.x,
                    scaleY,
                    transform.localScale.z);
                Color c = new Color(
                    Mathf.Cos(AudioPeer.AudioBandBuffer[m_Band]),
                    Mathf.Sin(AudioPeer.AudioBandBuffer[m_Band]),
                    Mathf.Tan(AudioPeer.AudioBandBuffer[m_Band]));
                m_Material.SetColor("_EmissionColor", c);
            }
            else
            {
                float scaleY = m_StartScale;
                float auxScale = (AudioPeer.AudioBand[m_Band] * m_ScaleMultiplier) + m_StartScale;
                if (!float.IsNaN(auxScale)) scaleY = auxScale;

                transform.localScale = new Vector3(
                   transform.localScale.x,
                   scaleY,
                   transform.localScale.z);

                Color c = new Color(
                    AudioPeer.AudioBand[m_Band],
                    AudioPeer.AudioBand[m_Band],
                    AudioPeer.AudioBand[m_Band]);
                m_Material.SetColor("_EmissionColor", c);
            }
        }
    }
}