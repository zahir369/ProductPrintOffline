//using MKSS.Core.IRepository.Base;
//using MKSS.Core.IRepository.SysIot;
//using MKSS.Core.IServices.SysIot;
//using MKSS.Core.IServices.SysCTWings;
//using MKSS.Core.IServices.SysBase;
//using MKSS.Core.Model.SysIot;
//using MKSS.Core.Model.ViewModels.SysIot;
//using MKSS.Core.Services.BASE;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MKSS.Core.Common.CTWings;
//using MKSS.Core.Model.ViewModels.SysCTwings;
//using MKSS.Core.Common.Helper;
//using System;

//namespace MKSS.Core.Services.SysIot
//{

//    //设备注册
//    public class IotSoldDeviceServices : BaseServices<IotSoldDevice>, IIotSoldDeviceServices
//    {
//        IIotSoldDeviceRepository dal;
//        IBaseThirdPartyAppServices baseThirdPartyAppServices;
//        ICTWingsProductServices cTWingsProductServices;
//        public IotSoldDeviceServices(IIotSoldDeviceRepository dal,
//            IBaseThirdPartyAppServices baseThirdPartyAppServices,
//            ICTWingsProductServices cTWingsProductServices)
//        {
//            this.dal = dal;
//            base.BaseDal = dal;
//            this.baseThirdPartyAppServices = baseThirdPartyAppServices;
//            this.cTWingsProductServices = cTWingsProductServices;
//        }
         
