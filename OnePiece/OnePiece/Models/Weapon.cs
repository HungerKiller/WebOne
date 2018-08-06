namespace OnePiece.Models
{
    /// <summary>
    /// 武器
    /// </summary>
    public class Weapon
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
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 武器照片
        /// </summary>
        public string ImagePath { get; set; }
    }
}
