﻿using BCA.Common;
using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.WindowsAdministrator
{
    public class PrestigeCustomizationsViewerAdministrator
    {
        public GameClient Client;

        public event Action<Customization[]> LoadPrestigeCustomizations;

        public PrestigeCustomizationsViewerAdministrator(GameClient client)
        {
            Client = client;

            Client.LoadPrestigeCustomizations += Client_LoadPrestigeCustomizations;
            Client.LoadSleeves += Client_LoadPrestigeCustomizations;
            Client.LoadBorders += Client_LoadPrestigeCustomizations;
            Client.LoadAvatars += Client_LoadPrestigeCustomizations;
        }

        private void Client_LoadPrestigeCustomizations(Customization[] customs)
        {
            if (customs[0].CustomizationType != CustomizationType.Avatar && customs[0].CustomizationType != CustomizationType.Border && customs[0].CustomizationType != CustomizationType.Sleeve)
                return;
            LoadPrestigeCustomizations?.Invoke(customs);
        }

        public void SendBuyPrestigeCustom(Customization custom)
        {
            Client.Send(PacketType.BuyPrestigeCustom, new StandardClientBuyPrestigeCustomization
            {
                CustomType = custom.CustomizationType,
                Id = custom.Id
            });
        }
        public void ChangeAvatar(int id)
        {
            StandardClientChangeAvatar packet = new StandardClientChangeAvatar { Id = id };
            Client.Send(PacketType.ChangeAvatar, packet);
        }
        public void ChangeSleeve(int id)
        {
            StandardClientChangeSleeve packet = new StandardClientChangeSleeve { Id = id };
            Client.Send(PacketType.ChangeSleeve, packet);
        }
        public void ChangeBorder(int id)
        {
            StandardClientChangeBorder packet = new StandardClientChangeBorder { Id = id };
            Client.Send(PacketType.ChangeBorder, packet);
        }
    }
}