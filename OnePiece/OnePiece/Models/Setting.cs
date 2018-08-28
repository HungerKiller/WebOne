using System.ComponentModel.DataAnnotations;

namespace OnePiece.Models
{
    public class Setting
    {
        public int Id { get; set; }

        [Range(3, 100, ErrorMessage = "The field 'Fruit Count Per Page' must be between 3 and 100.")]
        [Display(Name = "Fruit Count Per Page")]
        public int FruitCountPerPage { get; set; }

        [Range(3, 100, ErrorMessage = "The field 'Weapon Count Per Page' must be between 3 and 100.")]
        [Display(Name = "Weapon Count Per Page")]
        public int WeaponCountPerPage { get; set; }

        [Range(3, 100, ErrorMessage = "The field 'Pirate Group Count Per Page' must be between 3 and 100.")]
        [Display(Name = "Pirate Group Count Per Page")]
        public int PirateGroupCountPerPage { get; set; }

        [Range(3, 100, ErrorMessage = "The field 'Person Count Per Page' must be between 3 and 100.")]
        [Display(Name = "Person Count Per Page")]
        public int PersonCountPerPage { get; set; }
    }
}