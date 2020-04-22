﻿using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Helpers;
using hub_client.Network;
using hub_client.Windows;
using NLog;
using System;
using System.Windows.Media;

namespace hub_client.WindowsAdministrator
{
    public class ChatAdministrator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GameClient Client;

        private ChatCommandParser _cmdParser;

        public event Action LoginComplete;
        public event Action<Color, string, bool, bool> SpecialChatMessage;
        public event Action<Color, PlayerInfo, string> PlayerChatMessage;
        public event Action<PlayerInfo> AddHubPlayer;
        public event Action<PlayerInfo> RemoveHubPlayer;
        public event Action<string, string> ClearChat;

        public ChatAdministrator(GameClient client)
        {
            Client = client;
            Client.PlayerChatMessageRecieved += Client_PlayerChatMessageRecieved;
            Client.SpecialChatMessageRecieved += Client_SpecialChatMessageRecieved;
            Client.LoginComplete += Client_LoginComplete;
            Client.AddHubPlayer += Client_AddHubPlayer;
            Client.RemoveHubPlayer += Client_RemoveHubPlayer;
            Client.ClearChat += Client_ClearChat;
            Client.Banlist += Client_Banlist;

            _cmdParser = new ChatCommandParser();
        }

        private void Client_SpecialChatMessageRecieved(Color c, string text, bool isBold, bool isItalic)
        {
            SpecialChatMessage?.Invoke(c, text, isBold, isItalic);
        }

        private void Client_PlayerChatMessageRecieved(Color c, PlayerInfo p, string msg)
        {
            PlayerChatMessage?.Invoke(c, p, msg);
        }

