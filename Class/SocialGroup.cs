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

        public override string ToString()
        {
            string retStr = name;
            for (int i = 0; i < members.Count; ++i)
            {
                retStr += " " + members[i].id.ToString();
            }
            return retStr;
        }

        public static int Compare(SocialGroup socialGroup0, SocialGroup socialGroup1)
        {
            if (socialGroup0.id < socialGroup1.id)
            {
                return -1;
            }
            else if (socialGroup0.id == socialGroup1.id)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
