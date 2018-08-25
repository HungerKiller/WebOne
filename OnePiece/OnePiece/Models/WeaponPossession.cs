namespace OnePiece.Models
{
    public class WeaponPossession
    {
        public int PersonID { get; set; }
        public int WeaponID { get; set; }
        public Person Person { get; set; }
        public Weapon Weapon { get; set; }
    }
}
