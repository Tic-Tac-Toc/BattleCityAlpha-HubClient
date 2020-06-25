﻿using System;

namespace hub_client.Helpers
{
    public static class StartDisclaimer
    {
        public static string RankedText = "Ce bouton te permet de jouer en mode classé ! Tu vas rejoindre une file d'attente et chercher un adversaire pour combattre. Il est important de savoir que chaque partie en classé rapporte le triple de gains (BP et expérience)." + Environment.NewLine + "ATTENTION : Lorsque le duel va se lancer, il se lancera directement avec le deck sélectionné par défaut sur la fenêtre principale (en bas à gauche), fais le bon choix !";
        public static string BrocanteText = "Vous venez d'accéder pour la première fois au marché au carte ! C'est l'endroit où les joueurs peuvent vendre et acheter des cartes à d'autres joueurs. Le marché au carte est réinitialisée tous les dimanches à minuit, date à laquelle tu récupéreras les cartes que tu as déposé si elles ne sont pas vendues.";
        public static string ShadowDuelText = "ATTENTION ! Vous vous apprêtez à faire/recevoir une demande pour un duel des ombres. Faites bien attention à la mise de ce dernier ! En effet, les mises peuvent être des BPs (le gagnant remporte ceux du perdant), un banissement (le perdu est banni pour la durée indiquée) ou un mute... Si vous acceptez le défi, soyez sûr d'en mesurer le risque !";
        
