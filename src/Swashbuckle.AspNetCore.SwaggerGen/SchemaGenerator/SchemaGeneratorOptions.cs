﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public class SchemaGeneratorOptions
    {
        public SchemaGeneratorOptions()
        {
            CustomTypeMappings = new Dictionary<Type, Func<OpenApiSchema>>();
            SchemaIdSelector = DefaultSchemaIdSelector;
            SubTypesResolver = DefaultSubTypesResolver;
            DiscriminatorSelector = DefaultDiscriminatorSelector;
            SchemaFilters = new List<ISchemaFilter>();
        }

        public IDictionary<Type, Func<OpenApiSchema>> CustomTypeMappings { get; set; }

        public Func<Type, string> SchemaIdSelector { get; set; }

        public bool IgnoreObsoleteProperties { get; set; }

        public bool GeneratePolymorphicSchemas { get; set; }

        public Func<Type, IEnumerable<Type>> SubTypesResolver { get; set; }

        public Func<Type, string> DiscriminatorSelector { get; set; }

        public bool UseAllOfToExtendReferenceSchemas { get; set; }

        public bool UseInlineDefinitionsForEnums { get; set; }

        public IList<ISchemaFilter> SchemaFilters { get; set; }

        [Obsolete("If the serializer is configured for string enums (e.g. StringEnumConverter) Swashbuckle will reflect that automatically")]
        public bool DescribeAllEnumsAsStrings { get; set; }

        [Obsolete("If the serializer is configured for (camel-cased) string enums (e.g. StringEnumConverter) Swashbuckle will reflect that automatically")]
        public bool DescribeStringEnumsInCamelCase { get; set; }

        private string DefaultSchemaIdSelector(Type modelType)
        {
            if (!modelType.IsConstructedGenericType) return modelType.Name.Replace("[]", "Array");

            var prefix = modelType.GetGenericArguments()
                .Select(genericArg => DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + modelType.Name.Split('`').First();
        }

        private IEnumerable<Type> DefaultSubTypesResolver(Type baseType)
        {
            if (baseType == typeof(object))
                return Enumerable.Empty<Type>();

            return baseType.Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType));
        }

        private string DefaultDiscriminatorSelector(Type baseType)
        {
            return "$type";
        }
    }
}