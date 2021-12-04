﻿// Copyright (c) 2017 Eric Mellino
// Copyright (c) 2019  Jean-Philippe Bruyère <jp_bruyere@hotmail.com>
//
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.ObjectModel;

namespace Vulkan
{
	public struct VkApplicationInfoPtr2 : IDisposable
	{
		IntPtr handle;
		VkApplicationInfoPtr2 (IntPtr ptr) {
			handle = ptr;
		}
		VkApplicationInfoPtr2 (IEnumerable<VkApplicationInfo> str) {
			handle = str.ToArray().PinPointer();
		}
		public VkApplicationInfo Single {
			set {
				if (handle != IntPtr.Zero)
					handle.Unpin();
				handle = value.PinPointer();
			}
		}

		//public static implicit operator IEnumerable<VkApplicationInfo> (VkApplicationInfoPtr2 pt)	=> Marshal.PtrToStructure <VkApplicationInfo> (pt.handle);
		public static implicit operator IntPtr (VkApplicationInfoPtr2 pt) => pt.handle;
		public static implicit operator VkApplicationInfoPtr2 (IntPtr ptr) => new VkApplicationInfoPtr2 (ptr);
		public void Dispose() => handle.Unpin();
	}
	/*
	public partial struct VkApplicationInfo : IDisposable
	{
		//public static implicit operator VkApplicationInfoPtr2
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}*/
	public partial struct VkClearValue
	{
		public VkClearValue(float r, float g, float b)
		{
			depthStencil = default(VkClearDepthStencilValue);
			color = new VkClearColorValue(r, g, b);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct VkClearColorValue
	{
		public Span<float> floats
		{
			get
			{
				Span<VkClearColorValue> valSpan = MemoryMarshal.CreateSpan(ref this, 1);
				return MemoryMarshal.Cast<VkClearColorValue, float>(valSpan);
			}
		}
		public Span<int> ints
		{
			get
			{
				Span<VkClearColorValue> valSpan = MemoryMarshal.CreateSpan(ref this, 1);
				return MemoryMarshal.Cast<VkClearColorValue, int>(valSpan);
			}
		}
		public Span<uint> uints
		{
			get
			{
				Span<VkClearColorValue> valSpan = MemoryMarshal.CreateSpan(ref this, 1);
				return MemoryMarshal.Cast<VkClearColorValue, uint>(valSpan);
			}
		}
		public VkClearColorValue(float r, float g, float b, float a = 1.0f) : this()
		{
			floats[0] = r;
			floats[1] = g;
			floats[2] = b;
			floats[3] = a;
		}

		public VkClearColorValue(int r, int g, int b, int a = 255) : this()
		{
			ints[0] = r;
			ints[1] = g;
			ints[2] = b;
			ints[3] = a;
		}

		public VkClearColorValue(uint r, uint g, uint b, uint a = 255) : this()
		{
			uints[0] = r;
			uints[1] = g;
			uints[2] = b;
			uints[3] = a;
		}
	}
	/// <summary>
	/// Structure specifying a 3x4 row-major affine transformation matrix.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 48)]
	public struct VkTransformMatrixKHR
	{
		/// <summary>
		/// Get row by index.
		/// </summary>
		/// <returns>The span of the row.</returns>
		public Span<float> this[int i] => AsSpan.Slice(i * 4, 3);
		/// <summary>
		/// Get the whole matrix as a single span of floats.
		/// </summary>
		/// <value>The span of the matrix.</value>
		public Span<float> AsSpan
		{
			get
			{
				Span<VkTransformMatrixKHR> valSpan = MemoryMarshal.CreateSpan(ref this, 1);
				return MemoryMarshal.Cast<VkTransformMatrixKHR, float>(valSpan);
			}
		}
	}


	public partial struct VkClearDepthStencilValue
	{
		public VkClearDepthStencilValue(float depth, uint stencil)
		{
			this.depth = depth;
			this.stencil = stencil;
		}
	}
	public partial struct VkOffset3D
	{
		public VkOffset3D(int _x = 0, int _y = 0, int _z = 0)
		{
			x = _x; y = _y; z = _z;
		}
	}
	public partial struct VkImageSubresourceLayers
	{
		public VkImageSubresourceLayers(VkImageAspectFlags _aspectMask = VkImageAspectFlags.Color,
			uint _layerCount = 1, uint _mipLevel = 0, uint _baseArrayLayer = 0)
		{
			aspectMask = _aspectMask;
			layerCount = _layerCount;
			mipLevel = _mipLevel;
			baseArrayLayer = _baseArrayLayer;
		}
	}
	public partial struct VkImageSubresourceRange
	{
		public VkImageSubresourceRange(
			VkImageAspectFlags aspectMask,
			uint baseMipLevel = 0, uint levelCount = 1,
			uint baseArrayLayer = 0, uint layerCount = 1)
		{
			this.aspectMask = aspectMask;
			this.baseMipLevel = baseMipLevel;
			this.levelCount = levelCount;
			this.baseArrayLayer = baseArrayLayer;
			this.layerCount = layerCount;
		}
	}
	public partial struct VkDescriptorSetLayoutBinding
	{
		public VkDescriptorSetLayoutBinding(uint _binding, VkShaderStageFlags _stageFlags, VkDescriptorType _descriptorType, uint _descriptorCount = 1)
		{
			binding = _binding;
			this._descriptorType = (int)_descriptorType;
			descriptorCount = _descriptorCount;
			stageFlags = _stageFlags;
			_pImmutableSamplers = IntPtr.Zero;
		}
	}
	public partial struct VkDescriptorSetLayoutCreateInfo
	{
		public VkDescriptorSetLayoutCreateInfo(VkDescriptorSetLayoutCreateFlags flags, uint bindingCount, IntPtr pBindings)
		{
			this._sType = (int)VkStructureType.DescriptorSetLayoutCreateInfo;
			pNext = IntPtr.Zero;
			this.flags = flags;
			this.bindingCount = bindingCount;
			_pBindings = pBindings;
		}
	}
	public partial struct VkVertexInputBindingDescription
	{
		public VkVertexInputBindingDescription(uint _binding, uint _stride, VkVertexInputRate _inputRate = VkVertexInputRate.Vertex)
		{
			binding = _binding;
			stride = _stride;
			this._inputRate = (int)_inputRate;
		}
	}

	public partial struct VkVertexInputAttributeDescription
	{
		public VkVertexInputAttributeDescription(uint _binding, uint _location, VkFormat _format, uint _offset = 0)
		{
			location = _location;
			binding = _binding;
			this._format = (int)_format;
			offset = _offset;
		}
		public VkVertexInputAttributeDescription(uint _location, VkFormat _format, uint _offset = 0)
		{
			location = _location;
			binding = 0;
			this._format = (int)_format;
			offset = _offset;
		}
	}
	public partial struct VkPipelineColorBlendAttachmentState
	{
		public VkPipelineColorBlendAttachmentState(VkBool32 blendEnable,
			VkBlendFactor srcColorBlendFactor = VkBlendFactor.SrcAlpha,
			VkBlendFactor dstColorBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
			VkBlendOp colorBlendOp = VkBlendOp.Add,
			VkBlendFactor srcAlphaBlendFactor = VkBlendFactor.OneMinusSrcAlpha,
			VkBlendFactor dstAlphaBlendFactor = VkBlendFactor.Zero,
			VkBlendOp alphaBlendOp = VkBlendOp.Add,
			VkColorComponentFlags colorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A)
		{
			this.blendEnable = blendEnable;
			this._srcColorBlendFactor = (int)srcColorBlendFactor;
			this._dstColorBlendFactor = (int)dstColorBlendFactor;
			this._colorBlendOp = (int)colorBlendOp;
			this._srcAlphaBlendFactor = (int)srcAlphaBlendFactor;
			this._dstAlphaBlendFactor = (int)dstAlphaBlendFactor;
			this._alphaBlendOp = (int)alphaBlendOp;
			this.colorWriteMask = colorWriteMask;
		}
	}
	public partial struct VkPushConstantRange
	{
		public VkPushConstantRange(VkShaderStageFlags stageFlags, uint size, uint offset = 0)
		{
			this.stageFlags = stageFlags;
			this.size = size;
			this.offset = offset;
		}
	}
	public partial struct VkCommandBufferBeginInfo
	{
		public VkCommandBufferBeginInfo(VkCommandBufferUsageFlags usage = (VkCommandBufferUsageFlags)0)
		{
			this._sType = (int)VkStructureType.CommandBufferBeginInfo;
			pNext = _pInheritanceInfo = IntPtr.Zero;
			flags = usage;
		}
	}
	public partial struct VkQueryPoolCreateInfo
	{
		public static VkQueryPoolCreateInfo New(VkQueryType queryType,
			VkQueryPipelineStatisticFlags statisticFlags = (VkQueryPipelineStatisticFlags)0, uint count = 1)
		{
			VkQueryPoolCreateInfo ret = new VkQueryPoolCreateInfo();
			ret.sType = VkStructureType.QueryPoolCreateInfo;
			ret.pipelineStatistics = statisticFlags;
			ret.queryType = queryType;
			ret.queryCount = count;
			return ret;
		}
	}
	public partial struct VkDebugMarkerObjectNameInfoEXT
	{
		public VkDebugMarkerObjectNameInfoEXT(VkDebugReportObjectTypeEXT objectType, ulong handle)
		{
			this._sType = (int)VkStructureType.DebugMarkerObjectNameInfoEXT;
			pNext = IntPtr.Zero;
			this._objectType = (int)objectType;
			_object = handle;
			pObjectName = IntPtr.Zero;
		}
	}
	public partial struct VkDebugUtilsObjectNameInfoEXT
	{
		public VkDebugUtilsObjectNameInfoEXT(VkObjectType objectType, ulong handle)
		{
			this._sType = (int)VkStructureType.DebugUtilsObjectNameInfoEXT;
			pNext = IntPtr.Zero;
			this._objectType = (int)objectType;
			objectHandle = handle;
			pObjectName = IntPtr.Zero;
		}
	}
	public partial struct VkDescriptorPoolSize
	{
		public VkDescriptorPoolSize(VkDescriptorType descriptorType, uint count = 1)
		{
			this._type = (int)descriptorType;
			descriptorCount = count;
		}
	}
	public partial struct VkAttachmentReference
	{
		public VkAttachmentReference(uint attachment, VkImageLayout layout)
		{
			this.attachment = attachment;
			this._layout = (int)layout;
		}
	}
}

