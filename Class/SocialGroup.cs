using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public class SocialGroup : Group
    {
        public List<Person> members = new List<Person>();
        
        public static List<SocialGroup> socialGroups = new List<SocialGroup>();
        public int id;
        public new const string relationName = "群友";

        public SocialGroup(string name)
        {
            this.name = name;
            this.id = SocialGroup.socialGroups.Count;
            SocialGroup.socialGroups.Add(this);
        }
    }
}
