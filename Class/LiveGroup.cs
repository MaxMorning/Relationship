using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public class LiveGroup
    {
        public static List<LiveGroup> liveGroups = new List<LiveGroup>();
        public int stage;
        public List<LiveGroup> childGroups = new List<LiveGroup>();
    }
}
