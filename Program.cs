using RandomHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoZiPu
{
    class Program
    {
        static void Main(string[] args)
        {            
            #region 创建实例
            Stopwatch sp = new Stopwatch();//用于模拟开铺时间
            BunStore godmother = new BunStore();//包子铺的实例
            Random r = new Random();
            MyRamdom<RandomWeight> kindOfGuest = new MyRamdom<RandomWeight>(new NormalGuy(), new Student(), new RichGuy(), new IdiotGuy(), new PoorGuy(), new Gastronome());//所有客人的种类
            int openTime = godmother.CloseTime - godmother.OpenTime;
            #endregion

            #region 游戏的主逻辑           
            for (int i = 0; i <= 10; i++)
            {
                sp.Restart();
                while (sp.Elapsed.Seconds <= openTime)
                {
                    int nextComeTime = (r.Next(0, openTime - sp.Elapsed.Seconds)) / 2 + sp.Elapsed.Seconds;
                    bool Log = false;
                    while (sp.Elapsed.Seconds <= nextComeTime)
                    {
                        if (!Log)
                        {
                            Log = true;
                            Console.WriteLine("当前的时间是" + sp.Elapsed.Seconds + "下一个客人" + nextComeTime + "的时候来");
                        }
                    }
                    var t = kindOfGuest.GetTheNextValue();
                    if (t is Guests)
                        Console.WriteLine("来了老弟" + ((Guests)t).Buy(godmother));
                    else
                        Console.WriteLine("美食家来啦！！！！");
                }
                Console.WriteLine("一天结束啦~~~~");
                Console.ReadKey();
                Console.Clear();
            }
            #endregion

            Console.ReadKey();
        }
    }

    #region 客人类
    /// <summary>
    /// 抽象的客人类,所有的客人都继承自它
    /// </summary>
    public abstract class Guests
    {
        //public abstract Goods[] need { get; }
        /// <summary>
        /// 买东西
        /// </summary>
        /// <param name="shop">买东西的商店</param>
        /// <returns></returns>
        public abstract PayInfo Buy(Shop shop);
    }

    /// <summary>
    /// 富人:直接买下当前商店里最贵的包子(数量不限)
    /// </summary>
    public class RichGuy : Guests, RandomWeight
    {
        private int _weight = 2;
        public int Weight { get { return _weight; } }
        public override PayInfo Buy(Shop shop)
        {
            return new PayInfo();
        }
    }

    /// <summary>
    /// 学生:只买下最便宜的一种包子(不超过3个)
    /// </summary>
    public class Student : Guests, RandomWeight
    {
        private int _weight = 10;
        public int Weight { get { return _weight; } }
        public override PayInfo Buy(Shop shop)
        {
            return new PayInfo();
        }
    }

    /// <summary>
    ///普通人: 会询问有没有某个价钱的包子,如果有就买两个，
    /// 如果没有则买一个最接近目标价钱的包子
    /// </summary>
    public class NormalGuy : Guests, RandomWeight
    {
        private int _weight = 8;
        public int Weight { get { return _weight; } }

        public override PayInfo Buy(Shop shop)
        {
            return new PayInfo();
        }
    }

    /// <summary>
    /// 强迫症:会询问有没有某个价钱的包子或者名字，如果有则买三个，没有则不买
    /// </summary>
    public class IdiotGuy : Guests, RandomWeight
    {
        private int _weight = 3;
        public int Weight { get { return _weight; } }

        public override PayInfo Buy(Shop shop)
        {
            return new PayInfo();
        }
    }

    /// <summary>
    /// 乞丐:随机向你索要一个店里的包子，且不付钱
    /// </summary>
    public class PoorGuy : Guests, RandomWeight
    {
        private int _weight = 4;
        public static int ComeTimes { get { return _times; } }
        private static int _times;
        public int Weight { get { return _weight; } }
        public override PayInfo Buy(Shop shop)
        {
            _times++;
            return new PayInfo();
        }

        public static void ResetTimes()
        {
            _times = 0;
        }
    }

    /// <summary>
    /// 美食家:会马上提供给你某个新品种的包子(包含包子的名字以及价格)
    /// </summary>
    public class Gastronome : RandomWeight
    {
        private int _weight = 1;
        public int Weight { get { return _weight; } }
        
        /// <summary>
        /// 返回一个新研发的包子
        /// </summary>
        /// <returns></returns>
        public Bun ReportAnBun() {
            return new Bun("蝗虫包",300);
        }
    }
    #endregion

    /// <summary>
    /// 商店的抽象类
    /// </summary>
    public abstract class Shop
    {
        public abstract int OpenTime { get; }
        public abstract int CloseTime { get; }

        protected int _runDay;
        /// <summary>
        /// 总共的经营天数
        /// </summary>
        public int RunDay { get { return _runDay; } }

        /// <summary>
        /// 账单流水
        /// </summary>
        protected AccountBill _manageBill;
    }

    /// <summary>
    /// 包子铺        
    /// </summary>
    public class BunStore : Shop
    {
        /// <summary>
        /// 每日的目标流水
        /// </summary>
        private const decimal TARGET = 300;

        private int _openTime = 8;
        /// <summary>
        /// 开始营业的时间
        /// </summary>
        public override int OpenTime { get { return _openTime; } }

        private int _closeTime = 18;
        /// <summary>
        /// 结束营业的时间
        /// </summary>
        public override int CloseTime { get { return _closeTime; } }
    }

    /// <summary>
    /// 用于制作账单，本模板的规则是必须按顺序存储,如果跳天存储会被当成非法操作
    /// 使用方式是：实例名[天数] = 当天的营业额
    /// 获取营业额：实例名[天数]
    /// </summary>
    public class AccountBill
    {
        private List<int> _day = new List<int>();
        private List<decimal> _revenue = new List<decimal>();
        public virtual decimal this[int input_day]
        {
            get
            {
                if (input_day <= _day.Count)
                    return _revenue[input_day - 1];
                return (input_day <= 0 ? _revenue[0] : _revenue[_revenue.Count - 1]);
            }
            set
            {
                if (input_day == _day.Count + 1)
                {
                    _day.Add(input_day);
                    _revenue.Add(value);
                }
                else if (input_day <= _day.Count)
                    _revenue[input_day - 1] = value;
            }
        }
        /// <summary>
        /// 用于制作总和账单
        /// </summary>
        /// <param name="format">传入格式字符串,一共有3个下标:{0}:startDay,{1}:endDay,{2}:总和</param>
        /// <param name="startDay">开始清算的天数</param>
        /// <param name="endDay">结束清算的天数</param>
        /// <returns></returns>
        public virtual string MakeTheBill(string format, int startDay, int endDay)
        {
            decimal sum = 0;
            for (int i = startDay; i <= endDay; i++)
                sum += this[i];
            return String.Format(format, startDay, endDay, sum); 
        }
    }

    /// <summary>
    /// 商品类
    /// </summary>
    public class Goods
    {
        public readonly string name;
        public decimal price { get; set; }
        public Goods(string name, decimal price)
        {
            this.name = name;
            this.price = price;
        }
    }

    /// <summary>
    /// 包子类
    /// </summary>
    public class Bun : Goods
    {
        public int amountSold;
        public Bun(string name, decimal price) : base(name, price)
        {
        }
    }

    /// <summary>
    /// 支付信息
    /// </summary>
    public struct PayInfo
    {
        public decimal PayMoney;
        public Goods[] PayGoods;
        public Guests guest;
    }
}

