﻿using Entities.Models;
using Services.Contract;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DataShaper<T> : IDataShaper<T>
        where T : class
    {   
        public PropertyInfo[] Propertiees { get; set; }
        public DataShaper()
        {
            Propertiees=typeof(T).GetProperties(BindingFlags.Public|BindingFlags.Instance);
        }
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            var requiredFields=GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredFields);
        }

        public ShapedEntity ShapeData(T entity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(entity, requiredProperties);
        }
        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredField = new List<PropertyInfo>();
            if (!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields = fieldsString.Split(',',StringSplitOptions.RemoveEmptyEntries);
                foreach (var field in fields)
                {
                    var property = Propertiees
                        .FirstOrDefault(pi => pi.Name.Equals(field.Trim(),
                        StringComparison.InvariantCultureIgnoreCase));
                    if (property is null)
                    {
                        continue;
                    }
                    requiredField.Add(property);
                }
            }
            else
            {
                requiredField=Propertiees.ToList();
            }
            return requiredField;
        }
        private ShapedEntity FetchDataForEntity(T entity,IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject=new ShapedEntity();
            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            var objectProperty = entity.GetType().GetProperty("Id");
            shapedObject.Id = (int)objectProperty.GetValue(entity);

            return shapedObject;
        }
        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities,IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData=new List<ShapedEntity>();
            foreach (var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }
    }
}
