using Microsoft.AspNetCore.Components.Web; // Import Blazor components
using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Linq;
using AntDesign; // Reference UI framework Ant Design

namespace RaceTo21.Pages // Namespaces
{
    public partial class Index
    {
        
        private int round = 1; //game rounds
        private int currentBet = 10; //Amount per bet
        private int chipSum = 0; //Total chips for this round
        private bool _visible = false; //Controls the display of player input names
        private bool isBet; //A boolean Whether the bet button is disabled
        private int isDeal = 0; //A boolean whether the start deal button should be displayed
        private string _startGameVisible = ""; //Control the display of the game interface
        private int playerNum = 0; //number of players
        private bool _dealVisible; //Ask for licensing display
        private bool ruleModel;
        private string tips;
        private Player player = new Player();
        List<Player> Players = new List<Player>();

        protected override void OnInitialized() // Component initialization method
        {
            base.OnInitialized();
            _visible = true; // Controls the display of entered player names
        }

        private void HandleOk(MouseEventArgs e)
        {
            _visible = false;
            for (int i = 0; i < playerNum; i++) // According to the number of players, add a list of players
            {
                var num = i + 1;
                Players.Add(new Player { Id = num, Name = "", Chip = 100 });
            }
        }
        private void StartGame()
        {
            _startGameVisible = "display:none;";

        }
        private void GameOver()
        {
            _visible = false;
        }
        // Inquiry about licensing stage
        private void InquireDeal()
        {
            isBet = true;
            _dealVisible = true;
        }

        private void DealOk()
        {
            _dealVisible = false; // Set the deal visibility to false
            if (isDeal == 1)
            {
                // Serialize and print the current player list
                Console.WriteLine(JsonSerializer.Serialize(Players));
                // If all players are not active, the round ends and the winner is the player with the highest score who stayed
                if (Players.All(x => x.Status != PlayerStatus.active))
                {
                    //In the case of a hand, the score of not drawing any cards
                    Console.WriteLine("-----");
                    var winner = Players.Where(a=>a.Status == PlayerStatus.stay).OrderByDescending(x => x.Score).FirstOrDefault();
                    winner.Chip += chipSum;
                    winner.Status = PlayerStatus.win;
                    tips = $"player: {winner.Name} win!";
                    // Update the deal status
                    if (round == 3)
                    {
                        isDeal = 3;
                    }
                    else
                    {
                        isDeal = 2;
                    }
                    StateHasChanged();
                    return;
                }
                // If there are still active players, deal a card to each active player who bet
                foreach (var item in Players)
                {
                    if (item.Status == PlayerStatus.active && item.IsBet == true)
                    {
                        // Pick a random card from the player's card list
                        int r = new Random().Next(item.Cards.Count);
                        // Add the card to the player's hand cards and remove it from their card list
                        item.HandCards.Add(item.Cards[r]);
                        item.Cards.RemoveAt(r);
                        // Update the player's score
                        item.Score = item.Score + item.HandCards.Last().Score;
                        //If there is only one active or stay player left, set the status of the last player to win
                        if (Players.Where(x => x.IsBet == true).Count(x => x.Status == PlayerStatus.active || x.Status == PlayerStatus.stay) == 1)
                        {
                            int winnerIndex = Players.Where(x => x.IsBet == true).ToList().FindIndex(x => x.Status == PlayerStatus.active);
                            Players.Where(x => x.IsBet == true).ToList()[winnerIndex].Status = PlayerStatus.win;
                            tips = $"player: {Players.Where(x => x.IsBet == true).ToList()[winnerIndex].Name} win!";
                            //Winner gets all chips
                            Players.Where(x => x.IsBet == true).ToList()[winnerIndex].Chip += chipSum;
                            //The continue to deal button disappears, and the interface displays a prompt
                            if (round == 3)
                            {
                                isDeal = 3;
                            }
                            else
                            {
                                isDeal = 2;
                            }
                            StateHasChanged();
                            //Round ends
                            return;
                        }
                        else
                        {
                            //If someone happens to get blackjack
                            if (item.Score == 21)
                            {
                                item.Status = PlayerStatus.win;
                                tips = $"player: {item.Name} win!";
                                item.Chip += chipSum;
                                // Update the deal status
                                if (round == 3)
                                {
                                    isDeal = 3;
                                }
                                else
                                {
                                    isDeal = 2;
                                }
                                // Update the state of the component
                                StateHasChanged();
                                //Round ends
                                return;
                            }
                            // If the player's score exceeds 21, set their status to bust
                            else if (item.Score > 21)
                            {
                                item.Status = PlayerStatus.bust;
                            }
                        }
                    }
                }
            }
            else
            {
                //If there are no cards at the beginning, the round of the game is over and the bettor's bet will be returned
                if (Players.Where(x => x.IsBet == true).All(x => x.Status == PlayerStatus.stay))
                {
                    isDeal = 2;
                    Players.Where(x => x.IsBet == true).ForEach(y => y.Chip += currentBet);
                }
                else
                {
                    //Judgment If only one person asks for a card, and no one else is staying on the field, this person directly wins the round
                    if (Players.Where(x => x.Status == PlayerStatus.active).Count() == 1 && Players.Any(x=>x.Status != PlayerStatus.stay))
                    {
                        var thisWinner = Players.Find(x => x.Status == PlayerStatus.active);
                        tips = $"As long as player {thisWinner.Name} calls card，wins this round";
                        thisWinner.Chip += chipSum;
                        if (round == 3)
                        {
                            isDeal = 3;
                        }
                        else
                        {
                            isDeal = 2;
                        }
                        StateHasChanged();
                        return;
                    }
                    else
                    {
                        StartDeal();
                    }
                }
            }
        }

