﻿using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Assets;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Stuff;
using hub_client.Windows;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client
{
    public static class FormExecution
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //public static string debug_ip = "127.0.0.1";
        public static string debug_ip = "185.212.225.85";
        public static string test_ip = "185.212.226.12";
        public static string release_ip = "185.212.225.85";

        public static string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string HID;
        public static string Username;
        public static PlayerInfo PlayerInfos;


        public static string AppConfigPath = Path.Combine(AppDataPath, "BattleCityAlphaLauncher", "AppConfig.json");
        public static string AppDesignConfigPath = Path.Combine(path, "style.json");
        public static string ClientConfigPath = Path.Combine(path, "client_config.json");
        public static string YgoProConfigPath = Path.Combine(path, "ygopro_config.json");
        public static AppConfig AppConfig;
        public static AppDesignConfig AppDesignConfig;
        public static AssetsManager AssetsManager;
        public static ClientConfig ClientConfig;

        private static Dictionary<string, int> _banlists = new Dictionary<string, int>();

        public static GameClient Client { get; private set; }

        public static Dictionary<int, PrivateMessageAdministrator> PrivateForms = new Dictionary<int, PrivateMessageAdministrator>();

        #region Windows
        private static UpdateCardsStuffWindow _windowload;
        private static Login _login;
        private static Register _register;
        private static Chat _chat;
        private static Arena _arena;
        private static Shop _shop;
        private static Purchase _purchase;
        private static Brocante _brocante;
        private static PrestigeShop _pshop;
        private static Tools _tools;
        #endregion

        public static string GetIp()
        {
            if (ClientConfig.TestMode)
                return test_ip;
#if DEBUG
            return debug_ip;
#else
            return release_ip;
#endif
        }

        public static Chat GetChat()
        {
            return _chat;
        }

        public static void Init(bool restart = false)
        {
           if (!restart)
            {
                try
                {
                    HID = HardwareIdManager.GetId();
                }
                catch (Exception)
                {
                    HID = "";
                }


                if (File.Exists(AppConfigPath))
                    AppConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppConfigPath));
                else
                    AppConfig = new AppConfig();
                if (File.Exists(AppDesignConfigPath))
                    AppDesignConfig = JsonConvert.DeserializeObject<AppDesignConfig>(File.ReadAllText(AppDesignConfigPath));
                else
                    AppDesignConfig = new AppDesignConfig();
                if (File.Exists(ClientConfigPath))
                    ClientConfig = JsonConvert.DeserializeObject<ClientConfig>(File.ReadAllText(ClientConfigPath));
                else
                    ClientConfig = new ClientConfig();


                AssetsManager = new AssetsManager();

                if (!CheckCardsStuffUpdate())
                {
                    AssetsManager.LoadSmileys();
                    LoadCDB();
                }

                //ClientConfig.TestMode = false;

                BoosterManager.LoadList();
                LoadBanlist();
                //AppDesignConfig = new AppDesignConfig(); //To debug config

                SaveConfig();
            }
            Client = new GameClient();

            Client.PopMessageBox += Client_PopMessageBox;
            Client.ChoicePopBox += Client_ChoicePopBox;
            Client.RoomNeedPassword += Client_RoomNeedPassword;
            Client.Shutdown += Client_Shutdown;
            Client.PrivateMessageReceived += Client_PrivateMessageReceived;
            Client.LaunchYGOPro += Client_LaunchYGOPro;
            Client.LaunchYGOProWithoutRoom += Client_LaunchYGOProWithoutRoom;
            Client.LoadPlayerCustomizations += Client_LoadPlayerCustomizations;
            Client.LaunchTrade += Client_LaunchTrade;
            Client.CloseBrocante += Client_CloseBrocante;
            Client.LaunchBonusBox += Client_LaunchBonusBox;
            Client.LaunchDuelResultBox += Client_LaunchDuelResultBox;
            Client.LoadOfflineMessages += Client_LoadOfflineMessages;
            Client.RecieveDeck += Client_RecieveDeck;
            Client.RecieveReplay += Client_RecieveReplay;
            Client.Restart += Client_Restart;
            Client.CustomizationAchievement += Client_CustomizationAchievement;
            Client.LoadBoosterCollection += Client_LoadBoosterCollection;
            Client.GetMonthlyBonus += Client_GetMonthlyBonus;

            _chat = new Chat(Client.ChatAdmin);
            _login = new Login(Client.LoginAdmin);
            if (restart)
            {
                StartConnexion();
                _login.Focus();
                _login.Activate();
                _login.Show();
            }

            logger.Trace("FormExecution initialisation.");
        }

        private static void Client_GetMonthlyBonus(Dictionary<int, MonthlyBonus> bonus, int cnumber, int[] cards)
        {
            MonthlyBonusViewer viewer = new MonthlyBonusViewer(bonus, cnumber, cards);
            viewer.Show();
        }

        private static void Client_LoadBoosterCollection(string tag, List<int> ids, List<int> quantities, List<CardRarity> rarities)
        {
            CollectionViewer viewer = new CollectionViewer(BoosterManager.GetBoosterInfo(tag), ids, quantities, rarities);
            Task.Run(() => viewer.LoadCard());
            viewer.Show();
        }

        private static void Client_CustomizationAchievement(CustomizationType ctype, string text, int id)
        {
            CustomViewer viewer = new CustomViewer(ctype, text, id);
            viewer.Show();
        }

        public static void LoadCDB()
        {
            _windowload = new UpdateCardsStuffWindow(true);
            _windowload.Show();

            CardManager.LoadingFinished += CardManager_LoadingFinished;
            CardManager.LoadingProgress += CardManager_LoadingProgress;
            Task.Run(() => CardManager.LoadCDB(Path.Combine(path, "BattleCityAlpha", "cards.cdb"), true, true));
        }

        public static bool CheckCardsStuffUpdate()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string query = "http://battlecityalpha.xyz/BCA/UPDATEV2/CardsStuff/updates.txt";
                    string updateCardsStuff = wc.DownloadString(query);
                    string[] updatefilelines = updateCardsStuff.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    );
                    if (GetLastVersion(updatefilelines) != FormExecution.ClientConfig.CardsStuffVersion)
                    {
                        UpdateCardsStuff(updatefilelines);
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch { return false; }
        }
        public static int GetLastVersion(string[] updatefilelines)
        {
            return Convert.ToInt32(updatefilelines[0]);
        }
        private static void UpdateCardsStuff(string[] updatefilelines)
        {
            _windowload = new UpdateCardsStuffWindow(false);
            _windowload.Show();
            FormExecution.Client_PopMessageBox("Un mise à jour au niveau des cartes et des boosters est disponible !", "Mise à jour", true);

            List<string> updatesToDo = new List<string>();

            int i = 0;
            while (updatefilelines[i] != FormExecution.ClientConfig.CardsStuffVersion.ToString() && i < updatefilelines.Length - 1)
            {
                updatesToDo.Add(updatefilelines[i]);
                i++;
            }

            CardsUpdateDownloader.LoadingProgress += CardsUpdateDownloader_LoadingProgress;
            CardsUpdateDownloader.UpdateCompleted += CardsUpdateDownloader_UpdateCompleted;
            Task.Run(() => CardsUpdateDownloader.DownloadUpdates(updatesToDo.ToArray()));
        }

        private static void CardsUpdateDownloader_UpdateCompleted()
        {
            _windowload.EndDownload();
            _windowload.Close();

            CardsUpdateDownloader.LoadingProgress -= CardsUpdateDownloader_LoadingProgress;
            CardsUpdateDownloader.UpdateCompleted -= CardsUpdateDownloader_UpdateCompleted;

            LoadCDB(); 
            AssetsManager.LoadSmileys();
        }

        private static void CardsUpdateDownloader_LoadingProgress(int i, int n)
        {
            _windowload.SetProgressUpdate(i, n);
        }

        private static void Client_RecieveReplay(PlayerInfo sender, byte[] replay, string replayname)
        {
            if (!ClientConfig.AllowDeckShare)
                return;

            ChoicePopBox cpb = new ChoicePopBox(sender, new RoomConfig(), ChoiceBoxType.Replay, "", replayname);
            cpb.Choice += (r) => ReplayCPBChoice(r, replay, sender, replayname);
            cpb.Topmost = true;
            cpb.Show();
        }
        private static void ReplayCPBChoice(bool result, byte[] replay, PlayerInfo sender, string replayname)
        {
            if (!result)
                return;

            string name = sender.Username + "_" + replayname + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".yrp";
            name = name.Replace(' ', '_');
            File.WriteAllBytes(Path.Combine(path, "BattleCityAlpha", "replay", name), replay);
            YgoProHelper.LaunchYgoPro("-r " + name);
        }

        private static void Client_Restart()
        {
            Client_PopMessageBox("Vous avez été déconnecté du serveur.", "Problème", true);
            _chat.Restart = true;
            _chat.Close();
            Main.CheckClientUpdate();
            CheckCardsStuffUpdate();
            Init(true);
        }

        private static void LoadBanlist()
        {
            if (!File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "lflist.conf")))
                return;
            _banlists.Clear();
            var lines = File.ReadAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "lflist.conf"));

            foreach (string nonTrimmerLine in lines)
            {
                string line = nonTrimmerLine.Trim();

                if (line.StartsWith("!"))
                    _banlists.Add(line.Substring(1), _banlists.Count);
            }
        }

        public static Dictionary<string, int> GetBanlists()
        {
            return _banlists;
        }
        public static int GetBanlistValue(string key)
        {
            return _banlists.ContainsKey(key) ? _banlists[key] : 0;
        }
        public static string GetBanlistValue(int key)
        {
            foreach (var banlist in _banlists)
                if (banlist.Value == key)
                    return banlist.Key;
            return "Banlist inconnue";
        }

        private static void Client_RecieveDeck(PlayerInfo sender, string[] decklist, string deckname)
        {
            if (!ClientConfig.AllowDeckShare)
                return;

            ChoicePopBox cpb = new ChoicePopBox(sender, new RoomConfig(), ChoiceBoxType.Deck, "", deckname);
            cpb.Choice += (r) => DeckCPBChoice(r, decklist, sender, deckname);
            cpb.Topmost = true;
            cpb.Show();
        }
        private static void DeckCPBChoice(bool result, string[] decklist, PlayerInfo sender, string deckname)
        {
            if (!result)
                return;
            string name = sender.Username + "_" + deckname + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".ydk";
            name = name.Replace(' ', '_');
            File.WriteAllLines(Path.Combine(path, "BattleCityAlpha", "deck", name), decklist);
            YgoProHelper.LaunchYgoPro("-d " + name);
        }

        private static void CardManager_LoadingProgress(int i, int total)
        {
            double progress = Math.Round((i / (double)total) * 100, 1);
            _windowload.SetProgressValue(progress);
        }

        private static void CardManager_LoadingFinished()
        {
            logger.Trace("CDB Loaded.");
            _windowload.EndDownload();
            _windowload.Close();

            StartConnexion();
            _login.Focus();
            _login.Show();

            CardManager.LoadingProgress -= CardManager_LoadingProgress;
            CardManager.LoadingFinished -= CardManager_LoadingFinished;
        }

        public static void HideLogin()
        {
            _login.Hide();
        }
        public static void ShowLogin()
        {
            _login.Show();
        }

        private static void Client_RoomNeedPassword(int id, RoomType type)
        {
            InputText form = new InputText();
            form.Title = "Mot de passe";
            form.Owner = _arena;
            form.SelectedText += (obj) => RoomPassInput_SelectedText(obj, id, type);
            form.Topmost = true;
            form.Show();
        }
        private static void RoomPassInput_SelectedText(string pass, int id, RoomType type)
        {
            Client.SendRoomNeedPassword(id, pass, type);
        }

        private static void Client_LoadPlayerCustomizations(Customization avatar, Customization border, Customization sleeve, int pos)
        {
            YgoProHelper.LoadCustomization(avatar, border, sleeve, pos);
        }

        private static void Client_LaunchYGOProWithoutRoom(string arg)
        {
            YgoProHelper.LaunchYgoPro(arg);
        }

        private static void Client_LoadOfflineMessages(OfflineMessage[] messages)
        {
            OfflineMessagesBox box = new OfflineMessagesBox();
            box.Owner = _chat;
            box.LoadMessages(messages);
            box.Show();
        }

        private static void Client_LaunchDuelResultBox(int bps, int exps, bool win)
        {
            DuelResult box = new DuelResult(bps, exps, win);
            box.Owner = _chat;
            box.Topmost = true;
            box.Show();
        }

        private static void Client_LaunchBonusBox(BonusType type, int numberconnexion, string gift, int[] cards)
        {
            BonusBox box = new BonusBox(type, numberconnexion, gift);
            box.Owner = _chat;
            box.Topmost = true;
            box.Show();
            if (type == BonusType.Booster)
            {
                OpenPurchase(new BoosterInfo { Name = gift, Type = PurchaseType.Booster }, cards);
            }
        }

        private static void Client_CloseBrocante()
        {
            logger.Trace("Close Brocante");

            if (_brocante != null && _brocante.IsVisible)
                _brocante.Close();
        }

        private static void Client_LaunchTrade()
        {
            Trade trade = new Trade(Client.TradeAdmin);
            trade.Owner = _chat;
            trade.Show();
        }

        private static void Client_ChoicePopBox(PlayerInfo player, RoomConfig config, ChoiceBoxType type, string pass)
        {
            ChoicePopBox box = new ChoicePopBox(player, config, type, pass);
            box.Owner = _chat;
            box.Topmost = true;
            box.Show();
        }

        public static void Client_LaunchYGOPro(Room room, string arg)
        {
            if (room.IsRanked() && _arena != null && _arena.RankedTimer.IsEnabled)
                _arena.StopTimer();
            YgoProHelper.LaunchGame(room, arg);
        }

        private static void Client_PrivateMessageReceived(PlayerInfo user, string message)
        {
            if (Client.BlacklistManager.CheckBlacklist(user) || user.Username == FormExecution.Username)
                return;

            if (PrivateForms.ContainsKey(user.UserId))
                PrivateForms[user.UserId].PrivateMessageRecieved(user, message);
            else
            {
                OpenNewPrivateForm(user);
                PrivateForms[user.UserId].PrivateMessageRecieved(user, message);
            }
        }

        public static void OpenNewPrivateForm(PlayerInfo user)
        {
            if (user.Username == Username)
                return;
            if (PrivateForms.ContainsKey(user.UserId))
                return;

            string username = user.Username;
            PrivateMessageAdministrator admin = new PrivateMessageAdministrator(Client);
            PrivateMessage form = new PrivateMessage(username, admin);
            PrivateForms.Add(user.UserId, admin);
            form.Owner = _chat;
            form.Show();
            form.Activate();
            form.Closed += (sender, e) => PMClosed(sender, e, user.UserId);
        }

        private static void PMClosed(object sender, EventArgs e, int userID)
        {
            PrivateForms.Remove(userID);
        }

        public static Chat GetChatWindow()
        {
            return _chat;
        }

        private static void Client_Shutdown()
        {
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        public static bool CanCloseApp()
        {
            if (_chat == null)
                return true;
            return false;
        }

        public static void Client_PopMessageBox(string text, string title, bool showDialog)
        {
            PopBox box = new PopBox(text, title);
            box.Topmost = true;
            if (showDialog)
                box.ShowDialog();
            else
                box.Show();
        }

        public static void SaveConfig()
        {
            if (!Directory.Exists(AppConfigPath))
                Directory.CreateDirectory(Path.Combine(AppDataPath, "BattleCityAlphaLauncher"));
            File.WriteAllText(AppConfigPath, JsonConvert.SerializeObject(AppConfig));

            File.WriteAllText(AppDesignConfigPath, JsonConvert.SerializeObject(AppDesignConfig, Formatting.Indented));

            File.WriteAllText(ClientConfigPath, JsonConvert.SerializeObject(ClientConfig, Formatting.Indented));

            logger.Trace("Config Saved.");
        }

        public static void StartConnexion()
        {
            logger.Trace("Attempt of connexion...");

            Client.Connect(IPAddress.Parse(GetIp()), 9100);
            Client.Connected += Client_Connected;
            Task.Run(() => UpdateNetwork());
        }

        private static void Client_Connected()
        {
            logger.Trace("GameClient connected.");
        }

        private static void UpdateNetwork()
        {
            Client.Update();
            Task.Delay(1).ContinueWith((previous) => UpdateNetwork());
        }

        public static void OpenRegisterForm()
        {
            logger.Trace("Open register form");
            _register = new Register(Client.RegisterAdmin);
            _register.Owner = _login;
            _register.Show();
        }
        public static void OpenArena()
        {
            logger.Trace("Open arena");
            _arena = new Arena(Client.ArenaAdmin);
            _arena.Owner = _chat;
            _arena.Show();
        }
        public static void OpenShop()
        {
            logger.Trace("Open Shop");
            _shop = new Shop(Client.ShopAdmin);
            _shop.Owner = _chat;
            _shop.Show();
        }
        public static void OpenPrestigeShop()
        {
            logger.Trace("Open Prestige Shop");
            _pshop = new PrestigeShop(Client.PrestigeShopAdmin);
            Client.Send(PacketType.OpenPrestigeShop, new StandardClientOpenPrestigeShop { });
            _pshop.Owner = _shop;
            _pshop.Show();
        }
        public static void OpenPrestigeCustomizationsViewer()
        {
            logger.Trace("Open Prestige Customizations viewer");
            PrestigeCustomizationsViewerHorizontal viewer = new PrestigeCustomizationsViewerHorizontal(Client.PrestigeCustomizationsViewerAdmin, true);
            viewer.Owner = _pshop;
            viewer.Show();
        }
        public static void OpenPrestigeCustomizationsVerticalViewer()
        {
            logger.Trace("Open Prestige Customizations vertical viewer");
            PrestigeCustomizationViewerVertical viewer = new PrestigeCustomizationViewerVertical(Client.PrestigeCustomizationsViewerAdmin, true);
            viewer.Owner = _pshop;
            viewer.Show();
        }
        public static void OpenPrestigeTitleViewer()
        {
            TitlesHandle form = new TitlesHandle(Client.TitlesHandleAdmin, true);
            form.Owner = _pshop;
            form.Show();
        }
        public static void OpenPurchase(BoosterInfo booster)
        {
            logger.Trace("Open Purchase");
            _purchase = new Purchase(Client.PurchaseAdmin, booster);
            _purchase.Title = booster.Name;
            _purchase.Owner = _shop;
            _purchase.Show();
        }
        public static void OpenPurchase(BoosterInfo booster, int[] cards)
        {
            logger.Trace("Open Purchase");
            _purchase = new Purchase(Client.PurchaseAdmin, booster);
            _purchase.Title = booster.Name;
            _purchase.Owner = _chat;
            _purchase.Show();

            _purchase.UpdateCards(cards);
        }
        public static void OpenTools()
        {
            logger.Trace("Open Tools");
            _tools = new Tools(Client.ToolsAdmin);
            _tools.Owner = _chat;
            _tools.Show();
        }
        public static void OpenBrocante()
        {
            logger.Trace("Open Brocante");

            if (ClientConfig.FirstTimeBrocante)
            {
                Client_PopMessageBox(StartDisclaimer.BrocanteText, "Première ouverture de la brocante", true);
                ClientConfig.FirstTimeBrocante = false;
                ClientConfig.Save();
            }

            if (_brocante != null && _brocante.IsVisible)
                _brocante.Activate();

            _brocante = new Brocante(Client.BrocanteAdmin);
            _brocante.Owner = _shop;
            _brocante.Show();
        }
        public static void OpenDuelRequest(int id)
        {
            logger.Trace("Open Duel Request Form");

            DuelRequest request = new DuelRequest(Client.DuelRequestAdmin, id);
            request.Topmost = true;
            request.Show();
        }
        public static void OpenSoloModeWindow()
        {
            logger.Trace("Open Solo mode");

            SoloMode sm = new SoloMode();
            sm.Owner = _arena;
            sm.Show();
        }
        public static void OpenDatasRetrievalWindow()
        {
            DataRetrievalWindow window = new DataRetrievalWindow(Client.DataRetrievalAdmin);
            window.Owner = _tools;
            window.Show();
        }
        public static void OpenChangePicsWindow()
        {
            ChangePicsStyle window = new ChangePicsStyle();
            window.Owner = _tools;
            window.Show();
        }
        public static void OpenRankingWindow()
        {
            RankingWindow window = new RankingWindow(Client.RankingDisplayAdmin);
        }
        public static void OpenGamesHistory()
        {
            GamesHistory gh = new GamesHistory(Client.GamesHistoryAdmin);
        }

        public static void ActivateChat()
        {
            _chat.Activate();
        }
        public static void FlashChat()
        {
            _chat.Flash();
        }

        public static void ActivateShop()
        {
            _shop.Activate();
        }

        public static void RefreshChatStyle()
        {
            _chat.LoadStyle();
        }

        public static void AddNotes(string add)
        {
            string path = Path.Combine(FormExecution.path, "notes.bca");
            string notes = string.Empty;

            if (File.Exists(path))
                notes = File.ReadAllText(System.IO.Path.Combine(FormExecution.path, path));
            notes += Environment.NewLine + add;

            File.WriteAllText(path, notes);
        }

        public static void AddSmiley(string smileytxt)
        {
            _chat.AddSmiley(smileytxt);
        }
    }
}
