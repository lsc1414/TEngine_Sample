using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace GameLogic
{
    
    /// <summary>
    /// 定义加权项结构体，包含一个权重值和一个泛型类型的项
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    [Serializable]
    public struct WeightedItem<T>
    {
        // 权重
        [LabelText("权重")]
        public int weight;

        // 泛型类型的项，公共可访问
        public T item;

        // 构造函数，用于初始化加权项
        public WeightedItem(int weight, T item)
        {
            this.weight = weight;
            this.item = item;
        }

        // 权重属性，用于获取和设置权重值
        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }
    }
    
    /// <summary>
    /// 加权列表类，继承自List&lt;WeightedItem&lt;T&gt;&gt;，这里需要注意泛型参数的正确使用方式
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    [Serializable]
    public class WeightedList<T> : List<WeightedItem<T>>
    {
        private int? cachedTotalWeight;

        // 根据权重总和随机选择一个加权项的方法示例
        public WeightedItem<T> GetRandomWeightedItem()
        {
            int totalWeight = GetTotalWeight();
            if (totalWeight > 0)
            {
                Random random = new Random();
                int randomWeight = random.Next(totalWeight);
                int currentWeight = 0;
                foreach (var weightedItem in this)
                {
                    currentWeight += weightedItem.Weight;
                    if (randomWeight < currentWeight)
                    {
                        return weightedItem;
                    }
                }
            }

            // 如果列表为空或权重总和为0，返回默认值（这里假设T有默认构造函数）
            return new WeightedItem<T>(0, default(T));
        }

        private int GetTotalWeight()
        {
            if (cachedTotalWeight.HasValue)
            {
                return cachedTotalWeight.Value;
            }

            int totalWeight = this.Sum(item => item.Weight);
            cachedTotalWeight = totalWeight;
            return totalWeight;
        }

        // 当列表内容发生改变时（如添加、移除元素），需要清除缓存
        public new void Add(WeightedItem<T> item)
        {
            base.Add(item);
            cachedTotalWeight = null;
        }

        public new void Remove(WeightedItem<T> item)
        {
            base.Remove(item);
            cachedTotalWeight = null;
        }

        public new void Clear()
        {
            base.Clear();
            cachedTotalWeight = null;
        }
    }
}