        private void DealCancel()
        {
            _dealVisible = false;
        }
        // Licensing stage
        private void StartDeal()
        {
            List<string> suits = new List<string> { "Spades", "Hearts", "Clubs", "Diamonds" };
            foreach (var item in Players)
            {

                List<Card> cards = new List<Card>();
                Card card = null;

                for (int cardVal = 1; cardVal <= 13; cardVal++)
                {
                    var index = cardVal;
                    foreach (var cardSuit in suits)
                    {
                        string cardName;
                        string cardLongName;
                        switch (index)
                        {
                            case 1:
                                cardName = "A";
                                cardLongName = "Ace";
                                break;
                            case 11:
                                cardName = "J";
                                cardLongName = "Jack";
                                break;
                            case 12:
                                cardName = "Q";
                                cardLongName = "Queen";
                                break;
                            case 13:
                                cardName = "K";
                                cardLongName = "King";
                                break;
                            default:
                                cardName = index.ToString();
                                cardLongName = cardName;
                                break;
                        }
                        card = new Card();
                        card.CardName = cardName;
                        card.CardLongName = cardLongName;
                        card.Suit = cardSuit;
                        if (card.CardName == "J" || card.CardName == "Q" || card.CardName == "K")
                        {
                            card.Score = 10;
                        }
                        else
                        {
                            card.Score = index;
                        }
                        card.Id = index;
                        cards.Add(card);
                    }

                }
                item.Cards = cards.Distinct().ToList();
                //Console.WriteLine(JsonSerializer.Serialize(item.Cards));

                //Randomly choose a card from the deck to hand
                int r = new Random().Next(item.Cards.Count);
                item.HandCards = new List<Card>();
                //no bet no hands
                if (item.Status == PlayerStatus.active && item.IsBet == true)
                {
                    item.HandCards.Add(item.Cards[r]);
                    //Console.WriteLine("-----HandCards");
                    //Console.WriteLine(JsonSerializer.Serialize(item.HandCards));
                    item.Cards.RemoveAt(r);
                    //Console.WriteLine("-----Cards");
                    //Console.WriteLine(JsonSerializer.Serialize(item.Cards));
                    item.Score = item.Score + item.HandCards.Last().Score;
                }

            }
            isDeal = 1;
            StateHasChanged();
        }

        // continue to deal
        /* calling InquireDeal() to ask players if they want to hit or stay, 
         * and then calling StateHasChanged() to update the UI.
         */
        private void NextDeal()
        {
            InquireDeal();
            StateHasChanged();
        }

        // start next round
        /* called when the "Next Round" button is clicked, and it updates the game state for the next round. 
         * It increments the round number, increases the current bet by 10 chips, resets the chip sum and tips, 
         * resets the player status, score, cards, hand cards, and bet flag, 
         * and sets the isDeal flag to 0 to ask players for their move.
         */
        private void NextRoundClick()
        {
            // Increase the number of rounds and increase the current bet by 10 chips
            round++;
            currentBet += 10;
            // Reset the total number of chips and prompt information
            chipSum = 0;
            tips = "";
            // Set the status of all players to "Active", and reset the attributes of score, cards, hands, and whether to bet
            Players.ForEach(x => x.Status = PlayerStatus.active);
            Players.ForEach(x => x.Score = 0);
            Players.ForEach(x => x.Cards = new());
            Players.ForEach(x => x.HandCards = new());
            Players.ForEach(x => x.IsBet = false);
            // Set the flags of whether to bet and deal to false
            isBet = false; //place a bet
            isDeal = 0; //ask
        }
        /* called when the game is over and it determines the final winner by sorting 
         * the players by their chip count and selecting the first player. It then sets the tips
         */
        private void GetResult()
        {
            var finalWinner = Players.OrderByDescending(x => x.Chip).FirstOrDefault().Name;
            tips = $"Final Winner is {finalWinner}!";
            isDeal = 4;
        }
        /*Called at the start of a new game, resets the round, bet, chip count, 
         * tooltip, player list, visibility and IsDeal and IsBet to their default values
         */
        private void NewGame()
        {
            round = 0;
            currentBet = 10;
            chipSum = 0;
            tips = "";
            Players = new();
            _visible = true;
            _startGameVisible = "";
            isBet = false;
            isDeal = 0;
        }
    }
}
