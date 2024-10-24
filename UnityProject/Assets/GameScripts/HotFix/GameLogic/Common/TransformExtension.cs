using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.Common
{
    public static class TransformExtension
    {
        #region Transform Extension

        public static void SetLocalPositionX(this Transform transform, float x)
        {
            var value = transform.localPosition;
            value.x = x;
            transform.localPosition = value;
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            var value = transform.localPosition;
            value.y = y;
            transform.localPosition = value;
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            var value = transform.localPosition;
            value.z = z;
            transform.localPosition = value;
        }
        
        public static void SetPositionX(this Transform transform, float x)
        {
            var value = transform.position;
            value.x = x;
            transform.position = value;
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            var value = transform.position;
            value.y = y;
            transform.position = value;
        }
        
        public static void SetPositionZ(this Transform transform, float z)
        {
            var value = transform.position;
            value.z = z;
            transform.position = value;
        }
        
        public static void SetLocalScaleX(this Transform transform, float x)
        {
            var value = transform.localScale;
            value.x = x;
            transform.localScale = value;
        }

        public static void SetLocalScaleY(this Transform transform, float y)
        {
            var value = transform.localScale;
            value.y = y;
            transform.localScale = value;
        }

        public static void SetLocalScaleZ(this Transform transform, float z)
        {
            var value = transform.localScale;
            value.z = z;
            transform.localScale = value;
        }
        

        #endregion
    }
}