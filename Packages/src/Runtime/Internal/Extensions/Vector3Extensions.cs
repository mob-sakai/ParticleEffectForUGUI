using UnityEngine;

namespace Coffee.UIParticleInternal
{
    internal static class Vector3Extensions
    {
        public static Vector3 Inverse(this Vector3 self)
        {
            self.x = Mathf.Approximately(self.x, 0) ? 1 : 1 / self.x;
            self.y = Mathf.Approximately(self.y, 0) ? 1 : 1 / self.y;
            self.z = Mathf.Approximately(self.z, 0) ? 1 : 1 / self.z;
            return self;
        }

        public static Vector3 GetScaled(this Vector3 self, Vector3 other1)
        {
            self.Scale(other1);
            return self;
        }

        public static Vector3 GetScaled(this Vector3 self, Vector3 other1, Vector3 other2)
        {
            self.Scale(other1);
            self.Scale(other2);
            return self;
        }

        public static Vector3 GetScaled(this Vector3 self, Vector3 other1, Vector3 other2, Vector3 other3)
        {
            self.Scale(other1);
            self.Scale(other2);
            self.Scale(other3);
            return self;
        }

        public static bool IsVisible(this Vector3 self)
        {
            return 0 < Mathf.Abs(self.x * self.y * self.z);
        }

        public static bool IsVisible2D(this Vector3 self)
        {
            return 0 < Mathf.Abs(self.x * self.y);
        }
    }
}
