﻿using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client.WindowsAdministrator
{
    public class ProfilAdministrator
    {
        public GameClient Client;

        public event Action<StandardServerProfilInfo> UpdateProfil;

        public ProfilAdministrator(GameClient client)
        {
            Client = client;
            Client.ProfilUpdate += Client_ProfilUpdate;
        }

        private void Client_ProfilUpdate(StandardServerProfilInfo infos)
        {
            Application.Current.Dispatcher.Invoke(() => UpdateProfil?.Invoke(infos));
        }

        public void OpenAvatarsForm()
        {
            Client.Send(PacketType.LoadAvatar, new StandardClientLoadAvatars());
            AvatarsHandle form = new AvatarsHandle(Client.AvatarsHandleAdmin);
            form.Show();
        }
    }
}
