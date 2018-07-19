using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

namespace QueribusQuery 
{
	public static class QueribusExtensions 
	{
        public static bool QueryBoolOr(this GameObject go, string queryName)
        {
            return Queribus.Or(queryName, go);
        }

        public static bool QueryBoolAnd(this GameObject go, string queryName)
        {
            return Queribus.And(queryName, go);
        }

        public static int QueryIntSum(this GameObject go, string queryName)
        {
            return Queribus.SumInt(queryName, go);
        }

        public static float QueryFloatSum(this GameObject go, string queryName)
        {
            return Queribus.SumFloat(queryName, go);
        }

        public static T QueryVote<T>(this GameObject go, string queryName)
        {
            return Queribus.Vote<T>(queryName, go);
        }

        public static bool QueryBoolOr(this MonoBehaviour mb, string queryName)
        {
            return Queribus.Or(queryName, mb.gameObject);
        }

        public static bool QueryBoolAnd(this MonoBehaviour mb, string queryName)
        {
            return Queribus.And(queryName, mb.gameObject);
        }

        public static int QueryIntSum(this MonoBehaviour mb, string queryName)
        {
            return Queribus.SumInt(queryName, mb.gameObject);
        }

        public static int QueryIntMin(this MonoBehaviour mb, string queryName)
        {
            return Queribus.MinInt(queryName, mb.gameObject);
        }

        public static int QueryIntMax(this MonoBehaviour mb, string queryName)
        {
            return Queribus.MaxInt(queryName, mb.gameObject);
        }

        public static float QueryFloatSum(this MonoBehaviour mb, string queryName)
        {
            return Queribus.SumFloat(queryName, mb.gameObject);
        }

        public static float QueryFloatMin(this MonoBehaviour mb, string queryName)
        {
            return Queribus.MinFloat(queryName, mb.gameObject);
        }

        public static float QueryFloatMax(this MonoBehaviour mb, string queryName)
        {
            return Queribus.MaxFloat(queryName, mb.gameObject);
        }

        public static T QueryVote<T>(this MonoBehaviour mb, string queryName)
        {
            return Queribus.Vote<T>(queryName, mb.gameObject);
        }
    }
}