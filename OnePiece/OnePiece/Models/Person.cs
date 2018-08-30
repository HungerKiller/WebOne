using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnePiece.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    /// <summary>
    /// 人物
    /// </summary>
    public class Person
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

        [DataType(DataType.Currency)]
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

        [JsonIgnore]
        [Display(Name = "Pirate Group")]
        // 知识点1：默认情况下，含有成员object PirateGroup，EF就会自动创建PirateGroupID。这里显式的创建成员int PirateGroupID，就是为了更方便的使用
        // 知识点2：WeaponPossession里，插入Id和object就可以，但是这里就会出错
        // "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Person_PirateGroup_PirateGroupID\". The conflict occurred in database \"OnePiece\", table \"dbo.PirateGroup\", column 'PirateGroupID'.\r\nThe statement has been terminated."
        // 是因为WeaponPossession里PersonID和WeaponID共同构成主键，所以这两个字段不可能是null的。
        // 而这里PirateGroupID是外键，EF默认它不是nullable的，所以new一个Person对象，但是却没有指定对应的PirateGroupID和PirateGroup，就会出错。
        // 所以如果你需要PirateGroup可以为null，那么则定义PirateGroupID为int?。
        // 否则定义PirateGroupID为int，但是不管是DataIntializer还是Create/Edit方法，都要注意PirateGroup必须有值。
        public int? PirateGroupID { get; set; }

        [Display(Name = "Pirate Group")]
        /// <summary>
        /// 海贼团
        /// </summary>
        public PirateGroup PirateGroup { get; set; }

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

        [Display(Name = "Weapons")]
        /// <summary>
        /// 武器。一个人可以有多个武器
        /// </summary>
        public ICollection<WeaponPossession> WeaponPossessions { get; set; }
    }
}
