
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace UTJ
{
    /// <summary>
    /// 三次元とかの計算用のUtility
    /// </summary>
    public class MathUtility
    {
        // 0に近いか判定用
        const float NearCheck = 0.001f;

        // 平面の情報を取得します
        // 平面は ax+bx+cz+d=0で表します
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetPlane(float3 p1, float3 p2, float3 p3)
        {
            float3 normal = math.cross(p2 - p1, p3 - p1);
            normal = math.normalize(normal);
            float paramD = math.mul(normal, p1) * -1;

            float paramE = math.mul(normal, p2) * -1;
            float paramF = math.mul(normal, p3) * -1;
            return new float4(normal, paramD);
        }

        // 平面上に点があるかを判定します
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointOnPlane(float4 plane, float3 point)
        {
            float param = math.mul(plane.xyz, point) + plane.w;
            return IsNearEqualZero(param); ;
        }
        
        // 平面と直線の交差点を出します
        // 交わらないなら Nanを返します
        // xyzに座標が、dirがnormalizedされているなら wには距離が格納されます
        public static float4 RayCastPlane(float4 plane, float3 origin, float3 dir)
        {
            var xyz = plane.xyz;
            float divParam = math.mul(xyz, dir);
            if (IsNearEqualZero(divParam)) { 
                return new float4(float.NaN,float.NaN,float.NaN,float.NaN); 
            }
            float t = math.mul(xyz, origin) + plane.w;
            t /= -divParam;
            return new float4(origin + dir * t, t);
        }

        // XZ平面に対してRaycastします
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RayCastPlaneXZ(float4 plane,float3 origin)
        {
            // ax + by + cz + d =0
            // y = -(ax+cz+d)/b
            if( IsNearEqualZero(plane.y))
            {
                return float.NaN;
            }
            float y = -(plane.x * origin.x + plane.z * origin.z + plane.w) / plane.y;
            return new float3(origin.x,y,origin.z);
        }


        // 平面上にある前提で…
        // あるポイントが三角形の内部にあるかチェックします
        public static bool IsInPolygon(float3 src,float3 p1,float3 p2,float3 p3)
        {
            float3 c1 = math.cross(p3 - p1, p1 - src);
            float3 c2 = math.cross(p1 - p2, p2 - src);
            if (!IsSameDirection(c1, c2))
            {
                return false;
            }
            float3 c3 = math.cross(p2 - p3, p3 - src);
            if (!IsSameDirection(c2, c3))
            {
                return false;
            }

            return true;
        }

        // 直線上にある特定の点(src)に最も近い場所を返します。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 NearestLinePoint(float3 src, float3 origin, float3 dir)
        {
            dir = math.normalize(dir);
            float3 crossPoint = origin + math.dot(src - origin, dir) * dir;
            return crossPoint;
        }
        // dirのnormalizedなし
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 NearestLinePointWithoutNormalize(float3 src, float3 origin, float3 dir)
        {
            float3 crossPoint = origin + math.dot(src - origin, dir) * dir;
            return crossPoint;
        }
        // p1-p2の線分と srcの最も近い所を返します
        public static float3 NearestBetween2Point(float3 src, float3 p1,float3 p2)
        {
            float3 dir = p2-p1;
            float dirLengthSq = math.lengthsq(dir);
            dir = math.normalize(dir);
            float param = math.dot(src - p1, dir);

            float3 crossPoint = p1 + math.dot(src - p1, dir) * dir;
            if(param <= 0) { return p1; }
            if (param * param >= dirLengthSq) { return p2; }

            return crossPoint;
        }
        // Floatの値が0に近いか計算します
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearEqual(float3 p1,float3 p2)
        {
            return IsNearEqualZero( math.lengthsq(p2 - p1));
        }
        // Floatの値が0に近いか計算します
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSameDirection(float3 p1, float3 p2)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (p1[i] > 0.0f && p2[i] < 0.0f)
                {
                    return false;
                }
                if (p1[i] < 0.0f && p2[i] > 0.0f)
                {
                    return false;
                }
            }

            return true;
        }

        // Floatの値が0に近いか計算します
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearEqualZero(float param)
        {
            return (-NearCheck < param && param < NearCheck);
        }
    }
}