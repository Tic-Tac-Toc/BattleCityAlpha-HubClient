﻿using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Shop.xaml
    /// </summary>
    public partial class Shop : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        public ShopAdministrator _admin;

        BoosterInfo BoosterChoosen;

        private int _tutoIndex = 0;

        public Shop(ShopAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _admin = admin;

            _admin.UpdateBoosterInfo += UpdateBoosterInfo;
            _admin.UpdateBattlePoints += _admin_UpdateBattlePoints;

            cbBooster.Items.Add("Booster");
            cbBooster.Items.Add("Booster Pack");
            cbBooster.Items.Add("Tournoi Pack");
            cbBooster.Items.Add("Pack du duelliste");
            cbBooster.Items.Add("Boîtes");
            cbBooster.Items.Add("Deck de démarrage");
            cbBooster.Items.Add("Deck de structure");
            //cbBooster.Items.Add("Boosters spéciaux");

            tb_searchBooster.tbChat.TextChanged += TbChat_TextChanged;

            LoadStyle();

            this.MouseDown += Window_MouseDown;
        }

        private void TbChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> BoosterSelect = new List<string>();
            lb_booster.Items.Clear();

            foreach (PurchaseType type in Enum.GetValues(typeof(PurchaseType)))
                if (BoosterManager.Boosters.ContainsKey(type))
                    foreach (BoosterInfo info in BoosterManager.Boosters[type])
                        if (info.ToString().ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                            BoosterSelect.Add(info.ToString());

            if (tb_searchBooster.GetText() == "")
            {
                foreach (PurchaseType type in Enum.GetValues(typeof(PurchaseType)))
                    if (BoosterManager.Boosters.ContainsKey(type))
                        foreach (BoosterInfo info in BoosterManager.Boosters[type])
                            BoosterSelect.Add(info.ToString());
            }

            foreach (string name in BoosterSelect)
                lb_booster.Items.Add(name);
        }

        private void _admin_UpdateBattlePoints(int points)
        {
            tb_bps.Text = points.ToString();
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_prestigeshop, btn_purchase, btn_searchcard, btn_brocante });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ShopButton");
                btn.Color2 = style.GetGameColor("Color2ShopButton");
                btn.Update();
            }

            this.FontFamily = style.Font;

            if (style.ShopWidth != -1)
            {
                this.Width = style.ShopWidth;
                this.Height = style.ShopHeight;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lb_booster.Items.Clear();
            foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Booster])
                lb_booster.Items.Add(item);

            img_booster.MouseLeftButtonDown += Img_booster_MouseLeftButtonDown;

            lb_booster.SelectedIndex = 0;

            if (FormExecution.ClientConfig.DoTutoShop)
            {
                BCA_TutoPopup tutopopup = new BCA_TutoPopup();
                maingrid.Children.Add(tutopopup);
                tutopopup.HorizontalAlignment = HorizontalAlignment.Center;
                tutopopup.VerticalAlignment = VerticalAlignment.Center;
                tutopopup.tuto_popup.IsOpen = true;
                tutopopup.tuto_popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
                tutopopup.tuto_popup.PlacementTarget = maingrid;
                tutopopup.SetText(StartDisclaimer.ShopTutorial[_tutoIndex]);
                tutopopup.tuto_popup.MaxWidth = this.Width - 200;

                tutopopup.SkipTuto += SkipTutorial;
                tutopopup.NextStep += Tutopopup_NextStep;

                SetTutorialColor(btn_searchcard);
            }
        }

        private void Tutopopup_NextStep(BCA_TutoPopup popup)
        {
            _tutoIndex++;

            if (_tutoIndex == StartDisclaimer.ShopTutorial.Length - 1)
            {
                popup.btnNext.Visibility = Visibility.Hidden;
                popup.btnSkip.ButtonText = "Fin !";
                popup.btnSkip.Update();
            }

            switch (_tutoIndex)
            {
                case 1:
                    SetOriginalColor(btn_searchcard);
                    SetTutorialColor(btn_brocante);
                    break;
            }

            if (_tutoIndex >= StartDisclaimer.ShopTutorial.Length)
                SkipTutorial(popup);
            else
                popup.SetText(StartDisclaimer.ShopTutorial[_tutoIndex]);
        }

        private void SkipTutorial(BCA_TutoPopup popup)
        {
            popup.tuto_popup.IsOpen = false;
            LoadStyle();
            FormExecution.ClientConfig.DoTutoShop = false;
            FormExecution.ClientConfig.Save();
        }
        private void SetTutorialColor(BCA_ColorButton btn)
        {
            btn.Color1 = Colors.Red;
            btn.Color2 = Colors.PaleVioletRed;
            btn.Update();
        }
        private void SetOriginalColor(BCA_ColorButton btn)
        {
            btn.Color1 = style.GetGameColor("Color1ShopButton");
            btn.Color2 = style.GetGameColor("Color2ShopButton");
            btn.Update();
        }

        private void Img_booster_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.AskBoosterCollection(BoosterChoosen.PurchaseTag);
        }

        private void cbBooster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lb_booster.Items.Clear();
            string newitem = e.AddedItems[0].ToString();
            switch (newitem)
            {
                case "Booster":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Booster])
                        lb_booster.Items.Add(item);
                    break;
                case "Booster Pack":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Booster_Pack])
                        lb_booster.Items.Add(item);
                    break;
                case "Pack du duelliste":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Pack_Duelist])
                        lb_booster.Items.Add(item);
                    break;
                case "Deck de démarrage":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Demarrage])
                        lb_booster.Items.Add(item);
                    break;
                case "Deck de structure":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Structure])
                        lb_booster.Items.Add(item);
                    break;
                case "Boîtes":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Boite])
                        lb_booster.Items.Add(item);
                    break;
                case "Tournoi Pack":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Tournoi_Pack])
                        lb_booster.Items.Add(item);
                    break;
                case "Booster Spéciaux":
                    foreach (BoosterInfo item in BoosterManager.Boosters[PurchaseType.Special_Pack])
                        lb_booster.Items.Add(item);
                    break;
            }
        }

        private void lb_booster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Storyboard storyboard = new Storyboard();

                ScaleTransform scale = new ScaleTransform(1.0, 1.0);
                img_booster.RenderTransformOrigin = new Point(0.5, 0.5);
                img_booster.RenderTransform = scale;

                DoubleAnimation growAnimationClose = new DoubleAnimation();
                growAnimationClose.Duration = TimeSpan.FromMilliseconds(100);
                growAnimationClose.From = 1.0;
                growAnimationClose.To = 0.0;
                storyboard.Children.Add(growAnimationClose);

                Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
                Storyboard.SetTarget(growAnimationClose, img_booster);

                storyboard.Completed += Storyboard_Completed;
                storyboard.Begin();
            }
            catch (Exception ex)
            {
                logger.Warn("SHOP WARNING - {0}", ex.ToString());
                return;
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (lb_booster.SelectedItem == null)
                return;

            LoadPurchase(lb_booster.SelectedItem.ToString());

            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            img_booster.RenderTransformOrigin = new Point(0.5, 0.5);
            img_booster.RenderTransform = scale;

            DoubleAnimation growAnimationOpen = new DoubleAnimation();
            growAnimationOpen.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationOpen.From = 0.0;
            growAnimationOpen.To = 1.0;
            storyboard.Children.Add(growAnimationOpen);
            Storyboard.SetTargetProperty(growAnimationOpen, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationOpen, img_booster);

            storyboard.Begin();
        }

        private void LoadPurchase(string Item)
        {
            BoosterChoosen = BoosterManager.InitializeBooster(Item);
            _admin.AskBooster(BoosterChoosen.PurchaseTag);
            tb_numberpack.Text = "1";
            tb_date.Text = BoosterChoosen.Date + ".";
            if (BoosterChoosen.Type == PurchaseType.Demarrage)
                tb_numbercardpack.Text = "C'est un deck de Démarrage.";
            if (BoosterChoosen.Type == PurchaseType.Structure)
                tb_numbercardpack.Text = "C'est un deck de Structure.";

            tb_card1.Text = BoosterChoosen.MostImportantCards[0];
            tb_card2.Text = BoosterChoosen.MostImportantCards[1];
            tb_card3.Text = BoosterChoosen.MostImportantCards[2];
            tb_card4.Text = BoosterChoosen.MostImportantCards[3];

            img_booster.Source = FormExecution.AssetsManager.GetImage(new string[] { "Booster", "pics", BoosterChoosen.PurchaseTag + ".png" });
        }

        public void UpdateBoosterInfo(int cardgot, int totalcard, int price, int cardperpack, int bp)
        {
            tb_cardgot.Text = cardgot.ToString();
            tb_totalcard.Text = totalcard.ToString();
            tb_price.Text = price.ToString();
            BoosterChoosen.Price = price;
            tb_numbercardpack.Text = cardperpack.ToString();
            tb_bps.Text = bp.ToString();

            int numberPacket = Convert.ToInt32(tb_numberpack.Text);
            if (numberPacket > 24)
                numberPacket = 24;
            if (numberPacket < 0)
                numberPacket = 0;
            tb_realprice.Text = (numberPacket * BoosterChoosen.Price).ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tb_numberpack_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int numberPacket = Convert.ToInt32(tb_numberpack.Text);
                if (numberPacket > 3 && (BoosterChoosen.Type == PurchaseType.Demarrage || BoosterChoosen.Type == PurchaseType.Structure))
                    numberPacket = 3;
                if (numberPacket > 24)
                    numberPacket = 24;
                if (numberPacket < 0)
                    numberPacket = 0;
                tb_realprice.Text = (numberPacket * BoosterChoosen.Price).ToString();
                tb_numberpack.Text = numberPacket.ToString();
            }
            catch { tb_numbercardpack.Text = "1"; };
        }

        private void btn_purchase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int numberPacket = Convert.ToInt32(tb_numberpack.Text);
            _admin.Purchase(BoosterChoosen.PurchaseTag, numberPacket);
            FormExecution.OpenPurchase(BoosterChoosen);
        }
        private void btn_searchcard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchCard form = new SearchCard();
            form.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => form.Activate()));
        }
        private void btn_brocante_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenBrocante();
        }
        private void btn_prestigeshop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenPrestigeShop();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateBoosterInfo -= UpdateBoosterInfo;
            _admin.UpdateBattlePoints -= _admin_UpdateBattlePoints;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            style.ShopWidth = this.Width;
            style.ShopHeight = this.Height;
            style.Save();
            FormExecution.ActivateChat();
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(40, 0, 40, 40);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

    }
}