/// <summary>
/// 实现随机
/// </summary>
namespace RandomHelper
{
    /// <summary>
    /// 用于实现随机权重
    /// </summary>
    public interface RandomWeight
    {
        /// <summary>
        /// 权重
        /// </summary>
        int Weight { get; }
    }

    /// <summary>
    /// 根据权重生产随机值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyRamdom<T> where T : RandomWeight
    {
        private T[] _arr;
        private int[] _weight;
        private int _weightTotal;
        private Random _r;
        public MyRamdom(params T[] arr)
        {
            _arr = arr;
            _weight = new int[arr.Length];
            sort();
            _weightTotal = sortTheWeight();
            _r = new Random();
        }

        /// <summary>
        /// 获得该数组的下一个随机值
        /// </summary>
        /// <returns></returns>
        public T GetTheNextValue()
        {
            int t = _r.Next(1, _weightTotal);
            //Console.WriteLine("当前的随机值是" + t);
            for (int i = 0; i < _arr.Length - 1; i++)
                if (t <= _weight[i] && t < _weight[i + 1])
                    return _arr[i];
            return _arr[_arr.Length - 1];
        }

        /// <summary>
        ///优化版的插入排序
        /// </summary>
        private void sort()
        {
            for (int i = 0; i < _arr.Length; i++)
            {
                T test = _arr[i];
                int j;
                for (j = i; j > 0 && (_arr[j - 1].Weight > test.Weight); j--)
                    _arr[j] = _arr[j - 1];
                _arr[j] = test;
            }
        }

        /// <summary>
        ///  获得总权重并排序
        /// </summary>
        /// <returns>返回总权重值</returns>
        private int sortTheWeight()
        {
            //1个美食家 2个富人 3个强迫症  4个乞丐 8个普通人  10个学生 28个人 
            //1是美食家 2-3是富人 4-6 是强迫症  7-10 是乞丐 11-18是普通人 19-28是学生
            int weight = 0;
            for (int i = 0; i < this._weight.Length; i++)
            {
                weight += _arr[i].Weight;
                this._weight[i] = weight;
                //   Console.WriteLine(_arr[i].GetType() + "的权重" + weight);
            }
            return weight;
        }
    }
}