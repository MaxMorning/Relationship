using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relationship.Class
{
    public static class Database
    {
        public static void LoadProperties()
        {
            try
            {
                FileStream fileInStream = new FileStream("properties.prop", FileMode.Open);
                long fileLength = fileInStream.Length;
                byte[] bufferByte = new byte[fileLength];
                fileInStream.Read(bufferByte, 0, (int)fileLength);
                fileInStream.Close();

                string buffer = System.Text.Encoding.ASCII.GetString(bufferByte);
                string[] decodeResult = buffer.Split(new char[] { ' ', '=', '\n' });
                List<string> realResult = new List<string>();
                for (int idx = 0; idx < decodeResult.Length; ++idx)
                {
                    if (decodeResult[idx].Length != 0)
                    {
                        realResult.Add(decodeResult[idx]);
                    }
                }
                for (int i = 0; i < realResult.Count; i += 2)
                {
                    switch (realResult[i])
                    {
                        case "ThreadNum":
                            {
                                MainWindow.THREAD_NUM = int.Parse(realResult[i + 1]);
                                break;
                            }

                        case "FriendRate":
                            {
                                Person.friendRate = double.Parse(realResult[i + 1]);
                                break;
                            }

                        case "GroupmateRate":
                            {
                                Person.groupmateRate = double.Parse(realResult[i + 1]);
                                break;
                            }

                        case "SchoolmateRate":
                            {
                                Person.schoolmateRate = double.Parse(realResult[i + 1]);
                                break;
                            }

                        case "ColleagueRate":
                            {
                                Person.colleagueRate = double.Parse(realResult[i + 1]);
                                break;
                            }

                        case "CitizenRate":
                            {
                                Person.citizenRate = double.Parse(realResult[i + 1]);
                                break;
                            }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static bool ParseFile(string filePath)
        {
            try
            {
                FileStream fileInStream = new FileStream(filePath, FileMode.Open);
                long fileLength = fileInStream.Length;
                byte[] bufferByte = new byte[fileLength];
                fileInStream.Read(bufferByte, 0, (int)fileLength);
                fileInStream.Close();

                string buffer = System.Text.Encoding.UTF8.GetString(bufferByte);
                string[] decodeResult = buffer.Split(new char[] { '\n' });
                
                int personCount, groupCount, liveExpCount, eduExpCount, workExpCount, friendPairCount;
                string[] totalCounts = decodeResult[0].Split(new char[] { ' ' });
                personCount = int.Parse(totalCounts[0]);
                groupCount = int.Parse(totalCounts[1]);
                liveExpCount = int.Parse(totalCounts[2]);
                eduExpCount = int.Parse(totalCounts[3]);
                workExpCount = int.Parse(totalCounts[4]);
                friendPairCount = int.Parse(totalCounts[5]);

                int beginCnt = 1;
                int targetCnt = personCount + 1;
                // person part
                Person.persons.Clear();
                for (int i = beginCnt; i < targetCnt; ++i)
                {
                    string[] personStr = decodeResult[i].Split(new char[] { ' ' });
                    Person person = new Person(personStr[0]);
                    person.gender = personStr[1];
                    person.age = int.Parse(personStr[2]);
                    person.enable = bool.Parse(personStr[3]);
                }

                // group part
                beginCnt = targetCnt;
                targetCnt += groupCount;
                SocialGroup.socialGroups.Clear();
                for (int i = beginCnt; i < targetCnt; ++i)
                {
                    string[] socialGroupStr = decodeResult[i].Split(new char[] { ' ' });
                    SocialGroup socialGroup = new SocialGroup(socialGroupStr[0]);

                    for (int j = 1; j < socialGroupStr.Length; ++j)
                    {
                        int index = int.Parse(socialGroupStr[j]);
                        Person.persons[index].JoinSocialGroup(socialGroup);
                    }
                }

                // live exp part
                beginCnt = targetCnt;
                targetCnt += liveExpCount;
                LiveGroup.liveGroups.Clear();
                for (int i = beginCnt; i < targetCnt; ++i)
                {
                    string[] liveGroupStr = decodeResult[i].Split(new char[] { ' ' });
                    Experience experience = new Experience()
                    {
                        beginMonthIdx = int.Parse(liveGroupStr[0]),
                        endMonthIdx = int.Parse(liveGroupStr[1])
                    };

                    bool getSuccess = LiveGroup.liveGroups.TryGetValue(liveGroupStr[2], out LiveGroup liveGroup);
                    int personIdx = int.Parse(liveGroupStr[3]);
                    Person.persons[personIdx].liveExp.Add(experience);
                    experience.owner = Person.persons[personIdx];

                    if (!getSuccess)
                    {
                        liveGroup = new LiveGroup();
                        liveGroup.name = liveGroupStr[2];
                        LiveGroup.liveGroups.Add(liveGroupStr[2], liveGroup);
                    }

                    experience.relatedGroup = liveGroup;
                    liveGroup.relatedExp.Add(experience);
                }

                // edu exp part
                beginCnt = targetCnt;
                targetCnt += eduExpCount;
                EduGroup.eduGroups.Clear();
                for (int i = beginCnt; i < targetCnt; ++i)
                {
                    string[] eduGroupStr = decodeResult[i].Split(new char[] { ' ' });
                    Experience experience = new Experience()
                    {
                        beginMonthIdx = int.Parse(eduGroupStr[0]),
                        endMonthIdx = int.Parse(eduGroupStr[1])
                    };

                    bool getSuccess = EduGroup.eduGroups.TryGetValue(eduGroupStr[2], out EduGroup eduGroup);
                    int personIdx = int.Parse(eduGroupStr[3]);
                    Person.persons[personIdx].eduExp.Add(experience);
                    experience.owner = Person.persons[personIdx];

                    if (!getSuccess)
                    {
                        eduGroup = new EduGroup();
                        eduGroup.name = eduGroupStr[2];
                        EduGroup.eduGroups.Add(eduGroupStr[2], eduGroup);
                    }

                    experience.relatedGroup = eduGroup;
                    eduGroup.relatedExp.Add(experience);
                }

                // work group part
                beginCnt = targetCnt;
                targetCnt += workExpCount;
                WorkGroup.workGroups.Clear();
                for (int i = beginCnt; i < targetCnt; ++i)
                {
                    string[] workGroupStr = decodeResult[i].Split(new char[] { ' ' });
                    Experience experience = new Experience()
                    {
                        beginMonthIdx = int.Parse(workGroupStr[0]),
                        endMonthIdx = int.Parse(workGroupStr[1])
                    };

                    bool getSuccess = WorkGroup.workGroups.TryGetValue(workGroupStr[2], out WorkGroup workGroup);
                    int personIdx = int.Parse(workGroupStr[3]);
                    Person.persons[personIdx].workExp.Add(experience);
                    experience.owner = Person.persons[personIdx];

                    if (!getSuccess)
                    {
                        workGroup = new WorkGroup();
                        workGroup.name = workGroupStr[2];
                        WorkGroup.workGroups.Add(workGroupStr[2], workGroup);
                    }

                    experience.relatedGroup = workGroup;
                    workGroup.relatedExp.Add(experience);
                }

                // friend pair part
                beginCnt = targetCnt;
                targetCnt += friendPairCount;
                for (int i = beginCnt; i < targetCnt; ++i)
                {
                    string[] friendPairStr = decodeResult[i].Split(new char[] { ' ' });
                    int person0Idx = int.Parse(friendPairStr[0]);
                    int person1Idx = int.Parse(friendPairStr[1]);
                    Person.persons[person0Idx].MakeFriend(Person.persons[person1Idx]);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool StoreToFile(string filePath)
        {
            try
            {
                int personCount = Person.persons.Count;
                int groupCount = SocialGroup.socialGroups.Count;
                int liveExpCount = 0;
                int eduExpCount = 0;
                int workExpCount = 0;
                int friendPairCount = 0;

                List<string> allStr = new List<string>();
                for (int i = 0; i < personCount; ++i)
                {
                    allStr.Add(Person.persons[i].ToString());
                }

                for (int i = 0; i < groupCount; ++i)
                {
                    allStr.Add(SocialGroup.socialGroups[i].ToString());
                }
                // store live exp
                for (int i = 0; i < personCount; ++i)
                {
                    for (int j = 0; j < Person.persons[i].liveExp.Count; ++j)
                    {
                        ++liveExpCount;
                        allStr.Add(Person.persons[i].liveExp[j].ToString());
                    }
                }

                // store edu exp
                for (int i = 0; i < personCount; ++i)
                {
                    for (int j = 0; j < Person.persons[i].eduExp.Count; ++j)
                    {
                        ++eduExpCount;
                        allStr.Add(Person.persons[i].eduExp[j].ToString());
                    }
                }

                // store work exp
                for (int i = 0; i < personCount; ++i)
                {
                    for (int j = 0; j < Person.persons[i].workExp.Count; ++j)
                    {
                        ++workExpCount;
                        allStr.Add(Person.persons[i].workExp[j].ToString());
                    }
                }

                // friend pairs
                for (int i = 0; i < personCount; ++i)
                {
                    for (int j = 0; j < Person.persons[i].friends.Count; ++j)
                    {
                        if (i < Person.persons[i].friends[j].id)
                        {
                            ++friendPairCount;
                            allStr.Add(i.ToString() + " " + Person.persons[i].friends[j].id.ToString());
                        }
                    }
                }

                string finalStr = string.Format("{0} {1} {2} {3} {4} {5}\n", personCount, groupCount, liveExpCount, eduExpCount, workExpCount, friendPairCount);
                for (int i = 0; i < allStr.Count; ++i)
                {
                    finalStr += allStr[i] + '\n';
                }

                byte[] tempBytes = Encoding.UTF8.GetBytes(finalStr);
                FileStream fileOutStream = new FileStream(filePath, FileMode.Create);
                fileOutStream.Write(tempBytes, 0, tempBytes.Length - 1);
                fileOutStream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
