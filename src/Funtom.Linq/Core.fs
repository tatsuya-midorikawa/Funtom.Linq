﻿namespace Funtom.Linq

open System.Collections
open System.Collections.Generic
open System
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Runtime.InteropServices


module rec Core =
  let inline combine_predicates ([<InlineIfLambda>] p1: 'source -> bool) ([<InlineIfLambda>] p2: 'source -> bool) (x: 'source) = p1 x && p2 x

  /// <summary>
  /// 
  /// </summary>
  type WhereEnumerableIterator<'source> (source: seq<'source>, [<InlineIfLambda>] predicate: 'source -> bool) as self =
    let mutable state : int = 0
    let mutable enumerator : IEnumerator<'source> = Unchecked.defaultof<IEnumerator<'source>>
    let mutable current : 'source = Unchecked.defaultof<'source>
    let thread_id : int = Environment.CurrentManagedThreadId
    
    let clone () = 
      new WhereEnumerableIterator<'source>(source, predicate)

    let dispose () = 
      if enumerator <> Unchecked.defaultof<IEnumerator<'source>> then
        enumerator.Dispose()
        enumerator <- Unchecked.defaultof<IEnumerator<'source>>
        current <- Unchecked.defaultof<'source>
        state <- -1

    let get_enumerator () =
      if state = 0 && thread_id = Environment.CurrentManagedThreadId then
        state <- 1
        self
      else
        let enumerator = clone()
        enumerator.State <- 1
        enumerator
      
    let rec move_next () =
      if enumerator.MoveNext() then
        let item = enumerator.Current
        if predicate item then
          current <- item
          true
        else
          move_next ()
      else
        dispose()
        false

    interface IDisposable with
      member __.Dispose () = dispose ()

    interface IEnumerator with
      member __.MoveNext () : bool = 

        match state with
        | 1 -> 
          enumerator <- source.GetEnumerator()
          state <- 2
          move_next ()
        | 2 ->
          move_next ()
        | _ -> 
          false

      member __.Current with get() = current
      member __.Reset () = raise(NotSupportedException "not supported")

    interface IEnumerator<'source> with
      member __.Current with get() = current

    interface IEnumerable with
      member __.GetEnumerator () = get_enumerator ()

    interface IEnumerable<'source> with
      member __.GetEnumerator () = get_enumerator ()

    member __.where (predicate': 'source -> bool) =
      let p = combine_predicates predicate predicate'
      new WhereEnumerableIterator<'source>(source, p)

    member private __.State with get() = state and set v = state <- v

  /// <summary>
  /// 
  /// </summary>
  type WhereArrayIterator<'source> (source: array<'source>, [<InlineIfLambda>] predicate: 'source -> bool) as self =
    let mutable state : int = 0
    let mutable current : 'source = Unchecked.defaultof<'source>
    let thread_id : int = Environment.CurrentManagedThreadId
  
    let clone () = new WhereArrayIterator<'source>(source, predicate)
  
    let dispose () = 
      current <- Unchecked.defaultof<'source>
      state <- -1
  
    let get_enumerator () =
      if state = 0 && thread_id = Environment.CurrentManagedThreadId then
        state <- 1
        self
      else
        let enumerator = clone()
        enumerator.State <- 1
        enumerator
  
    let rec move_next (src: array<'source>, index: int) =
      if uint index < uint src.Length then
        let item = src[index]
        state <- state + 1
        if predicate item then
          current <- item
          true
        else
          move_next(src, index + 1)
      else
        dispose()
        false
  
    interface IDisposable with member __.Dispose () = dispose ()
  
    interface IEnumerator with
      member __.MoveNext () : bool = move_next (source, state - 1)
      member __.Current with get() = current
      member __.Reset () = raise(NotSupportedException "not supported")
  
    interface IEnumerator<'source> with member __.Current with get() = current
  
    interface IEnumerable with member __.GetEnumerator () = get_enumerator ()
  
    interface IEnumerable<'source> with member __.GetEnumerator () = get_enumerator ()
  
    member __.where (predicate': 'source -> bool) = new WhereArrayIterator<'source>(source, combine_predicates predicate predicate')
  
    member private __.State with get() = state and set v = state <- v

  /// <summary>
  /// 
  /// </summary>
  type WhereListIterator<'source> (source: ResizeArray<'source>, [<InlineIfLambda>] predicate: 'source -> bool) as self =
    let mutable state : int = 0
    let mutable current : 'source = Unchecked.defaultof<'source>
    let thread_id : int = Environment.CurrentManagedThreadId
  
    let clone () = new WhereListIterator<'source>(source, predicate)
  
    let dispose () = 
      current <- Unchecked.defaultof<'source>
      state <- -1
  
    let get_enumerator () =
      if state = 0 && thread_id = Environment.CurrentManagedThreadId then
        state <- 1
        self
      else
        let enumerator = clone()
        enumerator.State <- 1
        enumerator
  
    let rec move_next (src: ResizeArray<'source>, index: int) =
      if uint index < uint src.Count then
        let item = src[index]
        state <- state + 1
        if predicate item then
          current <- item
          true
        else
          move_next(src, index + 1)
      else
        dispose()
        false
  
    interface IDisposable with
      member __.Dispose () = dispose ()
  
    interface IEnumerator with
      member __.MoveNext () : bool = move_next (source, state - 1)  
      member __.Current with get() = current
      member __.Reset () = raise(NotSupportedException "not supported")
  
    interface IEnumerator<'source> with
      member __.Current with get() = current
  
    interface IEnumerable with
      member __.GetEnumerator () = get_enumerator ()
  
    interface IEnumerable<'source> with
      member __.GetEnumerator () = get_enumerator ()
  
    member __.where (predicate': 'source -> bool) =
      let p = combine_predicates predicate predicate'
      new WhereListIterator<'source>(source, p)
  
    member private __.State with get() = state and set v = state <- v
  
  /// <summary>
  /// 
  /// </summary>
  type WhereFsListIterator<'source> (source: list<'source>, [<InlineIfLambda>] predicate: 'source -> bool) as self =
    let mutable state : int = 0
    let mutable cache : list<'source> = source
    let mutable current : 'source = Unchecked.defaultof<'source>
    let thread_id : int = Environment.CurrentManagedThreadId
  
    let clone () = new WhereFsListIterator<'source>(source, predicate)
  
    let dispose () = 
      current <- Unchecked.defaultof<'source>
      state <- -1
  
    let get_enumerator () =
      if state = 0 && thread_id = Environment.CurrentManagedThreadId then
        state <- 1
        self
      else
        let enumerator = clone()
        enumerator.State <- 1
        enumerator
  
    let rec move_next (src: list<'source>) =
      match src with
      | h::tail ->
        if predicate h then
          current <- h
          cache <- tail
          true
        else
          move_next(tail)
      | _ ->
        dispose()
        false
  
    interface IDisposable with
      member __.Dispose () = dispose ()
  
    interface IEnumerator with
      member __.MoveNext () : bool = 
        move_next cache
  
      member __.Current with get() = current
      member __.Reset () = raise(NotSupportedException "not supported")
  
    interface IEnumerator<'source> with
      member __.Current with get() = current
  
    interface IEnumerable with
      member __.GetEnumerator () = get_enumerator ()
  
    interface IEnumerable<'source> with
      member __.GetEnumerator () = get_enumerator ()
  
    member __.where (predicate': 'source -> bool) =
      let p = combine_predicates predicate predicate'
      new WhereFsListIterator<'source>(source, p)
  
    member private __.State with get() = state and set v = state <- v

  /// <summary>
  /// 
  /// </summary>
  module SelectEnumerableIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: seq<'T>) = 
      { 
        SelectEnumerableIterator.source = source
        selector = selector
        thread_id = Environment.CurrentManagedThreadId
        enumerator = Unchecked.defaultof<IEnumerator<'T>>
        current = Unchecked.defaultof<'R>
        state = -1
      }
    let inline dispose (iter: SelectEnumerableIterator<'T, 'R>) = 
      if iter.enumerator <> Unchecked.defaultof<IEnumerator<'T>> then
        iter.enumerator.Dispose()
        iter.enumerator <- Unchecked.defaultof<IEnumerator<'T>>
        iter.current <- Unchecked.defaultof<'R>
        iter.state <- -1

    let inline get_enumerator (iter: SelectEnumerableIterator<'T, 'R>) =
      if iter.state = -1 && iter.thread_id = Environment.CurrentManagedThreadId then
        iter.enumerator <- iter.source.GetEnumerator()
        iter.state <- 0
        iter
      else
        { iter with thread_id = Environment.CurrentManagedThreadId; state = -1; current = Unchecked.defaultof<'R> }

    let inline move_next (iter: SelectEnumerableIterator<'T, 'R>) =
      if iter.enumerator.MoveNext() then
        iter.current <- iter.selector iter.enumerator.Current
        true
      else
        dispose iter
        false
  
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectEnumerableIterator<'T, 'R> =
    {
      source: seq<'T>
      selector: 'T -> 'R
      thread_id : int 
      mutable enumerator : IEnumerator<'T>
      mutable current : 'R
      mutable state : int
    }
    interface IDisposable with member __.Dispose () = SelectEnumerableIterator.dispose __
    interface IEnumerator with 
      member __.MoveNext () : bool = SelectEnumerableIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'R> with member __.Current with get() = __.current
    interface IEnumerable with member __.GetEnumerator () = SelectEnumerableIterator.get_enumerator __
    interface IEnumerable<'R> with member __.GetEnumerator () = SelectEnumerableIterator.get_enumerator __

  /// <summary>
  /// 
  /// </summary>
  module SelectArrayIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: array<'T>) = 
      { 
        SelectArrayIterator.source = source
        selector = selector
        thread_id = Environment.CurrentManagedThreadId
        state = -1
      }
    let inline dispose (iter: SelectArrayIterator<'source, 'result>) = iter.state <- -2
    let inline get_enumerator (iter: SelectArrayIterator<'source, 'result>) = 
      if iter.state = -2 && iter.thread_id = Environment.CurrentManagedThreadId then
        iter.state <- -1
        iter
      else
        { iter with state = -1; thread_id = Environment.CurrentManagedThreadId }
    let inline move_next (iter: SelectArrayIterator<'source, 'result>) =
      if iter.state < -1 || iter.state = iter.source.Length - 1 then
        dispose iter
        false
      else
        iter.state <- iter.state + 1
        true
        
  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectArrayIterator<'source, 'result> =
    {
      source: array<'source>
      selector: 'source -> 'result
      thread_id : int
      mutable state : int
    }
    interface IDisposable with member __.Dispose () = SelectArrayIterator.dispose __
    interface IEnumerator with
      member __.MoveNext () : bool = SelectArrayIterator.move_next __
      member __.Current with get() = (__.selector __.source[__.state]) :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'result> with member __.Current with get() = __.selector __.source[__.state]
    interface IEnumerable with member __.GetEnumerator () = SelectArrayIterator.get_enumerator __
    interface IEnumerable<'result> with member __.GetEnumerator () = SelectArrayIterator.get_enumerator __
  
  /// <summary>
  /// 
  /// </summary>
  module SelectListIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: ResizeArray<'T>) = 
      { 
        SelectListIterator.source = source
        selector = selector
        thread_id = Environment.CurrentManagedThreadId
        current = Unchecked.defaultof<'R>
        index = 0
      }
    let inline get_enumerator (iter: SelectListIterator<'source, 'result>) = 
      if iter.thread_id = Environment.CurrentManagedThreadId then iter
      else { iter with thread_id = Environment.CurrentManagedThreadId; current = Unchecked.defaultof<'result> }
    let inline move_next (iter: SelectListIterator<'source, 'result>) =
      if uint iter.index < uint iter.source.Count then
        iter.current <- iter.selector iter.source[iter.index]
        iter.index <- iter.index + 1
        true
      else
        false

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectListIterator<'source, 'result> =
    {
      source: ResizeArray<'source>
      selector: 'source -> 'result
      thread_id : int
      mutable current : 'result
      mutable index : int
    }
    interface IDisposable with member __.Dispose () = ()
    interface IEnumerator with
      member __.MoveNext () : bool = SelectListIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'result> with member __.Current with get() = __.current 
    interface IEnumerable with member __.GetEnumerator () = SelectListIterator.get_enumerator __
    interface IEnumerable<'result> with member __.GetEnumerator () = SelectListIterator.get_enumerator __

  /// <summary>
  /// 
  /// </summary>
  module SelectFsListIterator =
    let inline create ([<InlineIfLambda>]selector: 'T -> 'R) (source: list<'T>) = 
      { 
        SelectFsListIterator.selector = selector
        thread_id = Environment.CurrentManagedThreadId
        current = Unchecked.defaultof<'R>
        cache = source
      }
    let inline get_enumerator (iter: SelectFsListIterator<'source, 'result>) = 
      if iter.thread_id = Environment.CurrentManagedThreadId then iter
      else { iter with thread_id = Environment.CurrentManagedThreadId; current = Unchecked.defaultof<'result> }
    let inline move_next (iter: SelectFsListIterator<'source, 'result>) =
      match iter.cache with
      | h::tail ->
        iter.current <- iter.selector h
        iter.cache <- tail
        true
      | _ ->
        false

  /// <summary>
  /// 
  /// </summary>
  [<NoComparison;NoEquality>]
  type SelectFsListIterator<'T, 'R> =
    {
      selector: 'T -> 'R
      thread_id : int
      mutable current : 'R
      mutable cache : list<'T>
    }
    interface IDisposable with member __.Dispose () = ()
    interface IEnumerator with
      member __.MoveNext () : bool = SelectFsListIterator.move_next __
      member __.Current with get() = __.current :> obj
      member __.Reset () = raise(NotSupportedException "not supported")
    interface IEnumerator<'R> with member __.Current with get() = __.current 
    interface IEnumerable with member __.GetEnumerator () = SelectFsListIterator.get_enumerator __
    interface IEnumerable<'R> with member __.GetEnumerator () = SelectFsListIterator.get_enumerator __
    
  //[<NoComparison;NoEquality>]
  //type Iterator< ^C, ^T, ^R when ^C : (member GetEnumerator: unit -> IEnumerator< ^T>)> =
  //  {
  //    source: ^C
  //    selector: ^T -> ^R
  //  }
  //  member __.GetEnumerator () = (^C: (member GetEnumerator: unit -> IEnumerator< ^T>) __.source)


  let inline select< ^source, ^middle, ^result> ([<InlineIfLambda>] selector: ^source -> ^result) (source: seq< ^source>) : seq< ^result> =
    match source with
    | :? array< ^source> as ary -> SelectArrayIterator.create selector ary
    | :? ResizeArray< ^source> as ls -> SelectListIterator.create selector ls
    | :? list< ^source> as ls -> SelectFsListIterator.create selector ls
    | _ -> SelectEnumerableIterator.create selector source
