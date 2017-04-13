﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Draw2D.Wpf.Utilities
{
    public class NewtonsoftJsonSerializer
    {
        public class ProjectContractResolver : DefaultContractResolver
        {
            public override JsonContract ResolveContract(Type type)
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return base
                        .ResolveContract(typeof(ObservableCollection<>)
                        .MakeGenericType(type.GenericTypeArguments[0]));
                }
                return base.ResolveContract(type);
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
            }
        }

        public class DictionarySerializationBinder : DefaultSerializationBinder
        {
            private readonly IDictionary<Type, string> _typeToName = new Dictionary<Type, string>();
            private readonly IDictionary<string, Type> _nameToType = new Dictionary<string, Type>();

            public DictionarySerializationBinder Add(Type type, string name = null)
            {
                if (name == null)
                {
                    var attr = type.GetCustomAttribute<JsonObjectAttribute>();
                    if (attr == null || attr.Id == null)
                        throw new ArgumentException($"Name for type '{type}' not specified. Name must be specified either directly or by JsonObjectAttribute.Id.", nameof(name));
                    name = attr.Id;
                }
                _typeToName[type] = name;
                _nameToType[name] = type;
                return this;
            }

            public DictionarySerializationBinder Add<T>(string name = null)
            {
                return Add(typeof(T), name);
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    throw new InvalidOperationException($"Type '{typeName}' (assembly={assemblyName}) not defined. Using types from arbitrary assemblies not allowed.");
                }
                try
                {
                    return _nameToType[typeName];
                }
                catch (KeyNotFoundException e)
                {
                    throw new KeyNotFoundException($"Type '{typeName}' not defined.", e);
                }
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = _typeToName.ContainsKey(serializedType) ? _typeToName[serializedType] : null;
            }
        }

        private static readonly JsonSerializerSettings Settings;

        static NewtonsoftJsonSerializer()
        {
            var binder = new DictionarySerializationBinder();

            binder.Add<MatrixObject>("Matrix");
            binder.Add<DrawColor>("Color");
            binder.Add<DrawStyle>("Style");
            binder.Add<PointShape>("Point");
            binder.Add<LineShape>("Line");
            binder.Add<CubicBezierShape>("Cubic");
            binder.Add<QuadraticBezierShape>("Quad");
            binder.Add<PathShape>("Path");
            binder.Add<FigureShape>("Figure");
            binder.Add<ScribbleShape>("Scribble");
            binder.Add<RectangleShape>("Rect");
            binder.Add<EllipseShape>("Ellipse");
            binder.Add<GroupShape>("Group");
            binder.Add<ShapesContainer>("Container");

            Settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                SerializationBinder = binder,
                Converters = { new KeyValuePairConverter() }
            };
        }

        public static string ToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}