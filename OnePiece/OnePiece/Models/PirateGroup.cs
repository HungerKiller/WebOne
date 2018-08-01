using System.Collections.Generic;

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

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 船员。多个船员
        /// </summary>
        public ICollection<Person> Peoples { get; set; }

        /// <summary>
        /// 海贼旗
        /// </summary>
        public Image Flag { get; set; }
    }
}
