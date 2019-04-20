﻿using VKE;
using VK;

namespace vkvg_test {
	class Program : VkWindow {
		static void Main (string[] args) {
			using (Program vke = new Program ()) {
				vke.Run ();
			}
		}

		DescriptorPool descriptorPool;
		DescriptorSetLayout descLayout;
		DescriptorSet dsVkvg;

		GraphicPipeline uiPipeline;
		Framebuffer[] frameBuffers;

		#region vkvg tests
		vkvg.Device vkvgDev;
        vkvg.Surface vkvgSurf;
		Image vkvgImage;

		void clearAndClip (vkvg.Context ctx) {
			ctx.ClipPreserve ();
			ctx.Operator = vkvg.Operator.Clear;
			ctx.Fill ();
			ctx.Operator = vkvg.Operator.Over;
		}

		void vkvgDraw () {

            using (vkvg.Context ctx = new vkvg.Context (vkvgSurf)) {
				ctx.ResetClip ();
				ctx.Rectangle (50, 50, 200, 200);
				ctx.Rectangle (50, 50, 250, 250);
				clearAndClip (ctx);
				//ctx.Rectangle (60, 60, 200, 200);
				//clearAndClip (ctx);

				ctx.LineWidth = 1;
				ctx.SetSource (1.0, 0.1, 0.1, 0.2);
				ctx.Rectangle (5.5, 5.5, 400, 250);
				ctx.FillPreserve ();
				ctx.Flush ();
				ctx.SetSource (0.8, 0.8, 0.8);
				ctx.Stroke ();

				ctx.FontFace = "mono";
				ctx.FontSize = 20;
				int x = 60;
				int y = 80, dy = 16;
				ctx.MoveTo (x, y);
				ctx.ShowText (string.Format ($"fps:     {fps,5} "));
			}
		}
		#endregion

		Program () : base () {
			vkvgDev = new vkvg.Device (instance.Handle, phy.Handle, dev.VkDev.Handle, presentQueue.qFamIndex,
				vkvg.SampleCount.Sample_4, presentQueue.index);
					
			init ();

		}

		void init (VkSampleCountFlags samples = VkSampleCountFlags.SampleCount4) { 
			descriptorPool = new DescriptorPool (dev, 2,
				new VkDescriptorPoolSize (VkDescriptorType.CombinedImageSampler)
			);

			descLayout = new DescriptorSetLayout (dev,
				new VkDescriptorSetLayoutBinding (0, VkShaderStageFlags.Fragment, VkDescriptorType.CombinedImageSampler)
			);

			dsVkvg = descriptorPool.Allocate (descLayout);

			GraphicPipelineConfig cfg = GraphicPipelineConfig.CreateDefault (VkPrimitiveTopology.TriangleList, samples);

			cfg.Layout = new PipelineLayout (dev, descLayout);
			cfg.RenderPass = new RenderPass (dev, swapChain.ColorFormat, dev.GetSuitableDepthFormat (), samples);

			cfg.ResetShadersAndVerticesInfos ();
			cfg.AddShader (VkShaderStageFlags.Vertex, "shaders/FullScreenQuad.vert.spv");
			cfg.AddShader (VkShaderStageFlags.Fragment, "shaders/simpletexture.frag.spv");

			cfg.blendAttachments[0] = new VkPipelineColorBlendAttachmentState (true);

			uiPipeline = new GraphicPipeline (cfg);
		}

		void buildCommandBuffers () {
			for (int i = 0; i < swapChain.ImageCount; ++i) { 								
                cmds[i]?.Free ();

				cmds[i] = cmdPool.AllocateCommandBuffer ();
				cmds[i].Start ();

				recordDraw (cmds[i], frameBuffers[i]);
				cmds[i].End ();
			}
		} 
		void recordDraw (CommandBuffer cmd, Framebuffer fb) {

			uiPipeline.RenderPass.Begin (cmd, fb);

			cmd.SetViewport (fb.Width, fb.Height);
			cmd.SetScissor (fb.Width, fb.Height);

			uiPipeline.Bind (cmd);
			cmd.BindDescriptorSet (uiPipeline.Layout, dsVkvg);

			vkvgImage.SetLayout (cmd, VkImageAspectFlags.Color, VkImageLayout.ColorAttachmentOptimal, VkImageLayout.ShaderReadOnlyOptimal,
				VkPipelineStageFlags.ColorAttachmentOutput, VkPipelineStageFlags.FragmentShader);

			cmd.Draw (3, 1, 0, 0);

			vkvgImage.SetLayout (cmd, VkImageAspectFlags.Color, VkImageLayout.ShaderReadOnlyOptimal, VkImageLayout.ColorAttachmentOptimal,
				VkPipelineStageFlags.FragmentShader, VkPipelineStageFlags.BottomOfPipe);


			uiPipeline.RenderPass.End (cmd);
		}

		public override void Update () {
			vkvgDraw ();
			dev.WaitIdle ();
		}
		protected override void OnResize () {

			vkvgImage?.Dispose ();
			vkvgSurf?.Dispose ();
			vkvgSurf = new vkvg.Surface (vkvgDev, (int)swapChain.Width, (int)swapChain.Height);
			vkvgImage = new Image (dev, new VkImage ((ulong)vkvgSurf.VkImage.ToInt64 ()), VkFormat.B8g8r8a8Unorm,
				VkImageUsageFlags.ColorAttachment, (uint)vkvgSurf.Width, (uint)vkvgSurf.Height);
			vkvgImage.CreateView (VkImageViewType.ImageView2D, VkImageAspectFlags.Color);
			vkvgImage.CreateSampler (VkFilter.Nearest,VkFilter.Nearest, VkSamplerMipmapMode.Nearest, VkSamplerAddressMode.ClampToBorder);

			vkvgImage.Descriptor.imageLayout = VkImageLayout.ShaderReadOnlyOptimal;
			DescriptorSetWrites uboUpdate = new DescriptorSetWrites (dsVkvg, descLayout);				
			uboUpdate.Write (dev, vkvgImage.Descriptor);

			if (frameBuffers!=null)
				for (int i = 0; i < swapChain.ImageCount; ++i)
					frameBuffers[i]?.Dispose ();

			frameBuffers = new Framebuffer[swapChain.ImageCount];

			for (int i = 0; i < swapChain.ImageCount; ++i) {
				frameBuffers[i] = new Framebuffer (uiPipeline.RenderPass, swapChain.Width, swapChain.Height,
					(uiPipeline.Samples == VkSampleCountFlags.SampleCount1) ? new Image[] {
						swapChain.images[i],
						null
					} : new Image[] {
						null,
						null,
						swapChain.images[i]
					});
				frameBuffers[i].SetName ("main FB " + i);

			}

			buildCommandBuffers ();
		}	

		protected override void Dispose (bool disposing) {
			if (disposing) {
				if (!isDisposed) {
					dev.WaitIdle ();
					for (int i = 0; i < swapChain.ImageCount; ++i)
						frameBuffers[i]?.Dispose ();

					uiPipeline.Dispose ();

					descLayout.Dispose ();
					descriptorPool.Dispose ();
					vkvgImage?.Dispose ();
					vkvgSurf?.Dispose ();
					vkvgDev.Dispose ();
				}
			}

			base.Dispose (disposing);
		}
	}
}