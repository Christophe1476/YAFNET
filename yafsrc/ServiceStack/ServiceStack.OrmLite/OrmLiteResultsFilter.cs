﻿// ***********************************************************************
// <copyright file="OrmLiteResultsFilter.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ServiceStack.OrmLite;

using ServiceStack.Text;

/// <summary>
/// Interface IOrmLiteResultsFilter
/// </summary>
public interface IOrmLiteResultsFilter
{
    /// <summary>
    /// Gets the last insert identifier.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    long GetLastInsertId(IDbCommand dbCmd);

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    List<T> GetList<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the reference list.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>IList.</returns>
    IList GetRefList(IDbCommand dbCmd, Type refType);

    /// <summary>
    /// Gets the single.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    T GetSingle<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the reference single.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.Object.</returns>
    object GetRefSingle(IDbCommand dbCmd, Type refType);

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    T GetScalar<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    object GetScalar(IDbCommand dbCmd);

    /// <summary>
    /// Gets the long scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    long GetLongScalar(IDbCommand dbCmd);

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    List<T> GetColumn<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the column distinct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    HashSet<T> GetColumnDistinct<T>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the dictionary.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, V&gt;.</returns>
    Dictionary<K, V> GetDictionary<K, V>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the key value pairs.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
    List<KeyValuePair<K, V>> GetKeyValuePairs<K, V>(IDbCommand dbCmd);

    /// <summary>
    /// Gets the lookup.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
    Dictionary<K, List<V>> GetLookup<K, V>(IDbCommand dbCmd);

    /// <summary>
    /// Executes the SQL.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int32.</returns>
    int ExecuteSql(IDbCommand dbCmd);
}

/// <summary>
/// Class OrmLiteResultsFilter.
/// Implements the <see cref="ServiceStack.OrmLite.IOrmLiteResultsFilter" />
/// Implements the <see cref="System.IDisposable" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.IOrmLiteResultsFilter" />
/// <seealso cref="System.IDisposable" />
public class OrmLiteResultsFilter : IOrmLiteResultsFilter, IDisposable
{
    /// <summary>
    /// Gets or sets the results.
    /// </summary>
    /// <value>The results.</value>
    public IEnumerable Results { get; set; }
    /// <summary>
    /// Gets or sets the reference results.
    /// </summary>
    /// <value>The reference results.</value>
    public IEnumerable RefResults { get; set; }
    /// <summary>
    /// Gets or sets the column results.
    /// </summary>
    /// <value>The column results.</value>
    public IEnumerable ColumnResults { get; set; }
    /// <summary>
    /// Gets or sets the column distinct results.
    /// </summary>
    /// <value>The column distinct results.</value>
    public IEnumerable ColumnDistinctResults { get; set; }
    /// <summary>
    /// Gets or sets the dictionary results.
    /// </summary>
    /// <value>The dictionary results.</value>
    public IDictionary DictionaryResults { get; set; }
    /// <summary>
    /// Gets or sets the lookup results.
    /// </summary>
    /// <value>The lookup results.</value>
    public IDictionary LookupResults { get; set; }
    /// <summary>
    /// Gets or sets the single result.
    /// </summary>
    /// <value>The single result.</value>
    public object SingleResult { get; set; }
    /// <summary>
    /// Gets or sets the reference single result.
    /// </summary>
    /// <value>The reference single result.</value>
    public object RefSingleResult { get; set; }
    /// <summary>
    /// Gets or sets the scalar result.
    /// </summary>
    /// <value>The scalar result.</value>
    public object ScalarResult { get; set; }
    /// <summary>
    /// Gets or sets the long scalar result.
    /// </summary>
    /// <value>The long scalar result.</value>
    public long LongScalarResult { get; set; }
    /// <summary>
    /// Gets or sets the last insert identifier.
    /// </summary>
    /// <value>The last insert identifier.</value>
    public long LastInsertId { get; set; }
    /// <summary>
    /// Gets or sets the execute SQL result.
    /// </summary>
    /// <value>The execute SQL result.</value>
    public int ExecuteSqlResult { get; set; }

