﻿using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace TrippyGL
{
    /// <summary>
    /// An abstract class containing code shared among all BufferObject types.
    /// </summary>
    public abstract class BufferObject : IDisposable
    {
        // Note: BufferObject contains code shared among all BufferObject types.
        // The BufferObject class takes care of handling the object. This includes binding (to the correct target), creating the
        // object (storing the handle) and disposing (destroying the buffer object)
        
        // For binding, BufferObject manages some static fields that store the last BufferObject's handle bound to each target.
        // This is used to check whether a BufferObject is currently bound, and that gets used so to not bind the same BufferObject
        // twice consecutively, as this might affect performance.

        // BufferObject also provides private protected methods for initializing the buffer's storage and different check parameter
        // functions to be consisten across all BufferObjects when throwing exceptions on methods like SetData() and GetData()
        
        /// <summary>
        /// This list stores which BufferObject.Handle was las bound to each BufferTarget.
        /// Each index is associated to the BufferTarget on the same index in bindsTarget
        /// </summary>
        private protected static List<int> binds;
        
        /// <summary>Stores the associated BufferTarget of each index in the 'binds' list</summary>
        private protected static List<BufferTarget> bindsTargets;

        /// <summary>
        /// Initializes BufferObject's static fields. This internal function is called by once by TrippyGL.Init()
        /// </summary>
        internal static void Init()
        {
            binds = new List<int>(14);
            bindsTargets = new List<BufferTarget>(14);

        }

        /// <summary>
        /// Resets all BufferObject bind variables. These variables are used by Bind functions so to not unnecessarily bind the same BufferObject twice
        /// </summary>
        public static void ResetBindStates()
        {
            for (int i = 0; i < binds.Count; i++)
                binds[i] = -1;
        }

        /// <summary>
        /// Ensures a buffer's handle is bound to the specified bufferTarget
        /// </summary>
        /// <param name="bufferTarget"></param>
        /// <param name="bufferHandle"></param>
        public static void EnsureBufferBound(BufferTarget bufferTarget, int bufferHandle)
        {
            int index = GetBindingTargetIndex(bufferTarget);
            if (binds[index] != bufferHandle)
            {
                binds[index] = bufferHandle;
                GL.BindBuffer(bufferTarget, bufferHandle);
            }
        }

        /// <summary>
        /// Gets the index on the 'binds' list for the specified BufferTarget.
        /// If there's no index for that BufferTarget, it's created
        /// </summary>
        /// <param name="bufferTarget">The BufferTarget to get the binds list index for</param>
        private static int GetBindingTargetIndex(BufferTarget bufferTarget)
        {
            for (int i = 0; i < bindsTargets.Count; i++)
                if (bindsTargets[i] == bufferTarget)
                    return i;
            binds.Add(-1);
            bindsTargets.Add(bufferTarget);
            return binds.Count - 1;
        }



        /// <summary>The GL Buffer Object's name</summary>
        public readonly int Handle;

        /// <summary>The length of this buffer object's storage, measured in elements</summary>
        public abstract int StorageLength { get; }

        /// <summary>The length of this buffer object's storage, measured in bytes</summary>
        public abstract int StorageLengthInBytes { get; }

        /// <summary>The size of each element, measured in bytes</summary>
        public abstract int ElementSize { get; }

        /// <summary>Whether this buffer object is the currently bound one on it's BufferTarget</summary>
        public bool IsCurrentlyBound { get { return binds[bindingIndex] == Handle; } }

        /// <summary>The target this BufferObject will always bind to</summary>
        public readonly BufferTarget BufferTarget;

        /// <summary>The index on the static 'binds' array where this BufferObject's BufferTarget last bound handle is stored</summary>
        private protected readonly int bindingIndex;

        /// <summary>
        /// Creates a BufferObject with the specified bufferTarget.
        /// The buffer object's storage isn't initialized by this constructor
        /// </summary>
        /// <param name="bufferTarget"></param>
        private protected BufferObject(BufferTarget bufferTarget)
        {
            Handle = GL.GenBuffer();
            this.BufferTarget = bufferTarget;
            bindingIndex = GetBindingTargetIndex(bufferTarget);
        }

        ~BufferObject()
        {
            if (TrippyLib.isLibActive)
                dispose();
        }
        
        /// <summary>
        /// Initializes this buffer object's storage and copies the specified initial data from a specified location in a given data array. Checks validity of arguments.
        /// The InitializeStorage methods are intended to be used by the constructors of all classes that derive from BufferObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bufferTarget"></param>
        /// <param name="storageLength"></param>
        /// <param name="elementSize"></param>
        /// <param name="dataOffset"></param>
        /// <param name="data"></param>
        /// <param name="usageHint"></param>
        private protected void InitializeStorage<T>(int storageLength, int elementSize, int dataOffset, T[] data, BufferUsageHint usageHint) where T : struct
        {
            ValidateInitWithDataParams(storageLength, dataOffset, data);

            EnsureBound();
            GL.BufferData(this.BufferTarget, storageLength * elementSize, ref data[dataOffset], usageHint);
        }

        /// <summary>
        /// Initializes this buffer object's storage without copying any initial data to the storage. Checks validity of arguments.
        /// The InitializeStorage methods are intended to be used by the constructors of all classes that derive from BufferObject
        /// </summary>
        /// <param name="bufferTarget">The OpenGL buffer target this BufferObject binds to</param>
        /// <param name="storageLengthInBytes">The desired length for this buffer object's storage, measured in bytes</param>
        /// <param name="usageHint">The desired BufferUsageHint</param>
        private protected void InitializeStorage(int storageLengthInBytes, BufferUsageHint usageHint)
        {
            ValidateInitWithoutDataParams(storageLengthInBytes);

            EnsureBound();
            GL.BufferData(this.BufferTarget, storageLengthInBytes, IntPtr.Zero, usageHint);
        }

        /// <summary>
        /// Ensures this buffer object is the currently bound one on it's BufferTarget
        /// </summary>
        public void EnsureBound()
        {
            if (binds[bindingIndex] != Handle)
                Bind();
        }

        /// <summary>
        /// Binds this buffer object to it's BufferTarget. Prefer using EnsureBound() instead, so to prevent unnecesary binds
        /// </summary>
        public void Bind()
        {
            GL.BindBuffer(this.BufferTarget, Handle);
            binds[bindingIndex] = Handle;
        }

        /// <summary>
        /// This method disposes the buffer object with no checks at all
        /// </summary>
        private void dispose()
        {
            GL.DeleteBuffer(Handle);
        }

        /// <summary>
        /// Disposes this buffer object, deleting and releasing the resources it uses.
        /// The buffer object cannot be used after it's been disposed
        /// </summary>
        public void Dispose()
        {
            dispose();
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return String.Concat("Handle=", Handle, ". StorageLength=", StorageLength, ", ElementSize=", ElementSize, " IsCurrentlyBound=", IsCurrentlyBound);
        }

        /// <summary>
        /// Checks that all the parameters for an init operation are valid and throws an appropiate exception if any parameter is invalid
        /// </summary>
        /// <param name="storageLength">The length of the buffer object's storage, measured in elements</param>
        /// <param name="elementSize">The size of each element, measured in bytes</param>
        /// <param name="dataOffset">The first element's index in the data array to start reading from</param>
        /// <param name="data">The array containing the data to upload</param>
        private protected static void ValidateInitWithDataParams<T>(int storageLength, int dataOffset, T[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data array can't be null nor empty", "data");

            if (storageLength <= 0)
                throw new ArgumentOutOfRangeException("storageLength", storageLength, "Storage length must be greater than 0");

            //if (elementSize <= 0) //I'm just gonna assume this isn't necessary. This value isn't really user-controlled after all
            //    throw new ArgumentOutOfRangeException("elementSize", elementSize, "Element size must be greater than 0");

            if (dataOffset < 0 || dataOffset >= data.Length)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "Data offset must be in the range [0, data.Length)");

            if (data.Length - dataOffset < storageLength)
                throw new ArgumentOutOfRangeException("There data array isn't long enough to be able to read storageLength elements starting from dataOffset");
        }

        /// <summary>
        /// Checks that all paremeters for an init operation are valid and throws an appropiate exception if any parameter is invalid
        /// </summary>
        /// <param name="storageLengthInBytes">The length of the buffer object's storage, measured in bytes</param>
        private protected static void ValidateInitWithoutDataParams(int storageLengthInBytes)
        {
            if (storageLengthInBytes <= 0)
                throw new ArgumentOutOfRangeException("storageLengthInBytes", storageLengthInBytes, "Storage length must be greater than 0");
        }

        /// <summary>
        /// Checks that all parameters for a set operation are valid and throws an appropiate exception if any parameter is invalid
        /// </summary>
        /// <param name="storageOffset">The index of the first element in the storage to write</param>
        /// <param name="dataOffset">The index of the first element in the data array to read</param>
        /// <param name="dataLength">The amount of elements to copy</param>
        /// <param name="storageLength">The length of the buffer object's storage</param>
        /// <param name="data">The array containing the data to copy</param>
        private protected static void ValidateSetParams<T>(int storageOffset, int dataOffset, int dataLength, int storageLength, T[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data", "The data array can't be null nor empty");

            if (storageOffset < 0 || storageOffset >= storageLength)
                throw new ArgumentOutOfRangeException("storageOffset", storageOffset, "Storage offset must be in the range [0, StorageLength)");

            if (dataOffset < 0 || dataOffset >= data.Length)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "Data offset must be in the range [0, data.Length)");

            if (data.Length - dataOffset < dataLength)
                throw new ArgumentOutOfRangeException("There isn't enough data in the array to and read dataLength elements starting from index dataOffset");
            
            if (dataLength > storageLength - storageOffset)
                throw new ArgumentOutOfRangeException("The buffer's storage isn't big enough to write dataLength elements starting from storageOffset");
        }

        /// <summary>
        /// Checks that all parameters for a get operation are valid and throws an appropiate exception if any parameter is invalid
        /// </summary>
        /// <param name="storageOffset">The index of the first element in the storage to read</param>
        /// <param name="dataOffset">The index of the first element in the data array to write</param>
        /// <param name="dataLength">The amount of elements to copy</param>
        /// <param name="storageLength">The length of the buffer object's storage measured in elements</param>
        /// <param name="data">The array to which the data will be copied to</param>
        private protected static void ValidateGetParams<T>(int storageOffset, int dataOffset, int dataLength, int storageLength, T[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data array can't be null nor empty", "data");

            if (storageOffset < 0 || storageOffset >= storageLength)
                throw new ArgumentOutOfRangeException("storageOffset", storageOffset, "Storage offset must be in the range [0, StorageLength)");

            if (dataOffset < 0 || dataOffset >= data.Length)
                throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "Data offset must be in the range [0, data.Length)");

            if (dataLength > storageLength - storageOffset)
                throw new ArgumentOutOfRangeException("There isn't enough data in the buffer object's storage to read dataLength elements starting from index storageOffset");

            if (data.Length - dataOffset < dataLength)
                throw new ArgumentOutOfRangeException("There data array ins't big enough to write dataLength elements starting from index dataOffset");
        }

        /// <summary>
        /// Checks that all parameters for a get operation are valid and throws an appropiate exception if any parameter is invalid
        /// </summary>
        /// <param name="data">The array to which te data will be copied to</param>
        /// <param name="storageLength">The length of the buffer object's storage measured in elements</param>
        private protected static void ValidateGetParams<T>(T[] data, int storageLength)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("Data array can't be null nor empty", "data");

            if (data.Length < storageLength)
                throw new ArgumentException("The data array isn't big enough to write the entire buffer object's storage to it");
        }



        private interface IBufferBindingPoint
        {
            
        }

        private class BufferBindingPoint : IBufferBindingPoint
        {

        }
    }
}
