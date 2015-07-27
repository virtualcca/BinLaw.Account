using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BinLaw.Account.FE.Model.Common
{
    // This class can used as a jumpstart for implementing ISupportIncrementalLoading. 
    // Implementing the ISupportIncrementalLoading interfaces allows you to create a list that loads
    //  more data automatically when the user scrolls to the end of of a GridView or ListView.
    public class IncrementalLoadingModel<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        // 是否正在异步加载中
        private bool _isBusy = false;

        // 提供数据的 Func
        // 第一个参数：增量加载的起始索引；第二个参数：需要获取的数据量；第三个参数：获取到的数据集合
        private Func<int, int, Task<IEnumerable<T>>> _funcGetData;
        // 最大可显示的数据量
        private uint _totalCount = 0;

        public int TotalCount { get { return (int)_totalCount; } set { _totalCount = (uint)value; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalCount">最大可显示的数据量</param>
        /// <param name="getDataFunc">提供数据的 Func</param>
        public IncrementalLoadingModel(uint totalCount, Func<int, int, Task<IEnumerable<T>>> getDataFunc)
        {
            _funcGetData = getDataFunc;
            _totalCount = totalCount;
        }

        /// <summary>
        /// 是否还有更多的数据
        /// </summary>
        public bool HasMoreItems
        {
            get { return this.Count < _totalCount; }
        }

        /// <summary>
        /// 异步加载数据（增量加载）
        /// </summary>
        /// <param name="count">需要加载的数据量</param>
        /// <returns></returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_isBusy)
            {
                throw new InvalidOperationException("Busy...");
            }
            _isBusy = true;

            var dispatcher = Window.Current.Dispatcher;

            return AsyncInfo.Run(
                (token) =>
                    Task.Run<LoadMoreItemsResult>(
                       async () =>
                       {
                           try
                           {

                               // 增量加载的起始索引
                               var startIndex = this.Count;

                               var items = await _funcGetData(startIndex, (int)count);
                               await dispatcher.RunAsync(
                                    CoreDispatcherPriority.Normal,
                                    () =>
                                    {
                                        // 通过 Func 获取增量数据
                                        foreach (var item in items)
                                        {
                                            this.Add(item);
                                        }
                                    });

                               //await dispatcher.RunAsync(
                               //     CoreDispatcherPriority.Normal,
                               //     () =>
                               //     {
                               //         // 通过 Func 获取增量数据
                               //         var items = _funcGetData(startIndex, (int)count);
                               //         foreach (var item in items)
                               //         {
                               //             this.Add(item);
                               //         }
                               //     });

                               // Count - 实际已加载的数据量
                               return new LoadMoreItemsResult { Count = (uint)this.Count };
                           }
                           finally
                           {
                               _isBusy = false;
                           }
                       },
                       token));
        }
    }

}
