using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductMaster.Data
{
    /// <summary>
    /// DigitalCouponAdapter class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DigitalCouponAdapter : IDigitalCouponAdapter
    {

        # region Constants
        static string collection = Constant.COSMOS_DIGITALCOUPON_COLLECTION;
        #endregion

        #region Private Members
        ILoggerAdapter _logger;
        ICosmosDbService<dynamic> _cosmosDbService;
        #endregion

        #region Contructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cosmosDbService"></param>
        /// <param name="logger"></param>
        public DigitalCouponAdapter(ILoggerAdapter logger, ICosmosDbService<dynamic> cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        #endregion

        #region Method Implementation
        /// <summary>
        /// Fetches DigitalCoupon details
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetCouponDetails(FilterCriteria filterCriteria, string token)
        {
            try
            {
                var result = await _cosmosDbService.GetDataWithoutPartitionKey(collection, CreateQuery(filterCriteria), token, filterCriteria.Pagination);
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError(ex, "Custom Message# ProductMaster/GetDigitalCoupon Adadpter failed! => Unable to fetch the ProductDetails . | Request# " + JsonConvert.SerializeObject(filterCriteria) + "| Response# " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        private string CreateQuery(FilterCriteria filterCriteria)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT c.BackupOfferCouponId,c.BackupOfferCouponSourceId,c.CouponAttributeId,c.CouponBillNote,c.CouponBrandName,c.CouponClientCodeId,c.CouponClientOfferId,c.CouponClipCountDisplayStyleId," +
                    "c.CouponClipRedeemTypeId,c.CouponCoMarketingVendorId,c.CouponConditionTypeId,c.CouponConditionValidate,c.CouponCustomerInstructionText,c.CouponDescription,c.CouponDisplayEndTimestamp," +
                    "c.CouponDisplayIndicator,c.CouponDisplayName,c.CouponDisplayOptionId,c.CouponDisplayStartTimestamp,c.CouponFaceValue,c.CouponFilterSortOrderId,c.CouponIndividualLimitQuantity," +
                    "c.CouponKeyFreeOnBoardTypeId,c.CouponName,c.CouponOfferClassId,c.CouponOfferCodeGroupId,c.CouponOfferCustomerActionId,c.CouponOfferDisplayChannelId,c.CouponOfferDistributionItemId," +
                    "c.CouponOfferGroupId,c.CouponOfferMarketCodeText,c.CouponOfferMaximumClipQuantity,c.CouponOfferNotificationChannelId,c.CouponOfferOriginTypeId,c.CouponOfferRewardTypeId,c.CouponOfferScopeText," +
                    "c.CouponOfferStatusId,c.CouponOfferTypeId,c.CouponPhoneDisplayText,c.CouponPrioritySortOrderId,c.CouponProdEndTimestamp,c.CouponProdStartTimestamp,c.CouponPromoId,c.CouponReceiptMessageText," +
                    "c.CouponRedeemAmount,c.CouponRetailInstructionText,c.CouponSourceId,c.CouponSubmitByVendorId,c.CouponSuggestOfferTypeId,c.CouponTotalClipCount,c.CouponTotalRedeemCount,c.EcomBuyerId,c.id," +
                    "c.LogixCouponPromotionToTnIndicator,c.LogixProductGroupId,c.LogixServiceMethodTypeId,c.ManufacturerCouponIndicator,c.mCouponDistributionTypeId,c.mCouponId,c.mCouponMerchandiseCostAllocateId," +
                    "c.MeijerBusinessPartnerMessageId,c.MeijerBusinessPartnerOfferId,c.PromotionBlockId,c.RowUpdateTimestamp,c.ShopperMarketOfferAttributeId,c.URLId,c.UserMessageProductHierachyIndicator,c.Source," +
                    "c.TermCondition");
                query.Append(" FROM c WHERE c.mCouponId <> 0 ");


                if (filterCriteria.mCouponId != null)
                {
                    query.Append($" AND c.mCouponId IN ({string.Join(",", filterCriteria.mCouponId.Select(itm => itm))})");
                }

                if (filterCriteria.CouponSourceId != null)
                {
                    query.Append($" AND c.CouponSourceId IN ({string.Join(",", filterCriteria.CouponSourceId.Select(itm => itm))})");
                }

                query.Append(" ORDER BY c.mCouponId");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
