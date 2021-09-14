using Newtonsoft.Json;
using SCKRM.Language;
using SCKRM.Resources;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SCKRM.SaveData
{
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager instance;

        void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

            instance = this;

            LoadData();
        }

        void OnApplicationQuit() => SaveData();

        static IEnumerator AllRefresh()
        {
            yield return new WaitForEndOfFrame();
            Kernel.AllRefresh(true);
        }

        public static void SaveData()
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Save Data")))
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Save Data"));

            KernelSetting kernelSetting = new KernelSetting();
            string path = Path.Combine(Application.persistentDataPath, "Save Data", "Kernel Setting.json");



            kernelSetting.MainVolume = AudioListener.volume;
            kernelSetting.Language = LanguageManager.currentLanguage;
            kernelSetting.ResourcePack = ResourcesManager.ResourcePacks.ToList();
            kernelSetting.ResourcePack.RemoveAt(kernelSetting.ResourcePack.Count - 1);

            File.WriteAllText(path, JsonConvert.SerializeObject(kernelSetting, Formatting.Indented));
        }

        public static void LoadData()
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Save Data")))
                return;

            KernelSetting kernelSetting = new KernelSetting();
            string path = Path.Combine(Application.persistentDataPath, "Save Data", "Kernel Setting.json");
            if (File.Exists(path))
                kernelSetting = JsonConvert.DeserializeObject<KernelSetting>(File.ReadAllText(path));

            AudioListener.volume = kernelSetting.MainVolume;
            LanguageManager.currentLanguage = kernelSetting.Language;

            List<ResourcePack> resourcePacks = kernelSetting.ResourcePack;
            ResourcesManager.ResourcePacks.Clear();
            for (int i = 0; i < resourcePacks.Count; i++)
                ResourcesManager.ResourcePacks.Add(resourcePacks[i]);
            ResourcesManager.ResourcePacks.Add(ResourcePack.Default);

            instance.StartCoroutine(AllRefresh());
        }
    }

    public class KernelSetting
    {
        [JsonProperty("Main Volume")] public float MainVolume = 1;
        public string Language = "en_us";
        [JsonProperty("Resource Pack")] public List<ResourcePack> ResourcePack = new List<ResourcePack>();
        public Json.JRect jRect = new Json.JRect(0);
    }
}