    /// <summary>
    /// Gets or sets the execute SQL function.
    /// </summary>
    /// <value>The execute SQL function.</value>
    public Func<IDbCommand, int> ExecuteSqlFn { get; set; }
    /// <summary>
    /// Gets or sets the results function.
    /// </summary>
    /// <value>The results function.</value>
    public Func<IDbCommand, Type, IEnumerable> ResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the reference results function.
    /// </summary>
    /// <value>The reference results function.</value>
    public Func<IDbCommand, Type, IEnumerable> RefResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the column results function.
    /// </summary>
    /// <value>The column results function.</value>
    public Func<IDbCommand, Type, IEnumerable> ColumnResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the column distinct results function.
    /// </summary>
    /// <value>The column distinct results function.</value>
    public Func<IDbCommand, Type, IEnumerable> ColumnDistinctResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the dictionary results function.
    /// </summary>
    /// <value>The dictionary results function.</value>
    public Func<IDbCommand, Type, Type, IDictionary> DictionaryResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the lookup results function.
    /// </summary>
    /// <value>The lookup results function.</value>
    public Func<IDbCommand, Type, Type, IDictionary> LookupResultsFn { get; set; }
    /// <summary>
    /// Gets or sets the single result function.
    /// </summary>
    /// <value>The single result function.</value>
    public Func<IDbCommand, Type, object> SingleResultFn { get; set; }
    /// <summary>
    /// Gets or sets the reference single result function.
    /// </summary>
    /// <value>The reference single result function.</value>
    public Func<IDbCommand, Type, object> RefSingleResultFn { get; set; }
    /// <summary>
    /// Gets or sets the scalar result function.
    /// </summary>
    /// <value>The scalar result function.</value>
    public Func<IDbCommand, Type, object> ScalarResultFn { get; set; }
    /// <summary>
    /// Gets or sets the long scalar result function.
    /// </summary>
    /// <value>The long scalar result function.</value>
    public Func<IDbCommand, long> LongScalarResultFn { get; set; }
    /// <summary>
    /// Gets or sets the last insert identifier function.
    /// </summary>
    /// <value>The last insert identifier function.</value>
    public Func<IDbCommand, long> LastInsertIdFn { get; set; }

    /// <summary>
    /// Gets or sets the SQL filter.
    /// </summary>
    /// <value>The SQL filter.</value>
    public Action<string> SqlFilter { get; set; }
    /// <summary>
    /// Gets or sets the SQL command filter.
    /// </summary>
    /// <value>The SQL command filter.</value>
    public Action<IDbCommand> SqlCommandFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [print SQL].
    /// </summary>
    /// <value><c>true</c> if [print SQL]; otherwise, <c>false</c>.</value>
    public bool PrintSql { get; set; }

    /// <summary>
    /// The previous filter
    /// </summary>
    private readonly IOrmLiteResultsFilter previousFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrmLiteResultsFilter"/> class.
    /// </summary>
    /// <param name="results">The results.</param>
    public OrmLiteResultsFilter(IEnumerable results = null)
    {
        this.Results = results ?? new object[] { };

        previousFilter = OrmLiteConfig.ResultsFilter;
        OrmLiteConfig.ResultsFilter = this;
    }

    /// <summary>
    /// Filters the specified database command.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    private void Filter(IDbCommand dbCmd)
    {
        SqlFilter?.Invoke(dbCmd.CommandText);

        SqlCommandFilter?.Invoke(dbCmd);

        if (PrintSql)
        {
            Console.WriteLine(dbCmd.CommandText);
        }
    }

    /// <summary>
    /// Gets the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetResults<T>(IDbCommand dbCmd)
    {
        return ResultsFn != null ? ResultsFn(dbCmd, typeof(T)) : Results;
    }

    /// <summary>
    /// Gets the reference results.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetRefResults(IDbCommand dbCmd, Type refType)
    {
        return RefResultsFn != null ? RefResultsFn(dbCmd, refType) : RefResults;
    }

    /// <summary>
    /// Gets the column results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetColumnResults<T>(IDbCommand dbCmd)
    {
        return ColumnResultsFn != null ? ColumnResultsFn(dbCmd, typeof(T)) : ColumnResults;
    }

    /// <summary>
    /// Gets the column distinct results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IEnumerable.</returns>
    private IEnumerable GetColumnDistinctResults<T>(IDbCommand dbCmd)
    {
        return ColumnDistinctResultsFn != null ? ColumnDistinctResultsFn(dbCmd, typeof(T)) : ColumnDistinctResults;
    }

    /// <summary>
    /// Gets the dictionary results.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IDictionary.</returns>
    private IDictionary GetDictionaryResults<K, V>(IDbCommand dbCmd)
    {
        return DictionaryResultsFn != null ? DictionaryResultsFn(dbCmd, typeof(K), typeof(V)) : DictionaryResults;
    }

