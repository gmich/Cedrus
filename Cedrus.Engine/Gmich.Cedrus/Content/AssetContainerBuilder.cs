using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.Content
{

    public class AssetContainerBuilder<TAsset>
      where TAsset : class
    {
        private readonly Dictionary<string, Func<ContentManager, TAsset>> assets = new Dictionary<string, Func<ContentManager, TAsset>>();
        public AssetContainer<TAsset> Content { get; private set; }

        public Result Add(string id, string path)
        => Result
           .Ensure(
            () => !assets.ContainsKey(id),
            () => $"AssetContainerBuilder already contains asset {id}")
           .OnSuccess(
            () => assets.Add(id, content => content.Load<TAsset>(path)));

        public Result Add(string id, Func<ContentManager, TAsset> assetRetriever)
        => Result
          .Ensure(
            () => !assets.ContainsKey(id),
            () => $"AssetContainerBuilder already contains asset {id}")
           .OnSuccess(
            () => assets.Add(id, content => assetRetriever(content)));

        public Result<AssetContainer<TAsset>> Build(ContentManager content)
        => Result.Try(() =>
        {
            Content =  new AssetContainer<TAsset>(assets.ToDictionary(c => c.Key, c => c.Value(content)));
            return Content;
        },
        () => $"Failed to build the asset container");
    }

    public class CommonAssetBuilder
    {
        public AssetContainerBuilder<Texture2D> Textures { get; } = new AssetContainerBuilder<Texture2D>();
        public AssetContainerBuilder<SpriteFont> Font { get; } = new AssetContainerBuilder<SpriteFont>();
    }

}
