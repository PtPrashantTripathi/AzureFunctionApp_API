using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ProductMaster.Data
{
    /// <summary>
    /// FilterCriteria class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterCriteria
    {
        /// <summary>
        /// RowUpdatedStartDate
        /// </summary>
        public DateTime RowUpdatedStartDate { get; set; }

        /// <summary>
        /// RowUpdatedEndDate
        /// </summary>
        public DateTime RowUpdatedEndDate { get; set; }

        /// <summary>
        /// BrandCategoryId
        /// </summary>
        public string BrandCategoryId { get; set; }

        /// <summary>
        /// ProductStatusTypeName
        /// </summary>
        public string ProductStatusTypeName { get; set; }

        /// <summary>
        /// mProductTaxonomyID
        /// </summary>
        public string mProductTaxonomyID { get; set; }

        /// <summary>
        /// VendorId
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// LegacyPrimaryKeyId
        /// </summary>
        public int LegacyPrimaryKeyId { get; set; }

        /// <summary>
        /// ProductId
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// ItemSku
        /// </summary>
        public List<string> ItemSku { get; set; }

        /// <summary>
        /// mProductId
        /// </summary>
        public List<string> mProductId { get; set; }

        /// <summary>
        /// UPCTypeName
        /// </summary>
        public List<string> UPCTypeName { get; set; }

        /// <summary>
        /// StoreId
        /// </summary>
        public List<int?> StoreId { get; set; }

        /// <summary>
        /// CouponSourceId
        /// </summary>
        public List<int?> CouponSourceId { get; set; }

        /// <summary>
        /// mCouponId
        /// </summary>
        public List<int?> mCouponId { get; set; }

        /// <summary>
        /// mRelatedProductGroupId
        /// </summary>
        public List<string> mRelatedProductGroupId { get; set; }
        //public string mRelatedProductGroupId { get; set; }

        /// <summary>
        /// mProductGroupId
        /// </summary>
        public List<string> mProductGroupId { get; set; }

        /// <summary>
        /// Purpose
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// CasePackId
        /// </summary>
        public List<string> CasePackId { get; set; }

        /// <summary>
        /// InnerPackId
        /// </summary>
        public List<int> InnerPackId { get; set; }

        /// <summary>
        /// mVendorID
        /// </summary>
        public List<string> mVendorID { get; set; }

        /// <summary>
        /// ItemCodeId
        /// </summary>
        public List<string> ItemCodeId { get; set; }

        /// <summary>
        /// Pagination
        /// </summary>
        public int Pagination { get; set; }

        /// <summary>
        /// L5_id
        /// </summary>
        public List<string> L5_id { get; set; }
        /// <summary>
        /// ut_id
        /// </summary>
        public List<int> ut_id { get; set; }
    }
}
