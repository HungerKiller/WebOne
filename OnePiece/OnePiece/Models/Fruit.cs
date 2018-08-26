using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnePiece.Models
{
    /// <summary>
    /// 果实
    /// </summary>
    public class Fruit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        //[Remote(action: "NameExists", controller: "Fruits")]
        [Required(ErrorMessage = "The Name field is required.")]
        [Display(Name = "Name")]
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        [Display(Name = "Type")]
        /// <summary>
        /// 果实种类
        /// </summary>
        public FruitType Type { get; set; }

        [Display(Name = "Ability")]
        /// <summary>
        /// 能力
        /// </summary>
        public string Ability { get; set; }

        [Display(Name = "Description")]
        /// <summary>
        /// 详细描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 果实照片
        /// </summary>
        public string ImagePath { get; set; }

        // Person中也有这个字段，这是Person与Fruit的多对多关系。
        // 只要管理好Person里边的FruitPossessions，这里的FruitPossessions是被EF自动更新的。
        public ICollection<FruitPossession> FruitPossessions { get; set; }
    }
}
