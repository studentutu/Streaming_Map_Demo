//******************************************************************************
//
// Copyright (C) SAAB AB
//
// All rights, including the copyright, to the computer program(s)
// herein belong to SAAB AB. The program(s) may be used and/or
// copied only with the written permission of SAAB AB, or in
// accordance with the terms and conditions stipulated in the
// agreement/contract under which the program(s) have been
// supplied.
//
//
// Information Class:	COMPANY UNCLASSIFIED
// Defence Secrecy:		NOT CLASSIFIED
// Export Control:		NOT EXPORT CONTROLLED
//
//
// File			: Coordinate.cs
// Module		: Coordinate C#
// Description	: C# Bridge to gzCoordinate class
// Author		: Anders Mod�n		
// Product		: Coordinate 2.9.1
//		
//
//			
// NOTE:	Coordinate is a platform abstraction utility layer for C++. It contains 
//			design patterns and C++ solutions for the advanced programmer.
//
//
// Revision History...							
//									
// Who	Date	Description						
//									
// AMO	180301	Created file 	
//
//******************************************************************************

using System.Runtime.InteropServices;
using System;
using GizmoSDK.GizmoBase;

namespace GizmoSDK
{
    namespace Coordinate
    {
        public class CoordinateSystem : Reference
        {
            public CoordinateSystem(Datum datum=Datum.NOT_DEFINED, FlatGaussProjection proj=FlatGaussProjection.NOT_DEFINED, Type type=Type.NOT_DEFINED) : base(CoordinateSystem_create(datum,proj,type)) { }

            public CoordinateSystem(string definition) : base(CoordinateSystem_parse(definition)) { }

            public Datum Datum
            {
                get
                {
                    return CoordinateSystem_getDatum(GetNativeReference());
                }

                set
                {
                    CoordinateSystem_setDatum(GetNativeReference(),value);
                }
            }

            public HeightModel HeightModel
            {
                get
                {
                    return CoordinateSystem_getHeightModel(GetNativeReference());
                }
            }
            
            #region -------------- Native calls ------------------

            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr CoordinateSystem_create(Datum datum,FlatGaussProjection proj,Type type);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr CoordinateSystem_parse(string definition);

            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern Datum CoordinateSystem_getDatum(IntPtr reference);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void CoordinateSystem_setDatum(IntPtr reference,Datum value);

            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern HeightModel CoordinateSystem_getHeightModel(IntPtr reference);

            #endregion
        }
        

        /// <summary>
        /// Coordinate conversion instance. Converts between geodetic, projected and geocentric coordinate systems
        /// </summary>
        public class Coordinate : Reference
        {
            public static double DEG2RAD = Math.PI / 180.0;
            public static double RAD2DEG = 180.0 / Math.PI;
            
            public Coordinate() : base(Coordinate_create()) { }
            public Coordinate(IntPtr nativeReference) : base(nativeReference) { }

            public static Coordinate CreateFromString(Vec3D coordinate, string name)
            {
                var reference = Coordinate_parse(ref coordinate, name);
                return reference == IntPtr.Zero ? null : new Coordinate(reference);
            }

            public void SetLatPos(LatPos pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                Coordinate_setLatPos(GetNativeReference(), ref pos, datum);
            }

            public bool GetLatPos(out LatPos pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                pos = new LatPos();

                return Coordinate_getLatPos(GetNativeReference(), ref pos, datum);
            }

            public void SetCartPos(CartPos pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                Coordinate_setCartPos(GetNativeReference(), ref pos, datum);
            }

            public bool GetCartPos(out CartPos pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                pos = new CartPos();

                return Coordinate_getCartPos(GetNativeReference(), ref pos, datum);
            }

            public void SetProjPos(ProjPos pos, FlatGaussProjection projection = FlatGaussProjection.RT90)
            {
                Coordinate_setProjPos(GetNativeReference(), ref pos, projection);
            }

            public bool GetProjPos(out ProjPos pos, FlatGaussProjection projection = FlatGaussProjection.RT90)
            {
                pos = new ProjPos();

                return Coordinate_getProjPos(GetNativeReference(), ref pos, projection);
            }

            public void SetUTMPos(UTMPos pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                Coordinate_setUTMPos(GetNativeReference(),ref  pos, datum);
            }

            public bool GetUTMPos(out UTMPos pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                pos = new UTMPos();

                return Coordinate_getUTMPos(GetNativeReference(), ref pos, datum);
            }

            public void SetMGRS(string pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                Coordinate_setMGRSPos(GetNativeReference(), pos, datum);
            }

            public bool GetMGRS(out string pos, Datum datum = Datum.WGS84_ELLIPSOID)
            {
                IntPtr result=Coordinate_getMGRSPos(GetNativeReference(), datum);

                if(result!=IntPtr.Zero)
                {
                    pos = Marshal.PtrToStringUni(result);
                    return true;
                }
                else
                {
                    pos = "";
                    return false;
                }
            }

            /// <summary>
            /// Returns a local orientation matrix for a specific LatPos position with east,north and up base vectors
            /// </summary>
            /// <param name="latpos">Geodetic position</param>
            /// <param name="ellipsoid"></param>
            /// <returns>Matrix of [east][north][up] vectors</returns>
            public Matrix3 GetOrientationMatrix(LatPos latpos, Ellipsoid ellipsoid = Ellipsoid.WGS84)
            {
                Matrix3 mat=new Matrix3();

                Coordinate_getOrientationMatrix_LatPos(GetNativeReference(), ref latpos, ellipsoid,ref mat);

                return mat;
            }

            /// <summary>
            /// Returns a local orientation matrix for a specific CartPos position with east,north and up base vectors
            /// </summary>
            /// <param name="cartpos"></param>
            /// <param name="ellipsoid"></param>
            /// <returns>Matrix of [east][north][up] vectors</returns>
            public Matrix3 GetOrientationMatrix(CartPos cartpos, Ellipsoid ellipsoid = Ellipsoid.WGS84)
            {
                Matrix3 mat = new Matrix3();

                Coordinate_getOrientationMatrix_CartPos(GetNativeReference(), ref cartpos, ellipsoid,ref mat);

                return mat;
            }

            #region -------------- Native calls ------------------

            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr Coordinate_create();
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr Coordinate_parse(ref Vec3D coordinate, string name);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_setLatPos(IntPtr nativeReference,ref LatPos pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern bool Coordinate_getLatPos(IntPtr nativeReference,ref LatPos pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_setCartPos(IntPtr nativeReference, ref CartPos pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern bool Coordinate_getCartPos(IntPtr nativeReference, ref CartPos pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_setProjPos(IntPtr nativeReference, ref ProjPos pos, FlatGaussProjection projection);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern bool Coordinate_getProjPos(IntPtr nativeReference, ref ProjPos pos, FlatGaussProjection projection);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_setUTMPos(IntPtr nativeReference, ref UTMPos pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern bool Coordinate_getUTMPos(IntPtr nativeReference, ref UTMPos pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_setMGRSPos(IntPtr nativeReference, string pos, Datum datum);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr Coordinate_getMGRSPos(IntPtr nativeReference, Datum datum);

            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_getOrientationMatrix_LatPos(IntPtr nativeReference, ref LatPos pos, Ellipsoid ellipsoid,ref Matrix3 matrix);
            [DllImport(Platform.BRIDGE, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            private static extern void Coordinate_getOrientationMatrix_CartPos(IntPtr nativeReference, ref CartPos pos, Ellipsoid ellipsoid,ref Matrix3 matrix);



            #endregion
        }
    }
}

