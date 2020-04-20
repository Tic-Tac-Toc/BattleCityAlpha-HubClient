﻿using BCA.Common;
using BCA.Common.Enums;
using hub_client.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_CustomizationsViewer.xaml
    /// </summary>
    public partial class BCA_CustomizationsViewer : UserControl
    {
        private Customization[] _customs;
        private int _index = 2;
        string ctypetext = "";

        private Image[] imgs;

        AssetsManager PicsManager = new AssetsManager();

        public BCA_CustomizationsViewer()
        {
            InitializeComponent();

            imgs = new Image[] { img_center, img_center_left, img_center_right, img_left, img_right };
        }

        public void LoadFirstCustoms(Customization[] customs)
        {
            _customs = customs;
            switch (_customs[0].CustomizationType)
            {
                case CustomizationType.Avatar:
                    ctypetext = "Avatars";

                    img_center.Width = 256;
                    img_center.Height = 256;
                    img_center_left.Width = 128;
                    img_center_left.Height = 128;
                    img_center_right.Width = 128;
                    img_center_right.Height = 128;
                    img_left.Width = 100;
                    img_left.Height = 100;
                    img_right.Width = 100;
                    img_right.Height = 100;
                    break;
                case CustomizationType.Border:
                    ctypetext = "Borders";
                    break;
                case CustomizationType.Sleeve:
                    ctypetext = "Sleeves";

                    img_center.Width = 177;
                    img_center.Height = 254;
                    img_center_left.Width = 128;
                    img_center_left.Height = 184;
                    img_center_right.Width = 128;
                    img_center_right.Height = 184;
                    img_left.Width = 100;
                    img_left.Height = 144;
                    img_right.Width = 100;
                    img_right.Height = 144;
                    break;
            }

            LoadByIndex();
        }

        private void LoadByIndex()
        {

            if (_customs.Length - 1 < _index)
                _index--;
            img_center.Source = LoadCustom(_customs[_index]);

            if (_index - 1 < 0)
                img_center_left.Source = null;
            else
                img_center_left.Source = LoadCustom(_customs[_index - 1]);

            if (_index - 2 < 0)
                img_left.Source = null;
            else
                img_left.Source = LoadCustom(_customs[_index - 2]);

            if (_index + 1 > _customs.Length - 1)
                img_center_right.Source = null;
            else
                img_center_right.Source = LoadCustom(_customs[_index + 1]);

            if (_index + 2 > _customs.Length - 1)
                img_right.Source = null;
            else
                img_right.Source = LoadCustom(_customs[_index + 2]);
        }

        public void RightArrow()
        {
            if (_index + 1 > _customs.Length - 1)
                return;
            _index++;
            LoadByIndex();
        }
        public void LeftArrow()
        {
            if (_index - 1 < 0)
                return;
            _index--;
            LoadByIndex();
        }
        public int GetIndex()
        {
            return _index;
        }

        private BitmapImage LoadCustom(Customization custom)
        {
            if (!custom.IsHost)
                return GetImage(ctypetext, custom.Id.ToString());
            else
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(
                        new System.Uri(custom.URL),
                        Path.Combine(FormExecution.path, "Assets", ctypetext, "temp.png")
                        );
                }
                return GetImage(ctypetext, "temp");
            }
        }
        private BitmapImage GetImage(string directory, string img)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(FormExecution.path, "Assets", directory, img + ".png"));
            image.EndInit();
            return image;
        }
    }

    public enum CustomPos
    {
        Center,
        CenterLeft,
        CenterRight,
        Left,
        Right
    }
}