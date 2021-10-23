using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

namespace ImageUtil
{
    public class ImageUtil
    {
        public static IEnumerator ColorImage(string color,Image image)
        {
            string col = "";
            switch (color)
            {
                case "赤":
                    col = "R0";
                    break;
                case "緑":
                    col = "G0";
                    break;
                case "青":
                    col = "B0";
                    break;
                case "白":
                    col = "W0";
                    break;
                case "黒":
                    col = "P0";
                    break;
                case "黄":
                    col = "Y0";
                    break;
            }
            var illust = Addressables.LoadAssetAsync<Sprite>(col);
            yield return illust;
            image.sprite = illust.Result;
        }
        public static IEnumerator TypeImage(string type, Image image)
        {
            string ty = "";
            switch (type)
            {
                case "ユニット":
                    ty = "T0";
                    break;
                case "進化ユニット":
                    ty = "T1";
                    break;
                case "アイテム":
                    ty = "T2";
                    break;
                case "マジック":
                    ty = "T3";
                    break;
            }
            var illust = Addressables.LoadAssetAsync<Sprite>(ty);
            yield return illust;
            image.sprite = illust.Result;
        }
        public static IEnumerator IllustImage(string name, Image image)
        {
            var illust = Addressables.LoadAssetAsync<Sprite>(name);
            yield return illust;
            image.sprite = illust.Result;
        }
    }
}
