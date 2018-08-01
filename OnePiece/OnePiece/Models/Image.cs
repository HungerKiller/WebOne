namespace OnePiece.Models
{
    /// <summary>
    /// 图片
    /// </summary>
    public class Image
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
        /// 文件路径
        /// </summary>
        public string Path { get; set; }
    }
}
