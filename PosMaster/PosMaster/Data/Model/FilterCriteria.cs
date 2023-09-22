using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PosMaster.Data
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
        /// PosStatusTypeName
        /// </summary>
        public string PosStatusTypeName { get; set; }

        /// <summary>
        /// mPosTaxonomyID
        /// </summary>
        public string mPosTaxonomyID { get; set; }

        /// <summary>
        /// VendorId
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// LegacyPrimaryKeyId
        /// </summary>
        public int LegacyPrimaryKeyId { get; set; }

        /// <summary>
        /// PosId
        /// </summary>
        public int PosId { get; set; }

        /// <summary>
        /// Sku
        /// </summary>
        public List<string> Sku { get; set; }

        /// <summary>
        /// mPosId
        /// </summary> 
        public List<string> mPosId { get; set; }

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
        /// mRelatedPosGroupId
        /// </summary> 
        public List<string> mRelatedPosGroupId { get; set; }
        //public string mRelatedPosGroupId { get; set; }

        /// <summary> 
        /// mPosGroupId
        /// </summary> 
        public List<string> mPosGroupId { get; set; }

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
