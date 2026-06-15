using Silk.NET.OpenGL;
using System;

namespace Scop
{
    public static class BufferUtils
    {
        public static unsafe (uint vao, uint vbo, uint ebo) CreateBuffers<T>(GL Gl, T[] vertices, uint[] indices) where T : unmanaged
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            uint vbo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            unsafe
            {
                fixed (void* v = &vertices[0])
                {
                    Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(T)), v, BufferUsageARB.StaticDraw);
                }
            }

            uint ebo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            unsafe
            {
                fixed (void* i = &indices[0])
                {
                    Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
                }
            }

            uint stride = 8 * sizeof(float);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, (void*)12);

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, (void*)24);

            return (vao, vbo, ebo);
        }
    }
}
