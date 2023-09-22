using System.Diagnostics.CodeAnalysis;

namespace PosMaster.Data
{
    /// <summary>
    /// Holds all the information that are constant
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Constant
    {
        /// <summary>
        /// Cosmos Database name
        /// </summary>
        public const string COSMOS_DB = "ServePos";
        /// <summary>
        /// Pos collection
        /// </summary>
        public const string COSMOS_POS_COLLECTION = "Pos";
        /// <summary>
        /// Sku collection
        /// </summary>
        public const string COSMOS_ITEM_COLLECTION = "Sku";
        /// <summary>
        /// PosGroup collection
        /// </summary>
        public const string COSMOS_POSGROUP_COLLECTION = "PosGroup";
        /// <summary>
        /// RelatedPosGroup collection
        /// </summary>
        public const string COSMOS_RELATEDPOSGROUP_COLLECTION = "RelatedPosGroup";
        /// <summary>
        /// Digital coupon collection
        /// </summary>
        public const string COSMOS_DIGITALCOUPON_COLLECTION = "DigitalCoupon";
        /// <summary>
        /// Pos Taxonomy collection
        /// </summary>
        public const string COSMOS_POSTAXONOMY_COLLECTION = "PosTaxonomy";
        /// <summary>
        /// StorItem collection
        /// </summary>
        public const string COSMOS_STOREITEM_COLLECTION = "StoreItem";
        /// <summary>
        /// Pos Packaging collection
        /// </summary>
        public const string COSMOS_POSPACKAGING_COLLECTION = "PosPackaging";
        /// <summary>
        /// OSAALERTS collection
        /// </summary>
        public const string COSMOS_PSAALERTS_COLLECTION = "OSAAlert";
        public const string COSMOS_DB_Pos = "ServePos";

    }
}
