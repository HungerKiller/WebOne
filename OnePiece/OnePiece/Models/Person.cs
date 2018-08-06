using System;
using System.Collections.Generic;

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

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 外号。不正规的称呼
        /// </summary>
        public string Nickname { get; set; }

        // TODO 注意数据库中可以存的最大长度
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 悬赏金（单位：贝利）
        /// </summary>
        public float RewardMoney { get; set; }

        /// <summary>
        /// 种族
        /// </summary>
        public Race Race { get; set; }
        
        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 所属势力
        /// </summary>
        public FeatureType FeatureType { get; set; }

        /// <summary>
        /// 称号/头衔
        /// </summary>
        public Title Title { get; set; }

        /// <summary>
        /// 海贼团
        /// </summary>
        public PirateGroup PirateGroup { get; set; }

        /// <summary>
        /// 果实。一个人可以有多个果实
        /// </summary>
        public ICollection<Fruit> Fruits { get; set; }

        /// <summary>
        /// 武器。一个人可以有多个武器
        /// </summary>
        public ICollection<Weapon> Weapons { get; set; }

        //TODO
        /// <summary>
        /// 一个人可以有多张照片。第一张作为头像。
        /// </summary>
        public string ImagePath { get; set; }
    }
}
