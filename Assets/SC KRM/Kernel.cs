using SCKRM.Input;
using SCKRM.Language;
using SCKRM.Loading;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.Sound;
using SCKRM.Window;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SCKRM
{
    [AddComponentMenu("커널/커널", 0)]
    public class Kernel : MonoBehaviour
    {
        static IEnumerator allRefreshCoroutine;
        static LoadingBar loadingBar;

        public static Kernel instance;
        public static float fps { get; private set; } = 60;

        public static float deltaTime { get; private set; } = fps60second;
        public static float fpsDeltaTime { get; private set; } = 1;
        public static float unscaledDeltaTime { get; private set; } = fps60second;
        public static float fpsUnscaledDeltaTime { get; private set; } = 1;

        public const float fps60second = 1f / 60f;



        public static string dataPath { get; private set; }
        public static string streamingAssetsPath { get; } = Application.streamingAssetsPath;
        public static string persistentDataPath { get; private set; }

        public static string companyName { get; private set; }
        public static string productName { get; private set; }
        public static string version { get; private set; }
        public static string unityVersion { get; private set; }

        public static RuntimePlatform platform { get; } = Application.platform;



        public static bool isAFK { get; private set; } = false;



        public const int fpsLimit = 300;
        public const int notFocusFpsLimit = 30;
        public const int afkFpsLimit = 15;



        public static float afkTimer { get; private set; } = 0;
        public const float afkTimerLimit = 60;



        public static bool isReload { get; private set; } = false;
        public static int MainVolume { get; set; } = 100;
        public static float gameSpeed { get; set; } = 1;



        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else
                Destroy(gameObject);

            dataPath = Application.dataPath;
            persistentDataPath = Application.persistentDataPath;

            companyName = Application.companyName;
            productName = Application.productName;
            version = Application.version;
            unityVersion = Application.unityVersion;
        }

        void Update()
        {
            fps = 1f / deltaTime;
            deltaTime = Time.deltaTime;
            fpsDeltaTime = deltaTime * 60;
            unscaledDeltaTime = Time.unscaledDeltaTime;
            fpsUnscaledDeltaTime = unscaledDeltaTime * 60;

            //FPS Limit
            if (!isAFK && (Application.isFocused || Application.isEditor))
                Application.targetFrameRate = fpsLimit;
            else if (!isAFK && !Application.isFocused)
                Application.targetFrameRate = notFocusFpsLimit;
            else
                Application.targetFrameRate = afkFpsLimit;

            //AFK
            if (InputManager.GetAnyKeyDown())
                afkTimer = 0;

            if (afkTimer >= afkTimerLimit)
                isAFK = true;
            else
            {
                isAFK = false;
                afkTimer += unscaledDeltaTime;
            }

            if (MainVolume > 200)
                MainVolume = 200;
            else if (MainVolume < 0)
                MainVolume = 0;

            gameSpeed = gameSpeed.Clamp(0, 100);
            Time.timeScale = gameSpeed;
        }

#if !UNITY_EDITOR
        IEnumerator Start()
        {
            while (true)
            {
                if (InputManager.GetKeyDown("kernel.full_screen"))
                {
                    if (Screen.fullScreen)
                        Screen.SetResolution((int)(Screen.currentResolution.width / 1.5f), (int)(Screen.currentResolution.height / 1.5f), false);
                    else
                    {
                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
                        yield return new WaitForEndOfFrame();
                        yield return new WaitForEndOfFrame();
                        yield return new WaitForEndOfFrame();
                        yield return new WaitForEndOfFrame();
                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    }
                }
                yield return null;
            }
        }
#endif

        public static void ListMove<T>(List<T> ts, int index, int moveIndex)
        {
            T temp = ts[moveIndex];
            ts[moveIndex] = ts[index];
            ts[index] = temp;
        }

        public static void AllRefresh(bool coroutine = true, bool onlyText = false)
        {
            isReload = true;

            if (allRefreshCoroutine != null)
                instance.StopCoroutine(allRefreshCoroutine);

            if (loadingBar != null)
                loadingBar.Remove();

            allRefreshCoroutine = allRefresh(coroutine, onlyText);
            instance.StartCoroutine(allRefreshCoroutine);
        }

        static IEnumerator allRefresh(bool coroutine, bool onlyText)
        {
            if (coroutine)
            {
                loadingBar = LoadingBarManager.Create();
                yield return null;
            }

            if (onlyText)
            {
                LanguageManager.LangList.Clear();

                CustomText[] customTextRenderers = FindObjectsOfType<CustomText>(true);
                if (customTextRenderers.Length != 0)
                {
                    for (int i = 0; i < customTextRenderers.Length; i++)
                    {
                        customTextRenderers[i].Rerender();

                        if (coroutine)
                        {
                            loadingBar.text.text = $"Text Reload... ({i + 1}/{customTextRenderers.Length})";
                            loadingBar.value = i / (float)(customTextRenderers.Length - 1);
                            yield return null;
                        }
                    }
                }
                else
                    loadingBar.Remove();
            }
            else
            {
                //ResourcesManager.AudioList.Clear();
                LanguageManager.LangList.Clear();

                CustomRenderer[] customRenderers = FindObjectsOfType<CustomRenderer>(true);
                CustomText[] customTextRenderers = FindObjectsOfType<CustomText>(true);
                SoundObject[] soundObjects = SoundManager.instance.GetComponentsInChildren<SoundObject>(true);
                int renderersLength = customRenderers.Length - 1;
                int textRenderersLength = customTextRenderers.Length - 1;
                int soundObjectsLength = soundObjects.Length - 1;

                if (renderersLength == -1)
                    renderersLength = 0;
                if (textRenderersLength == -1)
                    textRenderersLength = 0;
                if (soundObjectsLength == -1)
                    soundObjectsLength = 0;

                if (renderersLength + textRenderersLength + soundObjectsLength == 0)
                    loadingBar.Remove();

                int forCount = 0;
                int i;
                int ii;
                for (i = 0; i < customRenderers.Length; i++)
                {
                    customRenderers[i].Rerender();

                    if (coroutine)
                    {
                        loadingBar.text.text = $"Texture Reload... ({i + 1}/{customRenderers.Length})";
                        loadingBar.value = i / (float)(renderersLength + textRenderersLength + soundObjectsLength);

                        if (forCount >= 100)
                        {
                            forCount = 0;
                            yield return null;
                        }
                        forCount++;
                    }
                }

                for (ii = 0; ii < customTextRenderers.Length; ii++)
                {
                    customTextRenderers[ii].Rerender();

                    if (coroutine)
                    {
                        loadingBar.text.text = $"Text Reload... ({ii + 1}/{customTextRenderers.Length})";
                        loadingBar.value = (i + ii) / (float)(renderersLength + textRenderersLength + soundObjectsLength);

                        if (forCount >= 100)
                        {
                            forCount = 0;
                            yield return null;
                        }
                        forCount++;
                    }
                }

                for (int iii = 0; iii < soundObjects.Length; iii++)
                {
                    soundObjects[iii].Reload();

                    if (coroutine)
                    {
                        loadingBar.text.text = $"Sound Reload... ({iii + 1}/{soundObjects.Length})";
                        loadingBar.value = (i + ii + iii) / (float)(renderersLength + textRenderersLength + soundObjectsLength);

                        if (forCount >= 100)
                        {
                            forCount = 0;
                            yield return null;
                        }
                        forCount++;
                    }
                }
            }

            isReload = false;
        }
    }

    public static class KernelExtensionMethod
    {
        #region Mathf
        public static int Abs(this int value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        public static float Abs(this float value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        public static double Abs(this double value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        public static int Sign(this int value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        public static int Sign(this float value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        public static int Sign(this double value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        public static int Clamp(this int value, int min, int max = int.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static float Clamp(this float value, float min, float max = float.PositiveInfinity)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static double Clamp(this double value, double min, double max = double.PositiveInfinity)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static int Lerp(this int a, int b, float t) => (int)(((1 - t) * a) + (b * t));
        public static int Lerp(this int a, int b, double t) => (int)(((1 - t) * a) + (b * t));

        public static float Lerp(this float a, float b, float t) => ((1 - t) * a) + (b * t);

        public static double Lerp(this double a, double b, double t) => ((1 - t) * a) + (b * t);

        public static int MoveTowards(this int current, int target, int maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        public static float MoveTowards(this float current, float target, float maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        public static double MoveTowards(this double current, double target, double maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }
        #endregion

        public static string EnvironmentVariable(this string value)
        {
            value = value.Replace("%DataPath%", Kernel.dataPath);
            value = value.Replace("%StreamingAssetsPath%", Kernel.streamingAssetsPath);
            value = value.Replace("%PersistentDataPath%", Kernel.persistentDataPath);

            value = value.Replace("%CompanyName%", Kernel.companyName);
            value = value.Replace("%ProductName%", Kernel.productName);
            value = value.Replace("%Version%", Kernel.version);

            value = value.Replace("%Platform%", Kernel.platform.ToString());

            return value;
        }

        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        public static string KeyCodeToString(this KeyCode keyCode)
        {
            string text;
            if (keyCode == KeyCode.Escape)
                text = "None";
            else if (keyCode == KeyCode.Return)
                text = "Enter";
            else if (keyCode == KeyCode.Alpha0)
                text = "0";
            else if (keyCode == KeyCode.Alpha1)
                text = "1";
            else if (keyCode == KeyCode.Alpha2)
                text = "2";
            else if (keyCode == KeyCode.Alpha3)
                text = "3";
            else if (keyCode == KeyCode.Alpha4)
                text = "4";
            else if (keyCode == KeyCode.Alpha5)
                text = "5";
            else if (keyCode == KeyCode.Alpha6)
                text = "6";
            else if (keyCode == KeyCode.Alpha7)
                text = "7";
            else if (keyCode == KeyCode.Alpha8)
                text = "8";
            else if (keyCode == KeyCode.Alpha9)
                text = "9";
            else if (keyCode == KeyCode.AltGr)
                text = "Alt Graph";
            else if (keyCode == KeyCode.Ampersand)
                text = "&";
            else if (keyCode == KeyCode.Asterisk)
                text = "*";
            else if (keyCode == KeyCode.At)
                text = "@";
            else if (keyCode == KeyCode.BackQuote)
                text = "`";
            else if (keyCode == KeyCode.Backslash)
                text = "\\";
            else if (keyCode == KeyCode.Caret)
                text = "^";
            else if (keyCode == KeyCode.Colon)
                text = ":";
            else if (keyCode == KeyCode.Comma)
                text = ",";
            else if (keyCode == KeyCode.Dollar)
                text = "$";
            else if (keyCode == KeyCode.DoubleQuote)
                text = "\"";
            else if (keyCode == KeyCode.Equals)
                text = "=";
            else if (keyCode == KeyCode.Exclaim)
                text = "!";
            else if (keyCode == KeyCode.Greater)
                text = ">";
            else if (keyCode == KeyCode.Hash)
                text = "#";
            else if (keyCode == KeyCode.Keypad0)
                text = "0";
            else if (keyCode == KeyCode.Keypad1)
                text = "1";
            else if (keyCode == KeyCode.Keypad2)
                text = "2";
            else if (keyCode == KeyCode.Keypad3)
                text = "3";
            else if (keyCode == KeyCode.Keypad4)
                text = "4";
            else if (keyCode == KeyCode.Keypad5)
                text = "5";
            else if (keyCode == KeyCode.Keypad6)
                text = "6";
            else if (keyCode == KeyCode.Keypad7)
                text = "7";
            else if (keyCode == KeyCode.Keypad8)
                text = "8";
            else if (keyCode == KeyCode.Keypad9)
                text = "9";
            else if (keyCode == KeyCode.KeypadDivide)
                text = "/";
            else if (keyCode == KeyCode.KeypadEnter)
                text = "Enter";
            else if (keyCode == KeyCode.KeypadEquals)
                text = "=";
            else if (keyCode == KeyCode.KeypadMinus)
                text = "-";
            else if (keyCode == KeyCode.KeypadMultiply)
                text = "*";
            else if (keyCode == KeyCode.KeypadPeriod)
                text = ".";
            else if (keyCode == KeyCode.KeypadPlus)
                text = "+";
            else if (keyCode == KeyCode.LeftApple)
                text = "Left Command";
            else if (keyCode == KeyCode.LeftBracket)
                text = "[";
            else if (keyCode == KeyCode.LeftCurlyBracket)
                text = "{";
            else if (keyCode == KeyCode.LeftParen)
                text = "(";
            else if (keyCode == KeyCode.Less)
                text = "<";
            else if (keyCode == KeyCode.Minus)
                text = "-";
            else if (keyCode == KeyCode.Mouse0)
                text = "Left Click";
            else if (keyCode == KeyCode.Mouse1)
                text = "Right Click";
            else if (keyCode == KeyCode.Mouse2)
                text = "Middle Click";
            else if (keyCode == KeyCode.Mouse3)
                text = "Mouse 3";
            else if (keyCode == KeyCode.Mouse4)
                text = "Mouse 4";
            else if (keyCode == KeyCode.Mouse5)
                text = "Mouse 5";
            else if (keyCode == KeyCode.Mouse6)
                text = "Mouse 6";
            else if (keyCode == KeyCode.Percent)
                text = "%";
            else if (keyCode == KeyCode.Period)
                text = ".";
            else if (keyCode == KeyCode.Pipe)
                text = "|";
            else if (keyCode == KeyCode.Plus)
                text = "+";
            else if (keyCode == KeyCode.Question)
                text = "?";
            else if (keyCode == KeyCode.Quote)
                text = "'";
            else if (keyCode == KeyCode.RightApple)
                text = "Right Command";
            else if (keyCode == KeyCode.RightBracket)
                text = "]";
            else if (keyCode == KeyCode.RightCurlyBracket)
                text = "}";
            else if (keyCode == KeyCode.RightParen)
                text = ")";
            else if (keyCode == KeyCode.Semicolon)
                text = ";";
            else if (keyCode == KeyCode.Slash)
                text = "/";
            else if (keyCode == KeyCode.SysReq)
                text = "Print Screen";
            else if (keyCode == KeyCode.Tilde)
                text = "~";
            else if (keyCode == KeyCode.Underscore)
                text = "_";
            else
                text = keyCode.ToString();

            return text.AddSpacesToSentence();
        }
    }
}