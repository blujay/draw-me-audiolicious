using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioVisualizer
{
    public class SpawnCubes : MonoBehaviour
    {
        [SerializeField] private GameObject m_SamplePrefab;

        private GameObject[] m_SampleCubes = new GameObject[128];

        [SerializeField] private float m_MaxScale;


        void Start()
        {
            float angleStep = (360.0f / m_SampleCubes.Length);

            for (int i = 0; i < m_SampleCubes.Length; i++)
            {
                GameObject instanceSample = Instantiate(m_SamplePrefab);
                instanceSample.transform.position = transform.position;
                instanceSample.transform.parent = transform;
                instanceSample.name = "SampleCube" + i;

                // Rotate to make a circle
                transform.eulerAngles = new Vector3(0.0f, -angleStep * i, 0.0f);
                instanceSample.transform.position = Vector3.forward * 10.0f;

                m_SampleCubes[i] = instanceSample;
            }
        }

        void Update()
        {
            if ((m_SampleCubes == null) || (AudioPeer.AudioBand == null)) return;
            for (int i = 0; i < m_SampleCubes.Length; i++)
            {
                if (i < AudioPeer.AudioBand.Length)
                {
                    m_SampleCubes[i].transform.localScale = new Vector3(2.0f, (AudioPeer.AudioBandBuffer[i] * m_MaxScale) + 2.0f, 2.0f);
                }
            }
        }
    }
}