    /// <summary>
    /// Gets the lookup results.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>IDictionary.</returns>
    private IDictionary GetLookupResults<K, V>(IDbCommand dbCmd)
    {
        return LookupResultsFn != null ? LookupResultsFn(dbCmd, typeof(K), typeof(V)) : LookupResults;
    }

    /// <summary>
    /// Gets the single result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    private object GetSingleResult<T>(IDbCommand dbCmd)
    {
        return SingleResultFn != null ? SingleResultFn(dbCmd, typeof(T)) : SingleResult;
    }

    /// <summary>
    /// Gets the reference single result.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.Object.</returns>
    private object GetRefSingleResult(IDbCommand dbCmd, Type refType)
    {
        return RefSingleResultFn != null ? RefSingleResultFn(dbCmd, refType) : RefSingleResult;
    }

    /// <summary>
    /// Gets the scalar result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    private object GetScalarResult<T>(IDbCommand dbCmd)
    {
        return ScalarResultFn != null ? ScalarResultFn(dbCmd, typeof(T)) : ScalarResult;
    }

    /// <summary>
    /// Gets the long scalar result.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    private long GetLongScalarResult(IDbCommand dbCmd)
    {
        return LongScalarResultFn?.Invoke(dbCmd) ?? LongScalarResult;
    }

    /// <summary>
    /// Gets the last insert identifier.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    public long GetLastInsertId(IDbCommand dbCmd)
    {
        return LastInsertIdFn?.Invoke(dbCmd) ?? LastInsertId;
    }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public List<T> GetList<T>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        return (from object result in GetResults<T>(dbCmd) select (T)result).ToList();
    }

    /// <summary>
    /// Gets the reference list.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>IList.</returns>
    public IList GetRefList(IDbCommand dbCmd, Type refType)
    {
        Filter(dbCmd);
        var list = (IList)typeof(List<>).GetCachedGenericType(refType).CreateInstance();
        foreach (object result in GetRefResults(dbCmd, refType).Safe())
        {
            list.Add(result);
        }
        return list;
    }

    /// <summary>
    /// Gets the single.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    public T GetSingle<T>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        if (SingleResult != null || SingleResultFn != null)
            return (T)GetSingleResult<T>(dbCmd);

        foreach (var result in GetResults<T>(dbCmd))
        {
            return (T)result;
        }
        return default(T);
    }

    /// <summary>
    /// Gets the reference single.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <param name="refType">Type of the reference.</param>
    /// <returns>System.Object.</returns>
    public object GetRefSingle(IDbCommand dbCmd, Type refType)
    {
        Filter(dbCmd);
        if (RefSingleResult != null || RefSingleResultFn != null)
            return GetRefSingleResult(dbCmd, refType);

        foreach (var result in GetRefResults(dbCmd, refType).Safe())
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>T.</returns>
    public T GetScalar<T>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        return ConvertTo<T>(GetScalarResult<T>(dbCmd));
    }

    /// <summary>
    /// Gets the long scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int64.</returns>
    public long GetLongScalar(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        return GetLongScalarResult(dbCmd);
    }

    /// <summary>
    /// Converts to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    private T ConvertTo<T>(object value)
    {
        if (value == null)
            return default(T);

        if (value is T)
            return (T)value;

        var typeCode = typeof(T).GetUnderlyingTypeCode();
        var strValue = value.ToString();
        switch (typeCode)
        {
            case TypeCode.Boolean:
                return (T)(object)Convert.ToBoolean(strValue);
            case TypeCode.Byte:
                return (T)(object)Convert.ToByte(strValue);
            case TypeCode.Int16:
                return (T)(object)Convert.ToInt16(strValue);
            case TypeCode.Int32:
                return (T)(object)Convert.ToInt32(strValue);
            case TypeCode.Int64:
                return (T)(object)Convert.ToInt64(strValue);
            case TypeCode.Single:
                return (T)(object)Convert.ToSingle(strValue);
            case TypeCode.Double:
                return (T)(object)Convert.ToDouble(strValue);
            case TypeCode.Decimal:
                return (T)(object)Convert.ToDecimal(strValue);
        }

        return (T)value;
    }

    /// <summary>
    /// Gets the scalar.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Object.</returns>
    public object GetScalar(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        return GetScalarResult<object>(dbCmd) ?? GetResults<object>(dbCmd).Cast<object>().FirstOrDefault();
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;T&gt;.</returns>
    public List<T> GetColumn<T>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        return (from object result in GetColumnResults<T>(dbCmd).Safe() select (T)result).ToList();
    }

    /// <summary>
    /// Gets the column distinct.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>HashSet&lt;T&gt;.</returns>
    public HashSet<T> GetColumnDistinct<T>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        var results = GetColumnDistinctResults<T>(dbCmd) ?? GetColumnResults<T>(dbCmd);
        return (from object result in results select (T)result).ToSet();
    }

    /// <summary>
    /// Gets the dictionary.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, V&gt;.</returns>
    public Dictionary<K, V> GetDictionary<K, V>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        var to = new Dictionary<K, V>();
        var map = GetDictionaryResults<K, V>(dbCmd);
        if (map == null)
            return to;

        foreach (DictionaryEntry entry in map)
        {
            to.Add((K)entry.Key, (V)entry.Value);
        }

        return to;
    }

    /// <summary>
    /// Gets the key value pairs.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>List&lt;KeyValuePair&lt;K, V&gt;&gt;.</returns>
    public List<KeyValuePair<K, V>> GetKeyValuePairs<K, V>(IDbCommand dbCmd) => GetDictionary<K, V>(dbCmd).ToList();

    /// <summary>
    /// Gets the lookup.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>Dictionary&lt;K, List&lt;V&gt;&gt;.</returns>
    public Dictionary<K, List<V>> GetLookup<K, V>(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        var to = new Dictionary<K, List<V>>();
        var map = GetLookupResults<K, V>(dbCmd);
        if (map == null)
            return to;

        foreach (DictionaryEntry entry in map)
        {
            var key = (K)entry.Key;

            if (!to.TryGetValue(key, out var list))
            {
                to[key] = list = new List<V>();
            }

            list.AddRange(from object item in (IEnumerable)entry.Value select (V)item);
        }

        return to;
    }

    /// <summary>
    /// Executes the SQL.
    /// </summary>
    /// <param name="dbCmd">The database command.</param>
    /// <returns>System.Int32.</returns>
    public int ExecuteSql(IDbCommand dbCmd)
    {
        Filter(dbCmd);
        return ExecuteSqlFn?.Invoke(dbCmd)
               ?? ExecuteSqlResult;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        OrmLiteConfig.ResultsFilter = previousFilter;
    }
}

