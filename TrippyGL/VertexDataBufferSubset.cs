using OpenTK.Graphics.OpenGL4;
using System;

namespace TrippyGL
{
    /// <summary>
    /// A <see cref="VertexDataBufferSubset{T}"/> whose purpose is to store vertex data.
    /// </summary>
    /// <typeparam name="T">The type of struct (element) type this <see cref="VertexDataBufferSubset{T}"/> will manage.</typeparam>
    public class VertexDataBufferSubset<T> : DataBufferSubset<T> where T : struct
    {
        /// <summary>
        /// Creates a <see cref="VertexDataBufferSubset{T}"/> with the given <see cref="BufferObject"/>
        /// and target, offset into the buffer in bytes and storage length in elements.
        /// </summary>
        /// <param name="bufferObject">The <see cref="BufferObject"/> this subset will belong to.</param>
        /// <param name="storageOffsetBytes">The offset into the buffer's storage where this subset begins.</param>
        /// <param name="storageLength">The length of this subset measured in elements.</param>
        public VertexDataBufferSubset(BufferObject bufferObject, int storageOffsetBytes, int storageLength)
            : base(bufferObject, BufferTarget.ArrayBuffer, storageOffsetBytes, storageLength)
        {

        }

        /// <summary>
        /// Creates a <see cref="VertexDataBufferSubset{T}"/> with the given <see cref="BufferObject"/>
        /// and target, offset into the buffer in bytes and storage length in elements.
        /// </summary>
        /// <param name="bufferObject">The <see cref="BufferObject"/> this subset will belong to.</param>
        /// <param name="storageOffsetBytes">The offset into the buffer's storage where this subset begins.</param>
        /// <param name="storageLength">The length of this subset measured in elements.</param>
        /// <param name="data">A <see cref="Span{T}"/> containing the initial data to set to the subset.</param>
        /// <param name="dataWriteOffset">The offset into the subset's storage at which to start writting the initial data.</param>
        public VertexDataBufferSubset(BufferObject bufferObject, int storageOffsetBytes, int storageLength, Span<T> data, int dataWriteOffset = 0)
            : base(bufferObject, BufferTarget.ArrayBuffer, storageOffsetBytes, storageLength, data, dataWriteOffset)
        {

        }

        /// <summary>
        /// Creates a <see cref="VertexDataBufferSubset{T}"/> with the given <see cref="BufferObject"/>
        /// and target, with the subset covering the entire buffer's storage.
        /// </summary>
        /// <param name="bufferObject">The <see cref="BufferObject"/> this subset will belong to.</param>
        public VertexDataBufferSubset(BufferObject bufferObject) : base(bufferObject, BufferTarget.ArrayBuffer)
        {

        }

        /// <summary>
        /// Creates a <see cref="VertexDataBufferSubset{T}"/> with the given <see cref="BufferObject"/>
        /// and target, with the subset covering the entire buffer's storage and sets initial data.
        /// </summary>
        /// <param name="bufferObject">The <see cref="BufferObject"/> this subset will belong to.</param>
        /// <param name="data">A <see cref="Span{T}"/> containing the initial data to set to the subset.</param>
        public VertexDataBufferSubset(BufferObject bufferObject, Span<T> data) : base(bufferObject, BufferTarget.ArrayBuffer, data)
        {

        }

        /// <summary>
        /// Creates a <see cref="VertexDataBufferSubset{T}"/> that occupies the same area in the same buffer as
        /// another buffer subset.
        /// </summary>
        /// <param name="subsetToCopy">The <see cref="BufferObjectSubset"/> to copy the range form.</param>
        public VertexDataBufferSubset(BufferObjectSubset subsetToCopy) : base(subsetToCopy, BufferTarget.ArrayBuffer)
        {

        }

        /// <summary>
        /// Creates a <see cref="VertexDataBufferSubset{T}"/> that occupies the same area in the same buffer and uses the
        /// same struct type as another <see cref="DataBufferSubset{T}"/>.
        /// </summary>
        /// <param name="copy">The <see cref="DataBufferSubset{T}"/> to copy the range from.</param>
        public VertexDataBufferSubset(DataBufferSubset<T> copy) : base(copy, BufferTarget.ArrayBuffer)
        {

        }
    }
}
