using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public class Person
    {
        public int id;
        public bool enable = true;
        public string name;
        public string gender;
        public int age;
        public List<Person> friends = new List<Person>();
        public List<Experience> liveExp = new List<Experience>();
        public List<Experience> eduExp = new List<Experience>();
        public List<Experience> workExp = new List<Experience>();
        public List<SocialGroup> socialGroups = new List<SocialGroup>();

        public static List<Person> persons = new List<Person>();

        public Person(string name)
        {
            this.name = name;
            enable = true;
            gender = "未知";
            age = 20;
            this.id = Person.persons.Count;
            Person.persons.Add(this);
        }

        public string GetRecentEdu(out int monthIdx)
        {
            int maxMonth = -2147483647;
            string latestEdu = "无";
            foreach (Experience experience in eduExp)
            {
                if (experience.endMonthIdx > maxMonth)
                {
                    maxMonth = experience.endMonthIdx;
                    latestEdu = experience.value;
                }
            }

            monthIdx = maxMonth;
            return latestEdu;
        }

        public string GetRecentWork(out int monthIdx)
        {
            int maxMonth = -2147483647;
            string latestWork = "无";
            foreach (Experience experience in workExp)
            {
                if (experience.endMonthIdx > maxMonth)
                {
                    maxMonth = experience.endMonthIdx;
                    latestWork = experience.value;
                }
            }

            monthIdx = maxMonth;
            return latestWork;
        }

        public string GetRecentLive(out int monthIdx)
        {
            int maxMonth = -2147483647;
            string latestLive = "无";
            foreach (Experience experience in liveExp)
            {
                if (experience.endMonthIdx > maxMonth)
                {
                    maxMonth = experience.endMonthIdx;
                    latestLive = experience.value;
                }
            }

            monthIdx = maxMonth;
            return latestLive;
        }
    }
}
