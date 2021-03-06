﻿namespace OnePiece.Models
{
    /// <summary>
    /// 种族：人类，鱼人，动物，未知
    /// </summary>
    public enum Race
    {
        //Human,
        //Fishman,
        //Animal,
        //Unknown
        人类,
        鱼人,
        动物,
        未知
    }

    /// <summary>
    /// 性别：男，女，双性，未知
    /// </summary>
    public enum Sex
    {
        //Male,
        //Female,
        //Bisexual,
        //Unknown
        男,
        女,
        双性,
        未知
    }

    /// <summary>
    /// 所属势力：世界政府，海军，海贼，革命军，居民
    /// </summary>
    public enum FeatureType
    {
        //Government,
        //Navy,
        //Pirate,
        //Revolutionist,
        //CommonPeople
        世界政府,
        海军,
        海贼,
        革命军,
        居民
    }

    // TODO
    /// <summary>
    /// 称号/头衔：海军中将,海军大将,海军元帅,四皇,七武海
    /// </summary>
    public enum Title
    {
        海军中将,
        海军大将,
        海军元帅,
        四皇,
        七武海,
        海贼王,
        无
    }

    /// <summary>
    /// 果实种类：自然系,动物系,动物系幻兽种,超人系
    /// </summary>
    public enum FruitType
    {
        自然系,
        动物系,
        超人系,
        未知
    }
}
