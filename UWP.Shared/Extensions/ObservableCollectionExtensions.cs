using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UWP.Shared.Extensions {

    /// <summary>
    /// Source on Panda-Sharp's Yugen.Toolkit repo
    /// https://github.com/Panda-Sharp/Yugen.Toolkit/blob/develop/Yugen.Toolkit.Uwp/Extensions/ObservableGroupedCollectionExtensions.cs
    /// </summary>
    public static class ObservableCollectionExtensions {
        /// <summary>
        /// Sorts the elements of a sequence in order according to a key.
        /// </summary>
        /// <param name="collection">
        /// The collection to sort.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of item within the collection.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type to order by.
        /// </typeparam>
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector, bool descending = true) {
            if (collection == null || collection.Count <= 1) {
                return;
            }

            var newIndex = 0;
            var sorted = descending ? collection.OrderByDescending(keySelector) : collection.OrderBy(keySelector);
            foreach (var oldIndex in sorted.Select(collection.IndexOf)) {
                if (oldIndex != newIndex) {
                    collection.Move(oldIndex, newIndex);
                }

                newIndex++;
            }
        }

        /// <summary>
        /// Sorts the elements of a sequence in order according to the default comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable<T> =>
            Sort(collection, Comparer<T>.Default);

        /// <summary>
        /// Sorts the elements of a sequence in order according to a comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
        public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer) {
            if (collection == null || collection.Count <= 1) {
                return;
            }

            var newIndex = 0;
            foreach (var oldIndex in collection.OrderBy(x => x, comparer).Select(collection.IndexOf)) {
                if (oldIndex != newIndex) {
                    collection.Move(oldIndex, newIndex);
                }

                newIndex++;
            }
        }

        /// <summary>
        /// Inserts an element into the ordered collection at the proper index.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="keySelector"></param>
        public static void AddSorted<TSource, TKey>(this ObservableCollection<TSource> collection, TSource item,
            Func<TSource, TKey> keySelector) where TKey : IComparable<TKey> {
            var i = collection.Select((Value, Index) => new { Value, Index })
                                .FirstOrDefault(x => keySelector(x.Value)
                                    .CompareTo(keySelector(item)) > 0);

            if (i == null) {
                collection.Add(item);
            }
            else {
                collection.Insert(i.Index, item);
            }
        }

        /// <summary>
        /// Inserts an element into the ordered collection at the proper index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        public static void AddSorted<T>(this ObservableCollection<T> collection, T item) where T : IComparable<T> =>
            AddSorted(collection, item, Comparer<T>.Default);

        /// <summary>
        /// Inserts an element into the ordered collection at the proper index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        public static void AddSorted<T>(this ObservableCollection<T> collection, T item, IComparer<T> comparer) {
            int i = 0;
            while (i < collection.Count && comparer.Compare(collection[i], item) < 0) {
                i++;
            }

            collection.Insert(i, item);
        }
    }
}
