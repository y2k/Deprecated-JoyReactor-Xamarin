using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Web;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model.Images
{
    public abstract class BaseImageRequest
    {
        protected abstract Task<object> DecodeImageAsync(byte[] data);

        protected abstract void SetToTarget(object target, object image);

        static readonly OperationTransaction Transaction = new OperationTransaction();
        static readonly DiskCache DiskCache = new DiskCache();
        static BaseMemoryCache MemoryCache;

        ThumbnailUriBuilder uriBilder = new ThumbnailUriBuilder();

        protected abstract BaseMemoryCache CreateMemoryCache();

        public BaseImageRequest CropIn(int width, int height)
        {
            uriBilder.SetFitWidth(width, height);
            return this;
        }

        public BaseImageRequest SetUri(string originalUrl)
        {
            uriBilder.SetOriginalUrl(originalUrl);
            return this;
        }

        public async void To(object target)
        {
            lock (DiskCache)
            {
                if (MemoryCache == null)
                    MemoryCache = CreateMemoryCache();
            }

            Transaction.Begin(target, this);
            try
            {
                if (uriBilder.IsEmpty)
                {
                    SetToTarget(target, null);
                    return;
                }

                var uri = uriBilder.ToUri();
                var imageFromCache = MemoryCache.Get(uri);
                if (imageFromCache != null)
                {
                    SetToTarget(target, imageFromCache);
                    return;
                }

                SetToTarget(target, null);

                var cachedBytes = await DiskCache.GetAsync(uri);
                if (IsInvalidState())
                    return;
                if (cachedBytes == null)
                {
                    byte[] data = await new Downloader().Download(uri, IsInvalidState);
                    if (data == null)
                        return;

                    await DiskCache.PutAsync(uri, data);
                    if (IsInvalidState())
                        return;
                    var image = await DecodeImageAsync(data);
                    if (IsInvalidState())
                        return;
                    MemoryCache.Put(uri, image);
                    SetToTarget(target, image);
                }
                else
                {
                    var image = await DecodeImageAsync(cachedBytes);
                    if (IsInvalidState())
                        return;
                    MemoryCache.Put(uri, image);
                    SetToTarget(target, image);
                }
            }
            finally
            {
                Transaction.End(target, this);
            }
        }

        bool IsInvalidState()
        {
            return !Transaction.IsValid(this);
        }

        class OperationTransaction
        {
            readonly Dictionary<object, BaseImageRequest> LockedTargets = new Dictionary<object, BaseImageRequest>();

            internal void Begin(object target, BaseImageRequest requste)
            {
                LockedTargets[target] = requste;
            }

            internal void End(object target, BaseImageRequest requste)
            {
                if (IsValid(requste))
                    LockedTargets.Remove(target);
            }

            internal bool IsValid(BaseImageRequest requste)
            {
                return LockedTargets.ContainsValue(requste);
            }
        }

        class Downloader
        {
            const int MaxDownloadAttempts = 5;

            public async Task<byte[]> Download(Uri uri, Func<bool> transactionFailChecker)
            {
                for (int n = 0; n < MaxDownloadAttempts; n++)
                {
                    try
                    {
                        if (transactionFailChecker())
                            return null;
                        return await DownloadAsync(uri);
                    }
                    catch
                    {
                        await Task.Delay(500 << n);
                    }
                }
                return null;
            }

            static async Task<byte[]> DownloadAsync(Uri uri)
            {
                var client = ServiceLocator.Current.GetInstance<WebDownloader>();
                using (var r = await client.ExecuteAsync(uri))
                {
                    var buffer = new MemoryStream();
                    await r.Stream.CopyToAsync(buffer);
                    return buffer.ToArray();
                }
            }
        }

        public class ThumbnailUriBuilder
        {
            const string ThumbnailDomain = "api-i-twister.net";
            const string ThumbnailTemplate = "https://" + ThumbnailDomain + ":8011/cache/fit?bgColor=ffffff&width={1}&height={2}&url={0}";
            const string OriginalTemplate = "https://" + ThumbnailDomain + ":8011/cache/original?url={0}";

            public bool IsEmpty { get { return originalUrl == null; } }

            string originalUrl;
            string format;
            int width;
            int height;

            public void SetOriginalUrl(string originalUrl)
            {
                this.originalUrl = originalUrl;
            }

            public void SetFormat(string format)
            {
                this.format = format;
            }

            public void SetFitWidth(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            internal Uri ToUri()
            {
                return IsCanCreateThumbnail() ? CreateThumbnailUri() : new Uri(originalUrl);
            }

            bool IsCanCreateThumbnail()
            {
                return originalUrl != null && !originalUrl.Contains(ThumbnailDomain);
            }

            Uri CreateThumbnailUri()
            {
                var template = (width > 0 || height > 0) ? ThumbnailTemplate : OriginalTemplate;
                var result = string.Format(template, Uri.EscapeDataString(originalUrl), width, height);
                if (format != null)
                    result += "&format=" + Uri.EscapeDataString(format);
                return new Uri(result);
            }
        }
    }
}