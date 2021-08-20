using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
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

        // temp sets
        public HashSet<Person> tempSchoolmates = null;
        public HashSet<Person> tempColleagues = null;
        public HashSet<Person> tempCitizens = null;

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

        public List<Person> PossibleFriend(out List<int> commonFriendCount)
        {
            List<Person> notFriend = new List<Person>(Person.persons.Count - friends.Count);
            commonFriendCount = new List<int>(Person.persons.Count - friends.Count);

            int friendListIdx = 0;
            for (int idx = 0; idx < Person.persons.Count; ++idx)
            {
                if (friendListIdx < friends.Count && friends[friendListIdx].id == idx)
                {
                    ++friendListIdx;
                }
                else if (Person.persons[idx].enable && Person.persons[idx] != this)
                {
                    notFriend.Add(Person.persons[idx]);
                }
            }

            for (int i = 0; i < notFriend.Count; ++i)
            {
                commonFriendCount.Add(GetCommonFriendsCount(notFriend[i]));
            }
            return notFriend;
        }

        public int GetCommonFriendsCount(Person person)
        {
            int commonCount = 0;
            int thisIdx = 0;
            int personIdx = 0;

            while (thisIdx < friends.Count && personIdx < person.friends.Count)
            {
                if (friends[thisIdx] == person.friends[personIdx])
                {
                    ++commonCount;
                    ++thisIdx;
                    ++personIdx;
                }
                else
                {
                    if (friends[thisIdx].id < person.friends[personIdx].id)
                    {
                        ++thisIdx;
                    }
                    else
                    {
                        ++personIdx;
                    }
                }
            }

            return commonCount;
        }

        public static Label GetRelationLabel(string relationship)
        {
            /*
             * HorizontalContentAlignment="Center"
                                VerticalContentAlignment="Center"
                                FontSize="20"
                                FontFamily="微软雅黑"
                                FontWeight="Normal"
                                Foreground="#555555"
                                Margin="0 5"
                                Content="⇓ 好友">
             */
            Label label = new Label();

            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            label.FontSize = 20;
            label.FontFamily = new System.Windows.Media.FontFamily("微软雅黑");
            label.Foreground = new SolidColorBrush(Color.FromRgb(85, 85, 85));
            label.Margin = new System.Windows.Thickness(0, 5, 0, 5);
            label.Content = "⇓ " + relationship;

            return label;
        }

        public HashSet<Person> GetRelatedSchoolmates()
        {
            if (tempSchoolmates != null)
            {
                return tempSchoolmates;
            }

            HashSet<Person> relatedSchoolmates = new HashSet<Person>();

            for (int i = 0; i < this.eduExp.Count; ++i)
            {
                EduGroup relatedGroup = (EduGroup)this.eduExp[i].relatedGroup;
                for (int j = 0; j < relatedGroup.relatedExp.Count; ++j)
                {
                    if (relatedGroup.relatedExp[j].owner.enable && this.eduExp[i].HaveRelation(relatedGroup.relatedExp[j]))
                    {
                        relatedSchoolmates.Add(relatedGroup.relatedExp[j].owner);
                    }
                }
            }

            relatedSchoolmates.Remove(this);

            tempSchoolmates = relatedSchoolmates;
            return relatedSchoolmates;
        }

        public HashSet<Person> GetRelatedColleagues()
        {
            if (tempColleagues != null)
            {
                return tempColleagues;
            }

            HashSet<Person> relatedColleagues = new HashSet<Person>();

            for (int i = 0; i < this.workExp.Count; ++i)
            {
                WorkGroup relatedGroup = (WorkGroup)this.workExp[i].relatedGroup;
                for (int j = 0; j < relatedGroup.relatedExp.Count; ++j)
                {
                    if (relatedGroup.relatedExp[j].owner.enable && this.workExp[i].HaveRelation(relatedGroup.relatedExp[j]))
                    {
                        relatedColleagues.Add(relatedGroup.relatedExp[j].owner);
                    }
                }
            }

            relatedColleagues.Remove(this);

            tempColleagues = relatedColleagues;
            return relatedColleagues;
        }

        public HashSet<Person> GetRelatedCitizens()
        {
            if (tempCitizens != null)
            {
                return tempCitizens;
            }

            HashSet<Person> relatedCitizens = new HashSet<Person>();

            for (int i = 0; i < this.liveExp.Count; ++i)
            {
                LiveGroup relatedGroup = (LiveGroup)this.liveExp[i].relatedGroup;
                for (int j = 0; j < relatedGroup.relatedExp.Count; ++j)
                {
                    if (relatedGroup.relatedExp[j].owner.enable && this.liveExp[i].HaveRelation(relatedGroup.relatedExp[j]))
                    {
                        relatedCitizens.Add(relatedGroup.relatedExp[j].owner);
                    }
                }
            }

            relatedCitizens.Remove(this);

            tempCitizens = relatedCitizens;
            return relatedCitizens;
        }

        // Dijkstra Algorithm find shortest path
        private class Record
        {
            public bool isSelected = false;
            public double currentCost = 0;
            public int currentFrom = -1;
            public string currentFromRelation;
        }

        public List<Person> GetRelationship(Person targetPerson, out List<string> relations)
        {
            List<Person> relationChain = new List<Person>();
            relations = new List<string>();

            // Algorithm begin
            Record[] records = new Record[Person.persons.Count];

            // init
            for (int i = 0; i < records.Length; ++i)
            {
                records[i] = new Record()
                {
                    currentFrom = this.id
                };
                Person.persons[i].tempSchoolmates = null;
                Person.persons[i].tempColleagues = null;
                Person.persons[i].tempCitizens = null;
            }
            records[this.id].isSelected = true;
            records[this.id].currentCost = 1;
            records[this.id].currentFrom = -1;
            records[this.id].currentFromRelation = "";

            // init friends part
            for (int i = 0; i < this.friends.Count; ++i)
            {
                if (records[this.friends[i].id].currentCost < friendRate)
                {
                    records[this.friends[i].id].currentCost = friendRate;
                    records[this.friends[i].id].currentFromRelation = "好友";
                }
            }

            // init edu part
            HashSet<Person> relatedSchoolmates = GetRelatedSchoolmates();
            foreach (Person person in relatedSchoolmates)
            {
                if (records[person.id].currentCost < schoolmateRate)
                {
                    records[person.id].currentCost = schoolmateRate;
                    records[person.id].currentFromRelation = "校友";
                }
            }

            // init work part
            HashSet<Person> relatedColleagues = GetRelatedColleagues();
            foreach (Person person in relatedColleagues)
            {
                if (records[person.id].currentCost < colleagueRate)
                {
                    records[person.id].currentCost = colleagueRate;
                    records[person.id].currentFromRelation = "同事";
                }
            }

            // init live part
            HashSet<Person> relatedCitizens = GetRelatedCitizens();
            foreach (Person person in relatedCitizens)
            {
                if (records[person.id].currentCost < citizenRate)
                {
                    records[person.id].currentCost = citizenRate;
                    records[person.id].currentFromRelation = "同乡";
                }
            }

            // init end

            // begin search
            while (!records[targetPerson.id].isSelected)
            {
                double maxCost = 0;
                int maxIdx = -1;

                for (int i = 0; i < records.Length; ++i)
                {
                    if (!records[i].isSelected && records[i].currentCost > maxCost)
                    {
                        maxCost = records[i].currentCost;
                        maxIdx = i;
                    }
                }

                if (maxIdx == -1)
                {
                    return null;
                }
                records[maxIdx].isSelected = true;

                // ease
                Person maxPerson = Person.persons[maxIdx];

                // ease friends
                for (int i = 0; i < maxPerson.friends.Count; ++i)
                {
                    Record record = records[maxPerson.friends[i].id];
                    if (!record.isSelected && record.currentCost < maxCost * friendRate)
                    {
                        record.currentCost = maxCost * friendRate;
                        record.currentFrom = maxIdx;
                        record.currentFromRelation = "好友";
                    }
                }

                // ease schoolmates
                HashSet<Person> maxRelatedSchoolmates = maxPerson.GetRelatedSchoolmates();
                foreach (Person person in maxRelatedSchoolmates)
                {
                    Record record = records[person.id];
                    if (!record.isSelected && record.currentCost < maxCost * schoolmateRate)
                    {
                        record.currentCost = maxCost * schoolmateRate;
                        record.currentFrom = maxIdx;
                        record.currentFromRelation = "校友";
                    }
                }

                // ease colleagues
                HashSet<Person> maxRelatedColleagues = maxPerson.GetRelatedColleagues();
                foreach (Person person in maxRelatedColleagues)
                {
                    Record record = records[person.id];
                    if (!record.isSelected && record.currentCost < maxCost * colleagueRate)
                    {
                        record.currentCost = maxCost * colleagueRate;
                        record.currentFrom = maxIdx;
                        record.currentFromRelation = "同事";
                    }
                }

                // ease citizens
                HashSet<Person> maxRelatedCitizens = maxPerson.GetRelatedCitizens();
                foreach (Person person in maxRelatedCitizens)
                {
                    Record record = records[person.id];
                    if (!record.isSelected && record.currentCost < maxCost * citizenRate)
                    {
                        record.currentCost = maxCost * citizenRate;
                        record.currentFrom = maxIdx;
                        record.currentFromRelation = "同乡";
                    }
                }
            }
            // search done

            // reverse
            int currentPersonIdx = targetPerson.id;
            while (currentPersonIdx >= 0)
            {
                relationChain.Add(Person.persons[currentPersonIdx]);
                relations.Add(records[currentPersonIdx].currentFromRelation);
                currentPersonIdx = records[currentPersonIdx].currentFrom;
            }
            relationChain.Reverse();
            relations.Reverse();

            return relationChain;
        }
    }
}
