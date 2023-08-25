using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Terraria;

namespace Wild.Common.AuxiliaryMeans
{
    /// <summary>
    /// 提供一些算法工具与常量
    /// </summary>
    public static class HcMath
    {
        public static Random HcRandom = new Random();

        /// <summary>
        /// 角度插值
        /// </summary>
        /// <param name="startAngle">起始角度</param>
        /// <param name="targetAngle">目标角度</param>
        /// <param name="amount">插值算子</param>
        /// <returns></returns>
        public static float LerpAngle(float startAngle, float targetAngle, float amount)
        {
            float difference = MathHelper.WrapAngle(targetAngle - startAngle);
            return MathHelper.WrapAngle(startAngle + difference * amount);
        }

        /// <summary>
        ///  HSV 到 RGB 的转换
        /// </summary>
        /// <param name="hue">色相</param>
        /// <param name="saturation">饱和度</param>
        /// <param name="value">亮度</param>
        /// <returns></returns>
        public static Color ColorFromHSV(float hue, float saturation, float value)
        {
            int hi = (int)Math.Floor(hue / 60) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);

            value = value * 255;
            int v = (int)value;
            int p = (int)(value * (1 - saturation));
            int q = (int)(value * (1 - f * saturation));
            int t = (int)(value * (1 - (1 - f) * saturation));

            if (hi == 0)
            {
                return new Color(v, t, p);
            }
            else if (hi == 1)
            {
                return new Color(q, v, p);
            }
            else if (hi == 2)
            {
                return new Color(p, v, t);
            }
            else if (hi == 3)
            {
                return new Color(p, q, v);
            }
            else if (hi == 4)
            {
                return new Color(t, p, v);
            }
            else
            {
                return new Color(v, p, q);
            }
        }

        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(Color color1, float weight1, Color color2, float weight2)
        {
            return new Color(color1.ToVector4() * weight1 + color2.ToVector4() * weight2);
        }
        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(Color color1, float weight1, Color color2, float weight2, Color color3, float weight3)
        {
            return new Color(color1.ToVector4() * weight1 + color2.ToVector4() * weight2 + color3.ToVector4() * weight3);
        }
        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(Color color1, float weight1, Color color2, float weight2, Color color3, float weight3, Color color4, float weight4)
        {
            return new Color(color1.ToVector4() * weight1 + color2.ToVector4() * weight2 + color3.ToVector4() * weight3 + color4.ToVector4() * weight4);
        }
        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(Color color1, float weight1, Color color2, float weight2, Color color3, float weight3, Color color4, float weight4, Color color5, float weight5)
        {
            return new Color(color1.ToVector4() * weight1 + color2.ToVector4() * weight2 + color3.ToVector4() * weight3 + color4.ToVector4() * weight4 + color5.ToVector4() * weight5);
        }
        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(Color color1, float weight1, Color color2, float weight2, Color color3, float weight3, Color color4, float weight4, Color color5, float weight5, Color color6, float weight6)
        {
            return new Color(color1.ToVector4() * weight1 + color2.ToVector4() * weight2 + color3.ToVector4() * weight3 + color4.ToVector4() * weight4 + color5.ToVector4() * weight5 + color6.ToVector4() * weight6);
        }
        /// <summary>
        /// 色彩混合
        /// </summary>
        public static Color RecombinationColor(params (Color color, float weight)[] colorWeightPairs)
        {
            Vector4 result = Vector4.Zero;

            for (int i = 0; i < colorWeightPairs.Length; i++)
            {
                result += colorWeightPairs[i].color.ToVector4() * colorWeightPairs[i].weight;
            }

            return new Color(result);
        }

        /// <summary>
        /// 获取一个随机方向的向量
        /// </summary>
        /// <param name="startAngle">开始角度,请输入角度单位的值</param>
        /// <param name="targetAngle">目标角度,请输入角度单位的值</param>
        /// <param name="ModeLength">返回的向量的长度</param>
        /// <returns></returns>
        public static Vector2 GetRandomVevtor(float startAngle, float targetAngle, float ModeLength)
        {
            float angularSeparation = targetAngle - startAngle;
            double randomPosx = (angularSeparation * HcRandom.NextDouble() + startAngle) * (MathHelper.Pi / 180);
            return new Vector2((float)Math.Cos(randomPosx), (float)Math.Sin(randomPosx)) * ModeLength;
        }

