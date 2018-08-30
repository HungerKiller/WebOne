using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnePiece.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    /// <summary>
    /// 武器
    /// </summary>
    public class Weapon
    {
        [JsonIgnore]
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
        /// 武器照片
        /// </summary>
        public string ImagePath { get; set; }

        [Display(Name = "Owner")]
        [JsonIgnore]
        public ICollection<WeaponPossession> WeaponPossessions { get; set; }
    }
}
