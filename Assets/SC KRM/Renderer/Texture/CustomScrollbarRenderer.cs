using SCKRM.InspectorEditor;
using SCKRM.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Renderer
{
    [RequireComponent(typeof(Scrollbar))]
    [AddComponentMenu("Ŀ��/Renderer/UI/Scrollbar", 4)]
    public class CustomScrollbarRenderer : CustomRenderer
    {
        Scrollbar scrollbar;

        [SerializeField] string _normalPath = "";
        [SerializeField] string _highlightedPath = "";
        [SerializeField] string _pressedPath = "";
        [SerializeField] string _selectedPath = "";
        [SerializeField] string _disabledPath = "";
        public string normalPath { get => _normalPath; set => _normalPath = value; }
        public string highlightedPath { get => _highlightedPath; set => _highlightedPath = value; }
        public string pressedPath { get => _pressedPath; set => _pressedPath = value; }
        public string selectedPath { get => _selectedPath; set => _selectedPath = value; }
        public string disabledPath { get => _disabledPath; set => _disabledPath = value; }

        [SerializeField] CustomButtonSetting buttonSprite = new CustomButtonSetting();

        public override void Rerender()
        {
            if (scrollbar == null)
                scrollbar = GetComponent<Scrollbar>();

            if (scrollbar == null)
            {
                Object = null;
                return;
            }

            scrollbar.transition = Selectable.Transition.SpriteSwap;

            SpriteSetting(normalPath, buttonSprite.normalSprite);
            SpriteSetting(highlightedPath, buttonSprite.highlightedSprite);
            SpriteSetting(pressedPath, buttonSprite.pressedSprite);
            SpriteSetting(selectedPath, buttonSprite.selectedSprite);
            SpriteSetting(disabledPath, buttonSprite.disabledSprite);

            SpriteState spriteState = new SpriteState();

            scrollbar.image.sprite = buttonSprite.normalSprite.sprite;
            spriteState.highlightedSprite = buttonSprite.highlightedSprite.sprite;
            spriteState.pressedSprite = buttonSprite.pressedSprite.sprite;
            spriteState.selectedSprite = buttonSprite.selectedSprite.sprite;
            spriteState.disabledSprite = buttonSprite.disabledSprite.sprite;

            Object = buttonSprite.normalSprite.sprite;
            scrollbar.spriteState = spriteState;
        }

        public void SpriteSetting(string path, CustomSpriteSetting spriteSetting)
        {
            NameSpaceAndPath nameSpaceAndPath = ResourcesManager.GetNameSpaceAndPath(path);
            Texture2D texture = ResourcesManager.Search<Texture2D>(ResourcePack.GuiPath + nameSpaceAndPath.Path, nameSpaceAndPath.NameSpace);

            if (texture == null)
                return;

            spriteSetting.RectMaxSet(texture);
            spriteSetting.PixelsPreUnitMaxSet();
            spriteSetting.sprite = Sprite.Create(texture, spriteSetting.rect, spriteSetting.pivot, spriteSetting.pixelsPerUnit, 1, SpriteMeshType.Tight, spriteSetting.border);
        }
    }
}