        /// <summary>
        /// 一个随机布尔值获取方法
        /// </summary>
        /// <param name="ProbabilityDenominator">概率分母</param>
        /// <param name="ProbabilityExpectation">期望倾向</param>
        /// <param name="DesiredObject">期望对象</param>
        /// <returns></returns>
        public static bool RandomBooleanValue(int ProbabilityDenominator, int ProbabilityExpectation, bool DesiredObject)
        {
            int randomInt = (int)HcRandom.NextInt64(0, ProbabilityDenominator);

            if (DesiredObject && randomInt == ProbabilityExpectation)
            {
                return true;
            }
            else if (randomInt == ProbabilityExpectation)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据向量的Y值来进行比较
        /// </summary>
        public class VeYSort : IComparer<Vector2>
        {
            public int Compare(Vector2 v1, Vector2 v2)
            {
                // 比较两个向量的Y值，根据Y值大小进行排序
                if (v1.Y < v2.Y)
                    return -1;
                else if (v1.Y > v2.Y)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 获取一组离散点
        /// </summary>
        /// <param name="numPoints">点数量，这将决定数组的索引值</param>
        /// <param name="Length">随机长度</param>
        /// <param name="Deviation">随机宽度</param>
        /// <param name="startPosition">开始位置</param>
        /// <param name="destinationPosition">结束位置，如果不输入则默认让点的分布末端离散</param>
        /// <returns>返回一串向量值类型的点数组</returns>
        public static Vector2[] GetRandomPos(int numPoints, float Length, float Deviation, Vector2 startPosition, Vector2 destinationPosition = default)
        {
            Vector2[] PosNillm = new Vector2[numPoints];
            Vector2 Pos = startPosition;
            float intervalY = Length / numPoints;
            float NewInterval = 0;
            if (destinationPosition == default)
            {
                for (int i = 0; i < numPoints; i++)
                {
                    intervalY += (float)HcRandom.NextDouble() * intervalY * 0.15f * HcRandom.Next(-1, 2);
                    NewInterval += intervalY;
                    float intervalX = HcRandom.Next(-1, 2) * NewInterval;
                    Vector2 RandomVr = new Vector2(intervalX, intervalY);
                    Pos += RandomVr;
                    PosNillm[i] = Pos;
                }
                return PosNillm;
            }
            else
            {
                for (int i = 0; i < numPoints / 2; i++)
                {
                    intervalY += (float)HcRandom.NextDouble() * intervalY * 0.15f * HcRandom.Next(-1, 2);
                    NewInterval += intervalY;
                    float intervalX = HcRandom.Next(-1, 2) * NewInterval;
                    Vector2 RandomVr = new Vector2(intervalX, intervalY);
                    Pos += RandomVr;
                    PosNillm[i] = Pos;
                }
                for (int i = numPoints / 2; i < numPoints; i++)
                {
                    intervalY += (float)HcRandom.NextDouble() * intervalY * 0.15f * HcRandom.Next(-1, 2);
                    NewInterval -= intervalY;
                    float intervalX = HcRandom.Next(-1, 2) * NewInterval;
                    Vector2 RandomVr = new Vector2(intervalX, intervalY);
                    Pos += RandomVr;
                    PosNillm[i] = Pos;
                }
                return PosNillm;
            }
        }

        public static void GenerateCurve(ref List<Vector2> points, Vector2 startPoint, Vector2 endPoint, int numPoints, Vector2 controlPoint, float gravityFactor)
        {
            points = new List<Vector2>();

            if (controlPoint == Vector2.Zero) controlPoint = (startPoint + endPoint) / 2f;

            float curveLength = Vector2.Distance(startPoint, endPoint);    // 曲线长度

            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);

                // 根据贝塞尔曲线公式计算点的位置
                float u = 1f - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                // 根据双曲函数调整控制点的 Y 坐标
                float adjustedControlPointY = controlPoint.Y + curveLength * gravityFactor * (float)Math.Tanh(t * Math.PI);

                Vector2 adjustedControlPoint = new Vector2(controlPoint.X, adjustedControlPointY);

                Vector2 point = uuu * startPoint + 3f * uu * t * adjustedControlPoint + 3f * u * tt * adjustedControlPoint + ttt * endPoint;

                points.Add(point);
            }
        }

        public static void GenerateCurve2(ref List<Vector2> points, Vector2 startPoint, Vector2 endPoint, int numPoints, Vector2 controlPoint, float gravityFactor)
        {
            points = new List<Vector2>();

            if (controlPoint == Vector2.Zero) controlPoint = (startPoint + endPoint) / 2f;

            float curveLength = Vector2.Distance(startPoint, endPoint);    // 曲线长度

            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);

                // 根据贝塞尔曲线公式计算点的位置
                float u = 1f - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                // 根据重力因子调整控制点的 Y 坐标
                Vector2 adjustedControlPoint = new Vector2(controlPoint.X, controlPoint.Y + curveLength * gravityFactor);

                Vector2 point = uuu * startPoint + 3f * uu * t * adjustedControlPoint + 3f * u * tt * adjustedControlPoint + ttt * endPoint;

                points.Add(point);

            }
        }

        private static Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 point = uuu * p0 + 3f * uu * t * p1;

            return point;
        }

        private static Vector2 CatmullRomInterpolation(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector2 point = 0.5f * ((2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p2) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p2) * t3);

            return point;
        }


        /// <summary>
        /// 用于截取浮点数
        /// </summary>
        /// <param name="number">截取对象,需要避免输入含0位数的浮点值,比如1.00200,这可能因为精度问题导致函数报错,需要输入整数为单位数的浮点数,如1.234,不能输入12.34,或者465,这会引发预想之外的结果</param>
        /// <param name="highestPosition">结束的位数,位数是从第0位开始算,比如2.718,第0位数是2,第3位数是8</param>
        /// <param name="lowestPosition">开始的位数,位数是从第0位开始算,比如2.718,第0位数是2,第3位数是8</param>
        /// <returns></returns>
        public static int GetDigitsInRange(float number, int highestPosition, int lowestPosition)
        {
            if (highestPosition < lowestPosition || lowestPosition < 0)
            {
                return -1;
            }

            // 将浮点数转换为字符串并去除小数点
            string numberString = number.ToString().Replace(".", "");

            // 创建一个数组来存储数字
            int[] digits = new int[numberString.Length];

            // 将字符串中的每个字符解析为整数并存储在数组中
            for (int i = 0; i < numberString.Length; i++)
            {
                digits[i] = int.Parse(numberString[i].ToString());
            }

            // 从数组中获取指定范围的数字
            int[] result = new int[highestPosition - lowestPosition + 1];
            Array.Copy(digits, lowestPosition, result, 0, highestPosition - lowestPosition + 1);
            int extractedNumber = 0;
            for (int i = 0; i < result.Length; i++)
            {
                extractedNumber = extractedNumber * 10 + result[i];
            }

            return extractedNumber;
        }

        /// <summary>
        /// 转化浮点数为整数集合
        /// </summary>
        /// <param name="number">目标浮点数</param>
        /// <returns></returns>
        public static int[] GetDigitsArray(float number)
        {
            // 将浮点数转换为字符串
            string numberStr = number.ToString("F15");

            // 去除小数点，保留整数部分
            string integerPart = numberStr.Split('.')[0];

            // 将整数部分转换为字符数组
            char[] charArray = integerPart.ToCharArray();

            // 将字符数组转换为整数数组
            int[] digitsArray = new int[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                digitsArray[i] = charArray[i] - '0';
            }

            return digitsArray;
        }

        /// <summary>
        /// 使用贝塞尔曲线算法
        /// </summary>
        public class LightningGenerator
        {
            public static Vector2[] GenerateLightning(Vector2 startPoint, Vector2 endPoint, int numPoints, float deviation)
            {
                Vector2[] controlPoints = GetControlPoints(startPoint, endPoint, numPoints, deviation);
                Vector2[] lightningPath = GenerateBezierCurve(controlPoints, numPoints);

                return lightningPath;
            }

            private static Vector2[] GetControlPoints(Vector2 startPoint, Vector2 endPoint, int numPoints, float deviation)
            {
                Vector2[] controlPoints = new Vector2[numPoints + 2];
                controlPoints[0] = startPoint;
                controlPoints[numPoints + 1] = endPoint;

                float length = Vector2.Distance(startPoint, endPoint);
                float segmentLength = length / (numPoints + 1);

                for (int i = 1; i <= numPoints; i++)
                {
                    float t = i / (float)(numPoints + 1);
                    float offset = deviation * (1 - 2 * t);
                    Vector2 direction = Vector2.Normalize(endPoint - startPoint);
                    Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
                    Vector2 controlPoint = startPoint + t * (endPoint - startPoint) + offset * perpendicular;

                    controlPoints[i] = controlPoint;
                }

                return controlPoints;
            }

            private static Vector2[] GenerateBezierCurve(Vector2[] controlPoints, int numPoints)
            {
                Vector2[] curvePoints = new Vector2[numPoints];
                for (int i = 0; i < numPoints; i++)
                {
                    float t = (i + 1) / (float)(numPoints + 1);
                    curvePoints[i] = BezierInterpolation(controlPoints, t);
                }

                return curvePoints;
            }

            private static Vector2 BezierInterpolation(Vector2[] points, float t)
            {
                int n = points.Length - 1;
                Vector2 result = Vector2.Zero;

                for (int i = 0; i <= n; i++)
                {
                    float binomialCoefficient = BinomialCoefficient(n, i);
                    float powerT = (float)Math.Pow(t, i);
                    float powerOneMinusT = (float)Math.Pow(1 - t, n - i);
                    result += binomialCoefficient * powerT * powerOneMinusT * points[i];
                }

                return result;
            }

            private static int BinomialCoefficient(int n, int k)
            {
                if (k == 0 || k == n)
                {
                    return 1;
                }

                int[] coefficients = new int[n + 1];
                coefficients[0] = 1;

                for (int i = 1; i <= n; i++)
                {
                    coefficients[i] = 1;
                    for (int j = i - 1; j > 0; j--)
                    {
                        coefficients[j] += coefficients[j - 1];
                    }
                }

                return coefficients[k];
            }
        }

        /// <summary>
        /// 将游戏中的极角值转化为顺时针的正角值，处理角度对象
        /// </summary>
        public static float PolarToAngle_D(float polar)
        {
            if (polar < 0)
            {
                return (360 + polar);
            }
            else
            {
                return polar;
            }
        }

        /// <summary>
        /// 将游戏中的极角值转化为顺时针的正角值，处理弧度对象
        /// </summary>
        public static float PolarToAngle_R(float polar)
        {
            polar = MathHelper.ToRadians(polar);
            if (polar < 0)
            {
                return (MathHelper.TwoPi + polar);
            }
            else
            {
                return polar;
            }
        }

        /// <summary>
        /// 会自动替补-1元素
        /// </summary>
        /// <param name="list">目标集合</param>
        /// <param name="valueToAdd">替换为什么值</param>
        /// <param name="valueToReplace">替换的目标对象的值，不填则默认为-1</param>
        public static void AddOrReplace(this List<int> list, int valueToAdd, int valueToReplace = -1)


        {
            int index = list.IndexOf(valueToReplace);
            if (index >= 0)
            {
                list[index] = valueToAdd;
            }
            else
            {
                list.Add(valueToAdd);
            }
        }

        /// <summary>
        /// 返回一个集合的干净数量，排除数默认为-1，该扩展方法不会影响原集合
        /// </summary>
        public static int GetIntListCount(this List<int> list, int valueToReplace = -1)
        {
            List<int> result = new List<int>(list);
            result.RemoveAll(item => item == -1);
            return result.Count;
        }

        /// <summary>
        /// 返回一个集合的筛选副本，排除数默认为-1，该扩展方法不会影响原集合
        /// </summary>
        public static List<int> GetIntList(this List<int> list, int valueToReplace = -1)
        {
            List<int> result = new List<int>(list);
            result.RemoveAll(item => item == -1);
            return result;
        }

        /// <summary>
        /// 去除目标集合中所有-1元素
        /// </summary>
        /// <param name="list"></param>
        public static void SweepLoadLists(ref List<int> list)
        {
            if (list.Count > 0 && list.All(num => num == -1))
            {
                list.RemoveAll(num => num == -1);
            }
        }

        /// <summary>
        /// 单独的重载集合方法
        /// </summary>
        public static void UnLoadList(ref List<int> Lists)
        {
            Lists = new List<int>();
        }

        /// <summary>
        /// 计算两个向量的点积
        /// </summary>
        public static float DotProduct(this Vector2 vr1, Vector2 vr2)
        {
            return vr1.X * vr2.X + vr1.Y * vr2.Y;
        }

        /// <summary>
        /// 计算两个向量的叉积
        /// </summary>
        public static float CrossProduct(this Vector2 vr1, Vector2 vr2)
        {
            return vr1.X * vr2.Y - vr1.Y * vr2.X;
        }

        /// <summary>
        /// 获取向量与另一个向量的夹角
        /// </summary>
        /// <returns>自动返回劣弧角</returns>
        public static float VetorialAngle(this Vector2 vr1, Vector2 vr2)
        {
            return (float)Math.Acos(DotProduct(vr1, vr2) / (vr1.Length() * vr2.Length()));
        }
    }
}
