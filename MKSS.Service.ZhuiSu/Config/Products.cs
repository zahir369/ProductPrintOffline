using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MKSS.Model;
using MKSS.Services;

namespace MKSS.Service.ZhuiSu
{
    public class Products
    {
        static Products _instance = null;
        public static Products Instance {
            get {
                if (_instance == null)
                {

                    _instance = new Products()
                    {
                        List = new List<ProductItem>()
                    };
                    List<DataItem> ares = _instance._DataItemServices.QuerySql("select * from pd_dataitem where F_ParentId='3001'").Result;
                    
                    foreach (var item in ares)
                    {
                        ProductItem p = ProductItem.FromXml(item.F_ItemDetail);
                        p.Name = item.F_ItemName;
                        p.Code = item.F_ItemCode;
                        p.Id = item.F_ItemId;
                        _instance.List.Add(p);
                    }

                }
                return _instance;
            }
        }

        DataItemServices _DataItemServices = new DataItemServices();
        BatchServices _BatchServices = new BatchServices();
        public List<ProductItem> List { get; set; }

        public static void Save(Batch batch, ProductItem item)
        {
            Products ps = Products.Instance;
            if (string.IsNullOrEmpty(item.Id))
            {
                DataItem _DataItem = new DataItem() {
                     F_ItemId = Guid.NewGuid().ToString(), F_ItemCode= item.Code, F_ParentId="3001", 
                      F_ItemDetail = item.ToXml(), F_ItemName=item.Name, F_SortCode=6
                };
                int su = ps._DataItemServices.Add(_DataItem).Result;
            }
            else {
                DataItem _DataItem = ps._DataItemServices.QueryById(item.Id).Result;
                _DataItem.F_ItemDetail = item.ToXml();
                _DataItem.F_ItemName = item.Name;
                _DataItem.F_ItemCode = item.Code;
                bool su = ps._DataItemServices.Update(_DataItem).Result;
            }
            Batch bat = ps._BatchServices.QueryById(batch.F_BatchId).Result;
            bat.F_XmlConfig = item.ToXml();
            bool sux = ps._BatchServices.Update(bat).Result;

        }
         

    }

}
