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
            for (int i = 0; i < amount; i++)
            {
                Banker banker = new Banker();
                _bankerCollect.Add(banker);
            }
            _unDoneCount = _bankerCollect.Count;
            _waitQueue = new List<Process>();
            init();
        }

        private void init()
        {
            // init avaliable resources
            _avaliable = new List<int>(Data.ResCount);
            for (int i = 0; i < Data.ResCount; i++)
            {
                int res = 0;
                for (int j = 0; j < _bankerCollect.Count; j++)
                {
                    if (_bankerCollect[j].Claim[i] > res)
                        res = _bankerCollect[j].Claim[i];
                }
                _avaliable.Add(res);
            }
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
            BankerCollect tempB = (BankerCollect)bc.Clone();   // create a temp BankerCollect
            for (int i = 0; i < tempB.Count; i++)
            {
                Banker tempBank = tempB[i];
                List<int> newA = listClone(avaliable);
                if (_isSafe)                // if recursion calculate that the sequence is safe, then quit function
                {
                    return;
                }
                if (tempBank.IsDone)
                {
                    tempB.Remove(tempBank);    // then remove it and recurse 
                    checkSafe(tempB, newA);
                    i--;
                }
                else
                {
                    bool isSatisfy = true;
                    for (int j = 0; j < Data.ResCount; j++)
                    {
                        if (tempBank.Need[j] > newA[j])
                        {
                            isSatisfy = false;
                            break;
                        }
                    }
                    if (isSatisfy)          // check if avaliable resources satisfy with one Banker's need
                    {
                        delAvailable(newA, tempBank.Need);
                        addAvailable(newA, tempBank.Claim);
                        tempB.Remove(tempBank);    // then remove it and recurse 
                        checkSafe(tempB, newA);
                        i--;
                    }
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
            List<int> newList = new List<int>(Data.ResCount);
            for (int i = 0; i < Data.ResCount; i++)
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
                _proIndex = Data.Random.Next(_bankerCollect.Count);
            } while (_bankerCollect[ProIndex].IsDone);
            _currentBanker = _bankerCollect[_proIndex];
            bool isZero = true;
            do
            {
                isZero = true;
                _requestRes = new List<int>(Data.ResCount);
                for (int i = 0; i < Data.ResCount; i++) // make sure that request resources <= need resources
                {
                    int requireLimit = _currentBanker.Need[i];
                    int res = Data.Random.Next(requireLimit + 1);
                    _requestRes.Add(res);
                    if (isZero && res > 0)
                        isZero = false;
                }
            } while (isZero); // make sure that all kinds of request resources are not 0 at the same time

            List<int> newA = listClone(_avaliable);

            for (int i = 0; i < Data.ResCount; i++)
            {
                if (_requestRes[i] > newA[i])
                {
                   // _waitQueue.Add(new Process(_proIndex, _requestRes));
                    return false;
                }
            }
            BankerCollect tempB = (BankerCollect)_bankerCollect.Clone();
            tempB[_proIndex].allocate(_requestRes);
            delAvailable(newA, _requestRes);
            if (tempB[_proIndex].IsDone)
            {
                addAvailable(newA, tempB[_proIndex].Claim);
            }

            ////---------------------------------------------------
            //ShowConsoleAvaliable();
            //for (int i = 0; i < tempB.Count; i++)
            //{
            //    Console.Write("Process " + i + " : ");
            //    for (int j = 0; j < tempB[i].Need.Count; j++)
            //    {
            //        Console.Write(tempB[i].Need[j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            if (!runCheck(tempB, newA))   // if no safe after allocating resources, then add request to waiting queue
            {
                //_waitQueue.Add(new Process(_proIndex, _requestRes));
                return false;
            }

            ////---------------------------------------------------
            //Console.WriteLine("safeer");
            //ShowConsoleAvaliable();
            //for (int i = 0; i < tempB.Count; i++)
            //{
            //    Console.Write("Process " + i + " : ");
            //    for (int j = 0; j < tempB[i].Need.Count; j++)
            //    {
            //        Console.Write(tempB[i].Need[j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            if (tempB[_proIndex].IsDone)
            {
                _unDoneCount--;
            }
            _avaliable = newA;
            _bankerCollect = tempB;
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
            for (int i = 0; i < _waitQueue.Count; i++)
            {
                BankerCollect tempB = (BankerCollect)_bankerCollect.Clone();
                List<int> newA = listClone(_avaliable);
                if (!tempB[_waitQueue[i].ProIndex].IsDone && tempB[_waitQueue[i].ProIndex].allocate(_waitQueue[i].RequestRes))
                {
                    delAvailable(newA, _waitQueue[i].RequestRes);
                    if (runCheck(tempB, newA))   // if no safe after allocating resources, then check next request in waiting queue
                    {
                        delAvailable(_avaliable, _waitQueue[i].RequestRes);

                        if (tempB[_waitQueue[i].ProIndex].IsDone)
                        {
                            addAvailable(_avaliable, tempB[_waitQueue[i].ProIndex].Claim);
                            _unDoneCount--;
                        }
                        _bankerCollect = tempB;
                        _requestRes = _waitQueue[i].RequestRes;
                        _proIndex = _waitQueue[i].ProIndex;
                        _waitQueue.Remove(_waitQueue[i]);
                        return true;
                    }
                }
                else   // if request in waiting queue > banker's need, then remove it.
                {
                    _waitQueue.Remove(_waitQueue[i]);
                    i--;
                }
            }
            return false;
        }

        public void ShowConsoleNeed()
        {
            for (int i = 0; i < _bankerCollect.Count; i++)
            {
                Console.Write(i + " process: ");
                for (int j = 0; j < Data.ResCount; j++)
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
            for (int i = 0; i < Data.ResCount; i++)
            {
                Console.Write(_requestRes[i] + " ");
            }
            Console.WriteLine();
        }

        public void ShowConsoleAvaliable()
        {
            Console.Write("Current Avaliable: ");
            for (int i = 0; i < Data.ResCount; i++)
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
