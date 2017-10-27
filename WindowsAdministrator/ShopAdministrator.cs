﻿using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class ShopAdministrator
    {
        public GameClient Client;

        public event Action<int, int, int, int, int> UpdateBoosterInfo;

        public ShopAdministrator(GameClient client)
        {
            Client = client;
            client.UpdateBoosterInfo += Client_UpdateBoosterInfo;
        }

        private void Client_UpdateBoosterInfo(int cardgot, int totalcard, int price, int cardperpack, int bp)
        {
            UpdateBoosterInfo?.Invoke(cardgot, totalcard, price, cardperpack, bp);
        }
    }
}