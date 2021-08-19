using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public class EduGroup : Group
    {
        public List<Experience> relatedExp = new List<Experience>();

        public new const string relationName = "同窗";
        public static Dictionary<string, EduGroup> eduGroups = new Dictionary<string, EduGroup>();
    }
}
