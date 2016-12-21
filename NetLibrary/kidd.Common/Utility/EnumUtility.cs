using System;
using System.Collections.Generic;
using System.Reflection;

namespace kidd.Common.Utility
{
    /// <summary>
    /// EnumHelper 的摘要描述
    /// </summary>
    public class EnumUtility
    {
        /// <summary>
        /// 將指定列舉轉換成集合列舉型別
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IEnumerable<EnumItem> GetEnumList<TEnum>() where TEnum : struct, IConvertible
        {
            if (typeof(TEnum).IsEnum)
            {

                foreach (var value in Enum.GetValues(typeof(TEnum)))
                {
                    string text = GetEnumDescription<TEnum>(value.ToString());
                    yield return new EnumItem() { Text = text, Value = (int)value };
                }
            }
            else
            {
                throw new ArgumentException("T 必須是列舉 Enum 型別");
            }
        }


        /// <summary>
        /// 取得列舉描述，若有使用 EnumDescription 自訂描述，則會顯示自訂描述文字
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(string value) where TEnum : struct, IConvertible
        {
            if (typeof(TEnum).IsEnum)
            {
                var e = Enum.Parse(typeof(TEnum), value);

                FieldInfo field = e.GetType().GetField(e.ToString());
                object[] attribs = field.GetCustomAttributes(typeof(EnumDescription), true);
                if (attribs != null && attribs.Length > 0)
                {
                    return ((EnumDescription)attribs[0]).Text;
                }

                return e.ToString();
            }
            else
            {
                throw new ArgumentException("T 必須是列舉 Enum 型別");
            }


        }
    }

    /// <summary>
    /// 自訂列舉描述屬性類別
    /// </summary>
    public class EnumDescription : Attribute
    {
        public string Text;
        public EnumDescription(string text)
        {
            Text = text;

        }
    }

    /// <summary>
    /// 列舉項目
    /// </summary>
    public class EnumItem
    {
        public String Text { get; set; }
        public int Value { get; set; }
    }

}

