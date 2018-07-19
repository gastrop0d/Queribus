using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

namespace QueribusQuery
{
	public static class Queribus
	{
        public static int SumInt(string queryName, object context)
        {
            return QueribusObject.Instance.SumInt(queryName, context);
        }

        public static int MinInt(string queryName, object context)
        {
            return QueribusObject.Instance.MinInt(queryName, context);
        }

        public static int MaxInt(string queryName, object context)
        {
            return QueribusObject.Instance.MaxInt(queryName, context);
        }

        public static float SumFloat(string queryName, object context)
        {
            return QueribusObject.Instance.SumFloat(queryName, context);
        }

        public static float MinFloat(string queryName, object context)
        {
            return QueribusObject.Instance.MinFloat(queryName, context);
        }

        public static float MaxFloat(string queryName, object context)
        {
            return QueribusObject.Instance.MaxFloat(queryName, context);
        }

        public static bool Or(string queryName, object context)
        {
            return QueribusObject.Instance.Or(queryName, context);
        }

        public static bool And(string queryName, object context)
        {
            return QueribusObject.Instance.And(queryName, context);
        }

        public static T Vote<T>(string queryName, object context)
        {
            return QueribusObject.Instance.Vote<T>(queryName, context);
        }

        public static void SubscribeNumber<T>(string queryName, QueryDelegate<T, GameObject> answer) where T : struct
        {
            QueribusObject.Instance.SubscribeToSum<T, GameObject>(queryName, answer);
        }

        public static void UnsubscribeNumber<T>(string queryName, QueryDelegate<T, GameObject> answer) where T : struct
        {
            QueribusObject.Instance.UnsubscribeFromSum<T, GameObject>(queryName, answer);
        }

        public static void SubscribeNumber<T, C>(string queryName, QueryDelegate<T, C> answer) where T : struct
        {
            QueribusObject.Instance.SubscribeToSum<T, C>(queryName, answer);
        }
        
        public static void UnsubscribeNumber<T, C>(string queryName, QueryDelegate<T, C> answer) where T : struct
        {
            QueribusObject.Instance.UnsubscribeFromSum<T, C>(queryName, answer);
        }

        public static void SubscribeBool(string queryName, QueryDelegate<BoolResult, GameObject> answer)
        {
            QueribusObject.Instance.SubscribeToBool<GameObject>(queryName, answer);
        }

        public static void UnsubscribeBool(string queryName, QueryDelegate<BoolResult, GameObject> answer)
        {
            QueribusObject.Instance.UnsubscribeFromBool<GameObject>(queryName, answer);
        }

        public static void SubscribeBool<C>(string queryName, QueryDelegate<BoolResult, C> answer)
        {
            QueribusObject.Instance.SubscribeToBool<C>(queryName, answer);
        }

        public static void UnsubscribeBool<C>(string queryName, QueryDelegate<BoolResult, C> answer)
        {
            QueribusObject.Instance.UnsubscribeFromBool<C>(queryName, answer);
        }

        public static void SubscribeVote<T, C>(string queryName, QueryDelegate<Vote<T>, C> answer)
        {
            QueribusObject.Instance.SubscribeToVote<T, C>(queryName, answer);
        }

        public static void UnsubscribeVote<T, C>(string queryName, QueryDelegate<Vote<T>, C> answer)
        {
            QueribusObject.Instance.UnsubscribeFromVote<T, C>(queryName, answer);
        }
    }
}