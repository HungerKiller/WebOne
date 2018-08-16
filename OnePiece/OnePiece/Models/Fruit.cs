using Microsoft.AspNetCore.Mvc;
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

        [Remote(action: "NameExists", controller: "Fruits")]
        [Required]
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 果实种类
        /// </summary>
        public FruitType Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 果实照片
        /// </summary>
        public string ImagePath { get; set; }
    }
}
