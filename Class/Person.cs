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
            if (!this.socialGroups.Contains(socialGroup))
            {
                socialGroups.Add(socialGroup);
                socialGroup.members.Add(this);
            }
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
                        for (int idx = 0; idx < liveExp.Count; ++idx)
                        {
                            MainWindow.mainWindow.spInfoLive.Children.Add(new ExpRecordGrid(liveExp[idx], 0));
                        }
                        break;
                    }

                case 1:
                    {
                        MainWindow.mainWindow.spInfoEdu.Children.Clear();
                        for (int idx = 0; idx < eduExp.Count; ++idx)
                        {
                            MainWindow.mainWindow.spInfoEdu.Children.Add(new ExpRecordGrid(eduExp[idx], 1));
                        }
                        break;
                    }

                case 2:
                    {
                        MainWindow.mainWindow.spInfoWork.Children.Clear();
                        for (int idx = 0; idx < workExp.Count; ++idx)
                        {
                            MainWindow.mainWindow.spInfoWork.Children.Add(new ExpRecordGrid(workExp[idx], 2));
                        }
                        break;
                    }
            }
        }

        public string GetRecentEdu(out int monthIdx)
        {
            int maxMonth = -2147483647;
            string latestEdu = "无";
            for (int idx = 0; idx < eduExp.Count; ++idx)
            {
                if (eduExp[idx].endMonthIdx > maxMonth)
                {
                    maxMonth = eduExp[idx].endMonthIdx;
                    latestEdu = eduExp[idx].relatedGroup.name;
                }
            }

            monthIdx = maxMonth;
            return latestEdu;
        }

        public string GetRecentWork(out int monthIdx)
        {
            int maxMonth = -2147483647;
            string latestWork = "无";
            for (int idx = 0; idx < workExp.Count; ++idx)
            {
                if (workExp[idx].endMonthIdx > maxMonth)
                {
                    maxMonth = workExp[idx].endMonthIdx;
                    latestWork = workExp[idx].relatedGroup.name;
                }
            }

            monthIdx = maxMonth;
            return latestWork;
        }

        public string GetRecentLive(out int monthIdx)
        {
            int maxMonth = -2147483647;
            string latestLive = "无";
            for (int idx = 0; idx < liveExp.Count; ++idx)
            {
                if (liveExp[idx].endMonthIdx > maxMonth)
                {
                    maxMonth = liveExp[idx].endMonthIdx;
                    latestLive = liveExp[idx].relatedGroup.name;
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