        public static string[] ChatTutorial = new string[]
        {
        "Salut ! Moi c'est Astral, je vais t'assister si tu l'acceptes dans tes premiers pas sur le jeu et te montrer au mieux son fonctionnement ! Si tu n'es pas intéressé tu peux bien sûr passer ce tutoriel !",
        "Alors comme tu le vois, le bouton qui vient de changer de couleur \"Arène\" te permet d'accéder à une fenêtre où les duellistes se rejoignent pour faire des duels mais aussi pour affronter le bot ou voir le classement !",
        "Ici c'est la boutique ! En effet, si tu es ici tu dois certainement déjà le savoir mais tu ne possèdes pas toutes les cartes, tu vas devoir les obtenir via des boosters, des decks de structure etc. Et ces achats c'est à partir de ce bouton que tu pourras les faire ! Pas d'inquiètude il suffit de quelques jours pour construire un deck compétitif.",
        "C'est bien beau d'avoir des cartes, mais faut-il encore pouvoir construide des decks ! Et bien tu pourras le faire en cliquant sur ce bouton. En accédant à l'éditeur de deck, tu pourras également voir les cartes que tu ne possèdes pas ainsi que les boosters dans lesquels elles se trouvent. Ah oui j'oubliais, l'éditeur dispose aussi d'un testeur de main pour être sûr que ton deck ne te fasse pas faux bond en duel !",
        "Si tu t'ennuies un peu sur le jeu, que tu cherches des défis ou simplement que tu aimes la compétition ce bouton est fait pour toi ! Tu y retrouveras le planning des animations de la semaine n'hésite pas à aller y jeter un oeil pour te tenir au courant de la vie de la communauté.",
        "Le bouton options ? Comme son nom l'indique il te permettra de modifier les options de ton jeu mais pas seulement ! Tu peux aussi totalement personnalisé le graphisme du jeu : couleur des boutons, fond des fênêtres, police de caractères... N'hésite pas à faire de ton jeu un environnement unique !",
        "Allez c'est bientôt fini pour cette fenêtre, regarde en bas, il y a encore quelques boutons qui pourraient t'être utile : voir ton profil, consulter tes replays, afficher les quêtes journalières te permettant de gagner des BPs, tu peux également aller les règles qui règnent sur le chat ici !",
        "ALORS ce bouton est SUPER IMPORTANT ! S'il te plaît avant de poser une question sur le chat, vérifie qu'elle ne se trouve pas dans la FAQ ! Il y a 9 chances sur 10 que tu trouves ta réponse dedans ! Si tu as du temps devant toi, la lire une fois ne peut pas faire de mal.",
        "Et enfin un bloc-notes si tu as peur d'oublier des choses ainsi que le lien du serveur discord du jeu, n'hésite pas à nous rejoindre si ce n'est pas déjà fait, tu trouveras toutes les informations de mises à jour là bas !" + Environment.NewLine + "Et bien je crois que tu sais tout, je peux désormais te souhaiter bon jeu ! Pour commencer, tu pourrais aller acheter des decks de structure en boutique pour construire ton premier deck ? Si tu rencontres des problèmes, tu peux contacter notre équipe."
        };
        public static string[] ArenaTutorial = new string[]
        {
            "Ca y est, tu t'es décidé à venir te frotter aux autres joueurs ? Tu as bien raison ! C'est ici que tu vas pouvoir remporter des points pour construire tes decks et agrandir ta collection. Laisse moi t'expliquer le fonctionnement de cette fenêtre.",
            "Ce bouton te permettra de jouer en \"classé\", je m'explique, dans ce mode de jeu tu te mesures aux autres joueurs et à l'issue du duel tu gagnes/perds des points ELO en fonction du résultat. C'est ici que tu peux atteindre le sommet du classement pour devenir un maître du duel ! Tu peux également consulter ce classement via le bouton du même nom.",
            "Ce bouton lui te permettra d'héberger un duel selon les paramètres de ton choix puis d'attendre qu'un adversaire te rejoigne pour combattre !",
            "Mode solo, tu te demandes ce que c'est ? Et bien c'est tout simplement un mode de jeu dans lequel tu vas pouvoir t'entraîner en affrontant un robot de duel. Tu ne gagnes pas de points à l'issue du duel cependant tu peux t'entraîner en jouant un deck composé de cartes que tu ne POSSEDES PAS, rien de mieux pour s'assurer de faire les bons achats !",
            "Enfin ce dernier bouton te permettra de visualiser : soit les salles de duels qui attendant des challengers, soit celles qui sont déjà en cours si tu souhaites aller regarder un combat en tant que spectateur ! En effet soutenir ses amis ou même observer les finales de tournois peut toujours être intéressant.",
            "Voilà j'en ai fini pour cette fenêtre ! Je te souhaite bonne chance pour tes futurs combats !"
        };
        public static string[] ShopTutorial = new string[]
        {
            "Bienvenue dans la boutique ! C'est ici que tu pourras assouvir tes désirs de collectionneur ! Alors si je peux te donner un conseil c'est vraiment de commencer en utilisant tes 2000 BPs pour acheter des decks de structure (attention un deck de structure te donnera les cartes qu'il contient en un seul exemplaire, pense à en acheter plusieurs pour construire un deck). Pour chercher dans quel booster se trouve une carte n'hésite pas à utiliser la fonction recherche de carte.",
            "J'ai entendu dire que les decks de structure Le dévastateur de Duel, la Confrontation des marionnettes de l'ombres, la rage du dinobroyeur ou Brûleur d'Ame étaient pas mal pour commencer ! Après si tu as une autre idée en tête je ne doute pas que tu construises un excellent deck ! " + Environment.NewLine + "Pour les cartes génériques, tu pourras en trouver dans le Dévastateur de Duel ou via le marché aux cartes (hôtel des ventes de cartes).",
            "Ah oui j'oubliais ! Tu peux chercher des boosters par leur nom ou leur initial à l'aide de la barre de recherche, tu peux aussi consulter à tout moment ta collection pour chaque booster en cliquant sur l'image de ce dernier ! Et enfin, si tu as envie de troquer ou d'acheter des cartes à bas prix n'hésite pas à aller faire un tour à le marché aux cartes !"
        };
        public static string QuestsTutorial = "Tu viens d'entrer dans la fenêtre des quêtes quotidiennes ! Chaque jour tu vas pouvoir réaliser 3 quêtes afin de gagner des BPs, la première te rapportera 200 BPs (quête basique), la seconde 300 BPs (quête spéciale) et enfin la 3ème 100 BPs (quête fun). Si tu arrives à terminer les 3 quêtes, tu remporteras 400 BPs supplémentaires ! Si tu trouves une quête trop dur tu peux la changer mais ATTENTION tu ne peux changer qu'une seule quête par jour ! Pour les quêtes sous forme de questions, n'hésite pas à demander de l'aide dans le chat.";
    }
}