//        /// <summary>
//        /// 注册NB设备
//        /// </summary>
//        /// <param name="deviceSN"></param>
//        /// <returns></returns>
//        public async Task<CreateDeviceReplyInfoModel> RegisterNBDevice(string deviceSN, string deviceName,string operatorName, string userid)
//        {
//            var nbDeviceInfo = await dal.QueryFirstOrDefault(it => it.F_SN.Equals(deviceSN));
//            if (nbDeviceInfo == null)
//            {
//                return new CreateDeviceReplyInfoModel()
//                {
//                    Code = 0,
//                    Message = "不存在的SN"
//                };
//            }
//            if (nbDeviceInfo.F_IsActive.Equals(0))
//            {
//                if (!string.IsNullOrEmpty(nbDeviceInfo.F_NBDeviceId))
//                {
//                    nbDeviceInfo.F_ActiveDate = DateTime.Now;
//                    nbDeviceInfo.F_IsActive = 1;
//                    nbDeviceInfo.F_ActiveUserId = userid;
//                    nbDeviceInfo.F_OwnerId = userid;
//                    nbDeviceInfo.F_ModifyDate = DateTime.Now;
//                    nbDeviceInfo.F_ModifyUserId = userid;
//                    nbDeviceInfo.F_ModifyUserName = operatorName;
//                    var updatedeviceResult = await dal.Update(nbDeviceInfo);
//                    if (updatedeviceResult)
//                    {
//                        var updatewingsResult = await UpdateNBDevice(deviceSN, deviceName, operatorName, userid);
//                        var replyResult= new CreateDeviceReplyInfoModel()
//                        {
//                            Code = 0,
//                            Message = "OK"
//                        };
//                        replyResult.Result.DeviceId = nbDeviceInfo.F_NBDeviceId;
//                        replyResult.Result.DeviceName = deviceName;
//                        replyResult.Result.DeviceSN = nbDeviceInfo.F_SN;
//                        replyResult.Result.IMEI = nbDeviceInfo.F_IMEI;
//                        replyResult.Result.ProductId = nbDeviceInfo.F_NBProductId.ObjToInt();
//                        replyResult.Result.TenantId = nbDeviceInfo.F_NBTenantId;
//                        return replyResult;
//                    }
//                    else
//                    {
//                        return new CreateDeviceReplyInfoModel()
//                        {
//                            Code = -2,
//                            Message = "更新本地记录异常"
//                        };
//                    }
//                }
//                var ctwingsConfigInfo = await baseThirdPartyAppServices.QueryFirstOrDefault(it => it.F_AppCode.Equals("ctwings"));
//                if (ctwingsConfigInfo == null)
//                {
//                    return new CreateDeviceReplyInfoModel()
//                    {
//                        Code = -3,
//                        Message = "未查到CTWings配置信息"
//                    };
//                }
//                var ctwingsProductInfo = await cTWingsProductServices.QueryFirstOrDefault(it => it.F_ProductId.Equals(nbDeviceInfo.F_NBProductId));
//                if (ctwingsProductInfo == null)
//                {
//                    return new CreateDeviceReplyInfoModel()
//                    {
//                        Code = -4,
//                        Message = "NB型号标识不存在"
//                    };
//                }
//                var createDeviceBody = new CreateDevicceRequestInfoModel();
//                createDeviceBody.DeviceName = deviceName;
//                createDeviceBody.DeviceSN = nbDeviceInfo.F_SN;
//                createDeviceBody.IMEI = nbDeviceInfo.F_IMEI;
//                createDeviceBody.ProductId = System.Convert.ToInt32(nbDeviceInfo.F_NBProductId);
//                createDeviceBody.Operator = operatorName;
//                createDeviceBody.Other.AutoObserver = 0;
//                createDeviceBody.Other.IMSI = string.Empty;
//                createDeviceBody.Other.PskValue = string.Empty;
//                var result = Aep_device_management.CreateDevice(ctwingsConfigInfo.F_AppKey, ctwingsConfigInfo.F_AppSecret, ctwingsProductInfo.F_APIkey, JsonHelper.ToJson(createDeviceBody));
//                if (!string.IsNullOrEmpty(result))
//                {
//                    var createResult = JsonHelper.ToObject<CreateDeviceReplyInfoModel>(result);
//                    if (createResult.Code.Equals(0))
//                    {
//                        nbDeviceInfo.F_ActiveDate = DateTime.Now;
//                        nbDeviceInfo.F_NBDeviceId = createResult.Result.DeviceId;
//                        nbDeviceInfo.F_NBDeviceName = createResult.Result.DeviceName;
//                        nbDeviceInfo.F_NBTenantId = createResult.Result.TenantId;
//                        nbDeviceInfo.F_IsActive = 1;
//                        nbDeviceInfo.F_ActiveUserId = userid;
//                        nbDeviceInfo.F_OwnerId = userid;
//                        var updatedeviceResult = await dal.Update(nbDeviceInfo);
//                        if (updatedeviceResult)
//                        {
//                            return createResult;
//                        }
//                        else
//                        {
//                            return new CreateDeviceReplyInfoModel()
//                            {
//                                Code = -2,
//                                Message = "更新本地记录异常"
//                            };
//                        }
//                    }
//                    return createResult;
//                }
//                else
//                {
//                    return new CreateDeviceReplyInfoModel()
//                    {
//                        Code = -2,
//                        Message = "注册Wings平台失败"
//                    };
//                }
//            }
//            else
//            {
//                return new CreateDeviceReplyInfoModel()
//                {
//                    Code = 0,
//                    Message = "设备已激活",
//                    Result = new CreateDeviceReplyResultInfoModel()
//                    {
//                        DeviceId = nbDeviceInfo.F_NBDeviceId,
//                        IMEI = nbDeviceInfo.F_IMEI,
//                        DeviceName = nbDeviceInfo.F_NBDeviceName,
//                        DeviceSN = nbDeviceInfo.F_SN,
//                        ProductId = nbDeviceInfo.F_NBProductId.ObjToInt(),
//                        TenantId = nbDeviceInfo.F_NBTenantId
//                    }
//                };
//            }

