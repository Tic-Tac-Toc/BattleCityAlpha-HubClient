﻿using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.Windows;

namespace hub_client.WindowsAdministrator
{
    public class MonthPackViewerAdministrator
    {
        public GameClient Client;

        public MonthPackViewerAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadMonthPack += Client_LoadMonthPack;
        }

        private void Client_LoadMonthPack(int avatar, int border, int sleeve)
        {
            MonthPackViewer viewer = new MonthPackViewer(avatar, border, sleeve);
            viewer.PurchaseBtnClick += MonthPack_Buy;
            viewer.Show();
        }

        private void MonthPack_Buy()
        {
            SendBuyMonthPack();
        }

        public void SendBuyMonthPack()
        {
            Client.Send(PacketType.BuyMonthPack, new StandardClientBuyMonthPack { });
        }
    }
}
