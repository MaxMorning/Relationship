using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public class Experience
    {
        public int beginMonthIdx = 24019;
        public int endMonthIdx = 24235;
        public Group relatedGroup;
        public Person owner;

        public static int Compare(Experience experience0, Experience experience1)
        {
            if (experience0.beginMonthIdx < experience1.beginMonthIdx)
            {
                return -1;
            }
            else if (experience0.beginMonthIdx == experience1.beginMonthIdx)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public bool HaveRelation(Experience experience)
        {
            if (beginMonthIdx >= experience.beginMonthIdx && beginMonthIdx <= experience.endMonthIdx)
            {
                return true;
            }

            if (endMonthIdx >= experience.beginMonthIdx && endMonthIdx <= experience.endMonthIdx)
            {
                return true;
            }

            if (beginMonthIdx <= experience.beginMonthIdx && endMonthIdx >= experience.endMonthIdx)
            {
                return true;
            }

            return false;
        }
    }
}
