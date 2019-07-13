﻿using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

namespace TrippyGL
{
    public class TextureCubemap : Texture
    {
        /// <summary>The size of a face from this cubemap</summary>
        public int Size { get; private set; }

        public TextureCubemap(GraphicsDevice graphicsDevice, int size, TextureImageFormat imageFormat = TextureImageFormat.Color4b) : base(graphicsDevice, TextureTarget.TextureCubeMap, imageFormat)
        {
            ValidateTextureSize(size);
            Size = size;
            GraphicsDevice.BindTextureSetActive(this);
            GL.TexParameter(TextureType, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureType, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureType, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureType, TextureParameterName.TextureMinFilter, (int)DefaultMinFilter);
            GL.TexParameter(TextureType, TextureParameterName.TextureMagFilter, (int)DefaultMagFilter);

            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, PixelInternalFormat, size, size, 0, PixelFormat, PixelType, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, PixelInternalFormat, size, size, 0, PixelFormat, PixelType, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, PixelInternalFormat, size, size, 0, PixelFormat, PixelType, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, PixelInternalFormat, size, size, 0, PixelFormat, PixelType, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, PixelInternalFormat, size, size, 0, PixelFormat, PixelType, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, PixelInternalFormat, size, size, 0, PixelFormat, PixelType, IntPtr.Zero);
        }

        /// <summary>
        /// Sets the data of a specified area of the texture, copying it from the specified pointer.
        /// The pointer is not checked nor deallocated, memory exceptions may happen if you don't ensure enough memory can be read
        /// </summary>
        /// <param name="dataPtr">The pointer for reading the data</param>
        /// <param name="rectX">The X coordinate of the first pixel to write</param>
        /// <param name="rectY">The Y coordinate of the first pixel to write</param>
        /// <param name="rectWidth">The width of the rectangle of pixels to write</param>
        /// <param name="rectHeight">The height of the rectangle of pixels to write</param>
        /// <param name="pixelFormat">The pixel format the data will be read as. 0 for this texture's default</param>
        public void SetData(CubeMapFace face, IntPtr dataPtr, int rectX, int rectY, int rectWidth, int rectHeight, OpenTK.Graphics.OpenGL4.PixelFormat pixelFormat = 0)
        {
            ValidateRectOperation(rectX, rectY, rectWidth, rectHeight);

            GraphicsDevice.BindTextureSetActive(this);
            GL.TexSubImage2D((TextureTarget)face, 0, rectX, rectY, rectWidth, rectHeight, pixelFormat == 0 ? PixelFormat : pixelFormat, PixelType, dataPtr);
        }

        /// <summary>
        /// Sets the data of a specified area of the texture, copying the new data from a specified array
        /// </summary>
        /// <typeparam name="T">The type of struct to save the data as. This struct's format should match the texture pixel's format</typeparam>
        /// <param name="data">The array containing the new texture data</param>
        /// <param name="dataOffset">The index of the first element in the data array to start reading from</param>
        /// <param name="rectX">The X coordinate of the first pixel to write</param>
        /// <param name="rectY">The Y coordinate of the first pixel to write</param>
        /// <param name="rectWidth">The width of the rectangle of pixels to write</param>
        /// <param name="rectHeight">The height of the rectangle of pixels to write</param>
        /// <param name="pixelFormat">The pixel format the data will be read as. 0 for this texture's default</param>
        public void SetData<T>(CubeMapFace face, T[] data, int dataOffset, int rectX, int rectY, int rectWidth, int rectHeight, OpenTK.Graphics.OpenGL4.PixelFormat pixelFormat = 0) where T : struct
        {
            ValidateSetOperation(data, dataOffset, rectX, rectY, rectWidth, rectHeight);

            GraphicsDevice.BindTextureSetActive(this);
            GL.TexSubImage2D((TextureTarget)face, 0, rectX, rectY, rectWidth, rectHeight, pixelFormat == 0 ? PixelFormat : pixelFormat, PixelType, ref data[dataOffset]);
        }

        /// <summary>
        /// Sets the data of the entire texture, copying the new data from a given array
        /// </summary>
        /// <typeparam name="T">The type of struct to save the data as. This struct's format should match the texture pixel's format</typeparam>
        /// <param name="data">The array containing the new texture data</param>
        /// <param name="dataOffset">The index of the first element in the array to start reading from</param>
        /// <param name="pixelFormat">The pixel format the data will be read as. 0 for this texture's default</param>
        public void SetData<T>(CubeMapFace face, T[] data, int dataOffset = 0, OpenTK.Graphics.OpenGL4.PixelFormat pixelFormat = 0) where T : struct
        {
            SetData(face, data, dataOffset, 0, 0, Size, Size, pixelFormat);
        }

