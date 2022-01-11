﻿namespace Funtom.Linq.SpeedOpt

open System.Runtime.CompilerServices
open System.Diagnostics

module ArrayOp =
  [<Literal>]
  let DefaultCapacity = 4
  [<Literal>]
  let StartingCapacity = 4
  [<Literal>]
  let ResizeLimit = 8
  [<Literal>]
  let MaxCoreClrArrayLength = 0x7fefffff  // For byte arrays the limit is slightly larger

  /// <summary>
  /// 
  /// </summary>
  let inline create<'T> length = 
    if 0 < length then Array.create length Unchecked.defaultof<'T> 
    else Array.empty<'T>

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.cs#L13">CopyPosition</see>
  [<Struct; IsReadOnly; DebuggerDisplay("{DebuggerDisplay, nq}")>]
  type CopyPosition = {
    row: int
    column: int
  } with
    static member Start with get() = { row = 0; column = 0; }
    member __.Normalize (endColumn: int) =
      if __.column = endColumn then { row = __.row + 1; column = 0}
      else __
    member __.DebuggerDisplay() = $"[%d{__.row}, %d{__.column}]"

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/ArrayBuilder.cs#L13">struct ArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality>]
  type ArrayBuilder<'T> =
    val mutable array': array<'T>
    val mutable count': int
    new (capacity: int) = { 
      array' = if 0 < capacity then create<'T> capacity else Array.empty<'T>
      count' = 0; }

    member __.Capacity with get() = __.array'.Length
    member __.Buffer with get() = __.array'
    member __.Count with get() = __.count'
    member __.Item with get(index: int) =
      assert (0 <= index && index < __.count')
      __.array'[index]

    member __.Add (item: 'T) =
      if __.count' = __.Capacity then
        __.EnsureCapacity(__.count' + 1)
      __.UncheckedAdd item

    member __.First () = __.array'[0]

    member __.Last () = __.array'[__.count' - 1]

    member __.ToArray () =
      let mutable result = __.array'
      if __.count' < result.Length then
        result <- create<'T> __.count'
        System.Array.Copy(__.array', 0, result, 0, __.count')
      result

    member __.UncheckedAdd (item: 'T) =
      __.array'[__.count'] <- item
      __.count' <- __.count' + 1

    member private __.EnsureCapacity (minimum: int) =
      let capacity = __.Capacity
      let mutable nextCapacity = if capacity = 0 then DefaultCapacity else 2 * capacity
      if uint MaxCoreClrArrayLength < uint nextCapacity then
        nextCapacity <- max (capacity + 1) MaxCoreClrArrayLength
      nextCapacity <- max nextCapacity minimum

      let next = create<'T> nextCapacity
      if 0 < __.count' then
        System.Array.Copy(__.array', 0, next, 0, __.count')
      __.array' <- next

  /// <summary>
  /// 
  /// </summary>
  /// <see href="https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L14">LargeArrayBuilder&lt;T&gt;</see>
  [<Struct;NoComparison;NoEquality>]
  type LargeArrayBuilder<'T> =
    val mutable maxCapacity': int
    val mutable first': array<'T>
    val mutable buffers': ArrayBuilder<array<'T>>
    val mutable current': array<'T>
    val mutable index': int
    val mutable count': int
    new (maxCapacity: int) = { 
      maxCapacity' = maxCapacity
      first' = Array.empty<'T>
      buffers' = ArrayBuilder(StartingCapacity)
      current' = Array.empty<'T>
      index' = 0
      count' = 0 }

    member __.Count = __.count'

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Add (item: 'T) =
      assert (__.count' < __.maxCapacity')
      let mutable index = __.index'
      let current = __.current'
      if uint current.Length <= uint index then
        __.AddWithBufferAllocation item
      else
       current[index] <- item
       __.index' <- index + 1
      __.count' <- __.count' + 1

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L105
    member __.AddRange (items: seq<'T>) =
      use enumerator = items.GetEnumerator()
      let mutable destination = __.current'
      let mutable index = __.index'

      while enumerator.MoveNext() do
        let item = enumerator.Current
        if uint destination.Length <= uint index then
          __.AddWithBufferAllocation(item, &destination, &index)
        else
          destination[index] <- item
        index <- index + 1 

      __.count' <- __.count' + (index - __.index')
      __.index' <- index

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L157
    member __.CopyTo (array: array<'T>, arrayIndex: int, count: int) =
      let mutable count' = count
      let mutable arrayIndex' = arrayIndex
      let length = count - 1

      for i = 0 to length do
        let buffer = __.GetBuffer(i)
        let toCopy = min count' buffer.Length
        System.Array.Copy(buffer, 0, array,  arrayIndex', toCopy)
        
        count' <- count' - toCopy
        arrayIndex' <- arrayIndex' + toCopy

    // https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L186
    member __.CopyTo (position: CopyPosition, array: array<'T>, arrayIndex: int, count: int) =
      let mutable arrayIndex = arrayIndex
      let mutable count = count

      let copyToCore (sourceBuffer: array<'T>, sourceIndex: int) =
        let copyCount = min (sourceBuffer.Length - sourceIndex) count
        System.Array.Copy(sourceBuffer, sourceIndex, array, arrayIndex, copyCount)
        arrayIndex <- arrayIndex + copyCount
        count <- count - copyCount
        copyCount
        
      let mutable row = position.row
      let col = position.column
      let mutable buf = __.GetBuffer row
      let mutable copied = copyToCore(buf, col)

      if count = 0 then
        let p = { row = row; column = col + copied }
        p.Normalize(buf.Length)
      else
        buf <- __.GetBuffer row
        row <- row + 1
        copied <- copyToCore(buf, 0)
        
        while 0 < count do 
          buf <- __.GetBuffer row
          row <- row + 1
          copied <- copyToCore(buf, 0)
        let p = { row = row; column = copied }
        p.Normalize(buf.Length)

    member __.GetBuffer (index: int) : array<'T> =
      if index = 0 then
        __.first'
      else if index <= __.buffers'.Count then
        __.buffers'[index-1]
      else
        __.current'

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member __.SlowAdd(item: 'T) = __.Add(item)

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L266
    member __.ToArray() =
      let mutable ary = Unchecked.defaultof<array<'T>>
      if __.TryMove &ary then
        ary
      else
        ary <- Array.zeroCreate<'T> __.count'
        __.CopyTo(ary, 0, __.count')
        ary

    member __.TryMove(array: outref<array<'T>>) =
      array <- __.first'
      __.count' = __.first'.Length

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member private __.AddWithBufferAllocation (item: 'T) =
      __.AllocateBuffer ()
      __.current'[__.index'] <- item
      __.index' <- __.index' + 1
      
    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L141
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member private __.AddWithBufferAllocation (item: 'T, destination: byref<array<'T>>, index: byref<int>) =
      __.count' <- __.count' + (index - __.index')
      __.index' <- index
      __.AllocateBuffer()
      destination <- __.current'
      index <- __.index'
      __.current'[index] <- item

    // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs#L290
    member private __.AllocateBuffer () =      
      if uint __.count' < uint ResizeLimit then
        let nextCapacity = min (if __.count' = 0 then StartingCapacity else 2 * __.count') __.maxCapacity'
        __.current' <- create<'T> nextCapacity
        System.Array.Copy(__.first', 0, __.current', 0, __.count')
        __.first' <- __.current'
      else
        let nextCapacity =
          if __.count' = ResizeLimit then ResizeLimit
          else __.buffers'.Add __.current'; min __.count' (__.maxCapacity' - __.count')
        __.current' <- create<'T> nextCapacity
        __.index' <- 0

  /// <summary>
  /// 
  /// </summary>
  // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/SparseArrayBuilder.cs#L13
  [<Struct; IsReadOnly; DebuggerDisplay("{DebuggerDisplay, nq}")>]
  type Maker = { count: int; index: int }
  with member __.DebuggerDisplay with get() = $"index: {__.index}, count: {__.count}"

  /// <summary>
  /// TODO: 
  /// </summary>
  // src: https://github.com/JonHanna/corefx/blob/master/src/Common/src/System/Collections/Generic/SparseArrayBuilder.cs#L49
  [<Struct;NoComparison;NoEquality;>]
  type SparseArrayBuilder<'T> = {
    mutable builder : LargeArrayBuilder<'T>
    mutable makers: ArrayBuilder<Maker>
    mutable reservedCount: int
  }
  with
    static member Create() = { builder = LargeArrayBuilder<'T>(System.Int32.MaxValue); makers = ArrayBuilder<Maker>(); reservedCount = 0 }
    member __.Count with get() = __.builder.Count + __.reservedCount
    member __.Add (item: 'T) = __.builder.Add(item)
    member __.AddRange (items: seq<'T>) = __.builder.AddRange(items)
    member __.CopyTo (array: array<'T>, arrayIndex: int, count: int) =
      let mutable arrayIndex = arrayIndex
      let mutable count = count
      let mutable copied = 0
      let mutable position = CopyPosition.Start

      for i = 0 to __.makers.Count - 1 do
        let maker = __.makers[i]
        let toCopy = min (maker.index - copied ) count
        if 0 < toCopy then
          position <- __.builder.CopyTo(position, array, arrayIndex, toCopy)
          arrayIndex <- arrayIndex + toCopy
          copied <- copied + toCopy
          count <- count - toCopy

        if count = 0 then
          ()
        else
          let reservedCount = min maker.count count
          arrayIndex <- arrayIndex + reservedCount
          copied <- copied + reservedCount
          count <- count - reservedCount
      
      if 0 < count then
        __.builder.CopyTo(position, array, arrayIndex, count) |> ignore

    member __.Reserve (count: int) =
      __.makers.Add { count = count; index = __.Count }
      __.reservedCount <- Checked.(+) __.reservedCount count

    member __.ReserveOrAdd (items: seq<'T>) =
      let mutable itemCount = 0
      if Funtom.Linq.Enumerable.tryGetCount<'T>(items, &itemCount) then
        if 0 < itemCount then __.Reserve(itemCount); true
        else false
      else
        __.AddRange(items)
        false

