using System;
using System.Collections.Generic;
using System.Linq;
using BeatLeader.Themes;
using JetBrains.Annotations;
using UnityEngine;

namespace BeatLeader {
    public static class BundleLoader {
        #region Initialize

        private const string BundleName = Plugin.ResourcesPath + ".AssetBundles.asset_bundle";
        private static bool _ready;

        public static void Initialize() {
            if (_ready) return;

            using var stream = ResourcesUtils.GetEmbeddedResourceStream(BundleName);
            var localAssetBundle = AssetBundle.LoadFromStream(stream);

            if (localAssetBundle == null) {
                throw new Exception("AssetBundle load error!");
            }

            LoadSprites(localAssetBundle);
            LoadMaterials(localAssetBundle);
            LoadPrefabs(localAssetBundle);

            localAssetBundle.Unload(false);
            _ready = true;
        }

        #endregion

        #region Prefabs

        public static GameObject MonkeyPrefab;
        public static GameObject AccuracyGraphPrefab;
        public static ThemesCollection ThemesCollection;

        private static void LoadPrefabs(AssetBundle assetBundle) {
            MonkeyPrefab = assetBundle.LoadAsset<GameObject>("TemplatePrefab");
            AccuracyGraphPrefab = assetBundle.LoadAsset<GameObject>("AccuracyGraph");
            ThemesCollection = assetBundle.LoadAsset<ThemesCollection>("ThemesCollection");
        }

        #endregion

        #region Materials

        public static Material LogoMaterial;
        public static Material DefaultAvatarMaterial;
        public static Material UIAdditiveGlowMaterial;
        public static Material ScoreBackgroundMaterial;
        public static Material ScoreUnderlineMaterial;
        public static Material AccGridBackgroundMaterial;
        public static Material HandAccIndicatorMaterial;
        public static Material AccDetailsRowMaterial;
        public static Material ClanTagBackgroundMaterial;
        public static Material VotingButtonMaterial;
        public static Material MiniProfileBackgroundMaterial;
        public static Material UIBlurMaterial;

        private static void LoadMaterials(AssetBundle assetBundle)
        {
            LogoMaterial = assetBundle.LoadAsset<Material>("LogoMaterial");
            DefaultAvatarMaterial = assetBundle.LoadAsset<Material>("DefaultAvatar");
            UIAdditiveGlowMaterial = assetBundle.LoadAsset<Material>("UIAdditiveGlow");
            ScoreBackgroundMaterial = assetBundle.LoadAsset<Material>("ScoreBackgroundMaterial");
            ScoreUnderlineMaterial = assetBundle.LoadAsset<Material>("ScoreUnderlineMaterial");
            AccGridBackgroundMaterial = assetBundle.LoadAsset<Material>("AccGridBackgroundMaterial");
            HandAccIndicatorMaterial = assetBundle.LoadAsset<Material>("HandAccIndicatorMaterial");
            AccDetailsRowMaterial = assetBundle.LoadAsset<Material>("AccDetailsRowMaterial");
            ClanTagBackgroundMaterial = assetBundle.LoadAsset<Material>("ClanTagBackgroundMaterial");
            VotingButtonMaterial = assetBundle.LoadAsset<Material>("VotingButtonMaterial");
            MiniProfileBackgroundMaterial = assetBundle.LoadAsset<Material>("UIMiniProfileBackgroundMaterial");
            UIBlurMaterial = assetBundle.LoadAsset<Material>("UIBlurMaterial");
        }

        #endregion

        #region Sprites

        [UsedImplicitly]
        public static Sprite LocationIcon;

        [UsedImplicitly]
        public static Sprite RowSeparatorIcon;

        [UsedImplicitly]
        public static Sprite BeatLeaderLogoGradient;

        [UsedImplicitly]
        public static Sprite TransparentPixel;

        [UsedImplicitly]
        public static Sprite FileError;

        [UsedImplicitly]
        public static Sprite ModifiersIcon;

        [UsedImplicitly]
        public static Sprite Overview1Icon;

        [UsedImplicitly]
        public static Sprite Overview2Icon;

        [UsedImplicitly]
        public static Sprite DetailsIcon;

        [UsedImplicitly]
        public static Sprite GridIcon;

        [UsedImplicitly]
        public static Sprite GraphIcon;

