using Silk.NET.OpenGL;
using System;

namespace Scop
{
    public static class BufferUtils
    {
        public static (uint vao, uint vbo, uint ebo) CreateBuffers<T>(GL gl, T[] vertices, uint[] indices) where T : unmanaged
        {
            uint vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            uint vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            unsafe
            {
                fixed (void* v = &vertices[0])
                {
                    gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(T)), v, BufferUsageARB.StaticDraw);
                }
            }

            uint ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            unsafe
            {
                fixed (void* i = &indices[0])
                {
                    gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
                }
            }

            return (vao, vbo, ebo);
        }
    }
}
