using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    class WorkGroup : Group
    {
        public List<Experience> relatedExp = new List<Experience>();

        public new const string relationName = "同事";
        public static Dictionary<string, WorkGroup> workGroups = new Dictionary<string, WorkGroup>();
    }
}
