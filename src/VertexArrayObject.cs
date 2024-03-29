﻿using Silk.NET.OpenGL;

internal sealed class VertexArrayObject<TVertexType, TIndexType> where TVertexType : unmanaged where TIndexType : unmanaged
{
    private readonly GL _gl;
    private readonly uint _handle;

    public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
    {
        _gl = gl;
        _handle = _gl.GenVertexArray();
        Bind();
        vbo.Bind();
        ebo.Bind();
    }

    public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
    {
        _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint)sizeof(TVertexType), (void*)(offSet * sizeof(TVertexType)));
        _gl.EnableVertexAttribArray(index);
    }

    public void Bind() => _gl.BindVertexArray(_handle);
}