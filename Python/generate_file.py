import random

persons = []

class Person:
    def __init__(self, name, gender, age, enable):
        self.id = len(persons)
        self.name = name
        self.gender = gender
        self.age = age
        self.enable = enable
        self.edu_exp = []
        self.live_exp = []
        self.work_exp = []
        self.friends = set()
        persons.append(self)

    def make_friend(self, person):
        self.friends.add(person)
        person.friends.add(self)

class Experience:
    def __init__(self, begin_time, end_time, value, owner):
        self.begin_time = begin_time
        self.end_time = end_time
        self.value = value
        self.owner = owner


class Group:
    def __init__(self, name):
        self.name = name
        self.member = set()


chinese_num = ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十']


def load_file(path):
    fin = open(path, 'r')
    lines = fin.readlines()
    fin.close()
    valid_lines = []
    for line in lines:
        valid_lines.append(line[:-1])

    return valid_lines


if __name__ == '__main__':
    num = int(input("Person count:\n"))

    cities = load_file('Resource/city.txt')
    male_names = load_file('Resource/male_name.txt')
    female_names = load_file('Resource/female_name.txt')
    universities = load_file('Resource/university.txt')
    workplaces = load_file('Resource/workplace.txt')
    group_names = ['游戏群#', '工作群#', '地域群#', '粉丝群#', '家族群#']
    for i in range(50):
        workplaces.append(cities[random.randint(0, len(cities) - 1)] + "人民政府")

    for i in range(num):
        if random.randint(0, 1) == 1:
            person = Person(male_names[random.randint(0, len(male_names) - 1)], '男', int(random.uniform(10, 60)),
                            random.randint(0, 99) < 80)
        else:
            person = Person(female_names[random.randint(0, len(female_names) - 1)], '女', int(random.uniform(10, 60)),
                            random.randint(0, 99) < 80)

        person.homeland = cities[random.randint(0, len(cities) - 1)]
        person.birth_month = random.randint(0, 11) + 24252 - 12 * person.age
        current_month = person.birth_month + 84 + random.randint(0, 11)

        # edu & live exp
        if random.randint(0, 99) < 90:
            # primary school
            next_month = current_month + 72
            if random.randint(0, 99) < 90:
                # local primary school
                experience = Experience(current_month, next_month,
                                        person.homeland + '第' + chinese_num[random.randint(0, 9)] + '小学', i)
                live_experience = Experience(person.birth_month, next_month, person.homeland, i)
            else:
                # primary school in another city
                city_idx = random.randint(0, len(cities) - 1)
                experience = Experience(current_month, next_month,
                                        cities[city_idx] + '第' + chinese_num[random.randint(0, 9)] + '小学', i)
                live_experience = Experience(current_month, next_month, cities[city_idx], i)
            person.edu_exp.append(experience)
            person.live_exp.append(live_experience)
            current_month = next_month

            if random.randint(0, 99) < 80:
                # high school
                next_month = current_month + random.randint(30, 72)
                if random.randint(0, 99) < 80:
                    # local
                    experience = Experience(current_month, next_month,
                                            person.homeland + '第' + chinese_num[random.randint(0, 9)] + '中学', i)
                    live_experience = Experience(current_month, next_month, person.homeland, i)
                else:
                    # another city
                    city_idx = random.randint(0, len(cities) - 1)
                    experience = Experience(current_month, next_month,
                                            cities[city_idx] + '第' + chinese_num[random.randint(0, 9)] + '中学', i)
                    live_experience = Experience(current_month, next_month, cities[city_idx], i)
                person.edu_exp.append(experience)
                person.live_exp.append(live_experience)
                current_month = next_month + random.randint(0, 6)

                if random.randint(0, 99) < 50:
                    # university
                    next_month = current_month + random.randint(48, 120)
                    experience = Experience(current_month, next_month, universities[random.randint(0, len(universities) - 1)], i)

                    city_idx = random.randint(0, len(cities) - 1)
                    live_experience = Experience(current_month, next_month, cities[city_idx], i)
                    current_month = next_month + random.randint(0, 60)
                    person.edu_exp.append(experience)
                    person.live_exp.append(live_experience)

            else:
                # quit in high school
                current_month += random.randint(30, 72)

        else:
            current_month += 72

        # work exp
        exit_prob = 30
        for time in range(5):
            if random.randint(0, 99) < exit_prob:
                break
            next_month = current_month + random.randint(6, 180)
            work_experience = Experience(current_month, next_month, workplaces[random.randint(0, len(workplaces) - 1)], i)
            live_experience = Experience(current_month, next_month, cities[random.randint(0, len(cities) - 1)], i)
            current_month = next_month
            current_month += random.randint(0, 24)
            person.live_exp.append(live_experience)
            person.work_exp.append(work_experience)

            exit_prob += 30

    # make friends
    for person in persons:
        friend_target_count = int(random.uniform(0, 20))
        for idx in range(friend_target_count):
            if person.id < len(persons) - 1:
                friend_idx = random.randint(person.id + 1, len(persons) - 1)
                person.make_friend(persons[friend_idx])

    group_count = random.randint(num // 20, num // 4)
    groups = []
    for i in range(group_count):
        group = Group(group_names[random.randint(0, 4)] + str(random.randint(0, 9999)))

        group_member_count = random.randint(10, 20)
        for mem in range(group_member_count):
            group.member.add(random.randint(0, len(persons) - 1))

        groups.append(group)

    # write to file
    lines = ['   ']

    for person in persons:
        lines.append(person.name + ' ' + person.gender + ' ' + str(person.age) + ' ' + ('True' if person.enable else 'False') + '\n')

    for group in groups:
        group_str = group.name
        for person in group.member:
            group_str += ' ' + str(person)

        lines.append(group_str + '\n')

    live_count = 0
    for person in persons:
        for live_exp in person.live_exp:
            lines.append(str(live_exp.begin_time) + ' ' + str(live_exp.end_time) + ' ' + live_exp.value + ' ' + str(live_exp.owner) + '\n')
            live_count += 1

    edu_count = 0
    for person in persons:
        for edu_exp in person.edu_exp:
            lines.append(str(edu_exp.begin_time) + ' ' + str(edu_exp.end_time) + ' ' + edu_exp.value + ' ' + str(
                edu_exp.owner) + '\n')
            edu_count += 1

    work_count = 0
    for person in persons:
        for work_exp in person.work_exp:
            lines.append(str(work_exp.begin_time) + ' ' + str(work_exp.end_time) + ' ' + work_exp.value + ' ' + str(
                work_exp.owner) + '\n')
            work_count += 1

    friend_count = 0
    for person in persons:
        for friend in person.friends:
            if friend.id > person.id:
                lines.append(str(person.id) + ' ' + str(friend.id) + '\n')
                friend_count += 1

    first_line = str(len(persons)) + ' ' + str(len(groups)) + ' ' + str(live_count) + ' ' + str(edu_count) + ' ' + str(work_count) + ' ' + str(friend_count) + '\n'
    lines[0] = first_line

    lines[-1] = lines[-1][:-1]

    file_out = open('Result.txt', 'w', encoding='utf-8')
    file_out.writelines(lines)
    file_out.close()
