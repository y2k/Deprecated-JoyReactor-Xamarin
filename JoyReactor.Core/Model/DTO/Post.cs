﻿using SQLite.Net.Attributes;
using System.Collections.Generic;

namespace JoyReactor.Core.Model.DTO
{
    [Table("posts")]
    public class Post
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //[Unique]
        public string PostId { get; set; }

        public string Image { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public string UserName { get; set; }

        public string Title { get; set; }

        public long Created { get; set; }

        public string UserImage { get; set; }

        public float Rating { get; set; }

        public string Coub { get; set; }

        public int CommentCount { get; set; }

        public long Timestamp { get; set; }

        public string Content { get; set; }

        public string Video { get; set; }

        [Ignore]
        public List<Attachment> Attachments { get; set; }

        [Ignore]
        public List<RelatedPost> RelatedPosts { get; set; }

        public override string ToString()
        {
            return string.Format("[Post: Id={0}, PostId={1}, Image={2}, ImageWidth={3}, ImageHeight={4}, UserName={5}, Title={6}, Created={7}, UserImage={8}, Rating={9}, Coub={10}, CommentCount={11}, Timestamp={12}, Content={13}, Video={14}, Attachments={15}, RelatedPosts={16}]", Id, PostId, Image, ImageWidth, ImageHeight, UserName, Title, Created, UserImage, Rating, Coub, CommentCount, Timestamp, Content, Video, Attachments, RelatedPosts);
        }

        public override bool Equals(object obj)
        {
            return (obj as Post)?.ToString() == ToString();
        }
    }
}