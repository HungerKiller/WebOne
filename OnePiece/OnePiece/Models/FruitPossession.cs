namespace OnePiece.Models
{
    public class FruitPossession
    {
        public int PersonID { get; set; }
        public int FruitID { get; set; }
        public Person Person { get; set; }
        public Fruit Fruit { get; set; }
    }
}
