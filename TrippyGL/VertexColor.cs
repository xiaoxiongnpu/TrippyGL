﻿using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace TrippyGL
{
    /// <summary>
    /// Represents a vertex with Vector3 Position and Color4b Color
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexColor : IVertex
    {
        /// <summary>The size of each VertexColor, measured in bytes</summary>
        public const int SizeInBytes = (3 + 1) * 4;

        /// <summary>The vertex's Position</summary>
        public Vector3 Position;

        /// <summary>The vertex's Color</summary>
        public Color4b Color;

        /// <summary>
        /// Creates a VertexColor with the specified position and color
        /// </summary>
        /// <param name="position">The vertex Position</param>
        /// <param name="color">The vertex Color</param>
        public VertexColor(Vector3 position, Color4b color)
        {
            this.Position = position;
            this.Color = color;
        }

        /// <summary>
        /// Creates a VertexColor with the specified position and white color
        /// </summary>
        /// <param name="position">The vertex Position</param>
        public VertexColor(Vector3 position)
        {
            this.Position = position;
            this.Color = new Color4b(255, 255, 255, 255);
        }

        public override string ToString()
        {
            return String.Concat("(", Position.X, ", ", Position.Y, ", ", Position.Z, "), (", Color.R, ", ", Color.G, ", ", Color.B, ", ", Color.A, ")");
        }

        /// <summary>
        /// Creates an array with the descriptions of all the vertex attributes present in a VertexColor
        /// </summary>
        public VertexAttribDescription[] AttribDescriptions
        {
            get
            {
                return new VertexAttribDescription[]
                {
                    new VertexAttribDescription(ActiveAttribType.FloatVec3),
                    new VertexAttribDescription(ActiveAttribType.FloatVec4, true, VertexAttribPointerType.UnsignedByte)
                };
            }
        }
    }
}