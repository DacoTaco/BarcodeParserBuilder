﻿using System;
using System.Collections.Generic;

namespace BarcodeParserBuilder.Abstraction
{
    internal interface IAimParser
    {
        public IEnumerable<Type> GetParsers(string barcodeString);
        public string StripPrefix(string barcodeString);
    }
}