using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relationship.Widget;

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
        public static double friendRate = 0.9;
        public static double groupmateRate = 0.8;
        public static double schoolmateRate = 0.7;
        public static double colleagueRate = 0.6;
        public static double citizenRate = 0.4;

        public Person(string name)
        {
            this.name = name;
            enable = true;
            gender = "未知";
            age = 20;
            this.id = Person.persons.Count;
            Person.persons.Add(this);
        }

        public void JoinSocialGroup(SocialGroup socialGroup)
        {
            socialGroups.Add(socialGroup);
            socialGroup.members.Add(this);
        }

        public void QuitSocialGroup(SocialGroup socialGroup)
        {
            socialGroups.Remove(socialGroup);
            socialGroup.members.Remove(this);
        }

        public void MakeFriend(Person newFriend)
        {
            this.friends.Add(newFriend);
            newFriend.friends.Add(this);
        }

        public void BreakUpFriend(Person newFriend)
        {
            this.friends.Remove(newFriend);
            newFriend.friends.Remove(this);
        }

        public void SortExp(List<Experience> list)
        {
            list.Sort(Experience.Compare);
        }

        public void RepaintExp(int expType)
        {
            switch (expType)
            {
                case 0:
                    {
                        MainWindow.mainWindow.spInfoLive.Children.Clear();
                        foreach (Experience experience in liveExp)
                        {
                            MainWindow.mainWindow.spInfoLive.Children.Add(new ExpRecordGrid(experience, 0));
                        }
                        break;
                    }

                case 1:
                    {
                        MainWindow.mainWindow.spInfoEdu.Children.Clear();
                        foreach (Experience experience in eduExp)
                        {
                            MainWindow.mainWindow.spInfoEdu.Children.Add(new ExpRecordGrid(experience, 1));
                        }
                        break;
                    }

                case 2:
                    {
                        MainWindow.mainWindow.spInfoWork.Children.Clear();
                        foreach (Experience experience in workExp)
                        {
                            MainWindow.mainWindow.spInfoWork.Children.Add(new ExpRecordGrid(experience, 2));
                        }
                        break;
                    }
            }
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
                    latestEdu = experience.relatedGroup.name;
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
                    latestWork = experience.relatedGroup.name;
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
                    latestLive = experience.relatedGroup.name;
                }
            }

            monthIdx = maxMonth;
            return latestLive;
        }

        public static int Compare(Person person0, Person person1)
        {
            if (person0.id < person1.id)
            {
                return -1;
            }
            else if (person0.id == person1.id)
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
