using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankerLib
{
    /// <summary>
    /// Control Banker algorithm
    /// </summary>
    public class BankerControl
    {
        #region Field
        private BankerCollect _bankerCollect;

        private int _count;

        private int _proIndex;

        private Banker _currentBanker;

        private List<int> _requestRes;

        private List<int> _avaliable;

        private List<Process> _waitQueue;

        private bool _isSafe;

        private int _unDoneCount;

        /// <summary>
        /// count of banker which has been finished
        /// </summary>
        public int UnDoneCount
        {
            get { return _unDoneCount; }
        }


        #endregion

        #region Field Capsule
        public BankerCollect BankerCollect
        {
            get { return _bankerCollect; }
        }

        /// <summary>
        /// process amount
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// current process index
        /// </summary>
        public int ProIndex
        {
            get { return _proIndex; }
        }

        /// <summary>
        /// a process requests resources
        /// </summary>
        public List<int> RequestRes
        {
            get { return _requestRes; }
        }

        /// <summary>
        /// current process
        /// </summary>
        public Banker CurrentBanker
        {
            get { return _currentBanker; }
        }

        /// <summary>
        /// avalible resources
        /// </summary>
        public List<int> Avaliable
        {
            get { return _avaliable; }
        }

        /// <summary>
        /// a queue to hold all waiting processes
        /// </summary>
        public List<Process> WaitQueue
        {
            get { return _waitQueue; }
        }

        /// <summary>
        /// check if the sequence is safe or not
        /// </summary>
        public bool IsSafe
        {
            get { return _isSafe; }
        }

        #endregion

        public BankerControl(int amount)
        {
            _bankerCollect = new BankerCollect();
            _count = amount;
            _waitQueue = new List<Process>();
            init();
        }

        private void init()
        {
            for (int i = 0; i < _count; i++)
            {
                Banker banker = new Banker();
                _bankerCollect.Add(banker);
            }

            // init avaliable resources
            do
            {
                _avaliable = new List<int>(Data.ResCount);
                for (int i = 0; i < Data.ResCount; i++)
                {
                    int res = Data.Random.Next(Data.ResAvaliableMin, Data.ResAvaliableMax);
                    _avaliable.Add(res);
                }
            } while (!runCheck(_bankerCollect, _avaliable));
            _unDoneCount = _count;
        }

        /// <summary>
        /// use safe checking algorithm
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        private bool runCheck(BankerCollect bc, List<int> avaliable)
        {
            _isSafe = false;
            checkSafe(bc, avaliable);
            return _isSafe;
        }

        /// <summary>
        /// core safe check recursion algorithm
        /// </summary>
        /// <param name="bc"></param>
        private void checkSafe(BankerCollect bc, List<int> avaliable)
        {
            if (bc.Count == 0)
            {
                _isSafe = true;
                return;
            }
            for (int i = 0; i < bc.Count; i++)
            {
                BankerCollect tempB = (BankerCollect)bc.Clone();   // create a temp BankerCollect
                List<int> newA = listClone(avaliable);
                if (_isSafe)                // if recursion calculate that the sequence is safe, then quit function
                {
                    return;
                }
                bool isSatisfy = true;
                if (!bc[i].IsDone)
                {
                    for (int j = 0; j < Data.ResCount; j++)
                    {
                        if (bc[i].Need[j] > newA[j])
                        {
                            isSatisfy = false;
                            break;
                        }
                    }
                }
                if (isSatisfy)          // check if avaliable resources satisfy with one Banker's need
                {
                    addAvailable(newA, tempB[i].Claim);
                    tempB.RemoveAt(i);    // then remove it and recurse 
                    checkSafe(tempB, newA);
                }
            }
        }

        private void addAvailable(List<int> avaliable, List<int> resources)
        {
            for (int i = 0; i < Data.ResCount; i++)
            {
                avaliable[i] += resources[i];
            }
        }

        private void delAvailable(List<int> avaliable, List<int> resources)
        {
            for (int i = 0; i < Data.ResCount; i++)
            {
                avaliable[i] -= resources[i];
            }
        }

        private List<int> listClone(List<int> resources)
        {
            List<int> newList = new List<int>(resources.Count);
            for (int i = 0; i < resources.Count; i++)
            {
                newList.Add(resources[i]);
            }
            return newList;
        }

        /// <summary>
        /// request resource in random way
        /// </summary>
        /// <returns>true: allocate ; false: add into the waiting queue</returns>
        public bool Request()
        {
            if (_unDoneCount <= 0)
            {
                throw new Exception("All processes have been allocated and finished!!");
            }
            do
            {
                _proIndex = Data.Random.Next(_count);
            } while (_bankerCollect[ProIndex].IsDone);
            _requestRes = new List<int>(Data.ResCount);
            _currentBanker = _bankerCollect[_proIndex];
            bool isZero = true;
            for (int i = 0; i < Data.ResCount; i++) // make sure that request resources <= need resources
            {
               // int requireLimit = (_currentBanker.Need[i] < _avaliable[i]) ? _currentBanker.Need[i] : _avaliable[i];
                int requireLimit = _currentBanker.Need[i];
                int res = Data.Random.Next(requireLimit + 1);
                _requestRes.Add(res);
                if (isZero && res > 0)
                    isZero = false;
            }
            if (isZero)
                return false;
            for (int i = 0; i < Data.ResCount; i++)
            {
                if (_requestRes[i] > _avaliable[i])
                {
                    _waitQueue.Add(new Process(_proIndex, _requestRes));
                    return false;
                }
            }
            BankerCollect tempB = (BankerCollect)_bankerCollect.Clone();
            tempB[_proIndex].allocate(_requestRes);
            if (!runCheck(tempB, _avaliable))   // if no safe after allocating resources, then add request to waiting queue
            {
                _waitQueue.Add(new Process(_proIndex, _requestRes));
                return false;
            }
            delAvailable(_avaliable, _requestRes);
            _bankerCollect = tempB;
            if (_bankerCollect[_proIndex].IsDone)
            {
                addAvailable(_avaliable, _bankerCollect[_proIndex].Claim);
                _unDoneCount--;
            }
            return true;
        }

        /// <summary>
        /// excute the requests in waiting queue
        /// </summary>
        /// <returns>if finish a request or not</returns>
        public bool ExcuteQueue()
        {
            if (_waitQueue.Count <= 0 || _unDoneCount <= 0)
                return false;
            List<Process> newQ = new List<Process>();
            for (int i = 0; i < _waitQueue.Count; i++)
            {
                newQ.Add(_waitQueue[i]);
            }
            for (int i = 0; i < _waitQueue.Count; i++)
            {
                Process tempP = (Process)_waitQueue[i].Clone();
                BankerCollect tempB = (BankerCollect)_bankerCollect.Clone();
                if (tempB[tempP.ProIndex].allocate(tempP.RequestRes))
                {
                    delAvailable(_avaliable, tempP.RequestRes);
                    if (runCheck(tempB, _avaliable))   // if no safe after allocating resources, then add request to waiting queue
                    {
                        _bankerCollect = tempB;
                        if (tempB[tempP.ProIndex].IsDone)
                        {
                            addAvailable(_avaliable, tempB[tempP.ProIndex].Claim);
                            _unDoneCount--;
                        }
                        _requestRes = tempP.RequestRes;
                        _proIndex = tempP.ProIndex;
                        newQ.Remove(tempP);
                        _waitQueue = newQ;
                        return true;
                    }
                    addAvailable(_avaliable, tempB[tempP.ProIndex].Claim);
                }
                else   // if request in waiting queue > banker's need, then remove it.
                {
                    newQ.Remove(tempP);
                }
            }
            return false;
        }

        public void ShowConsoleNeed()
        {
            for (int i = 0; i < _count; i++)
            {
                Console.Write(i + " process: ");
                for (int j = 0; j < _bankerCollect[i].Need.Count; j++)
                {
                    Console.Write(_bankerCollect[i].Need[j] + " ");
                }
                Console.WriteLine();
            }
        }

        public void ShowConsoleRequest()
        {
            Console.WriteLine("Current Process: " + _proIndex);
            Console.Write("Current Request : ");
            for (int i = 0; i < _requestRes.Count; i++)
            {
                Console.Write(_requestRes[i] + " ");
            }
            Console.WriteLine();
        }

        public void ShowConsoleAvaliable()
        {
            Console.Write("Current Avaliable: ");
            for (int i = 0; i < _avaliable.Count; i++)
            {
                Console.Write(_avaliable[i] + " ");
            }
            Console.WriteLine();
        }
    }

    public class Process
    {
        private readonly List<int> _requestRes;

        private readonly int _proIndex;

        public List<int> RequestRes
        {
            get { return _requestRes; }
        }

        public int ProIndex
        {
            get { return _proIndex; }
        }

        public Process(int index, List<int> requestRes)
        {
            _proIndex = index;
            _requestRes = requestRes;
        }

        public Object Clone()
        {
            List<int> newRequestRes = new List<int>();
            int newProIndex = _proIndex;
            for (int i = 0; i < _requestRes.Count; i++)
            {
                newRequestRes.Add(_requestRes[i]);
            }
            Process newP = new Process(_proIndex, newRequestRes);
            return newP;
        }
    }
}
