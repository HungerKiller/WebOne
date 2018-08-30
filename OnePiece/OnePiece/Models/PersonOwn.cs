using Newtonsoft.Json;
using System.Collections.Generic;

namespace OnePiece.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PersonOwn
    {
        public string PersonName { get; set; }

        public List<string> OwnedFruitNames { get; set; }

        public List<string> OwnedWeaponNames { get; set; }

        public string PirateGroupName { get; set; }
    }
}
