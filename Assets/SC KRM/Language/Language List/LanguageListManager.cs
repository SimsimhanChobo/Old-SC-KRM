using SCKRM.Input;
using SCKRM.InspectorEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Language.UI
{
    [AddComponentMenu("커널/Language/언어 리스트/언어 리스트 설정", 0)]
    public class LanguageListManager : MonoBehaviour
    {
        [SerializeField, SetName("Exit 키를 눌렀을때 보여질 오브젝트")]
        GameObject visibleGameObject;

        void Update()
        {
            if (InputManager.GetKeyDown("gui.exit"))
                Exit();
        }

        public void Exit()
        {
            gameObject.SetActive(false);
            visibleGameObject.SetActive(true);

            Kernel.AllRefresh(true, true);
        }
    }
}