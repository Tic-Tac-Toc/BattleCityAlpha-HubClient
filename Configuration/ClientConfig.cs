﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Configuration
{
    public class ClientConfig
    {
        public bool Greet = true;
        public bool Trade = false;
        public bool Request = false;
        public bool Connexion_Message = true;
        public bool Autoscroll = true;

        public bool BCA_Card_Design = true;

        public int CardsStuffVersion = 0;

        public void Save()
        {
            File.WriteAllText(FormExecution.ClientConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
