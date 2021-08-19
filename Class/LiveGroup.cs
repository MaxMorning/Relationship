using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public class LiveGroup : Group
    {
        public List<Experience> relatedExp = new List<Experience>();

        public new const string relationName = "同乡";
        public static Dictionary<string, LiveGroup> liveGroups = new Dictionary<string, LiveGroup>();
    }
}