        [UsedImplicitly]
        public static Sprite NotificationIcon;

        [UsedImplicitly]
        public static Sprite WebsiteLinkIcon;

        [UsedImplicitly]
        public static Sprite DiscordLinkIcon;

        [UsedImplicitly]
        public static Sprite PatreonLinkIcon;

        [UsedImplicitly]
        public static Sprite TwitterIcon;

        [UsedImplicitly]
        public static Sprite TwitchIcon;

        [UsedImplicitly]
        public static Sprite YoutubeIcon;

        [UsedImplicitly]
        public static Sprite ProfileIcon;

        [UsedImplicitly]
        public static Sprite FriendsIcon;

        [UsedImplicitly]
        public static Sprite IncognitoIcon;

        [UsedImplicitly]
        public static Sprite ReplayIcon;

        [UsedImplicitly]
        public static Sprite UIIcon;

        [UsedImplicitly]
        public static Sprite DebrisIcon;

        [UsedImplicitly]
        public static Sprite SceneIcon;

        [UsedImplicitly]
        public static Sprite SaveIcon;

        private static List<Sprite> _loadedSprites;
        
        public static Sprite GetSpriteFromBundle(string name)
        {
            return _ready ? _loadedSprites.Where(x => x.name == name).FirstOrDefault() : null;
        }
        public static bool TryGetSpriteFromBundle(string name, out Sprite sprite)
        {
            return (sprite = GetSpriteFromBundle(name)) != null;
        }
        private static void LoadSprites(AssetBundle assetBundle)
        {
            LocationIcon = assetBundle.LoadAsset<Sprite>("LocationIcon");
            RowSeparatorIcon = assetBundle.LoadAsset<Sprite>("RowSeparatorIcon");
            BeatLeaderLogoGradient = assetBundle.LoadAsset<Sprite>("BeatLeaderLogoGradient");
            TransparentPixel = assetBundle.LoadAsset<Sprite>("TransparentPixel");
            FileError = assetBundle.LoadAsset<Sprite>("FileError");
            ModifiersIcon = assetBundle.LoadAsset<Sprite>("ModifiersIcon");
            Overview1Icon = assetBundle.LoadAsset<Sprite>("BL_Overview1Icon");
            Overview2Icon = assetBundle.LoadAsset<Sprite>("BL_Overview2Icon");
            DetailsIcon = assetBundle.LoadAsset<Sprite>("BL_DetailsIcon");
            GridIcon = assetBundle.LoadAsset<Sprite>("BL_GridIcon");
            GraphIcon = assetBundle.LoadAsset<Sprite>("BL_GraphIcon");
            NotificationIcon = assetBundle.LoadAsset<Sprite>("BL_NotificationIcon");
            WebsiteLinkIcon = assetBundle.LoadAsset<Sprite>("BL_Website");
            DiscordLinkIcon = assetBundle.LoadAsset<Sprite>("BL_Discord");
            PatreonLinkIcon = assetBundle.LoadAsset<Sprite>("BL_Patreon");
            TwitterIcon = assetBundle.LoadAsset<Sprite>("BL_TwitterIcon");
            TwitchIcon = assetBundle.LoadAsset<Sprite>("BL_TwitchIcon");
            YoutubeIcon = assetBundle.LoadAsset<Sprite>("BL_YoutubeIcon");
            ProfileIcon = assetBundle.LoadAsset<Sprite>("BL_ProfileIcon");
            FriendsIcon = assetBundle.LoadAsset<Sprite>("BL_FriendsIcon");
            IncognitoIcon = assetBundle.LoadAsset<Sprite>("BL_IncognitoIcon");
            ReplayIcon = assetBundle.LoadAsset<Sprite>("BL_ReplayIcon");
            UIIcon = assetBundle.LoadAsset<Sprite>("BL_UIIcon");
            DebrisIcon = assetBundle.LoadAsset<Sprite>("BL_DebrisIcon");
            SceneIcon = assetBundle.LoadAsset<Sprite>("BL_SceneIcon");
            SaveIcon = assetBundle.LoadAsset<Sprite>("BL_SaveIcon");
            
            _loadedSprites = assetBundle.LoadAllAssets<Sprite>().ToList();
        }

        #endregion
    }
}