using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace FaceTrackingBasics
{
    class Calc
    {
        //public static void calculateAngle(Vector3DF position, double theta, double alpha, Vector3DF n)
        public static void calculateAngle(Vector3DF eyePosition, Vector3DF reference, out double theta, out double alpha, out Vector3DF n)
        {
            Vector3DF normalizedEyePosition = Normalize(eyePosition);
            Vector3DF normalizedReference = Normalize(reference);

            float nX = (normalizedEyePosition.X + normalizedReference.X) / 2;
            float nY = (normalizedEyePosition.Y + normalizedReference.Y) / 2;
            float nZ = (normalizedEyePosition.Z + normalizedReference.Z) / 2;

            n = new Vector3DF(nX, nY, nZ);

            Vector3DF zNorm = new Vector3DF(0, 0, 1);
            Vector3DF yNorm = new Vector3DF(0, 1, 0);

            Vector3DF normalizedN = Normalize(n);

            theta = Math.Acos(DotProduct(normalizedN, zNorm));
            alpha = Math.Acos(DotProduct(normalizedN, yNorm)); 
        }

        public static Vector3DF Normalize(Vector3DF input)
        {
            float vectorLength = VectorLength(input);
            return new Vector3DF(input.X / vectorLength, input.Y / vectorLength, input.Z / vectorLength);
        }

        public static float VectorLength(Vector3DF input)
        { 
            return (float) Math.Sqrt(Math.Pow(input.X, 2) + Math.Pow(input.Y, 2) + Math.Pow(input.Z, 2));
        }

        public static float DotProduct(Vector3DF first, Vector3DF second)
        {
            return (first.X * second.X) + (first.Y * second.Y) + (first.Z * second.Z);
        }
    }
}
