using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using AspNetInsight;
using AspNetInsight.Dto;

namespace AspNetInsight.Tests
{
    /// <summary>
    /// unit tests for ResponseBanner class
    /// </summary>
    public class ResponseBannerTests
    {
        [Fact]
        public void GeneratedBanner_with_empty_template()
        {
            // arrange
            string tt = null;
            TempBanner banner = new TempBanner() { template = tt };
            TempBanner banner_ws = new TempBanner() { template = "   " };
            BasicSts data = new BasicSts(20);

            // act and assert
            Assert.Throws<ArgumentNullException>(() => banner.GeneratedBanner(data));
            Assert.Throws<ArgumentNullException>(() => banner_ws.GeneratedBanner(data));
        }

        [Fact]
        public void GeneratedBanner_with_defualt_prefix_suffix()
        {
            // arrange
            var tt = "[RECENT]-[MIN]-[AVG]-[MAX]-[TOTAL]";
            TempBanner banner = new TempBanner() { template = tt };
            BasicSts data = new BasicSts(20);

            // act
            var rslt = banner.GeneratedBanner(data);

            // assert
            Assert.True(banner.TemplateText == tt);
            Assert.True(!string.IsNullOrWhiteSpace(rslt));
            Assert.True(rslt == "20-20-20-20-1");
            Assert.True(banner.Prefix == "[");
            Assert.True(banner.Suffix == "]");
        }

        [Fact]
        public void GeneratedBanner_with_invalid_template()
        {
            // arrange
            var tt = "[RECENT1]-[MIN1]-[AVG2]-[MAX23]-[TOTAL5]";
            TempBanner banner = new TempBanner() { template = tt };
            BasicSts data = new BasicSts(20);

            // act
            var rslt = banner.GeneratedBanner(data);

            // assert
            Assert.True(banner.TemplateText == tt);
            Assert.True(!string.IsNullOrWhiteSpace(rslt));
            Assert.True(rslt == tt);
            Assert.True(banner.Prefix == "[");
            Assert.True(banner.Suffix == "]");
        }

        [Fact]
        public void GeneratedBanner_for_decimal_with_defualt_prefix_suffix()
        {
            // arrange
            var tt = "[RECENT]-[AVG]-[MAX]-[TOTAL]-[MIN]";
            TempBanner banner = new TempBanner() { template = tt };
            BasicSts data = new BasicSts()
            {
                Total = 5,
                Min = 0.2345,
                Max = 20.2332,
                Avg = 5.2376,
                Recent = 12.2397
            };
            
            // act
            var rslt = banner.GeneratedBanner(data);

            // assert
            Assert.True(banner.TemplateText == tt);
            Assert.True(!string.IsNullOrWhiteSpace(rslt));
            Assert.True(rslt == "12.24-5.24-20.23-5-0.23");
            Assert.True(banner.Prefix == "[");
            Assert.True(banner.Suffix == "]");
        }

        [Fact]
        public void GeneratedBanner_for_decimal_with_custom_prefix_suffix()
        {
            // arrange
            var tt = "{RECENT}-{AVG}-{MAX}-{TOTAL}-{MIN}";
            TempBanner banner = new TempBanner("{", "}") { template = tt };
            BasicSts data = new BasicSts()
            {
                Total = 5,
                Min = 0.2345,
                Max = 20.2332,
                Avg = 5.2376,
                Recent = 12.2397
            };

            // act
            var rslt = banner.GeneratedBanner(data);

            // assert
            Assert.True(banner.TemplateText == tt);
            Assert.True(!string.IsNullOrWhiteSpace(rslt));
            Assert.True(rslt == "12.24-5.24-20.23-5-0.23");
            Assert.True(banner.Prefix == "{");
            Assert.True(banner.Suffix == "}");
        }

        class TempBanner : ResponseBanner
        {
            public string template { get; set; }

            public TempBanner() { }
            public TempBanner(string prefix, string suffix)
                :base(prefix, suffix)
            {

            }
            protected override string GetTemplateText() => template;
        }
    }
}
