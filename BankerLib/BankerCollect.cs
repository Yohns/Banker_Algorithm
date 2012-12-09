using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankerLib
{
    public class BankerCollect : CollectionBase
    {
        public Banker this[int index]
        {
            get
            {
                return ((Banker)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(Banker value)
        {
            return (List.Add(value));
        }

        public int IndexOf(Banker value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, Banker value)
        {
            List.Insert(index, value);
        }

        public void Remove(Banker value)
        {
            List.Remove(value);
        }

        public bool Contains(Banker value)
        {
            // If value is not of type Int16, this will return false.
            return (List.Contains(value));
        }

        protected override void OnInsert(int index, Object value)
        {
            // Insert additional code to be run only when inserting values.
        }

        protected override void OnRemove(int index, Object value)
        {
            // Insert additional code to be run only when removing values.
        }

        protected override void OnSet(int index, Object oldValue, Object newValue)
        {
            // Insert additional code to be run only when setting values.
        }

        protected override void OnValidate(Object value)
        {
            if (value.GetType() != typeof(Banker))
                throw new ArgumentException("value must be of type Banker.", "value");
        }

        public Object Clone()
        {
            BankerCollect newBC = new BankerCollect();
            for (int i = 0; i < this.Count; i++)
            {
                newBC.Add(this[i].Clone());
            }
            return newBC;
        }
    }
}
