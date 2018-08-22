using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnePiece.Models
{
    /// <summary>
    /// 海贼团
    /// </summary>
    public class PirateGroup
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [Display(Name = "Name")]
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        [Display(Name = "Description")]
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 海贼旗
        /// </summary>
        public string ImagePath { get; set; }

        [Display(Name = "Persons")]
        /// <summary>
        /// 船员。多个船员
        /// </summary>
        public ICollection<Person> Persons { get; set; }
    }
}