        private void Client_Banlist(string[] players)
        {
            string bl = "Banlist : ";
            foreach (string player in players)
                bl += player + ",";

            SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("StaffMessageColor"), bl, false, false);
        }

        private void Client_ClearChat(string username, string reason)
        {
            ClearChat?.Invoke(username, reason);
        }

        private void Client_RemoveHubPlayer(PlayerInfo infos)
        {
            RemoveHubPlayer?.Invoke(infos);
        }

        private void Client_AddHubPlayer(PlayerInfo infos)
        {
            AddHubPlayer?.Invoke(infos);
        }

        private void Client_LoginComplete()
        {
            LoginComplete?.Invoke();
        }

        public void SendMessage(string msg)
        {
            NetworkData data = ParseMessage(msg);
            if (data != null && data.Packet != null)
            {
                Client.Send(data);
                logger.Info("Chat send : {0}.", data);
            }
        }
        public void SendProfileAsking()
        {
            NetworkData data = new NetworkData(PacketType.Profil, new StandardClientProfilAsk { Username = Client.GetPlayerInfo(FormExecution.Username) });
            Client.Send(data);
        }
        public void SendDeck()
        {
            NetworkData data = new NetworkData(PacketType.UpdateCollection, new StandardClientAskCollection());
            Client.Send(data);
        }
        public void SendTradeRequest(PlayerInfo target)
        {
            Client.Send(PacketType.TradeRequest, new StandardClientTradeRequest { Player = target });
        }
        public void SendSpectateRequest(PlayerInfo target)
        {
            Client.Send(PacketType.SpectatePlayer, new StandardClientSpectate { Target = target });
        }
        public void AddBlacklistPlayer(PlayerInfo target)
        {
            Client.BlacklistManager.AddPlayer(target);
            Client.BlacklistManager.Save();
        }
        public void SendDonationBP(string amount, PlayerInfo target)
        {
            int pts;
            if (int.TryParse(amount, out pts))
            {
                Client.Send(PacketType.DonPoints, new StandardClientDonPoints
                {
                    Target = target,
                    Amount = pts
                });
            }
            else
                SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "Vous n'avez pas indiqué un nombre valable de BPs.", false, false);
        }
        public void AskSelectCard()
        {
            Client.Send(PacketType.AskSelectCard, new StandardClientAskSelectCard());
        }
        public void SendCardDonation(PlayerCard card, int quantity, PlayerInfo target)
        {
            Client.Send(PacketType.CardDonation, new StandardClientCardDonation
            {
                Target = target,
                Quantity = quantity,
                Card = card
            });
        }


        private NetworkData ParseMessage(string txt)
        {
            try
            {
                if (txt[0] == '/')
                {
                    txt = txt.Substring(1);
                    string cmd = txt.Split(' ')[0].ToString().ToUpper();
                    switch (cmd)
                    {
                        case "ANIM":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.AnimationMessage(txt.Substring(cmd.Length + 1)));
                        case "INFO":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.InformationMessage(txt.Substring(cmd.Length + 1)));
                        case "SETMOTD":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.SetMessageOfTheDay(txt.Substring(cmd.Length + 1)));
                        case "SETGREET":
                            return new NetworkData(PacketType.ChatMessage, _cmdParser.SetGreet(txt.Substring(cmd.Length + 1)));
                        case "KICK":
                            return new NetworkData(PacketType.Kick, _cmdParser.Kick(txt.Substring(cmd.Length + 1)));
                        case "BAN":
                            return new NetworkData(PacketType.Ban, _cmdParser.Ban(txt.Substring(cmd.Length + 1)));
                        case "UNBAN":
                            return new NetworkData(PacketType.Unban, _cmdParser.Unban(txt.Substring(cmd.Length + 1)));
                        case "MUTE":
                            return new NetworkData(PacketType.Mute, _cmdParser.Mute(txt.Substring(cmd.Length + 1)));
                        case "UNMUTE":
                            return new NetworkData(PacketType.Unmute, _cmdParser.Unmute(txt.Substring(cmd.Length + 1)));
                        case "CLEAR":
                            return new NetworkData(PacketType.Clear, _cmdParser.ClearChat(txt.Substring(cmd.Length + 1)));
                        case "MPALL":
                            return new NetworkData(PacketType.MPAll, _cmdParser.MPAll(txt.Substring(cmd.Length + 1)));
                        case "PANEL":
                            Panel panel = new Panel(Client.PanelAdmin);
                            panel.Owner = FormExecution.GetChatWindow();
                            panel.Show();
                            return null;
                        case "BANLIST":
                            return new NetworkData(PacketType.Banlist, new StandardClientBanlist { });
                        case "HELP":
                            return new NetworkData(PacketType.Help, new StandardClientAskHelp { });
                        case "GIVEBATTLEPOINTS":
                            return new NetworkData(PacketType.GivePoints, _cmdParser.GiveBattlePoints(txt.Substring(cmd.Length + 1)));
                        case "GIVEPRESTIGEPOINTS":
                            return new NetworkData(PacketType.GivePoints, _cmdParser.GivePrestigePoints(txt.Substring(cmd.Length + 1)));
                        case "GIVECARD":
                            return new NetworkData(PacketType.GiveCard, _cmdParser.GiveCard(txt.Substring(cmd.Length + 1)));
                        case "GIVEAVATAR":
                            return new NetworkData(PacketType.GiveAvatar, _cmdParser.GiveAvatar(txt.Substring(cmd.Length + 1)));
                        case "GIVEBORDER":
                            return new NetworkData(PacketType.GiveBorder, _cmdParser.GiveBorder(txt.Substring(cmd.Length + 1)));
                        case "GIVESLEEVE":
                            return new NetworkData(PacketType.GiveSleeve, _cmdParser.GiveSleeve(txt.Substring(cmd.Length + 1)));
                        case "GIVETITLE":
                            return new NetworkData(PacketType.GiveTitle, _cmdParser.GiveTitle(txt.Substring(cmd.Length + 1)));
                        case "ENABLED":
                            return new NetworkData(PacketType.EnabledAccount, _cmdParser.EnabledAccount(txt.Substring(cmd.Length + 1)));
                        case "DISABLED":
                            return new NetworkData(PacketType.DisabledAccount, _cmdParser.DisabledAccount(txt.Substring(cmd.Length + 1)));
                        case "PROMOTE":
                            return new NetworkData(PacketType.Ranker, _cmdParser.Ranker(txt.Substring(cmd.Length + 1)));
                        case "MAINTENANCE":
                            return new NetworkData(PacketType.Maintenance, _cmdParser.AskMaintenance(txt.Substring(cmd.Length + 1)));
                        case "MAINTENANCESTOP":
                            return new NetworkData(PacketType.StopMaintenance, _cmdParser.StopMaintenance());
                        case "BLACKLIST":
                            Blacklist blacklist = new Blacklist(Client.BlacklistManager);
                            blacklist.Owner = FormExecution.GetChatWindow();
                            blacklist.Show();
                            return null;
                        default:
                            SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "••• Cette commande n'existe pas.", false, false);
                            return null;
                    }
                }
                return new NetworkData(PacketType.ChatMessage, _cmdParser.StandardMessage(txt));
            }
            catch (Exception ex)
            {
                SpecialChatMessage?.Invoke(FormExecution.AppDesignConfig.GetGameColor("LauncherMessageColor"), "••• Une erreur s'est produite.", false, false);
                logger.Error("Chat input error : {0}", ex.ToString());
                return null;
            }
        }
    }
}
