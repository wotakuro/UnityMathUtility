
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace UTJ
{
    /// <summary>
    /// �O�����Ƃ��̌v�Z�p��Utility
    /// </summary>
    public class MathUtility
    {
        // 0�ɋ߂�������p
        const float NearCheck = 0.001f;

        // ���ʂ̏����擾���܂�
        // ���ʂ� ax+bx+cz+d=0�ŕ\���܂�
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

        // ���ʏ�ɓ_�����邩�𔻒肵�܂�
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointOnPlane(float4 plane, float3 point)
        {
            float param = math.mul(plane.xyz, point) + plane.w;
            return IsNearEqualZero(param); ;
        }
        
        // ���ʂƒ����̌����_���o���܂�
        // �����Ȃ��Ȃ� Nan��Ԃ��܂�
        // xyz�ɍ��W���Adir��normalized����Ă���Ȃ� w�ɂ͋������i�[����܂�
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

        // XZ���ʂɑ΂���Raycast���܂�
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


        // ���ʏ�ɂ���O��Łc
        // ����|�C���g���O�p�`�̓����ɂ��邩�`�F�b�N���܂�
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

        // ������ɂ������̓_(src)�ɍł��߂��ꏊ��Ԃ��܂��B
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 NearestLinePoint(float3 src, float3 origin, float3 dir)
        {
            dir = math.normalize(dir);
            float3 crossPoint = origin + math.dot(src - origin, dir) * dir;
            return crossPoint;
        }
        // dir��normalized�Ȃ�
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 NearestLinePointWithoutNormalize(float3 src, float3 origin, float3 dir)
        {
            float3 crossPoint = origin + math.dot(src - origin, dir) * dir;
            return crossPoint;
        }
        // p1-p2�̐����� src�̍ł��߂�����Ԃ��܂�
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
        // Float�̒l��0�ɋ߂����v�Z���܂�
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearEqual(float3 p1,float3 p2)
        {
            return IsNearEqualZero( math.lengthsq(p2 - p1));
        }
        // Float�̒l��0�ɋ߂����v�Z���܂�
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

        // Float�̒l��0�ɋ߂����v�Z���܂�
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearEqualZero(float param)
        {
            return (-NearCheck < param && param < NearCheck);
        }
    }
}