//        }
//        /// <summary>
//        /// 解除注册
//        /// </summary>
//        /// <param name="deviceSN"></param>
//        /// <param name="operatorName"></param>
//        /// <param name="userid"></param>
//        /// <returns></returns>
//        public async Task<UpdateDeviceReplyInfoModel> UnRegisterNBDevice(string deviceSN, string operatorName, string userid)
//        {
//            var nbDeviceInfo = await dal.QueryFirstOrDefault(it => it.F_SN.Equals(deviceSN));
//            if (nbDeviceInfo == null)
//            {
//                return new UpdateDeviceReplyInfoModel()
//                {
//                    Code = 0,
//                    Message = "不存在的SN"
//                };
//            }
//            else
//            {
//                if (nbDeviceInfo.F_IsActive.Equals(0))
//                {
//                    return new UpdateDeviceReplyInfoModel() { Code = 0, Message = "解除成功" };
//                }
//                else
//                {
//                    nbDeviceInfo.F_IsActive = 0;
//                    nbDeviceInfo.F_ActiveDate= DateTime.Now;
//                    nbDeviceInfo.F_ActiveUserId = string.Empty;
//                    nbDeviceInfo.F_ModifyDate = DateTime.Now;
//                    nbDeviceInfo.F_OwnerId = string.Empty;
//                    nbDeviceInfo.F_ModifyUserId = userid;
//                    nbDeviceInfo.F_ModifyUserName = operatorName;
//                    var updateResult = await dal.Update(nbDeviceInfo);
//                    if (updateResult)
//                    {
//                        return new UpdateDeviceReplyInfoModel() { Code = 0, Message = "解除成功" };
//                    }
//                    else
//                    {
//                        return new UpdateDeviceReplyInfoModel() { Code = -5, Message = "解除失败" };
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// 更新NB设备
//        /// </summary>
//        /// <param name="deviceSN"></param>
//        /// <param name="deviceName"></param>
//        /// <param name="operatorName"></param>
//        /// <param name="userid"></param>
//        /// <returns></returns>
//        public async Task<UpdateDeviceReplyInfoModel> UpdateNBDevice(string deviceSN, string deviceName, string operatorName, string userid)
//        {
//            var nbDeviceInfo = await dal.QueryFirstOrDefault(it => it.F_SN.Equals(deviceSN));
//            if (nbDeviceInfo == null)
//            {
//                return new UpdateDeviceReplyInfoModel()
//                {
//                    Code = 0,
//                    Message = "不存在的SN"
//                };
//            }
//            else
//            {
//                if (nbDeviceInfo.F_IsActive.Equals(0))
//                {
//                    return new UpdateDeviceReplyInfoModel()
//                    {
//                        Code = -2,
//                        Message = "设备未激活，无法更新"
//                    };
//                }
//                else
//                {
//                    var ctwingsConfigInfo = await baseThirdPartyAppServices.QueryFirstOrDefault(it => it.F_AppCode.Equals("ctwings"));
//                    if (ctwingsConfigInfo == null)
//                    {
//                        return new UpdateDeviceReplyInfoModel()
//                        {
//                            Code = -3,
//                            Message = "未查到CTWings配置信息"
//                        };
//                    }
//                    var ctwingsProductInfo = await cTWingsProductServices.QueryFirstOrDefault(it => it.F_ProductId.Equals(nbDeviceInfo.F_NBProductId));
//                    if (ctwingsProductInfo == null)
//                    {
//                        return new UpdateDeviceReplyInfoModel()
//                        {
//                            Code = -4,
//                            Message = "NB型号标识不存在"
//                        };
//                    }
//                    var updateBody = new UpdateDeviceRequestInfoModel() {
//                        DeviceName = deviceName,
//                        ProductId = nbDeviceInfo.F_NBProductId.ObjToInt(),
//                        Operator = operatorName,
//                    };
//                    updateBody.Other.AutoObserver = 0;
//                    updateBody.Other.IMSI = string.Empty;
//                    var result = Aep_device_management.UpdateDevice(ctwingsConfigInfo.F_AppKey,
//                        ctwingsConfigInfo.F_AppSecret,
//                        ctwingsProductInfo.F_APIkey,
//                        nbDeviceInfo.F_NBDeviceId, JsonHelper.ToJson(updateBody));
//                    if (!string.IsNullOrEmpty(result))
//                    {
//                        var updateResult = JsonHelper.ToObject<UpdateDeviceReplyInfoModel>(result);
//                        if (updateResult.Code.Equals(0))
//                        {
//                            nbDeviceInfo.F_NBDeviceName = deviceName;
//                            nbDeviceInfo.F_ModifyDate = DateTime.Now;
//                            nbDeviceInfo.F_ModifyUserId = userid;
//                            nbDeviceInfo.F_ModifyUserName = operatorName;
//                            var updatedeviceResult = await dal.Update(nbDeviceInfo);
//                            if (updatedeviceResult)
//                            {
//                                return updateResult;
//                            }
//                            else
//                            {
//                                return new UpdateDeviceReplyInfoModel()
//                                {
//                                    Code = -2,
//                                    Message = "更新本地记录异常"
//                                };
//                            }
//                        }
//                        else
//                        {
//                            return updateResult;
//                        }
//                    }
//                    else
//                    {
//                        return new UpdateDeviceReplyInfoModel()
//                        {
//                            Code = -5,
//                            Message = "更新失败"
//                        };
//                    }
//                }
//            }
//        }
//    }
//}
