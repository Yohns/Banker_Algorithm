using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankerLib
{
    public class Banker
    {
        #region Field
        private List<int> _claim;

        private List<int> _need;

        private List<int> _allocation;

        private bool _isDone;
        #endregion

        #region Field Capsule
        /// <summary>
        /// Max claim resources
        /// </summary>
        public List<int> Claim
        {
            get { return _claim; }
        }

        /// <summary>
        /// resources need
        /// </summary>
        public List<int> Need
        {
            get { return _need; }
        }

        /// <summary>
        /// resources already have
        /// </summary>
        public List<int> Allocation
        {
            get { return _allocation; }
        }

        public bool IsDone
        {
            get { return _isDone; }
        }
        #endregion

        public Banker()
        {
            init();
        }

        private void init()
        {
            //Random random = new Random();
            do
            {
                _need = new List<int>(Data.ResCount);
                for (int i = 0; i < Data.ResCount; i++)
                {
                    int temp = Data.Random.Next(Data.ResLimitMax);
                    _need.Add(temp);

                }
            } while (checkIsDone());
            _allocation = new List<int>();
            _claim = new List<int>();
            for (int i = 0; i < Data.ResCount; i++)
            {
                _allocation.Add(0);
                _claim.Add(_need[i]);
            }
            _isDone = false;
        }

        public bool allocate(List<int> alRes)
        {
            List<int> tempA = new List<int>();
            List<int> tempN = new List<int>();
            for (int i = 0; i < Data.ResCount; i++)
            {
                if (alRes[i] > _need[i])
                    return false;
                else
                {
                    tempA.Add(_allocation[i] + alRes[i]);
                    tempN.Add(_need[i] - alRes[i]);
                }
            }
            _allocation = tempA;
            _need = tempN;
            _isDone = checkIsDone();
            return true;
        }

        private bool checkIsDone()
        {
            for (int i = 0; i < Data.ResCount; i++)
            {
                if (_need[i] > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public Banker Clone()
        {
            Banker newBanker = new Banker();
            newBanker._allocation = new List<int>();
            newBanker._claim = new List<int>();
            newBanker._need = new List<int>();
            for (int i = 0; i < Data.ResCount; i++)
            {
                newBanker._allocation.Add(this._allocation[i]);
                newBanker._claim.Add(this._claim[i]);
                newBanker._need.Add(this._need[i]);         
            }
            newBanker._isDone = this._isDone;
            return newBanker;
        }
    }
}
