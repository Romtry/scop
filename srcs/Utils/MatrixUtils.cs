using Silk.NET.Maths;
using System;

namespace Scop
{
    static class MatrixUtils
    {
        /// <summary>
        /// Crée une matrice de projection perspective
        /// </summary>
        public static Matrix4X4<float> CreatePerspectiveProjection(float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            float f = 1.0f / MathF.Tan(fov / 2.0f);
            float nf = 1.0f / (nearPlane - farPlane);

            return new Matrix4X4<float>(
                f / aspectRatio, 0, 0, 0,
                0, f, 0, 0,
                0, 0, (farPlane + nearPlane) * nf, -1,
                0, 0, (2 * farPlane * nearPlane) * nf, 0
            );
        }

        /// <summary>
        /// Crée une matrice de projection orthographique (pour 2D)
        /// </summary>
        public static Matrix4X4<float> CreateOrthographicProjection(float left, float right, float bottom, float top, float nearPlane, float farPlane)
        {
            float rl = 1.0f / (right - left);
            float tb = 1.0f / (top - bottom);
            float fn = 1.0f / (farPlane - nearPlane);

            return new Matrix4X4<float>(
                2 * rl, 0, 0, 0,
                0, 2 * tb, 0, 0,
                0, 0, -2 * fn, 0,
                -(right + left) * rl, -(top + bottom) * tb, -(farPlane + nearPlane) * fn, 1
            );
        }

        /// <summary>
        /// Crée une matrice de vue (caméra)
        /// </summary>
        public static Matrix4X4<float> CreateLookAt(Vector3D<float> eye, Vector3D<float> center, Vector3D<float> up)
        {
            Vector3D<float> f = Vector3D.Normalize(center - eye);
            Vector3D<float> s = Vector3D.Normalize(Vector3D.Cross(f, up));
            Vector3D<float> u = Vector3D.Cross(s, f);

            return new Matrix4X4<float>(
                s.X, u.X, -f.X, 0,
                s.Y, u.Y, -f.Y, 0,
                s.Z, u.Z, -f.Z, 0,
                -Vector3D.Dot(s, eye), -Vector3D.Dot(u, eye), Vector3D.Dot(f, eye), 1
            );
        }

        /// <summary>
        /// Crée une matrice de modèle (identité de base)
        /// </summary>
        public static Matrix4X4<float> CreateIdentity()
        {
            return Matrix4X4<float>.Identity;
        }

        /// <summary>
        /// Applique une translation à une matrice
        /// </summary>
        public static Matrix4X4<float> Translate(Matrix4X4<float> matrix, Vector3D<float> translation)
        {
            return matrix * Matrix4X4.CreateTranslation(translation);
        }

        /// <summary>
        /// Applique une rotation autour de X
        /// </summary>
        public static Matrix4X4<float> RotateX(Matrix4X4<float> matrix, float radians)
        {
            return matrix * Matrix4X4.CreateRotationX(radians);
        }

        /// <summary>
        /// Applique une rotation autour de Y
        /// </summary>
        public static Matrix4X4<float> RotateY(Matrix4X4<float> matrix, float radians)
        {
            return matrix * Matrix4X4.CreateRotationY(radians);
        }

        /// <summary>
        /// Applique une rotation autour de Z
        /// </summary>
        public static Matrix4X4<float> RotateZ(Matrix4X4<float> matrix, float radians)
        {
            return matrix * Matrix4X4.CreateRotationZ(radians);
        }

        /// <summary>
        /// Applique une scale à une matrice
        /// </summary>
        public static Matrix4X4<float> Scale(Matrix4X4<float> matrix, Vector3D<float> scale)
        {
            return matrix * Matrix4X4.CreateScale(scale);
        }
    }
}
