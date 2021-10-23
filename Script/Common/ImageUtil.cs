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
                case "��":
                    col = "R0";
                    break;
                case "��":
                    col = "G0";
                    break;
                case "��":
                    col = "B0";
                    break;
                case "��":
                    col = "W0";
                    break;
                case "��":
                    col = "P0";
                    break;
                case "��":
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
                case "���j�b�g":
                    ty = "T0";
                    break;
                case "�i�����j�b�g":
                    ty = "T1";
                    break;
                case "�A�C�e��":
                    ty = "T2";
                    break;
                case "�}�W�b�N":
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