        public void SetData(CubeMapFace face, string file)
        {
            if (ImageFormat != TextureImageFormat.Color4b)
                throw new InvalidOperationException("To set a cubemap's face from a file, the cubemap's format must be");

            using (Bitmap b = new Bitmap(file))
            {
                if (b.Width != Size || b.Height != Size)
                    throw new InvalidOperationException("The size of the image must match the size of the cubemap faces");
                BitmapData bits = b.LockBits(new System.Drawing.Rectangle(0, 0, Size, Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GraphicsDevice.BindTextureSetActive(this);
                GL.TexSubImage2D((TextureTarget)face, 0, 0, 0, Size, Size, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType, bits.Scan0);
                b.UnlockBits(bits);
            }
        }

        /// <summary>
        /// Gets the data of the entire texture and copies it to a specified pointer.
        /// The pointer is not checked nor deallocated, memory exceptions may happen if you don't ensure enough memory can be read
        /// </summary>
        /// <param name="dataPtr">The pointer for writting the data</param>
        /// <param name="pixelDataFormat">The format of the pixel data in dataPtr. Accepted values are: Red, Rg, Rgb, Bgr, Rgba, Bgra, DepthComponent and StencilIndex</param>
        /// <param name="pixelFormat">The pixel format the data will be read as. 0 for this texture's default</param>
        public void GetData(CubeMapFace face, IntPtr dataPtr, OpenTK.Graphics.OpenGL4.PixelFormat pixelFormat = 0)
        {
            GraphicsDevice.BindTextureSetActive(this);
            GL.GetTexImage((TextureTarget)face, 0, pixelFormat == 0 ? PixelFormat : pixelFormat, PixelType, dataPtr);
        }

        /// <summary>
        /// Gets the data of the entire texture, copying the texture data to a specified array
        /// </summary>
        /// <typeparam name="T">The type of struct to save the data as. This struct's format should match the texture pixel's format</typeparam>
        /// <param name="data">The array in which to write the texture data</param>
        /// <param name="dataOffset">The index of the first element in the data array to start writing from</param>
        /// <param name="pixelFormat">The pixel format the data will be read as. 0 for this texture's default</param>
        public void GetData<T>(CubeMapFace face, T[] data, int dataOffset = 0, OpenTK.Graphics.OpenGL4.PixelFormat pixelFormat = 0) where T : struct
        {
            ValidateGetOperation(data, dataOffset);

            GraphicsDevice.BindTextureSetActive(this);
            GL.GetTexImage((TextureTarget)face, 0, pixelFormat == 0 ? PixelFormat : pixelFormat, PixelType, ref data[dataOffset]);
        }


        /// <summary>
        /// Sets the texture coordinate wrapping modes for when a texture is sampled outside the [0, 1] range
        /// </summary>
        /// <param name="sWrapMode">The wrap mode for the S (or texture-X) coordinate</param>
        /// <param name="tWrapMode">The wrap mode for the T (or texture-Y) coordinate</param>
        /// <param name="rWrapMode">The wrap mode for the R (or texture-Z) coordinate</param>
        public void SetWrapModes(TextureWrapMode sWrapMode, TextureWrapMode tWrapMode, TextureWrapMode rWrapMode)
        {
            GraphicsDevice.BindTextureSetActive(this);
            GL.TexParameter(TextureType, TextureParameterName.TextureWrapS, (int)sWrapMode);
            GL.TexParameter(TextureType, TextureParameterName.TextureWrapT, (int)tWrapMode);
            GL.TexParameter(TextureType, TextureParameterName.TextureWrapR, (int)rWrapMode);
        }


        private protected void ValidateTextureSize(int size)
        {
            if (size <= 0 || size > GraphicsDevice.MaxCubeMapTextureSize)
                throw new ArgumentOutOfRangeException("size", size, "Cubemap size must be in the range (0, MAX_TEXTURE_CUBEMAP_SIZE]");
        }

        private protected void ValidateSetOperation<T>(T[] data, int dataOffset, int rectX, int rectY, int rectWidth, int rectHeight) where T : struct
        {
            if (dataOffset < 0 || dataOffset >= data.Length)
                throw new ArgumentOutOfRangeException("dataOffset", "dataOffset must be in the range [0, data.Length)");

            ValidateRectOperation(rectX, rectY, rectWidth, rectHeight);

            if (data.Length - dataOffset < rectWidth * rectHeight)
                throw new ArgumentException("The data array isn't big enough to read the specified amount of data", "data");
        }

        private protected void ValidateGetOperation<T>(T[] data, int dataOffset) where T : struct
        {
            if (dataOffset < 0 || dataOffset >= data.Length)
                throw new ArgumentOutOfRangeException("dataOffset", "dataOffset must be in the range [0, data.Length)");

            if (data.Length - dataOffset < Size * Size)
                throw new ArgumentException("The provided data array isn't big enough for the texture starting from dataOffset", "data");
        }

        internal void ValidateRectOperation(int rectX, int rectY, int rectWidth, int rectHeight)
        {
            if (rectX < 0 || rectY >= Size)
                throw new ArgumentOutOfRangeException("rectX", rectX, "rectX must be in the range [0, this.Size)");

            if (rectY < 0 || rectY >= Size)
                throw new ArgumentOutOfRangeException("rectY", rectY, "rectY must be in the range [0, this.Size)");

            if (rectWidth <= 0 || rectHeight <= 0)
                throw new ArgumentOutOfRangeException("rectWidth and rectHeight must be greater than 0");

            if (rectWidth > Size - rectX)
                throw new ArgumentOutOfRangeException("rectWidth", rectWidth, "rectWidth is too large");

            if (rectHeight > Size - rectY)
                throw new ArgumentOutOfRangeException("rectHeight", rectHeight, "rectHeight is too large");
        }
    }
}