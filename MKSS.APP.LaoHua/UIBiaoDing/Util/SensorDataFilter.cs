using System;
using System.Collections.Generic;

namespace MKSS.APP.UIBiaoDing.Util
{
    /// <summary>
    ///  简单滤波算法
    /// </summary>
    public class SensorDataFilter {


		/// <summary>
		///  是否启用
		/// </summary>
		public static bool Enabled { get; set; } = true;
		static Dictionary<string, SensorDataFilter> Cache = new Dictionary<string, SensorDataFilter>();
		public static SensorDataFilter CreateSensorDataFilter(Address adr, int pos, string[] arr)
		{
			string key = string.Format("{0}_{1}", adr, pos);
			lock (Cache) {
				if (!Cache.ContainsKey(key)) Cache.Add(key, new SensorDataFilter(arr) { Address = adr, Position = pos });
				if (Cache.ContainsKey(key)) return Cache[key];
			}
			return Cache[key];
		}


		public Address Address
		{
			get; set;
		}

		public int Position
		{
			get; set;
		}

		public class QueueValue { 
			public DateTime D { get; set; }
			public int? V { get; set; }
		}
		/// <summary>
		///  滤波算法
		///  历史数据连续三次最新值是 170 或 0 才认 170
		/// </summary>
		public Dictionary<string, Queue<QueueValue>> DataCache = new Dictionary<string, Queue<QueueValue>>(); 
		public Dictionary<string,int> EmpCount = new Dictionary<string, int>();
		public Dictionary<string, int?> DataCacheLastNotEmp = new Dictionary<string, int?>();
		public Dictionary<string, int?> DataCacheLast = new Dictionary<string, int?>();
		public Dictionary<string, int?> DataCacheReturn = new Dictionary<string, int?>();


		private SensorDataFilter(string[] arr) {
			foreach (var item in arr)
			{
				if (!DataCache.ContainsKey(item))
				{
					DataCache.Add(item, new Queue<QueueValue>());
					EmpCount.Add(item, 0);
					DataCacheLastNotEmp.Add(item, null);
					DataCacheReturn.Add(item, null);
					DataCacheLast.Add(item, null);
				}
			}
		} 
		public void PushData(string name, int? value) {
			
			//Enqueue():在队列的末端添加元素
			DataCache[name].Enqueue(new QueueValue() { V=value,D=DateTime.Now });
			DataCacheLast[name] = value;

			//跳变 170 的处理
			bool emp = Emp(value);
			if (emp)
			{
				EmpCount[name]++;
				if (DataCacheLastNotEmp[name] != null)
				{
					if (EmpCount[name] <= 100)
					{
						//历史数据不空，空值连续 2 次以内，不认可空值 
						DataCacheReturn[name] = DataCacheLastNotEmp[name];
					}
					else {
						DataCacheReturn[name] = value;
					}
				}
				else {
					DataCacheReturn[name] = value;
				}
				 
			}
			else
			{
				EmpCount[name] = 0;
				DataCacheLastNotEmp[name] = value;
				DataCacheReturn[name] = value;
			}

			if (DataCache[name].Count > 200)
			{
				var v = DataCache[name].Dequeue();//最多留存 10个数据 Queue队列就是先进先出
			}

		}

		bool Emp(int? v) {
			return v == null || v.Value <= 0 || v.Value == 170;
		}

        internal void Reset()
        {
            foreach (var item in DataCache.Keys)
            {
				DataCache[item].Clear();
				EmpCount[item] = 0;
				DataCacheLastNotEmp[item] = null;
				DataCacheLast[item] = null;
				DataCacheReturn[item] = null;
			} 

		}

        /// <summary>
        ///  连续三次空值
        /// </summary>
        int emp_count = 3;
		public int? this[string name]{
			get {
				if(!Enabled) return DataCacheLast[name];
				return DataCacheReturn[name];
			}
		}

	}

}
