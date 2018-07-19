using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

namespace QueribusQuery
{
    public delegate T QueryDelegate<T, C>(C context);
    public delegate object QueryDelegateWrapper(object context);
    public delegate Vote<object> VoteQueryDelegateWrapper(object context);

    public class QueryKey
    {
        public string queryName;
        public System.Type queryReturnType;

        public QueryKey(string queryName, System.Type queryReturnType)
        {
            this.queryName = queryName;
            this.queryReturnType = queryReturnType;
        }

        public override int GetHashCode()
        {
            return queryName.GetHashCode() ^ queryReturnType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is QueryKey)
            {
                QueryKey key = (QueryKey)obj;
                return queryName.Equals(key.queryName) && queryReturnType.Equals(key.queryReturnType);
            }

            return false;
        }
    }
    
    public class Vote<T>
    {
        public bool isApplicable;
        public T candidate;
        public int weight;
        
        public Vote(bool isApplicable, T candidate, int weight) 
	    {
            this.isApplicable = isApplicable;
		    this.candidate = candidate;
		    this.weight = weight;
	    }
    }

    public class BoolResult
    {
        public bool isApplicable = false;
        public bool result = false;

        public BoolResult( bool isApplicable, bool result )
        {
            this.isApplicable = isApplicable;
            this.result = result;
        }
    }

    public class QueribusObject : SingletonMonoBehaviour<QueribusObject>
	{
        // Should probably derive specialised classes for Sums, Bools and Votes to better adhere to DRY principle

        Dictionary<QueryKey, Dictionary<int, QueryDelegateWrapper>> _numberListeners;
        Dictionary<QueryKey, Dictionary<int, QueryDelegateWrapper>> numberListeners
        {
            get
            {
                if (_numberListeners == null)
                {
                    _numberListeners = new Dictionary<QueryKey, Dictionary<int, QueryDelegateWrapper>>();
                }

                return _numberListeners;
            }
        }

        Dictionary<QueryKey, Dictionary<int, QueryDelegateWrapper>> _boolListeners;
        Dictionary<QueryKey, Dictionary<int, QueryDelegateWrapper>> boolListeners
        {
            get
            {
                if (_boolListeners == null)
                {
                    _boolListeners = new Dictionary<QueryKey, Dictionary<int, QueryDelegateWrapper>>();
                }

                return _boolListeners;
            }
        }


        Dictionary<QueryKey, Dictionary<int, VoteQueryDelegateWrapper>> _voteListeners;
        Dictionary<QueryKey, Dictionary<int, VoteQueryDelegateWrapper>> voteListeners
        {
            get
            {
                if( _voteListeners == null )
                {
                    _voteListeners = new Dictionary<QueryKey, Dictionary<int, VoteQueryDelegateWrapper>>();
                }

                return _voteListeners;
            }
        }
        
        public int SumInt(string queryName, object context)
        {
            return QuerySum<int>(queryName, context);
        }

        public float SumFloat(string queryName, object context)
        {
            return QuerySum<float>(queryName, context);
        }
        
        protected T QuerySum<T>(string queryName, object context) where T : struct
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!numberListeners.ContainsKey(key))    
            {
                return default(T);
            }

            Dictionary<int, QueryDelegateWrapper> subscribers = numberListeners[key];

            object total = default(T);
            System.Type tType = typeof(T);
            
            // Messy workaround for requiring a + operator defined for generic T
            if (tType == typeof(int))
            {
                foreach (QueryDelegateWrapper sub in subscribers.Values)
                {
                    total = ((int)total) + ((int)sub(context));
                }
            }
            else if (tType == typeof(float))
            {
                foreach (QueryDelegateWrapper sub in subscribers.Values)
                {
                    total = ((float)total) + ((float)sub(context));
                }
            }
            
            return (T)total;
        }

        public int MinInt(string queryName, object context)
        {
            return QueryMin<int>(queryName, context);
        }

        public float MinFloat(string queryName, object context)
        {
            return QueryMin<float>(queryName, context);
        }

        protected T QueryMin<T>(string queryName, object context) where T : struct
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!numberListeners.ContainsKey(key))
            {
                return default(T);
            }

            Dictionary<int, QueryDelegateWrapper> subscribers = numberListeners[key];

            object min = default(T);
            System.Type tType = typeof(T);

            // Messy workaround for requiring a min operator defined for generic T
            if (tType == typeof(int))
            {
                foreach (QueryDelegateWrapper sub in subscribers.Values)
                {
                    min = min.Equals(default(T)) ? sub(context) : Mathf.Min(((int)min), ((int)sub(context)));
                }
            }
            else if (tType == typeof(float))
            {
                foreach (QueryDelegateWrapper sub in subscribers.Values)
                {
                    min = min.Equals(default(T)) ? sub(context) : Mathf.Min(((float)min), ((float)sub(context)));
                }
            }

            return (T)min;
        }

        public int MaxInt(string queryName, object context)
        {
            return QueryMax<int>(queryName, context);
        }

        public float MaxFloat(string queryName, object context)
        {
            return QueryMax<float>(queryName, context);
        }

        protected T QueryMax<T>(string queryName, object context) where T : struct
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!numberListeners.ContainsKey(key))
            {
                return default(T);
            }

            Dictionary<int, QueryDelegateWrapper> subscribers = numberListeners[key];

            object max = default(T);
            System.Type tType = typeof(T);

            // Messy workaround for requiring a min operator defined for generic T
            if (tType == typeof(int))
            {
                foreach (QueryDelegateWrapper sub in subscribers.Values)
                {
                    max = max.Equals(default(T)) ? sub(context) : Mathf.Max(((int)max), ((int)sub(context)));
                }
            }
            else if (tType == typeof(float))
            {
                foreach (QueryDelegateWrapper sub in subscribers.Values)
                {
                    max = max.Equals(default(T)) ? sub(context) : Mathf.Max(((float)max), ((float)sub(context)));
                }
            }

            return (T)max;
        }

        public bool Or(string queryName, object context)
        {
            QueryKey key = new QueryKey(queryName, typeof(BoolResult));
            if (!boolListeners.ContainsKey(key))
            {
                return false;
            }

            Dictionary<int, QueryDelegateWrapper> subscribers = boolListeners[key];
            foreach (QueryDelegateWrapper sub in subscribers.Values)
            {
                BoolResult answer = (BoolResult)sub(context);
                if( answer.isApplicable && answer.result )
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool And(string queryName, object context)
        {
            QueryKey key = new QueryKey(queryName, typeof(BoolResult));
            if (!boolListeners.ContainsKey(key))
            {
                return false;
            }

            bool hasAnswer = false;

            Dictionary<int, QueryDelegateWrapper> subscribers = boolListeners[key];
            foreach (QueryDelegateWrapper sub in subscribers.Values)
            {
                BoolResult answer = (BoolResult)sub(context);

                if( answer.isApplicable )
                {
                    hasAnswer = true;
                    if (!answer.result)
                    {
                        return false;
                    }
                }
            }

            return hasAnswer;
        }

        public T Vote<T>(string queryName, object context)
        {
            QueryKey key = new QueryKey(queryName, typeof(T));

            if (!voteListeners.ContainsKey(key) )
            {
                return default(T);
            }

            Dictionary<int, VoteQueryDelegateWrapper> subscribers = voteListeners[key];
            Dictionary<T, int> votes = new Dictionary<T, int>();

            foreach (VoteQueryDelegateWrapper sub in subscribers.Values)
            {
                Vote<object> vote = sub(context);

                if( vote.isApplicable )
                {
                    T candidate = (T)vote.candidate;
                    if (!votes.ContainsKey(candidate))
                    {
                        votes.Add(candidate, 0);
                    }

                    votes[candidate] += vote.weight;
                }
            }

            int highestWeight = 0;
            T bestCandidate = default(T);
            foreach (T candidate in votes.Keys)
            {
                if (votes[candidate] > highestWeight)
                {
                    bestCandidate = candidate;
                    highestWeight = votes[candidate];
                }
            }

            return bestCandidate;
        }

        public void SubscribeToSum<T, C>(string queryName, QueryDelegate<T, C> answer) where T : struct
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!numberListeners.ContainsKey(key) )
            {
                numberListeners.Add(key, new Dictionary<int, QueryDelegateWrapper>());
            }

            int answerHash = answer.GetHashCode();
            if (!numberListeners[key].ContainsKey(answerHash))
            {
                QueryDelegateWrapper wrappedAnswer = (object context) => { return answer((C)context); };
                numberListeners[key].Add(answerHash, wrappedAnswer);
            }
        }

        public void UnsubscribeFromSum<T, C>(string queryName, QueryDelegate<T, C> answer) where T : struct
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!numberListeners.ContainsKey(key))
            {
                numberListeners.Add(key, new Dictionary<int, QueryDelegateWrapper>());
            }

            int answerHash = answer.GetHashCode();
            if (numberListeners[key].ContainsKey(answerHash))
            {
                numberListeners[key].Remove(answerHash);
            }
        }

        public void SubscribeToBool<C>(string queryName, QueryDelegate<BoolResult, C> answer)
        {
            QueryKey key = new QueryKey(queryName, typeof(BoolResult));
            if (!boolListeners.ContainsKey(key))
            {
                boolListeners.Add(key, new Dictionary<int, QueryDelegateWrapper>());
            }

            int answerHash = answer.GetHashCode();
            if (!boolListeners[key].ContainsKey(answerHash))
            {
                QueryDelegateWrapper wrappedAnswer = (object context) => { return answer((C)context); };
                boolListeners[key].Add(answerHash, wrappedAnswer);
            }
        }

        public void UnsubscribeFromBool<C>(string queryName, QueryDelegate<BoolResult, C> answer)
        {
            QueryKey key = new QueryKey(queryName, typeof(BoolResult));
            if (!boolListeners.ContainsKey(key))
            {
                boolListeners.Add(key, new Dictionary<int, QueryDelegateWrapper>());
            }

            int answerHash = answer.GetHashCode();
            if (boolListeners[key].ContainsKey(answerHash))
            {
                boolListeners[key].Remove(answerHash);
            }
        }

        public void SubscribeToVote<T, C>(string queryName, QueryDelegate<Vote<T>, C> answer)
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!voteListeners.ContainsKey(key))  
            {
                voteListeners.Add(key, new Dictionary<int, VoteQueryDelegateWrapper>());
            }

            int answerHash = answer.GetHashCode();
            if (!voteListeners[key].ContainsKey(answerHash))
            {
                VoteQueryDelegateWrapper wrappedAnswer = (object context) => 
                {
                    Vote<T> answerVote = answer((C)context);
                    return new Vote<object>(answerVote.isApplicable, answerVote.candidate, answerVote.weight);
                };
                
                voteListeners[key].Add(answerHash, wrappedAnswer);
            }
        }

        public void UnsubscribeFromVote<T, C>(string queryName, QueryDelegate<Vote<T>, C> answer)
        {
            QueryKey key = new QueryKey(queryName, typeof(T));
            if (!voteListeners.ContainsKey(key) )
            {
                voteListeners.Add(key, new Dictionary<int, VoteQueryDelegateWrapper>());
            }

            int answerHash = answer.GetHashCode();
            if (voteListeners[key].ContainsKey(answerHash))
            {
                voteListeners[key].Remove(answerHash);
            }
        }
    }
}