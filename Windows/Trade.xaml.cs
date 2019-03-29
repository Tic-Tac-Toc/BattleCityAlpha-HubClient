﻿using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Cards;
using hub_client.Network;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Trade.xaml
    /// </summary>
    public partial class Trade : Window
    {
        bool validate = false;

        private TradeAdministrator _admin;

        private int _id;
        private PlayerInfo[] _players = new PlayerInfo[2];
        List<PlayerCard> _cardsToOffer = new List<PlayerCard>();

        public Trade(TradeAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;

            Closed += Trade_Closed;


            _admin.InitTrade += _admin_InitTrade;
            _admin.GetMessage += _admin_GetMessage;
            _admin.UpdateCardsToOffer += _admin_UpdateCardsToOffer;
            _admin.TradeExit += _admin_TradeExit;

            CollectionJ1.GetListview().SelectionChanged += lvPlayer1_SelectionChanged;
            CollectionJ2.GetListview().SelectionChanged += lvPlayer2_SelectionChanged;
        }

        private void _admin_TradeExit()
        {
            Close();
        }

        private void Trade_Closed(object sender, EventArgs e)
        {
            _admin.Client.Send(PacketType.TradeExit, new StandardClientTradeExit { Id = _id });

            Closed -= Trade_Closed;


            _admin.InitTrade -= _admin_InitTrade;
            _admin.GetMessage -= _admin_GetMessage;
            _admin.UpdateCardsToOffer -= _admin_UpdateCardsToOffer;
            _admin.TradeExit -= _admin_TradeExit;

            CollectionJ1.GetListview().SelectionChanged -= lvPlayer1_SelectionChanged;
            CollectionJ2.GetListview().SelectionChanged -= lvPlayer2_SelectionChanged;
        }

        private void _admin_UpdateCardsToOffer(List<PlayerCard> cards)
        {
            CollectionJ1.Clear();
            foreach (PlayerCard card in _cardsToOffer)
                CollectionJ1.Add(card);
            CollectionJ2.Clear();
            foreach (PlayerCard card in cards)
                CollectionJ2.Add(card);

            btnValidate.IsEnabled = true;
        }

        private void _admin_GetMessage(string username, string message)
        {
            Dispatcher.InvokeAsync(delegate { chat.OnColoredMessage(FormExecution.AppDesignConfig.StandardMessageColor, "["+username+"]:"+message, false, false); });
        }

        private void _admin_InitTrade(int id, PlayerInfo[] players, Dictionary<int, BCA.Common.PlayerCard>[] Collections)
        {
            _id = id;
            _players = players;
            this.Title = _players[0].Username + " & " + _players[1].Username;

            CollectionJ1.UpdateCollection(Collections[0]);
            CollectionJ2.UpdateCollection(Collections[1]);
        }      

        private void lvPlayer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionJ1.SelectedItem() == null) return;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", ((PlayerCard)CollectionJ1.SelectedItem()).Id + ".jpg" });
        }
        private void lvPlayer2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionJ2.SelectedItem() == null) return;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", ((PlayerCard)CollectionJ2.SelectedItem()).Id + ".jpg" });
        }

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CollectionJ1.SelectedIndex() == -1) return;

            PlayerCard card = ((PlayerCard)CollectionJ1.SelectedItem());
            if (!CollectionJ1.RemoveCard(card))
                return;

            lb_choice.Items.Add(card.Name + "("+card.Id+")");
            _cardsToOffer.Add(card);
        }

        private void tbChat_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    _admin.Client.Send(PacketType.TradeMessage, new StandardClientTradeMessage { Id = _id, Message = tbChat.GetText() });
                    tbChat.Clear();
                    break;
            }
            e.Handled = true;
        }

        private void lb_choice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string item = lb_choice.SelectedItem.ToString();
            if (lb_choice.SelectedIndex == -1) return;

            string id = item.Split('(')[1].Replace(")", string.Empty);
            PlayerCard card = CollectionJ1.AddCard(Convert.ToInt32(id));
            lb_choice.Items.RemoveAt(lb_choice.SelectedIndex);
            _cardsToOffer.Remove(card);
        }

        private void BCA_ColorButton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (lb_choice.Items.Count <= 0)
                return;
            if (!validate)
            {
                btnProposition.IsEnabled = false;
                btnValidate.IsEnabled = false;
                validate = true;
                _admin.Client.Send(PacketType.TradeProposition, new StandardClientTradeProposition { Id = _id, Cards = _cardsToOffer });
            }
            else
            {
                btnProposition.IsEnabled = false;
                btnValidate.IsEnabled = false;
                _admin.Client.Send(PacketType.TradeAnswer, new StandardClientTradeAnswer { Id = _id, Result = true });
            }
        }
    }
}