using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnePiece.Models
{
    /// <summary>
    /// 人物
    /// </summary>
    public class Person
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

        [Display(Name = "Nickname")]
        /// <summary>
        /// 外号。不正规的称呼
        /// </summary>
        public string Nickname { get; set; }

        [Display(Name = "Description")]
        // TODO 注意数据库中可以存的最大长度
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        [Display(Name = "Reward Money")]
        /// <summary>
        /// 悬赏金（单位：贝利）
        /// </summary>
        public float? RewardMoney { get; set; }

        [Required(ErrorMessage = "The Birthday field is required.")]
        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd}")]
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        [Display(Name = "Race")]
        /// <summary>
        /// 种族
        /// </summary>
        public Race Race { get; set; }

        [Display(Name = "Sex")]
        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }

        [Display(Name = "Feature Type")]
        /// <summary>
        /// 所属势力
        /// </summary>
        public FeatureType FeatureType { get; set; }

        [Display(Name = "Title")]
        /// <summary>
        /// 称号/头衔
        /// </summary>
        public Title Title { get; set; }

        [Display(Name = "Pirate Group")]
        /// <summary>
        /// 海贼团
        /// </summary>
        public PirateGroup PirateGroup { get; set; }

        [Display(Name = "Weapons")]
        /// <summary>
        /// 武器。一个人可以有多个武器
        /// </summary>
        public ICollection<Weapon> Weapons { get; set; }

        //TODO
        /// <summary>
        /// 一个人可以有多张照片。第一张作为头像。
        /// </summary>
        public string ImagePath { get; set; }

        [Display(Name = "Fruits")]
        /// <summary>
        /// 一个人可以有多个果实
        /// </summary>
        public ICollection<FruitPossession> FruitPossessions { get; set; }
    }
}