/// <summary>
/// Class CaptureSqlFilter.
/// Implements the <see cref="ServiceStack.OrmLite.OrmLiteResultsFilter" />
/// </summary>
/// <seealso cref="ServiceStack.OrmLite.OrmLiteResultsFilter" />
public class CaptureSqlFilter : OrmLiteResultsFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CaptureSqlFilter"/> class.
    /// </summary>
    public CaptureSqlFilter()
    {
        SqlCommandFilter = CaptureSqlCommand;
        SqlCommandHistory = new List<SqlCommandDetails>();
    }

    /// <summary>
    /// Captures the SQL command.
    /// </summary>
    /// <param name="command">The command.</param>
    private void CaptureSqlCommand(IDbCommand command)
    {
        SqlCommandHistory.Add(new SqlCommandDetails(command));
    }

    /// <summary>
    /// Gets or sets the SQL command history.
    /// </summary>
    /// <value>The SQL command history.</value>
    public List<SqlCommandDetails> SqlCommandHistory { get; set; }

    /// <summary>
    /// Gets the SQL statements.
    /// </summary>
    /// <value>The SQL statements.</value>
    public List<string> SqlStatements
    {
        get { return SqlCommandHistory.Map(x => x.Sql); }
    }
}

/// <summary>
/// Class SqlCommandDetails.
/// </summary>
public class SqlCommandDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCommandDetails"/> class.
    /// </summary>
    /// <param name="command">The command.</param>
    public SqlCommandDetails(IDbCommand command)
    {
        if (command == null)
            return;

        Sql = command.CommandText;
        if (command.Parameters.Count <= 0)
            return;

        Parameters = new Dictionary<string, object>();

        foreach (IDataParameter parameter in command.Parameters)
        {
            if (!Parameters.ContainsKey(parameter.ParameterName))
                Parameters.Add(parameter.ParameterName, parameter.Value);
        }
    }

    /// <summary>
    /// Gets or sets the SQL.
    /// </summary>
    /// <value>The SQL.</value>
    public string Sql { get; set; }
    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    /// <value>The parameters.</value>
    public Dictionary<string, object> Parameters { get; set; }
}