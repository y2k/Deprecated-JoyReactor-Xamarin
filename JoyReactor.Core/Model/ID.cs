﻿using System;
using System.Collections.Generic;

namespace JoyReactor.Core
{
	public class ID
	{
		public enum IdConst
		{
			ReactorGood,
			ReactorBest,
			ReactorAll,
			ReactorFavorite
		}

		const char Divider = '-';
		static readonly Dictionary<IdConst, ID> Consts = new Dictionary<IdConst, ID> {
			[IdConst.ReactorGood] = new ID { Site = SiteParser.JoyReactor, Type = TagType.Good },
			[IdConst.ReactorBest] = new ID { Site = SiteParser.JoyReactor, Type = TagType.Best },
			[IdConst.ReactorAll] = new ID { Site = SiteParser.JoyReactor, Type = TagType.All },
			[IdConst.ReactorFavorite] = new ID { Site = SiteParser.JoyReactor, Type = TagType.Favorite }
		};

		internal SiteParser Site { get; set; }

		internal TagType Type { get; set; }

		internal string Tag { get; set; }

		public enum SiteParser
		{
			JoyReactor,
			Chan4,
			Chan7,
			Chan2
		}

		public enum TagType
		{
			Best,
			Good,
			All,
			Favorite
		}

		public string SerializeToString ()
		{
			return  "" + Site + Divider + Type + Divider + Tag;
		}

        public override string ToString()
        {
            return string.Format("[ID: Site = {0}, Type = {1}, Tag = {2}]", Site, Type, Tag);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as ID;
            return o != null && Site == o.Site && Type == o.Type && Tag == o.Tag;
        }

		public static ID DeserializeFromString (string value)
		{
			var p = value.Split (Divider);
			return new ID {
				Site = (SiteParser)Enum.Parse (typeof(SiteParser), p [0]),
				Type = (TagType)Enum.Parse (typeof(TagType), p [1]),
				Tag = p [2]
			};
		}

		public static ID Parser (string flatId)
		{
			var p = flatId.Split ('-');
			var id = new ID {
				Site = (SiteParser)Enum.Parse (typeof(SiteParser), p [0]),
				Type = (TagType)Enum.Parse (typeof(TagType), p [1]),
				Tag = p [2]
			};
			if ("" == id.Tag)
				id.Tag = null;
			return id;
		}

		public class TagID : ID
		{
			// TODO Reserver for future
		}

		public class Factory
		{
			public static ID New (SiteParser site, string tag)
			{
				return new ID { Site = site, Type = TagType.Good, Tag = tag };
			}

			public static ID New (IdConst c)
			{
				return Consts [c];
			}

			public static ID NewTag (string tag)
			{
				return new ID { Site = SiteParser.JoyReactor, Type = TagType.Good, Tag = tag };
			}

			public static ID NewFavoriteForUser (string username)
			{
                return new ID { Site = SiteParser.JoyReactor, Type = TagType.Favorite, Tag = username };
			}
		}
	}
}