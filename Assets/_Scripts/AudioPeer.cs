using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioVisualizer
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPeer : MonoBehaviour
    {
        private AudioSource m_AudioSource;

        const int NSAMPLES = 512;
        const int NBANDS = 8;

        //private float[] m_Samples = new float[NSAMPLES];

        private float[] m_SamplesLeft = new float[NSAMPLES];
        private float[] m_SamplesRight = new float[NSAMPLES];

        private float[] m_FreqBand = new float[NBANDS];
        private float[] m_BandBuffer = new float[NBANDS];
        private float[] m_BufferDecrease = new float[NBANDS];

        private float[] m_FreqBandHighest = new float[NBANDS];
        public static float[] AudioBand = new float[NBANDS];
        public static float[] AudioBandBuffer = new float[NBANDS];

        public static float Amplitude;
        public static float AmplitudeBuffer;
        private float m_AmplitudeHighest;
        [SerializeField] private float m_AudioProfile;

        public enum EChannel { Stereo, Left, Right };
        [SerializeField] private EChannel m_Channel;

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            AudioProfile(m_AmplitudeHighest);
        }

        void Update()
        {
            GetSpectrumAudioSource();
            MakeFrequenceBands();
            BandBuffer();
            CreateAudioBands();
            GetAmplitude();
        }

        void AudioProfile(float audioProfile)
        {
            for (int i = 0; i < NBANDS; ++i)
            {
                m_FreqBandHighest[i] = audioProfile;
            }
        }

        void GetAmplitude()
        {
            float currentAmplitude = 0;
            float currentAmplitudeBuffer = 0;

            for (int i = 0; i < NBANDS; ++i)
            {
                currentAmplitude += AudioBand[i];
                currentAmplitudeBuffer += AudioBandBuffer[i];
            }

            if (currentAmplitude > m_AmplitudeHighest)
            {
                m_AmplitudeHighest = currentAmplitude;
            }

            Amplitude = currentAmplitude / m_AmplitudeHighest;
            AmplitudeBuffer = currentAmplitudeBuffer / m_AmplitudeHighest;
        }

        void CreateAudioBands()
        {
            for (int i = 0; i < NBANDS; ++i)
            {
                if (m_FreqBand[i] > m_FreqBandHighest[i])
                {
                    m_FreqBandHighest[i] = m_FreqBand[i];
                }
                AudioBand[i] = (m_FreqBand[i] / m_FreqBandHighest[i]);
                AudioBandBuffer[i] = (m_BandBuffer[i] / m_FreqBandHighest[i]);
            }
        }

        void BandBuffer()
        {
            for (int g = 0; g < NBANDS; ++g)
            {
                if (m_FreqBand[g] > m_BandBuffer[g])
                {
                    m_BandBuffer[g] = m_FreqBand[g];
                    m_BufferDecrease[g] = 0.005f;

                }

                if (m_FreqBand[g] < m_BandBuffer[g])
                {
                    m_BandBuffer[g] -= m_BufferDecrease[g];
                    m_BufferDecrease[g] *= 1.2f;
                }
            }
        }

        void GetSpectrumAudioSource()
        {
            // Get all the samples from the audio source
            m_AudioSource.GetSpectrumData(m_SamplesLeft, 0, FFTWindow.Blackman);
            m_AudioSource.GetSpectrumData(m_SamplesRight, 1, FFTWindow.Blackman);

        }

        void MakeFrequenceBands()
        {
            // Current song 22050Hz / 512 samples = 43hertz per sample
            // Bands
            // 60 - 250 Hertz
            // 250 - 500 Hertz
            // 500 - 2000 Hertz
            // 2000 - 4000 Hertz
            // 4000 - 6000 Hertz
            // 6000 - 20000 Hertz

            // Create frequence bands (8 in total)
            // 0. - 2 Samples = 86 Hertzs 
            // 1. - 4 Samples = 172 Hertzs  - Range 87 - 258
            // 2. - 8 Samples = 344 Hertzs  - Range 259 - 602
            // 3. - 16 Samples = 688 Hertzs  - Range 605 - 1290
            // 4. - 32 Samples = 1376 Hertzs  - Range 1291 - 2666
            // 5. - 64 Samples = 2752 Hertzs  - Range 2667 - 5418
            // 6. - 128 Samples = 5504 Hertzs  - Range 5419 - 10922
            // 7. - 256 Samples = 11008 Hertzs  - Range 10923 - 21930
            // Total 510

            // Current sample
            int count = 0;
            for (int i = 0; i < NBANDS; i++)
            {
                float average = 0;
                int sampleCount = (int)Mathf.Pow(2, i) * 2; // We get 2,4,8,16,32,64,128,210 number
                                                            // We need 512 with this operation we get 510 only we add 2 for the last one
                if (i == 7)
                {
                    sampleCount += 2;
                }
                for (int j = 0; j < sampleCount; j++)
                {
                    if (m_Channel == EChannel.Stereo)
                    {
                        average += m_SamplesLeft[count] + m_SamplesRight[count] * (count + 1);
                    }
                    if (m_Channel == EChannel.Left)
                    {
                        average += m_SamplesLeft[count] * (count + 1);
                    }
                    if (m_Channel == EChannel.Right)
                    {
                        average += m_SamplesRight[count] * (count + 1);
                    }

                    count++;
                }

                average /= count;

                // The average will be a bit smaller, multiplying this by 10 will be a bit bigger
                m_FreqBand[i] = average * 10;

            }

        }
    }
}