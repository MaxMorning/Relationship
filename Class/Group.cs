using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public abstract class Group
    {
        public int id;
        public string name;
        public List<Experience> relatedExp = new List<Experience>();
        public string relationName;
    }
}
