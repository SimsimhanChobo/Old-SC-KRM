using SCKRM.Object;
using SCKRM.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Sound
{
#pragma warning disable CS0618 // ���� �Ǵ� ����� ������ �ʽ��ϴ�.
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Ŀ��/Audio/����� ��� ������Ʈ", 0)]
    public class SoundObject : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        AudioSource _audioSource;
        public AudioSource audioSource { get => _audioSource; }

        /// <summary>
        /// This variable is managed by the Sound Manager script, do not touch it if possible.
        /// </summary>
        #region Variables managed by scripts
        public SoundType soundType { get; set; } = SoundType.All;

        public string path { get; set; } = "";

        public bool bgm { get; set; }

        public bool rhythmPitchUse { get; set; }


        [SerializeField, Range(0, 1)] float _volume = 1;
        [SerializeField, Range(0, 3)] float _pitch = 1;
        public float volume { get => _volume; set => _volume = value; }
        public float pitch { get => _pitch; set => _pitch = value; }
        #endregion

        public void Reload()
        {
            if (audioSource == null)
                GetComponent<AudioSource>();

            if (audioSource == null)
                return;

            OnDestroy();
            if (bgm)
            {
                audioSource.clip = ResourcesManager.Search<AudioClip>(ResourcePack.BGMPath + path);
                SoundManager.BGMList.Add(this);
            }
            else
            {
                audioSource.clip = ResourcesManager.Search<AudioClip>(ResourcePack.SoundPath + path);
                SoundManager.SoundList.Add(this);
            }

            SoundManager.AllList.Add(this);
            audioSource.Play();
        }

        void Update()
        {
            if (audioSource == null)
                GetComponent<AudioSource>();

            if (audioSource == null)
            {
                ObjectPoolingSystem.ObjectRemove("Sound Object", gameObject, OnDestroy);
                return;
            }

            audioSource.volume = volume;
            audioSource.pitch = pitch;

            if (!audioSource.isPlaying)
                ObjectPoolingSystem.ObjectRemove("Sound Object", gameObject, OnDestroy);
        }

        public void OnDestroy()
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;

            if (bgm)
                SoundManager.BGMList.Remove(this);
            else
                SoundManager.SoundList.Remove(this);

            SoundManager.AllList.Remove(this);

            audioSource.Stop();

            if (audioSource.clip != null) //&& !ResourcesManager.AudioList.ContainsValue(audioSource.clip))
                Destroy(audioSource.clip);

            audioSource.clip = null;
        }
    }
#pragma warning restore CS0618 // ���� �Ǵ� ����� ������ �ʽ��ϴ�.

    [System.Flags]
    public enum SoundType
    {
        All = 0,
        BGM = 1 << 1,
        Sound = 1 << 2
    }
}