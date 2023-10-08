using UnityEditorInternal;
using UnityEngine;

namespace Factions
{
    public interface IFactionManagerAssetHandler
    {
        public void OnSaveAsset(GameObject asset, AssetType type);
    }
}