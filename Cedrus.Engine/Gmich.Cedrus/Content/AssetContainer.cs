using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.Content
{

    public class AssetContainer<TAsset>
        where TAsset : class
    {

        private readonly Dictionary<string, TAsset> assets;

        public AssetContainer(Dictionary<string, TAsset> assets)
        {
            this.assets = assets;
        }

        public TAsset this[string id]
        {
            get
            {
                if (assets.ContainsKey(id))
                {
                    return assets[id];
                }
                else
                {
                    throw new ArgumentException(id);
                }
            }
        }
    }
}
