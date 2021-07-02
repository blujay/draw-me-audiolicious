using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RecordSaveAudio
{
    [RequireComponent(typeof(AudioSource))]
    public class RecordSaveAudio : MonoBehaviour
    {

        #region Constants &  Static Variables
        /// <summary>
        /// Audio Source to store Microphone Input, An AudioSource Component is required by default
        /// </summary>
        static AudioSource audioSource;
        /// <summary>
        /// The samples are floats ranging from -1.0f to 1.0f, representing the data in the audio clip
        /// </summary>
        static float[] samplesData;

        #endregion

        #region Editor Exposed Variables
        
        public Button RecordAndSaveButton;
        /// <summary>
        /// What should the saved file name be, the file will be saved in Streaming Assets Directory
        /// </summary>
        [Tooltip("What should the saved file name be, the file will be saved in Streaming Assets Directory, Don't add .wav at the end")]
        public string fileName;

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 22050);  // third argument restrict the duration of the audio to 10 seconds 
            if (RecordAndSaveButton == null)
            {
                return;
            }
            RecordAndSaveButton.onClick.AddListener(() =>
            {
                RecordAndSave(fileName);
            });
        }

        public static void RecordAndSave(string fileName = "test")
        {

            while (!(Microphone.GetPosition(null) > 0)) { }
            samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(samplesData, 0);
            string filePath = Path.Combine(Application.persistentDataPath, fileName + ".wav");

            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            try
            {
                SavWav.TrimSilence(audioSource.clip, 0.9f);
                SavWav.Save(fileName, audioSource.clip);
                Debug.Log("File Saved Successfully at: " + filePath);
            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogError("Please, Create a StreamingAssets Directory in the Assets Folder");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);    //check for other Exceptions 
            }

        }

        

        #endregion
    }
}