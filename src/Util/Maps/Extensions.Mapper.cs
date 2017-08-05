﻿using System;
using AutoMapper;
using System.Collections.Generic;
using System.Reflection;

namespace Util.Maps {
    /// <summary>
    /// 对象映射
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// 将源对象映射到目标对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        public static TDestination MapTo<TSource, TDestination>( this TSource source, TDestination destination ) {
            return MapTo<TDestination>( source, destination );
        }

        /// <summary>
        /// 将源对象映射到目标对象
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        public static TDestination MapTo<TDestination>( this object source ) where TDestination : new() {
            return MapTo( source, new TDestination() );
        }

        /// <summary>
        /// 将源对象映射到目标对象
        /// </summary>
        private static TDestination MapTo<TDestination>( object source, TDestination destination ) {
            if( source == null )
                throw new ArgumentNullException( nameof( source ) );
            if( destination == null )
                throw new ArgumentNullException( nameof( destination ) );
            var sourceType = GetType( source );
            var destinationType = GetType( destination );
            try {
                var map = Mapper.Configuration.FindTypeMapFor( sourceType, destinationType );
                if( map != null )
                    return Mapper.Map( source, destination );
                var maps = Mapper.Configuration.GetAllTypeMaps();
                Mapper.Initialize( config => {
                    foreach( var item in maps )
                        config.CreateMap( item.SourceType, item.DestinationType );
                    config.CreateMap( sourceType, destinationType );
                } );
            }
            catch( InvalidOperationException ) {
                Mapper.Initialize( config => {
                    config.CreateMap( sourceType, destinationType );
                } );
            }
            return Mapper.Map( source, destination );
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        private static Type GetType( object obj ) {
            var type = obj.GetType();
            if( type.IsArray )
                return type.GetElementType();
            if( ( obj is System.Collections.IEnumerable ) == false )
                return type;
            var genericArgumentsTypes = type.GetTypeInfo().GetGenericArguments();
            if( genericArgumentsTypes == null || genericArgumentsTypes.Length == 0 )
                throw new ArgumentException( "泛型类型参数不能为空" );
            return genericArgumentsTypes[0];
        }

        /// <summary>
        /// 将源集合映射到目标集合
        /// </summary>
        /// <typeparam name="TDestination">目标元素类型,范例：Sample,不要加List</typeparam>
        /// <param name="source">源集合</param>
        public static List<TDestination> MapToList<TDestination>( this System.Collections.IEnumerable source ) {
            return MapTo<List<TDestination>>( source );
        }
    }
}