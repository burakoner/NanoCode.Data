﻿using Nanocode.Data.Attributes;
using System;
using System.Collections.Generic;

namespace Nanocode.Data.Converters
{
    public class EnumLabelConverter<T> : BaseConverter<T> where T : struct
    {
        public EnumLabelConverter() : this(true) { }
        public EnumLabelConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<T, string>> Mapping
        {
            get
            {
                var kvp = new List<KeyValuePair<T, string>>();
                foreach (T val in Enum.GetValues(typeof(T)))
                {
                    kvp.Add(new KeyValuePair<T, string>(val, (val as Enum).GetLabel()));
                }

                return kvp;
            }
        }
    }
}