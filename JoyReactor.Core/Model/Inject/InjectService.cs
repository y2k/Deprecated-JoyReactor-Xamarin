﻿using System;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.Model.Web;
using System.Collections.Generic;
using JoyReactor.Core.Model.Image;
using Autofac;
using Autofac.Core;
using Autofac.Util;
using Autofac.Features;
using Autofac.Builder;
using System.Linq;

namespace JoyReactor.Core.Model.Inject
{
	public class InjectService 
	{
		public static IContainer Instance { get; private set; }

		public static void Initialize(params IModule[] modules) 
		{
			var b = new ContainerBuilder ();
			b.RegisterModule (new DefaultModule ());
			foreach (var m in modules)
				b.RegisterModule (m);

			Instance = b.Build ();
		}

		private class DefaultModule : Module 
		{
			protected override void Load (ContainerBuilder b)
			{
				b.RegisterType<WebDownloader> ().As<IWebDownloader> ();
				b.RegisterType<ReactorParser> ().As<ISiteParser> ();
				b.RegisterType<DefaultDiskCache> ().As<IDiskCache> ();
				b.RegisterType<MemoryCache> ().As<IMemoryCache> ();
				b.RegisterType<PostCollectionModel> ().As<IPostCollectionModel> ();
				b.RegisterType<ImageModel> ().As<IImageModel> ();
				b.RegisterType<SubscriptionCollectionModel> ().As<ISubscriptionCollectionModel> ();
				b.RegisterType<ProfileModel> ().As<IProfileModel> ();
			}
		}
	}
}