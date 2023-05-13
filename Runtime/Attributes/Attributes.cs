using System;
using UnityEngine;

namespace NBC.ActionEditor
{
    /// <summary>
    /// 类排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderAttribute : Attribute
    {
        public int Order;

        public OrderAttribute(int order)
        {
            this.Order = order;
        }
    }

    /// <summary>
    /// 菜单自定义名称
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class MenuNameAttribute : Attribute
    {
        public MenuNameAttribute(string name)
        {
            showName = name;
        }

        public string showName;
    }

    /// <summary>
    /// 关联某个类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionParamAttribute : Attribute
    {
        public Type classType;

        public OptionParamAttribute(Type type)
        {
            classType = type;
        }
    }

    /// <summary>
    /// 选项排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionSortAttribute : Attribute
    {
        public int sort;

        public OptionSortAttribute(int sort)
        {
            this.sort = sort;
        }
    }

    /// <summary>
    /// 关联某个字段的某个值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionRelateParamAttribute : Attribute
    {
        public string argsName;
        public object[] argsValue;

        public OptionRelateParamAttribute(string name, params object[] values)
        {
            argsName = name;
            argsValue = values;
        }
    }

    /// <summary>
    /// 选择对象路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectObjectPathAttribute : Attribute
    {
        public Type type;

        public SelectObjectPathAttribute(Type type)
        {
            this.type = type;
        }
    }

    /// <summary>
    /// 自定义检视面板
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInspectors : Attribute
    {
        public Type _inspectedType;
        public bool _editorForChildClasses;

        public CustomInspectors(Type inspectedType)
        {
            _inspectedType = inspectedType;
        }

        public CustomInspectors(Type inspectedType, bool editorForChildClasses)
        {
            _inspectedType = inspectedType;
            _editorForChildClasses = editorForChildClasses;
        }
    }

    /// <summary>
    /// 自定义名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public readonly string name;

        public NameAttribute(string name)
        {
            this.name = name;
        }
    }


    /// <summary>
    /// 指定类别
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute : Attribute
    {
        public readonly string category;

        public CategoryAttribute(string category)
        {
            this.category = category;
        }
    }

    /// <summary>
    /// 指定描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public readonly string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }

    /// <summary>
    /// 指定类型的图标
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ShowIconAttribute : Attribute
    {
        public readonly string iconPath;
        public readonly Type fromType;
        public readonly Texture texture;

        public ShowIconAttribute(Texture texture)
        {
            this.texture = texture;
        }

        public ShowIconAttribute(string iconPath)
        {
            this.iconPath = iconPath;
        }

        public ShowIconAttribute(Type fromType)
        {
            this.fromType = fromType;
        }
    }

    /// <summary>
    /// 指定显示的颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ColorAttribute : Attribute
    {
        public readonly Color Color;

        public ColorAttribute(float r, float g, float b, float a = 1)
        {
            this.Color = new Color(r, g, b, a);
        }

        public ColorAttribute(Color color)
        {
            this.Color = color;
        }
    }

    /// <summary>
    /// 指定附加类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AttachableAttribute : Attribute
    {
        public readonly Type[] Types;

        public AttachableAttribute(params Type[] types)
        {
            this.Types = types;
        }
    }

    /// <summary>
    /// 组内唯一性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueAttribute : Attribute
    {
    }

    /// <summary>
    /// 自定义片段预览
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPreviewAttribute : Attribute
    {
        public Type PreviewType;

        public CustomPreviewAttribute(Type type)
        {
            PreviewType = type;
        }
    }
}