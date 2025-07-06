using System.Diagnostics.CodeAnalysis;

namespace ProductMaster.Data
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
        public const string COSMOS_DB = "ServeProduct";
        /// <summary>
        /// Product collection
        /// </summary>
        public const string COSMOS_PRODUCT_COLLECTION = "Product";
        /// <summary>
        /// Item collection
        /// </summary>
        public const string COSMOS_ITEM_COLLECTION = "Item";
        /// <summary>
        /// ProductGroup collection
        /// </summary>
        public const string COSMOS_PRODUCTGROUP_COLLECTION = "ProductGroup";
        /// <summary>
        /// RelatedProductGroup collection
        /// </summary>
        public const string COSMOS_RELATEDPRODUCTGROUP_COLLECTION = "RelatedProductGroup";
        /// <summary>
        /// Digital coupon collection
        /// </summary>
        public const string COSMOS_DIGITALCOUPON_COLLECTION = "DigitalCoupon";
        /// <summary>
        /// Product Taxonomy collection
        /// </summary>
        public const string COSMOS_PRODUCTTAXONOMY_COLLECTION = "ProductTaxonomy";
        /// <summary>
        /// StorItem collection
        /// </summary>
        public const string COSMOS_STOREITEM_COLLECTION = "StoreItem";
        /// <summary>
        /// Product Packaging collection
        /// </summary>
        public const string COSMOS_PRODUCTPACKAGING_COLLECTION = "ProductPackaging";
        /// <summary>
        /// OSAALERTS collection
        /// </summary>
        public const string COSMOS_OSAALERTS_COLLECTION = "OSAAlert";
    }
}
