﻿using hub_client.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hub_client.Cards
{
    public class CardInfos : ICloneable
    {
        public int source
        {
            get;
            private set;
        }
        public CardInfos()
        {

        }
        public CardInfos(IList<string> carddata)
        {
            Id = int.Parse(carddata[0]);
            Ot = int.Parse(carddata[1]);
            AliasId = int.Parse(carddata[2]);
            SetCode = long.Parse(carddata[3]);
            Type = int.Parse(carddata[4]);
            uint level = uint.Parse(carddata[5]);
            Level = level & 0xff;
            LScale = (level >> 24) & 0xff;
            RScale = (level >> 16) & 0xff;
            Race = int.Parse(carddata[6]);
            Attribute = int.Parse(carddata[7]);
            Atk = int.Parse(carddata[8]);
            Def = int.Parse(carddata[9]);
            Category = long.Parse(carddata[10]);
        }

        public void SetCardText(string[] cardtext)
        {
            Name = cardtext[1];
            Description = cardtext[2];
            var effects = new List<string>();

            for (var i = 3; i < cardtext.Length; i++)
            {
                if (cardtext[i] != "")
                    effects.Add(cardtext[i]);
            }
            EffectStrings = effects.ToArray();

        }

        public CardType[] GetCardTypes()
        {
            var typeArray = Enum.GetValues(typeof(CardType));
            return typeArray.Cast<CardType>().Where(type => ((Type & (int)type) != 0)).ToArray();
        }

        public long[] GetCardSets(List<long> setArray)
        {
            var sets = new List<long> { setArray.IndexOf(SetCode & 0xffff), setArray.IndexOf(SetCode >> 0x10) };
            return sets.ToArray();
        }

        public int GetLevelCode()
        {
            MemoryStream m_stream = new MemoryStream();
            BinaryWriter m_writer = new BinaryWriter(m_stream);
            m_writer.Write((byte)Level);
            m_writer.Write((byte)0);
            m_writer.Write((byte)RScale);
            m_writer.Write((byte)LScale);
            return BitConverter.ToInt32(m_stream.ToArray(), 0);
        }

        public string GetRace()
        {
            return ((CardRace)Race).ToString();
        }

        public string GetAttribute()
        {
            return ((CardAttribute)Attribute).ToString();
        }

        public string GetCardType()
        {
            CardType[] typeArray = GetCardTypes();
            string type = "";
            foreach (CardType t in typeArray)
                type += t.ToString() + "|";
            type = type.Remove(type.Length - 1);
            return type;
        }

        public override string ToString()
        {
            CardType[] typeArray = GetCardTypes();

            string attribut = ((CardAttribute)Attribute).ToString();
            string type = GetCardType();
            string race = ((CardRace)Race).ToString();
            string level = typeArray.Contains(CardType.Xyz) ? "Rang" : "Niveau";
            string setname = GetSetName();
            string ot = ((CardFormat)Ot).ToString();

            string toReturn = "**`Informations:`**" + Environment.NewLine;
            toReturn += "`" + Environment.NewLine;
            toReturn += string.Format("{0} ` \t ` \n {1} ` \t ` \n {2} `", Name, Id, ot) + Environment.NewLine;
            toReturn += "```xl" + Environment.NewLine;
            if (typeArray.Contains(CardType.Monstre))
            {
                toReturn += string.Format("[{0} - {1}] \t {2} \t {3}", type, race, attribut.ToUpper(), setname) + Environment.NewLine;
                toReturn += string.Format("{0}: {1}★ \t {2}/{3}", level, Level, Atk, Def) + Environment.NewLine;
                if (typeArray.Contains(CardType.Pendule))
                    toReturn += string.Format("Echelles: {0}/{1}", LScale, RScale) + Environment.NewLine;
            }
            else
                toReturn += string.Format("[{0}]", type) + Environment.NewLine;
            toReturn += "```";
            toReturn += "```xl" + Environment.NewLine;
            toReturn += Description + "```";

            return toReturn;
        }

        public string GetSetName()
        {
            string setnames = "";
            long setcode = SetCode & 0xffff;
            if (CardManager.SetCodes.ContainsKey((int)setcode))
                setnames += CardManager.SetCodes[(int)setcode] + "/";
            else
                setnames = "Aucun ";
            setcode = setcode >> 16 & 0xffff;
            if (CardManager.SetCodes.ContainsKey((int)setcode))
                setnames += CardManager.SetCodes[(int)setcode] + "/";
            setcode = setcode >> 32 & 0xffff;
            if (CardManager.SetCodes.ContainsKey((int)setcode))
                setnames += CardManager.SetCodes[(int)setcode] + "/";
            setcode = setcode >> 48 & 0xffff;
            if (CardManager.SetCodes.ContainsKey((int)setcode))
                setnames += CardManager.SetCodes[(int)setcode] + "/";
            return setnames.Remove(setnames.Length - 1);
        }
        public LinkMarker[] GetLinkMarkers()
        {
            List<LinkMarker> Markers = new List<LinkMarker>();
            foreach (LinkMarker marker in Enum.GetValues(typeof(LinkMarker)))
                if ((Def & (int)marker) != 0)
                    Markers.Add(marker);
            return Markers.ToArray();
        }

        public bool BelongSetname(int setcode)
        {
            return ((int)SetCode & 0xffff) == setcode || ((int)SetCode & 0xffff >> 16 & 0xffff) == setcode || ((int)SetCode & 0xffff >> 16 & 0xffff >> 32 & 0xffff) == setcode || ((int)SetCode & 0xffff >> 16 & 0xffff >> 32 & 0xffff >> 48 & 0xffff) == setcode;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool IsSpell()
        {
            return GetCardTypes().Contains(CardType.Magie);
        }
        public bool IsTrap()
        {
            return GetCardTypes().Contains(CardType.Piège);
        }
        public bool IsMonster()
        {
            return GetCardTypes().Contains(CardType.Monstre);
        }

        public int AliasId { get; set; }

        public int Atk { get; set; }

        public int Attribute { get; set; }

        public int Def { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }

        public uint Level { get; set; }

        public uint LScale { get; set; }

        public uint RScale { get; set; }

        public string Name = "";

        public int Race { get; set; }

        public int Type { get; set; }

        public long Category { get; set; }

        public int Ot { get; set; }

        public string[] EffectStrings { get; set; }

        public long SetCode { get; set; }

    }
}
