using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace FaceTrackingBasics
{
    class Calc
    {
        //public static void calculateAngle(Vector3DF position, double theta, double alpha, Vector3DF n)
        public static void calculateAngle(Vector3DF eyePosition, Vector3DF reference, out double theta, out double alpha, out Vector3DF n)
        {
            var mirrorPos = new Vector3D(0.0, 0.0, 0.0);
            var kinectPos = new Vector3D(0.3, 0.0, 0.2);
            var kinectOrientation = new Vector3D(0.0, 0.0, 1.0);
            var eyePos = new Vector3D(-eyePosition.X, eyePosition.Y, eyePosition.Z);
            var unitVectorI = new Vector3D(1,0,0);
            var unitVectorJ = new Vector3D(0,1,0);
            var unitVectorK = new Vector3D(0,0,1);

            var relativeEyePos = kinectPos
                + eyePos.Z * kinectOrientation
                + unitVectorI * (   eyePos.X * Math.Sin(Vector3D.AngleBetween(unitVectorI, kinectOrientation) * Math.PI / 180.0)
                                -   eyePos.Y * Math.Cos(Vector3D.AngleBetween(unitVectorI, kinectOrientation) * Math.PI / 180.0) )
                + unitVectorK * (   eyePos.X * Math.Sin(Vector3D.AngleBetween(unitVectorK, kinectOrientation) * Math.PI / 180.0)
                                -   eyePos.Y * Math.Cos(Vector3D.AngleBetween(unitVectorK, kinectOrientation) * Math.PI / 180.0) )
                + unitVectorJ * (   eyePos.Y * Math.Sin(Vector3D.AngleBetween(unitVectorJ, kinectOrientation) * Math.PI / 180.0) );

            Debug.WriteLine("Orig: " + eyePos.X + " : " + eyePos.Y + " : " + eyePos.Z);
            Debug.WriteLine("Relative: " + relativeEyePos.X + " : " + relativeEyePos.Y + " : " + relativeEyePos.Z);

            relativeEyePos.Normalize();
            var newReference = new Vector3D(reference.X, reference.Y, reference.Z);
            newReference.Normalize();
            var mirrorNormalPLane = (relativeEyePos + newReference) / 2;
            
            var zNorm = new Vector3D(1, 0, 0);
            var yNorm = new Vector3D(0, 1, 0);
            
            theta = Math.Acos(Vector3D.DotProduct(mirrorNormalPLane, zNorm));
            alpha = Math.Acos(Vector3D.DotProduct(mirrorNormalPLane, yNorm));

            n = new Vector3DF(0,0,0);
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
