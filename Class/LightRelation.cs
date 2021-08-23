using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relationship.Widget;

namespace Relationship.Class
{
    class LightRelation
    {
        public Person person0;
        public Person person1;
        public double rate;
        public string relationName;

        public LightRelation(Person person0, Person person1, double rate, string relationName)
        {
            if (person0.id > person1.id)
            {
                this.person0 = person1;
                this.person1 = person0;
            }
            else
            {
                this.person0 = person0;
                this.person1 = person1;
            }
            this.rate = rate;
            this.relationName = relationName;
        }

        public RelationLine ConvertToWidget()
        {
            return new RelationLine(person0.relatedDot, person1.relatedDot, rate, relationName);
        }
    }
}
