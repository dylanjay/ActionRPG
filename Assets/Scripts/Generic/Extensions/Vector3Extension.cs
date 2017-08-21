using UnityEngine;

namespace BenCo.Extensions
{
	public static class Vector3Extension
	{
		public static Vector3 xz(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        public static Vector3 xy(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }

        public static Vector3 yx(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }

        public static Vector3 yz(this Vector3 vector)
        {
            return new Vector3(0f, vector.y, vector.z);
        }

        public static Vector3 zx(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        public static Vector3 zy(this Vector3 vector)
        {
            return new Vector3(0f, vector.y, vector.z);
        }
    }
}