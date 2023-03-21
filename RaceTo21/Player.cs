using System.Collections.Generic;

namespace RaceTo21.Pages
{
    public class Player
    {
        public int Id { get; set; }
        public string Name {set;get;} // player name
        public int Chip {set;get;} // chips
        public PlayerStatus Status{set;get;} = PlayerStatus.bust; // Status: active-0, stay-1, bust-2, win-3, leave-4
        public int Score {set;get;} // accumulated points
        public List<Card> Cards {set;get;} // Each player's deck
        public List<Card> HandCards {set;get;} // hand shown
        public bool IsBet{set;get;} // whether to bet
    }
}
