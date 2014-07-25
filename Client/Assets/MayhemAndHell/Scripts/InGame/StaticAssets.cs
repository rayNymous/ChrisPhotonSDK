using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MayhemAndHell.Scripts.InGame
{
    /// <summary>
    /// Initialized on one zone load ZoneLoad.ZoneLoadHandler
    /// </summary>
    public class StaticAssets
    {
        private static StaticAssets _instance;

        public static void Initialize()
        {
            _instance = new StaticAssets();
        }

        public static StaticAssets Instance
        {
            get { return _instance; }
        }

        public Sprite[] SheetSprites;

        public StaticAssets()
        {
            SheetSprites = Resources.LoadAll<Sprite>("Sprites/sheet");
        }

        
    }
}
