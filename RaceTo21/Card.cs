namespace RaceTo21.Pages
{
    public class Card
    {
        public int Id { get; set; }
        public string CardName { get; set; } // Abbreviation
        public string CardLongName { get; set; } // full name
        public string Suit { get; set; } // color
        public int Score { get; set; } // points
    }
}
