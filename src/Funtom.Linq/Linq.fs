﻿namespace Funtom.Linq

open System.Linq
open System.Collections
open System.Collections.Generic
open Funtom.Linq.Core
open System.Runtime.CompilerServices

module Linq =
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.toarray?view=net-6.0
  let inline toArray<'T> (src: seq<'T>) = src.ToArray()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.todictionary?view=net-6.0
  let inline toDictionary< ^T, ^Key> ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToDictionary(selector)
  let inline toDictionary'< ^T, ^Key> (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToDictionary(selector, comparer)
  let inline toDictionary2< ^T, ^Element, ^Key> ([<InlineIfLambda>]elementSelector: ^T -> ^Element) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToDictionary(keySelector, elementSelector)
  let inline toDictionary2'< ^T, ^Element, ^Key> (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]elementSelector: ^T -> ^Element) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToDictionary(keySelector, elementSelector, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tohashset?view=net-6.0
  let inline toHashSet< ^T> (src: seq< ^T>) = src.ToHashSet()
  let inline toHashSet'< ^T> (comparer: IEqualityComparer< ^T>) (src: seq< ^T>) = src.ToHashSet(comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolist?view=net-6.0
  let inline toList< ^T> (src: seq< ^T>) = src.ToList()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.aggregate?view=net-6.0
  let inline aggregate (seed: ^Accumulate) ([<InlineIfLambda>]fx: ^Accumulate -> ^T -> ^Accumulate) (src: seq< ^T>) =
    src.Aggregate(seed, fx)
  let inline aggregate' (seed: ^Accumulate) ([<InlineIfLambda>]fx: ^Accumulate -> ^T -> ^Accumulate) ([<InlineIfLambda>]resultSelector: ^Accumulate -> ^Result) (src: seq< ^T>) =
    src.Aggregate(seed, fx, resultSelector)
  let inline aggregate'' ([<InlineIfLambda>]fx: ^T -> ^T -> ^T) (src: seq< ^T>) =
    src.Aggregate(fx)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.all?view=net-6.0
  let inline all< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.All predicate
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.any?view=net-6.0#System_Linq_Enumerable_Any__1_System_Collections_Generic_IEnumerable___0__
  let inline any< ^T> (src: seq< ^T>) = src.Any()
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.any?view=net-6.0#System_Linq_Enumerable_Any__1_System_Collections_Generic_IEnumerable___0__System_Func___0_System_Boolean__
  let inline any'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.Any predicate

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.append?view=net-6.0
  let inline append< ^T> (element: ^T) (src: seq< ^T>) = src.Append element

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.asenumerable?view=net-6.0
  let inline asEnumerable< ^T> (src: seq< ^T>) = src.AsEnumerable()
    
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.cast?view=net-6.0
  let inline cast< ^T> (source: IEnumerable) : IEnumerable< ^T> = 
    match source with
    | :? IEnumerable< ^T> as src -> src
    | _ -> CastIterator< ^T> source

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.chunk?view=net-6.0
  let inline chunk< ^T> (size: int) (src: seq< ^T>) = src.Chunk size

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.concat?view=net-6.0
  let inline concat< ^T> (fst: seq< ^T>) (snd: seq< ^T>) = fst.Concat snd

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.contains?view=net-6.0
  let inline contains< ^T> (target: ^T) (src: seq< ^T>) = src.Contains target
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.count?view=net-6.0
  let inline count< ^T> (src: seq< ^T>) =
    match src with
    | :? ICollection as xs -> xs.Count
    | :? ICollection< ^T> as xs -> xs.Count
    | :? IReadOnlyCollection< ^T> as xs -> xs.Count
    | _ -> src.Count()
  let inline count'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = Enumerable.Count (src, predicate)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.defaultifempty?view=net-6.0
  let inline defaultIfEmpty< ^T> (src: seq< ^T>) = src.DefaultIfEmpty()
  let inline defaultIfEmpty'< ^T> (defaultValue: ^T) (src: seq< ^T>) = src.DefaultIfEmpty defaultValue

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinct?view=net-6.0
  let inline distinct< ^T> (src: seq< ^T>) = src.Distinct()
  let inline distinct'< ^T> (comparer: IEqualityComparer< ^T>) (src: seq< ^T>) = src.Distinct comparer

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.distinctby?view=net-6.0
  let inline distinctBy< ^T, ^U> ([<InlineIfLambda>]selector: ^T -> ^U) (src: seq< ^T>) = src.DistinctBy selector
  let inline distinctBy'< ^T, ^U> ([<InlineIfLambda>]selector: ^T -> ^U) (comparer: IEqualityComparer< ^U>) (src: seq< ^T>) = src.DistinctBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementat?view=net-6.0
  let inline elementAt< ^T> (index: int) (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[index]
    | :? IReadOnlyList< ^T> as xs -> xs[index]
    | _ -> src.ElementAt index
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.elementatordefault?view=net-6.0
  let inline elementAtOrDefault< ^T> (index: int) (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> if index < 0 || xs.Count <= index then Unchecked.defaultof< ^T> else xs[index]
    | :? IReadOnlyList< ^T> as xs -> if index < 0 || xs.Count <= index then Unchecked.defaultof< ^T> else xs[index]
    | _ -> src.ElementAtOrDefault index

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.empty?view=net-6.0
  let inline empty< ^T> () = Enumerable.Empty< ^T>()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline except< ^T> (fst: seq< ^T>) (snd: seq< ^T>) = fst.Except snd
  let inline except'< ^T> (comparer: IEqualityComparer< ^T>) (fst: seq< ^T>) (snd: seq< ^T>) = fst.Except (snd, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.except?view=net-6.0
  let inline exceptBy< ^T, ^U> ([<InlineIfLambda>]selector: ^T -> ^U) (fst: seq< ^T>) (snd: seq< ^U>) = fst.ExceptBy(snd, selector)
  let inline exceptBy'< ^T, ^U> ([<InlineIfLambda>]selector: ^T -> ^U) (comparer: IEqualityComparer< ^U>)  (fst: seq< ^T>) (snd: seq< ^U>) = fst.ExceptBy(snd, selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.first?view=net-6.0
  let inline first< ^T> (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[0]
    | :? IReadOnlyList< ^T> as xs -> xs[0]
    | _ -> src.First()
  let inline first'< ^T> (predicate: ^T -> bool) (src: seq< ^T>) = src.First predicate

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.firstordefault?view=net-6.0
  let inline firstOrDefault< ^T> (src: seq< ^T>) = src.FirstOrDefault()
  let inline firstOrDefault'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.FirstOrDefault predicate
  let inline firstOrDefaultWith< ^T> (defaultValue: ^T) (src: seq< ^T>) = src.FirstOrDefault(defaultValue)
  let inline firstOrDefaultWith'< ^T> (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.FirstOrDefault(predicate, defaultValue)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupby?view=net-6.0
  let inline groubBy< ^Source, ^Key, ^Element, ^Result> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Source> -> ^Result) (source: seq< ^Source>) =
    source.GroupBy(keySelector, resultSelector)
  let inline groubBy'< ^Source, ^Key, ^Element, ^Result> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Source> -> ^Result) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, resultSelector, comparer)
  let inline groubByElement< ^Source, ^Key, ^Element, ^Result> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector)
  let inline groubByElement'< ^Source, ^Key, ^Element, ^Result> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, comparer)
  let inline groubBy2< ^Source, ^Key, ^Element, ^Result> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Element> -> ^Result) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, resultSelector)
  let inline groubBy2'< ^Source, ^Key, ^Element, ^Result> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) ([<InlineIfLambda>]elementSelector: ^Source -> ^Element) ([<InlineIfLambda>]resultSelector: ^Key -> seq< ^Element> -> ^Result) (comparer: IEqualityComparer< ^Key>) (source: seq< ^Source>) =
    source.GroupBy(keySelector, elementSelector, resultSelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.groupjoin?view=net-6.0
  let inline groupJoin< ^Outer, ^Inner, ^Key, ^Result> (inner: seq< ^Inner>)  ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (outer: seq< ^Outer>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline groupJoin'< ^Outer, ^Inner, ^Key, ^Result> (inner: seq< ^Inner>)  ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer)
  let inline groupJoin2< ^Outer, ^Inner, ^Key, ^Result> ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline groupJoin2'< ^Outer, ^Inner, ^Key, ^Result> ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> seq< ^Inner> -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersect?view=net-6.0
  let inline intersect< ^Source> (second: seq< ^Source>) (first: seq< ^Source>)  = first.Intersect second
  let inline intersect'< ^Source> (second: seq< ^Source>) (first: seq< ^Source>) (comparer: IEqualityComparer< ^Source>) = first.Intersect (second, comparer)
  let inline intersect2< ^Source> (first: seq< ^Source>, second: seq< ^Source>) = first.Intersect second
  let inline intersect2'< ^Source> (comparer: IEqualityComparer< ^Source>) (first: seq< ^Source>, second: seq< ^Source>) = first.Intersect (second, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.intersectby?view=net-6.0
  let inline intersectBy< ^Source, ^Key> (second: seq< ^Key>) ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (first: seq< ^Source>)  = first.IntersectBy (second, keySelector)
  let inline intersectBy'< ^Source, ^Key> (second: seq< ^Key>) ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (comparer: IEqualityComparer< ^Key>) (first: seq< ^Source>) = first.IntersectBy (second, keySelector, comparer)
  let inline intersectBy2< ^Source, ^Key> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (first: seq< ^Source>, second: seq< ^Key>)  = first.IntersectBy (second, keySelector)
  let inline intersectBy2'< ^Source, ^Key> ([<InlineIfLambda>]keySelector: ^Source -> ^Key) (comparer: IEqualityComparer< ^Key>) (first: seq< ^Source>, second: seq< ^Key>) = first.IntersectBy (second, keySelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.join?view=net-6.0
  let inline join< ^Outer, ^Inner, ^Key, ^Result> (inner: seq< ^Inner>) ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (outer: seq< ^Outer>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline join'< ^Outer, ^Inner, ^Key, ^Result> (inner: seq< ^Inner>) ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector, comparer)
  let inline join2< ^Outer, ^Inner, ^Key, ^Result> ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector)
  let inline join2'< ^Outer, ^Inner, ^Key, ^Result> ([<InlineIfLambda>]outerKeySelector: ^Outer -> ^Key) ([<InlineIfLambda>]innerKeySelector: ^Inner -> ^Key) ([<InlineIfLambda>]resultSelector: ^Outer -> ^Inner -> ^Result) (comparer: IEqualityComparer< ^Key>) (outer: seq< ^Outer>, inner: seq< ^Inner>) =
    outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.last?view=net-6.0
  let inline last< ^T> (src: seq< ^T>) =
    match src with
    | :? IList< ^T> as xs -> xs[xs.Count - 1]
    | :? IReadOnlyList< ^T> as xs -> xs[xs.Count - 1]
    | _ -> src.Last()
  let inline last'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.Last predicate
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.lastordefault?view=net-6.0
  let inline lastOrDefault< ^T> (src: seq< ^T>) = src.LastOrDefault()
  let inline lastOrDefault'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.LastOrDefault predicate
  let inline lastOrDefaultWith< ^T> (defaultValue: ^T) (src: seq< ^T>) = src.LastOrDefault(defaultValue)
  let inline lastOrDefaultWith'< ^T> (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.LastOrDefault(predicate, defaultValue)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.longcount?view=net-6.0
  let inline longCount< ^T> (src: seq< ^T>) : int64 =
    match src with
    | :? ICollection as xs -> xs.Count
    | :? ICollection< ^T> as xs -> xs.Count
    | :? IReadOnlyCollection< ^T> as xs -> xs.Count
    | _ -> src.LongCount()
  let inline longCount'< ^T> ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.LongCount predicate
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.max?view=net-6.0
  let inline max (src: seq< ^T>) =
    match src with
    | :? list< ^T> as ls ->
      let rec max' (ls: list< ^T>, v: ^T) =
        match ls with
        | h::tail -> max' (tail, if h < v then v else h)
        | _ -> v
      match ls with
      | h::tail -> max' (tail, h)
      | _ -> Unchecked.defaultof< ^T>
    | :? array< ^T> as ary -> 
      if 0 < ary.Length then
        let mutable v = ary[0]
        for i = 1 to ary.Length - 1 do
          let current = ary[i]
          if v < current then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | :? ResizeArray< ^T> as ls -> 
      if 0 < ls.Count then
        let mutable v = ls[0]
        for i = 1 to ls.Count - 1 do
          let current = ls[i]
          if v < current then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | _ ->
      let iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if v < c then v <- c
        v
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.maxby?view=net-6.0
  let inline maxBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.MaxBy(selector)
  let inline maxBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.MaxBy(selector, comparer)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.min?view=net-6.0
  let inline min (src: seq< ^T>) =
    match src with
    | :? list< ^T> as ls ->
      let rec min' (ls: list< ^T>, v: ^T) =
        match ls with
        | h::tail -> min' (tail, if h < v then h else v)
        | _ -> v
      match ls with
      | h::tail -> min' (tail, h)
      | _ -> Unchecked.defaultof< ^T>
    | :? array< ^T> as ary -> 
      if 0 < ary.Length then
        let mutable v = ary[0]
        for i = 1 to ary.Length - 1 do
          let current = ary[i]
          if current < v then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | :? ResizeArray< ^T> as ls -> 
      if 0 < ls.Count then
        let mutable v = ls[0]
        for i = 1 to ls.Count - 1 do
          let current = ls[i]
          if current < v then v <- current
        v
      else
        Unchecked.defaultof< ^T>
    | _ ->
      let iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable v = iter.Current
        while iter.MoveNext() do
          let c = iter.Current
          if c < v then v <- c
        v
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.minby?view=net-6.0
  let inline minBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.MinBy(selector)
  let inline minBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.MinBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.oftype?view=net-6.0
  let inline ofType< ^T> (src: IEnumerable) = OfTypeIterator< ^T> src
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderby?view=net-6.0
  let inline orderBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.OrderBy(selector)
  let inline orderBy' ([<InlineIfLambda>]selector: ^T -> ^Key) (comparer: IComparer< ^Key>) (src: seq< ^T>) = src.OrderBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.orderbydescending?view=net-6.0
  let inline orderByDescending ([<InlineIfLambda>]selector: 'source -> 'key) (src: seq<'source>) = src.OrderByDescending(selector)
  let inline orderByDescending' ([<InlineIfLambda>]selector: 'source -> 'key) (comparer: IComparer<'key>) (src: seq<'source>) = src.OrderByDescending(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.prepend?view=net-6.0
  let inline prepend (element: ^T) (src: seq< ^T>) = src.Prepend(element)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.reverse?view=net-6.0
  let inline reverse (src: seq< ^T>) = src.Reverse()

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.select?view=net-6.0
  let inline select ([<InlineIfLambda>] selector: ^T -> ^Result) (source: seq< ^T>) : seq< ^Result> =
    match source with
    | :? array< ^T> as ary -> ary.Select selector //SelectArrayIterator.create selector ary
    | :? ResizeArray< ^T> as ls -> ls.Select selector // SelectListIterator.create selector ls
    | :? list< ^T> as ls -> SelectFsListIterator.create selector ls
    | _ -> source.Select selector //SelectEnumerableIterator.create selector source

  //let inline select<'T, 'U> ([<InlineIfLambda>]selector: 'T -> 'U) (src: seq<'T>): seq<'U> = src.Select selector
  let inline select' ([<InlineIfLambda>]selector: ^T -> int -> ^Result) (src: seq< ^T>): seq< ^Result> = src.Select selector

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.selectmany?view=net-6.0
  let inline selectMany ([<InlineIfLambda>]selector: ^T -> seq< ^Result>) (src: seq< ^T>) = src.SelectMany(selector)
  let inline selectMany' ([<InlineIfLambda>]selector: ^T -> int -> seq< ^Result>) (src: seq< ^T>) = src.SelectMany(selector)
  let inline selectMany2 ([<InlineIfLambda>]resultSelector: ^T -> ^Collection -> ^Result) ([<InlineIfLambda>]collectionSelector: ^T -> seq< ^Collection>) (src: seq< ^T>) = src.SelectMany(collectionSelector, resultSelector)
  let inline selectMany2' ([<InlineIfLambda>]resultSelector: ^T -> ^Collection -> ^Result) ([<InlineIfLambda>]collectionSelector: ^T -> int -> seq< ^Collection>) (src: seq< ^T>) = src.SelectMany(collectionSelector, resultSelector)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.sequenceequal?view=net-6.0
  let inline sequenceEqual (snd: seq< ^T>) (fst: seq< ^T>) = fst.SequenceEqual(snd)
  let inline sequenceEqual' (comparer: IEqualityComparer< ^T>) (snd: seq< ^T>) (fst: seq< ^T>) = fst.SequenceEqual(snd, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.single?view=net-6.0
  let inline single (src: seq< ^T>) = src.Single()
  let inline single' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.Single(predicate)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.singleordefault?view=net-6.0
  let inline singleOrDefault (src: seq< ^T>) = src.SingleOrDefault()
  let inline singleOrDefault' ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SingleOrDefault(predicate)
  let inline singleOrDefaultWith (defaultValue: ^T) (src: seq< ^T>) = src.SingleOrDefault(defaultValue)
  let inline singleOrDefaultWith' (defaultValue: ^T) ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SingleOrDefault(predicate, defaultValue)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skip?view=net-6.0
  let inline skip (count: int) (src: seq< ^T>) = src.Skip(count)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skiplast?view=net-6.0
  let inline skipLast (count: int) (src: seq< ^T>) = src.SkipLast(count)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.skipwhile?view=net-6.0
  let inline skipWhile ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.SkipWhile(predicate)
  let inline skipWhile' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) = src.SkipWhile(predicate)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.sum?view=net-6.0
  let inline sum (src: seq< ^T>) =
    match src with
    | :? array< ^T> as ary ->
      if 0 < ary.Length then
        let mutable acc = ary[0]
        for i = 1 to ary.Length - 1 do
          acc <- acc + ary[i]
        acc
      else Unchecked.defaultof< ^T>
    | :? ResizeArray< ^T> as ary ->
      if 0 < ary.Count then
        let mutable acc = ary[0]
        for i = 1 to ary.Count - 1 do
          acc <- acc + ary[i]
        acc
      else Unchecked.defaultof< ^T>
    | :? list< ^T> as ls ->
      let rec f xs acc =
        match xs with
        | h::tail -> f tail (acc + h)
        | _ -> acc
      match ls with
      | h::tail -> f tail h
      | _ -> Unchecked.defaultof< ^T>
    | _ ->
      let iter = src.GetEnumerator()
      if iter.MoveNext() then
        let mutable acc = iter.Current
        while iter.MoveNext() do
          acc <- acc + iter.Current
        acc
      else
        Unchecked.defaultof< ^T>

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.take?view=net-6.0
  let inline take (count: int) (src: seq< ^T>) = src.Take(count)
  let inline take' (range: System.Range) (src: seq< ^T>) = src.Take(range)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takelast?view=net-6.0
  let inline takeLast (count: int) (src: seq< ^T>) = src.TakeLast(count)
  
  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.takewhile?view=net-6.0
  let inline takeWhile ([<InlineIfLambda>]predicate: ^T -> bool) (src: seq< ^T>) = src.TakeWhile(predicate)
  let inline takeWhile' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) = src.TakeWhile(predicate)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenby?view=net-6.0
  let inline thenBy ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenBy(selector)
  let inline thenBy' (comparer: IComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenBy(selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.thenbydescending?view=net-6.0
  let inline thenByDescending([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenByDescending (selector)
  let inline thenByDescending' (comparer: IComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: IOrderedEnumerable< ^T>) = src.ThenByDescending (selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.tolookup?view=net-6.0
  let inline toLookup ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(selector)
  let inline toLookup' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(selector, comparer)
  let inline toLookup2 ([<InlineIfLambda>]elementSelector: ^T -> ^Key) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(keySelector, elementSelector)
  let inline toLookup2' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]elementSelector: ^T -> ^Key) ([<InlineIfLambda>]keySelector: ^T -> ^Key) (src: seq< ^T>) = src.ToLookup(keySelector, elementSelector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.trygetnonenumeratedcount?view=net-6.0
  let inline tryGetNonEnumeratedCount (count: outref<int>) (src: seq< ^T>) = src.TryGetNonEnumeratedCount(&count)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.union?view=net-6.0
  let inline union (snd: seq< ^T>) (fst: seq< ^T>) = fst.Union snd
  let inline union' (comparer: IEqualityComparer< ^T>) (snd: seq< ^T>) (fst: seq< ^T>) = fst.Union(snd, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.unionby?view=net-6.0
  let inline unionBy ([<InlineIfLambda>]selector: ^T -> ^Key) (snd: seq< ^T>) (fst: seq< ^T>) = fst.UnionBy (snd, selector)
  let inline unionBy' (comparer: IEqualityComparer< ^Key>) ([<InlineIfLambda>]selector: ^T -> ^Key) (snd: seq< ^T>) (fst: seq< ^T>) = fst.UnionBy (snd, selector, comparer)

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.where?view=net-6.0
  let inline where ([<InlineIfLambda>] predicate: ^T -> bool) (source: seq< ^T>) : seq< ^T> =
    match source with
    | :? WhereIterator< ^T> as iterator -> iterator.where predicate
    | :? WhereArrayIterator< ^T> as iterator -> iterator.where predicate
    | :? WhereResizeArrayIterator< ^T> as iterator -> iterator.where predicate
    | :? WhereListIterator< ^T> as iterator -> iterator.where predicate
    | :? array< ^T> as ary ->
      if ary.Length = 0 then System.Array.Empty< ^T>() 
      else new WhereArrayIterator< ^T>(ary, predicate)
    | :? ResizeArray< ^T> as ls -> new WhereResizeArrayIterator<'T> (ls, predicate)
    | :? list< ^T> as ls -> new WhereListIterator< ^T>(ls, predicate)
    | _ -> new WhereIterator< ^T> (source, predicate)

  let inline where' ([<InlineIfLambda>]predicate: ^T -> int -> bool) (src: seq< ^T>) =
    let mutable i = -1
    seq {
      for v in src do
        i <- i + 1
        if predicate v i then
          yield v
    }

  // https://docs.microsoft.com/ja-jp/dotnet/api/system.linq.enumerable.zip?view=net-6.0
  let inline zip (snd: seq< ^Fst>) (fst: seq< ^Snd>) = fst.Zip(snd)
  let inline zip' (snd: seq< ^Snd>) ([<InlineIfLambda>]selector: ^Fst -> ^Snd -> ^Result) (fst: seq< ^Fst>) = fst.Zip(snd, selector)
  let inline zip3 (snd: seq< ^Snd>) (thd: seq< ^Thd>) (fst: seq< ^Fst>) = fst.Zip(snd, thd)